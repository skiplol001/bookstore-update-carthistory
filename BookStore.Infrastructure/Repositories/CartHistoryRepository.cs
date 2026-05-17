using BookStore.Application.DTO;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Infrastructure.Repositories
{
    public class CartHistoryRepository : ICartHistoryRepository
    {
        private readonly BookStoreDbContext _context;

        public CartHistoryRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách lịch sử từ DB theo UserId
        public async Task<List<CartHistoryDTO>> GetByUserIdAsync(string userId)
        {
            return await _context.CartHistories
                .Where(ch => ch.UserId == userId)
                .Include(ch => ch.Product)
                .ThenInclude(p => p.Images)
                .OrderByDescending(ch => ch.AddedDate)
                .Select(ch => new CartHistoryDTO
                {
                    ProductId = ch.ProductId,
                    ProductName = ch.Product.Name,
                    Price = ch.Product.Price,
                    Quantity = ch.Quantity,
                    AddedDate = ch.AddedDate
                })
                .ToListAsync();
        }

        // Lưu mới hoặc cập nhật số lượng nếu sản phẩm đã có trong lịch sử
        public async Task SaveToDbAsync(string userId, int productId, int quantity)
        {
            var existingHistory = await _context.CartHistories
                .FirstOrDefaultAsync(ch => ch.UserId == userId && ch.ProductId == productId);

            if (existingHistory != null)
            {
                existingHistory.Quantity += quantity;
                existingHistory.AddedDate = DateTime.Now;
            }
            else
            {
                var history = new CartHistory
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                await _context.CartHistories.AddAsync(history);
            }
            await _context.SaveChangesAsync();
        }

        // Lấy thông tin cơ bản của sách để phục vụ lưu Session tạm thời
        public async Task<CartHistoryDTO> GetProductDetailsAsync(int productId)
        {
            return await _context.Products
                .Where(p => p.Id == productId)
                .Select(p => new CartHistoryDTO
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price,
                    Quantity = 1,
                    AddedDate = DateTime.Now
                })
                .FirstOrDefaultAsync();
        }
    }
}