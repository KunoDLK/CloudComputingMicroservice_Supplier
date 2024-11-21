using LaMaCo.Comments.Api.Data;
using Microsoft.EntityFrameworkCore;
using Products_Service.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProductDbContext>(options =>
{
      if (builder.Environment.IsDevelopment())
      {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = System.IO.Path.Join(path, "comments.db");
            options.UseSqlite($"Data Source={dbPath}");
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
      }
      else
      {
            var cs = builder.Configuration.GetConnectionString("CommentsContext");
            options.UseSqlServer(cs);
      }
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
      var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
      if (builder.Environment.IsDevelopment())
      {
            db.Database.EnsureDeleted();
      }
      db.Database.EnsureCreated();
      db.Database.Migrate();

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
      app.UseSwagger();
      app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
