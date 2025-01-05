using LaMaCo.Comments.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Products_Service.Data
{
        public class SupplierProduct
        {
                public int Id { get; set; }

                public int OurProductId { get; set; }

                public int Supplier { get; set; }

                public int SupplierProductId { get; set; }
        }

        public class SupplierProductInfo : SupplierProduct
        {
                public decimal Price { get; set; }

                public int AvailableStock { get; set; }
        }

        
}