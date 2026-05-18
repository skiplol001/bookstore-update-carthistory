using BookStore.Application.DTO;
using BookStore.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    public class CartHistoryController : Controller
    {
        private readonly CartHistoryService _cartHistoryService;
        private const string SessionKey = "CartHistorySession";

        public CartHistoryController(CartHistoryService cartHistoryService)
        {
            _cartHistoryService = cartHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dbHistory = await _cartHistoryService.GetCartHistoryByUserIdAsync(userId!);
                return Json(new { success = true, data = dbHistory });
            }
            else
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                var sessionList = string.IsNullOrEmpty(sessionData)
                    ? new List<CartHistoryDTO>()
                    : JsonSerializer.Deserialize<List<CartHistoryDTO>>(sessionData);

                return Json(new { success = true, data = sessionList, isLocalSession = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToHistory(int productId, int quantity)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _cartHistoryService.AddOrIncrementHistoryAsync(userId!, productId, quantity);
                return Json(new { success = true, message = "Đã lưu lịch sử vào Database" });
            }
            else
            {
                var sessionData = HttpContext.Session.GetString(SessionKey);
                var sessionList = string.IsNullOrEmpty(sessionData)
                    ? new List<CartHistoryDTO>()
                    : JsonSerializer.Deserialize<List<CartHistoryDTO>>(sessionData) ?? new List<CartHistoryDTO>();

                var existingItem = sessionList.Find(x => x.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.AddedDate = System.DateTime.Now;
                }
                else
                {
                    var productDto = await _cartHistoryService.GetProductDetailsForSessionAsync(productId);
                    if (productDto == null) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                    productDto.Quantity = quantity;
                    sessionList.Add(productDto);
                }

                HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(sessionList));
                return Json(new { success = true, message = "Đã lưu lịch sử tạm thời vào Session" });
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
                    var sessionList = JsonSerializer.Deserialize<List<CartHistoryDTO>>(sessionData);
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (sessionList != null && userId != null)
                    {
                        foreach (var item in sessionList)
                        {
                            await _cartHistoryService.AddOrIncrementHistoryAsync(userId, item.ProductId, item.Quantity);
                        }
                        HttpContext.Session.Remove(SessionKey);
                    }
                }
                return Json(new { success = true, message = "Đồng bộ giỏ hàng thành công" });
            }
            return Json(new { success = false, message = "Chưa đăng nhập để đồng bộ" });
        }
    }
}