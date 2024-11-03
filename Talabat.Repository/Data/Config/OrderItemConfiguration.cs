using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Mapping Product with Order
            builder.OwnsOne(orderItem => orderItem.Product, Product => Product.WithOwner());


            // Handle decimal type in Database
            builder.Property(orderItem => orderItem.Price).HasColumnType("decimal(18,2)");
        }
    }
}
