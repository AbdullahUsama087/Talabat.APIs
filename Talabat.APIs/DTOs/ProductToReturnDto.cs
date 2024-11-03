using Talabat.Core.Entities;

namespace Talabat.APIs.Dtos
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int ProductBrandId { get; set; } // Foriegn Key : Not allow Null
        public string ProductBrand { get; set; } // Nvigational Property [ONE]


        public int ProductTypeId { get; set; } // Foriegn Key
        public string ProductType { get; set; } // Navigational Property [ONE]
    }
}
