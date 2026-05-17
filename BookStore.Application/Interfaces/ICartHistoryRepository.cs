using BookStore.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Application.Interfaces
{
    public interface ICartHistoryRepository
    {
        Task<List<CartHistoryDTO>> GetByUserIdAsync(string userId);
        Task SaveToDbAsync(string userId, int productId, int quantity);
        Task<CartHistoryDTO> GetProductDetailsAsync(int productId);
    }
}