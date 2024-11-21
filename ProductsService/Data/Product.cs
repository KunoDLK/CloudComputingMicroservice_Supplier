using LaMaCo.Comments.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Products_Service.Data
{
      public class Product
      {
            public int Id { get; set; }
            public string Name { get; set; } = String.Empty;

            public decimal Price { get; set; }  
      }
}