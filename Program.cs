using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;

namespace MoveIT {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);
			
			builder.Services.AddSqlServer<ProductContext>(builder.Configuration.GetConnectionString("cnMoveIT"));

			// CORS config.
			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder.AllowAnyOrigin()
						   .AllowAnyMethod()// any http method
						   .AllowAnyHeader(); 
				});
			});

			var app = builder.Build();

			app.UseCors();


			app.MapGet("/create", async ([FromServices] ProductContext dbContext) => {
				dbContext.Database.EnsureCreated();

				return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
			});

			var resultadosConsulta = app.MapGet("/api/get/all", async ([FromServices] ProductContext dbContext) => {
				var allProducts = dbContext.Products.ToList(); // Obtén todos los productos de la base de datos

				return Results.Json(allProducts);
			});

			app.Run();
		}
	}
}