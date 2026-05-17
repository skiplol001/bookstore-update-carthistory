using BookStore.Application.DTO;
using BookStore.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Application.Services
{
    public class CartHistoryService
    {
        private readonly ICartHistoryRepository _cartHistoryRepository;
        public CartHistoryService(ICartHistoryRepository cartHistoryRepository)
        {
            _cartHistoryRepository = cartHistoryRepository;
        }

        public async Task<List<CartHistoryDTO>> GetByUserIdAsync(string userId)
        {
            return await _cartHistoryRepository.GetByUserIdAsync(userId);
        }

        public async Task SaveToDbAsync(string userId, int productId, int quantity)
        {
            await _cartHistoryRepository.SaveToDbAsync(userId, productId, quantity);
        }

        public async Task<CartHistoryDTO> GetProductDetailsAsync(int productId)
        {
            return await _cartHistoryRepository.GetProductDetailsAsync(productId);
        }
    }
}