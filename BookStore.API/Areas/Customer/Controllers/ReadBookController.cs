using BookStore.Application.DTOs;
using BookStore.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    [ApiController] // Kích hoạt các tính năng bổ trợ API đời mới
    [Route("api/[controller]/[action]")] // Định tuyến đồng bộ cấu trúc: api/ReadBook/Tên_Hàm
    public class ReadBookController : Controller
    {
        private readonly ReadBookService _readBookService;
        private const string SessionKey = "ReadBookSession";

        public ReadBookController(ReadBookService readBookService)
        {
            _readBookService = readBookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dbHistory = await _readBookService.GetReadBookByUserIdAsync(userId!);
                return Json(new { success = true, data = dbHistory });
            }
            else
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                var sessionList = string.IsNullOrEmpty(sessionData)
                    ? new List<ReadBookDTO>()
                    : JsonSerializer.Deserialize<List<ReadBookDTO>>(sessionData);

                return Json(new { success = true, data = sessionList, isLocalSession = true });
            }
        }

        [HttpPost]
        // Thêm [FromForm] để ép C# đọc đúng các tham số URL-encoded từ body request gửi tới
        public async Task<IActionResult> AddToHistory([FromForm] int productId, [FromForm] int quantity)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _readBookService.AddOrIncrementReadBookAsync(userId!, productId, quantity);
                return Json(new { success = true, message = "Đã lưu vào danh sách đọc" });
            }
            else
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                var sessionList = string.IsNullOrEmpty(sessionData)
                    ? new List<ReadBookDTO>()
                    : JsonSerializer.Deserialize<List<ReadBookDTO>>(sessionData) ?? new List<ReadBookDTO>();

                var existingItem = sessionList.Find(x => x.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.AddedDate = System.DateTime.Now;
                }
                else
                {
                    var productDto = await _readBookService.GetProductDetailsForSessionAsync(productId);
                    if (productDto == null) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                    productDto.Quantity = quantity;
                    sessionList.Add(productDto);
                }

                HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(sessionList));
                return Json(new { success = true, message = "Đã lưu tạm thời vào Session" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SyncSessionToDb()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                if (!string.IsNullOrEmpty(sessionData))
                {
                    var sessionList = JsonSerializer.Deserialize<List<ReadBookDTO>>(sessionData);
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (sessionList != null && userId != null)
                    {
                        foreach (var item in sessionList)
                        {
                            await _readBookService.AddOrIncrementReadBookAsync(userId, item.ProductId, item.Quantity);
                        }
                        HttpContext.Session.Remove(SessionKey);
                    }
                }
                return Json(new { success = true, message = "Đồng bộ danh sách đọc thành công" });
            }
            return Json(new { success = false, message = "Chưa đăng nhập" });
        }
    }
}