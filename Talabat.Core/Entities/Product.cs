using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int ProductBrandId { get; set; } // Foriegn Key : Not allow Null
        public ProductBrand ProductBrand { get; set; } // Nvigational Property [ONE]


        public int ProductTypeId { get; set; } // Foriegn Key
        public ProductType ProductType { get; set; } // Navigational Property [ONE]
    }
}
