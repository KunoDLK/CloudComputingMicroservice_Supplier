using LaMaCo.Comments.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Products_Service.Data
{
public class SupplierProduct
{
        public int OurProductId { get; set;}

        public int Supplier {get; set;}

        public int SupplierProductId { get; set;}
}
}