namespace Shop.Core.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AddedAt { get; set; }
        public Guid PublisherId { get; set; }

        public User? Publisher { get; set; }

        public Product(string name, string desc, decimal price, Guid publisherId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = desc;
            Price = price;
            IsAvailable = true;
            AddedAt = DateTime.UtcNow;
            PublisherId = publisherId;
        }

        public Product() { }

        public void Available() => IsAvailable = true;
        public void NotAvailable() => IsAvailable = false;

        public void Update(string name, string description, decimal price)
        {
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
