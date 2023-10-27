using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MoveIT {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddSqlServer<ProductContext>(builder.Configuration.GetConnectionString("cnMoveIT"));


			var app = builder.Build();

			app.MapGet("/create", async ([FromServices] ProductContext dbContext) => {
				dbContext.Database.EnsureCreated();

				return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
			});

			var resultadosConsulta = app.MapGet("/api/get/all", async ([FromServices] ProductContext dbContext) => {
				var allProducts = dbContext.Products.ToList(); // Obtén todos los productos de la base de datos

				return Results.Ok(allProducts);
			});


			app.Run();




			//// Add services to the container.
			//builder.Services.AddAuthorization();


			//var app = builder.Build();



			// Configure the HTTP request pipeline.

			//app.UseHttpsRedirection();

			//app.UseAuthorization();

			//	var summaries = new[]
			//	{
			//	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
			//};

			//	app.MapGet("/weatherforecast", (HttpContext httpContext) => {
			//		var forecast = Enumerable.Range(1, 5).Select(index =>
			//			new WeatherForecast {
			//				Date = DateTime.Now.AddDays(index),
			//				TemperatureC = Random.Shared.Next(-20, 55),
			//				Summary = summaries[Random.Shared.Next(summaries.Length)]
			//			})
			//			.ToArray();
			//		return forecast;
			//	});


		}
	}
}