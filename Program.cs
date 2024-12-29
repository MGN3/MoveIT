using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using MoveIT.Models;
using static MoveIT.Program;
using System.Text.Json;
using Newtonsoft.Json;
using Azure;

namespace MoveIT {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			var chatMessagesByUser = @"E:\MGN\source\repos\MoveIT\chatMessagesByUser.json"; // JSON for chat messages

			builder.Services.AddDbContext<MoveITDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("cnMoveIT")));

			builder.Services.AddDbContext<MoveITDbContext>(options =>
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

			// A list of possible customer service-like responses
			List<string> responses = new List<string>
			{
				"Hello, how can I assist you today?",
				"Hi there, how may I help you?",
				"Good day! What can I do for you?",
				"Welcome! What do you need help with?",
				"Hi, how can I be of service?"
				// Add more responses as needed
			};

			////// PRODUCT /////
			app.MapGet("/createProducts", async ([FromServices] MoveITDbContext dbContext) => {
				dbContext.Database.EnsureCreated();

				return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
			});

			app.MapGet("/api/Products", async ([FromServices] MoveITDbContext dbContext) => {
				var allProducts = dbContext.Products.ToList(); // All database products

				return Results.Json(allProducts);
			});

			////// USER /////
			app.MapGet("/createUsers", async ([FromServices] MoveITDbContext dbContext) => {
				dbContext.Database.EnsureCreated();

				return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
			});

			app.MapPost("/api/Users", async ([FromServices] MoveITDbContext dbContext, User newUser) => {
				//The user to be added
				User forgedUser;
				//Verifying if user already exists using email, since it is a unique index.
				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);

				if (existingUser != null) {
					return Results.BadRequest("Email not availeable.");
				} else {
					forgedUser = new User(Guid.NewGuid(), newUser.Name, newUser.Email, newUser.Password);
					dbContext.Users.Add(forgedUser);

					await dbContext.SaveChangesAsync();

					var userSubset = new { Name = forgedUser.Name, Email = forgedUser.Email };

					return Results.Ok(userSubset);
				}
			});

			app.MapGet("/api/checkUserByEmail", async ([FromServices] MoveITDbContext dbContext, string email) => {

				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

				if (existingUser != null) {
					var userExistsData = new { Name = existingUser.Name, Email = existingUser.Email };
					return Results.Ok(userExistsData);
				} else {
					return Results.NotFound("User not found");
				}
			});

			app.MapGet("/api/nameAvaileable", async ([FromServices] MoveITDbContext dbContext, string name) => {
				bool nameAvaileable;
				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Name == name);

				if (existingUser != null) {
					nameAvaileable = false;
				} else {
					nameAvaileable = true;
				}
				return Results.Ok(nameAvaileable);
			});

			/////////////// POST VERSION OF THE PREVIOUS ENDPOINT ////////
			app.MapPost("/api/nameAvaileable2", async ([FromServices] MoveITDbContext dbContext, [FromBody] string name) => {
				bool nameAvaileable;
				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Name == name);

				if (existingUser != null) {
					nameAvaileable = false;
				} else {
					nameAvaileable = true;
				}
				return Results.Ok(nameAvaileable);
			});

			app.MapPost("/api/emailAvailable", async ([FromServices] MoveITDbContext dbContext, [FromBody] JsonElement jsonElement) => {
				string email = jsonElement.GetProperty("email").GetString().ToLower(); // Convertimos el email a minúsculas
				bool emailAvailable;
				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email); // Comparamos en minúsculas

				emailAvailable = existingUser == null;

				return Results.Json(new { emailAvailable });
			});



			///METODO CON CLASES APARTE
			//var usersData = new UsersData {
			//	Users = new List<Dictionary<string, UserJson>>()
			//};

			//app.MapPost("/addMessage", async (HttpContext context) =>
			//{
			//	using var reader = new System.IO.StreamReader(context.Request.Body);
			//	var body = await reader.ReadToEndAsync();
			//	var requestBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);

			//	if (requestBody.TryGetValue("user", out var username) && requestBody.TryGetValue("message", out var messageValue)) {
			//		usersData.AddMessageToUser(username, messageValue, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK"));
			//		return Results.Ok("Message added successfully.");
			//	} else {
			//		return Results.BadRequest("Invalid request format: 'user' and 'message' fields are required.");
			//	}
			//});

			app.MapPost("/insertData", async (HttpContext context) => {
				string jsonFilePath = "chatMessagesByUser.json";

				// Read existing JSON
				string json = await File.ReadAllTextAsync(jsonFilePath);
				// ChatData object initialization or var?
				//ChatData/chatdata is a List of dictionaries with username and a list of objects from Message class.
				var chatData = System.Text.Json.JsonSerializer.Deserialize<ChatData>(json);

				// Read the request content
				using var reader = new StreamReader(context.Request.Body);
				string requestBody = await reader.ReadToEndAsync();
				var content = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<Message>>>(requestBody);

				// Obtain first key-value pair in the body with name and message
				var userEntry = content.First();
				string username = userEntry.Key;
				var newMessage = userEntry.Value.First();

				// Find user inside the object ChatData that represents the JSON received.
				var userMessages = chatData.Users.FirstOrDefault(u => u.ContainsKey(username));
				//If user doesn't exists, create one and add a list of messages
				if (userMessages == null) {
					userMessages = new Dictionary<string, List<Message>>();
					userMessages.Add(username, new List<Message>());
					chatData.Users.Add(userMessages);
				}
				userMessages[username].Add(newMessage);

				// Serialize the updated object to JSON
				json = System.Text.Json.JsonSerializer.Serialize(chatData);

				// Write the updated JSON in the file
				await File.WriteAllTextAsync(jsonFilePath, json);

				//TODO->Call to a chatbot/ai chat to return an answer based on the imput??

				// Select a random response from a list of typical messages 
				Random rnd = new Random();
				string message = responses[rnd.Next(responses.Count)];

				// Return the response as JSON
				return Results.Json(new { message });
			});


			app.MapGet("/getMessagesByUser/{username}", async (string username) => {
				string jsonFilePath = "chatMessagesByUser.json";

				// Reading existing JSON
				string json = await File.ReadAllTextAsync(jsonFilePath);
				var chatData = System.Text.Json.JsonSerializer.Deserialize<ChatData>(json);

				//Find messages by given user
				var userMessages = chatData.Users.FirstOrDefault(u => u.ContainsKey(username));
				if (userMessages == null) {
					return Results.NotFound(new { message = $"No messages found for user {username}." }); //refactor, avoid multiple returns if possible
				}

				// Return messages from user.
				return Results.Json(userMessages[username]);
			});


			app.MapGet("/user", (int number) => {
				return "El doble es: " + number * 2;
				//For a fetchAPI request, return a JSON
			});

			app.MapPost("/api/emaildata", async (HttpContext context) => {
				// El string de prueba que deseas devolver
				string stringResponse = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
				"sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim" +
				" veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo " +
				"consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum " +
				"dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, " +
				"sunt in culpa qui officia deserunt mollit anim id est laborum.";

				// Devolver una respuesta JSON con el string
				return Results.Ok(stringResponse);
			});

			app.Run();
		}
	}
	public class ChatData {
		public List<Dictionary<string, List<Message>>> Users { get; set; } = new List<Dictionary<string, List<Message>>>();
	}

	public class Message {
		public string Messages { get; set; }
		public string DateTime { get; set; }
	}
}


//////////OPENAI CHATGTP request for the chatbox?///////////
/*
 using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapPost("/endpointt", async (HttpRequest request, IHttpClientFactory clientFactory) =>
{
    var openaiApiKey = "key-api";
    var httpClient = clientFactory.CreateClient();

    string requestBody;
    using (var reader = new StreamReader(request.Body, Encoding.UTF8))
    {
        requestBody = await reader.ReadToEndAsync();
    }

    var openaiResponse = await httpClient.PostAsync(
        "https://api.openai.com/v1/engines/gpt-3.5-turbo/completions",
        new StringContent(
            JsonSerializer.Serialize(new
            {
                prompt = requestBody,
                max_tokens = 150
            }),
            Encoding.UTF8,
            "application/json"
        ),
        new HttpRequestMessage().Headers.Add("Authorization", $"Bearer {openaiApiKey}")
    );

    var responseContent = await openaiResponse.Content.ReadAsStringAsync();
    var chatGptResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

    return Results.Json(chatGptResponse);
});

app.Run();

public class OpenAIResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public Choice[] Choices { get; set; }
}

public class Choice
{
    public string Text { get; set; }
}
 
 */