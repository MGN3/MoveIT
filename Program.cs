using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using MoveIT.Models;

namespace MoveIT {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<ProductContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("cnMoveIT")));

			builder.Services.AddDbContext<UserContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("cnMoveIT")));

			// CORS config.
			builder.Services.AddCors(options => {
				options.AddDefaultPolicy(builder => {
					builder.AllowAnyOrigin()
						   .AllowAnyMethod()// any http method
						   .AllowAnyHeader();
				});
			});

			/////// APP //////
			var app = builder.Build();

			app.UseCors();

			////// PRODUCT /////
			app.MapGet("/createProducts", async ([FromServices] ProductContext dbContext) => {
				dbContext.Database.EnsureCreated();

				return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
			});

			app.MapGet("/api/get/all", async ([FromServices] ProductContext dbContext) => {
				var allProducts = dbContext.Products.ToList(); // All database products

				return Results.Json(allProducts);
			});

			////// USER /////
			app.MapGet("/createUsers", async ([FromServices] UserContext dbContext) => {
				dbContext.Database.EnsureCreated();

				return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
			});

			app.MapPost("/addUser", async ([FromServices] UserContext dbContext, User newUser) => {
				//The user to be added
				User forgedUser;
				//Verifying if user already exists using email, since it is a unique index.
				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);

				if (existingUser != null) {
					return Results.BadRequest("You already have an accout with that email.");
				} else {
					forgedUser = new User(Guid.NewGuid(), newUser.Name, newUser.Email, newUser.Password);
					dbContext.Users.Add(forgedUser);

					await dbContext.SaveChangesAsync();

					var userSubset = new { Name = forgedUser.Name, Email = forgedUser.Email };

					return Results.Ok(userSubset);
				}
			});

			app.MapGet("/api/checkUserByEmail", async ([FromServices] UserContext dbContext, string email) => {

				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

				if (existingUser != null) {

					var userExistsData = new { Name = existingUser.Name, Email = existingUser.Email };
					return Results.Ok(userExistsData);
				} else {

					return Results.NotFound("User don't exist");
				}
			});

			app.MapGet("/user", (int number) => {
				return "El doble es: " + number * 2;
				//For a fetchAPI request, return a JSON
			});

			app.Run();
		}
	}
}