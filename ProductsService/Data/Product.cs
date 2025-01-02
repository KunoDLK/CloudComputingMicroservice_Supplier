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

            public string Description { get; set; } = String.Empty;

            public decimal Price { get; set; }  
      }

      public class SearchSettings
      {
            public string SearchTerm { get; set;}  = String.Empty;

            public int ItemLimit { get; set; } = 0;
      }
}