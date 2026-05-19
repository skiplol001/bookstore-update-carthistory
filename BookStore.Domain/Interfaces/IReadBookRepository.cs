using BookStore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Domain.Interfaces
{
    public interface IReadBookRepository : IGenericRepository<ReadBook>
    {
        Task<List<ReadBook>> GetByUserIdAsync(string userId);
        Task<ReadBook?> GetExistingReadBookAsync(string userId, int productId);
    }
}