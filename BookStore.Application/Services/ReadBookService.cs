using BookStore.Application.DTOs;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Application.Services
{
    public class ReadBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReadBookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ReadBookDTO>> GetReadBookByUserIdAsync(string userId)
        {
            var domainBooks = await _unitOfWork.ReadBooks.GetByUserIdAsync(userId);

            return domainBooks.Select(rb => new ReadBookDTO
            {
                ProductId = rb.ProductId,
                ProductName = rb.Product.Name,
                ProductImage = rb.Product.Images?.FirstOrDefault()?.ImageUrl,
                Price = rb.Product.Price,
                Quantity = rb.Quantity,
                AddedDate = rb.AddedDate
            }).ToList();
        }

        public async Task AddOrIncrementReadBookAsync(string userId, int productId, int quantity)
        {
            var existingBook = await _unitOfWork.ReadBooks.GetExistingReadBookAsync(userId, productId);

            if (existingBook != null)
            {
                existingBook.Quantity += quantity;
                existingBook.AddedDate = DateTime.Now;
                _unitOfWork.ReadBooks.UpdateAsync(existingBook);
            }
            else
            {
                var newBook = new ReadBook
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                await _unitOfWork.ReadBooks.AddAsync(newBook);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ReadBookDTO?> GetProductDetailsForSessionAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return null;

            return new ReadBookDTO
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.Images?.FirstOrDefault()?.ImageUrl,
                Price = product.Price,
                Quantity = 1,
                AddedDate = DateTime.Now
            };
        }
    }
}