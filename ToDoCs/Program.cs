using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApi;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Context
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

// Register Services
builder.Services.AddScoped<Service>();
builder.Services.AddScoped<AuthService>();

// ========== JWT AUTH =============

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAll");

// Enable Auth
app.UseAuthentication();
app.UseAuthorization();

// ------------- AUTH ROUTES -------------

app.MapPost("/auth/register", async (string username, string password, AuthService auth) =>
{
     Console.WriteLine($"uuu:{username} ppp:{password}");
    var user = await auth.RegisterAsync(username, password);
   
    if (user == null) return Results.BadRequest("User already exists");

    return Results.Ok("User registered");
});

app.MapPost("/auth/login", async (LoginRequest request, AuthService auth) =>
{
    Console.WriteLine(request.username);
    Console.WriteLine($"[/auth/login] Received login request - username: {request.username}, password: {request.password}");
    var token = await auth.LoginAsync(request.username, request.password);
    Console.WriteLine($"[/auth/login] Token result: {(token is null ? "NULL" : "SUCCESS")}");
    return token is null ? Results.Unauthorized() : Results.Ok(new { token });
});

// -------- ITEMS ROUTES (Protected) ----------

app.MapGet("/items", async (Service service) =>
{
    var items = await service.GetAllAsync();
    return Results.Ok(items);
}).RequireAuthorization();

app.MapGet("/items/{id}", async (int id, Service service) =>
{
    var item = await service.GetItemAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
}).RequireAuthorization();

app.MapPost("/items", async (Item newItem, Service service) =>
{
    var created = await service.AddItemAsync(newItem);
    return Results.Created($"/items/{created.Id}", created);
}).RequireAuthorization();

app.MapPut("/items/{id}", async (int id, Item updatedItem, Service service) =>
{
    var updated = await service.UpdateItemAsync(id, updatedItem);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
}).RequireAuthorization();

app.MapDelete("/items/{id}", async (int id, Service service) =>
{
    var deleted = await service.DeleteItemAsync(id);
    return deleted ? Results.Ok() : Results.NotFound();
}).RequireAuthorization();


app.Run();
