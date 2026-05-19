using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Infrastructure.Repositories
{
    public class ReadBookRepository : GenericRepository<ReadBook>, IReadBookRepository
    {
        private readonly BookStoreDbContext _context;

        public ReadBookRepository(BookStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ReadBook>> GetByUserIdAsync(string userId)
        {
            return await _context.ReadBooks
                .Where(rb => rb.UserId == userId)
                .Include(rb => rb.Product)
                .ThenInclude(p => p.Images)
                .OrderByDescending(rb => rb.AddedDate)
                .ToListAsync();
        }

        public async Task<ReadBook?> GetExistingReadBookAsync(string userId, int productId)
        {
            return await _context.ReadBooks
                .FirstOrDefaultAsync(rb => rb.UserId == userId && rb.ProductId == productId);
        }
    }
}