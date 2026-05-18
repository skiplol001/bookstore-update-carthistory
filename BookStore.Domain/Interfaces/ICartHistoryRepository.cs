using BookStore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Domain.Interfaces
{
    public interface ICartHistoryRepository : IGenericRepository<CartHistory>
    {
        Task<List<CartHistory>> GetByUserIdAsync(string userId);
        Task<CartHistory> GetExistingHistoryAsync(string userId, int productId);
    }
}