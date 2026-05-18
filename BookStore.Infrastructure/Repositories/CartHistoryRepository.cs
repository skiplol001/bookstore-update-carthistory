using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Infrastructure.Repositories
{
    public class CartHistoryRepository : GenericRepository<CartHistory>, ICartHistoryRepository
    {
        private new readonly BookStoreDbContext _context;

        public CartHistoryRepository(BookStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CartHistory>> GetByUserIdAsync(string userId)
        {
            return await _context.CartHistories
                .Where(ch => ch.UserId == userId)
                .Include(ch => ch.Product)
                .ThenInclude(p => p.Images)
                .OrderByDescending(ch => ch.AddedDate)
                .ToListAsync();
        }

        public async Task<CartHistory> GetExistingHistoryAsync(string userId, int productId)
        {
            return await _context.CartHistories
                .FirstOrDefaultAsync(ch => ch.UserId == userId && ch.ProductId == productId);
        }
    }
}