using System;

namespace BookStore.Application.DTOs
{
    public class ReadBookDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
    }
}