using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.DTOs
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Status { get; set; }
        public Address ShippingAddress { get; set; }

        //public int DeliveryMethodId { get; set; } // Foreign Key
        //public DeliveryMethod DeliveryMethod { get; set; } // Navigational Property [ONE]
        public string DeliveryMethod { get; set; }

        public decimal DeliveryMethodCost { get; set; }

        public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>(); // Navigational Property [MANY]

        public decimal SubTotal { get; set; }

        //[NotMapped]
        //public decimal Total => SubTotal + DeliveryMethod.Cost; // Derived Att

        public string PaymentIntentId { get; set; } = string.Empty;

        public decimal Total { get; set; }
    }
}
