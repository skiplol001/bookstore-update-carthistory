using BookStore.Application.DTO;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Application.Services
{
    public class CartHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Lấy danh sách lịch sử và map sang DTO trả cho API
        public async Task<List<CartHistoryDTO>> GetCartHistoryByUserIdAsync(string userId)
        {
            var domainHistories = await _unitOfWork.CartHistories.GetByUserIdAsync(userId);

            return domainHistories.Select(ch => new CartHistoryDTO
            {
                ProductId = ch.ProductId,
                ProductName = ch.Product.Name,
                ProductImage = ch.Product.Images.FirstOrDefault()?.ImageUrl, // Lấy hình đại diện sách
                Price = ch.Product.Price,
                Quantity = ch.Quantity,
                AddedDate = ch.AddedDate
            }).ToList();
        }

        // Xử lý logic nghiệp vụ: Thêm mới hoặc tăng số lượng tích lũy
        public async Task AddOrIncrementHistoryAsync(string userId, int productId, int quantity)
        {
            var existingHistory = await _unitOfWork.CartHistories.GetExistingHistoryAsync(userId, productId);

            if (existingHistory != null)
            {
                existingHistory.Quantity += quantity;
                existingHistory.AddedDate = DateTime.Now;
                await _unitOfWork.CartHistories.UpdateAsync(existingHistory);
            }
            else
            {
                var newHistory = new CartHistory
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                await _unitOfWork.CartHistories.AddAsync(newHistory); // Dùng hàm AddAsync của Generic Repo
            }

            await _unitOfWork.SaveChangesAsync(); // Lưu thông qua Unit of Work
        }

        // Phục vụ lấy thông tin sách cho Session (Gọi qua Repo của Product có sẵn trong UoW)
        public async Task<CartHistoryDTO> GetProductDetailsForSessionAsync(int productId)
        {
            // Giả sử ông có Products repo trong UoW, hoặc dùng trực tiếp DbContext tùy dự án, ở đây dùng chuẩn UoW:
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return null;

            return new CartHistoryDTO
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.Images.FirstOrDefault()?.ImageUrl,
                Price = product.Price,
                Quantity = 1,
                AddedDate = DateTime.Now
            };
        }
    }
}