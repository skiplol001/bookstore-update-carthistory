using System;

namespace BookStore.Domain.Entities
{
    public class ReadBook
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}