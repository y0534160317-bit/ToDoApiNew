using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace TodoApi.Services;

public class AuthService
{
    private readonly ToDoDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(ToDoDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // Hash password with BCrypt
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verify password with BCrypt
    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public async Task<loginUser> RegisterAsync(string username, string password)
    {
        if (await _db.Login_users.AnyAsync(u => u.username == username))
            return null;

        var myUser = new loginUser
        {
            username = username,
            password = HashPassword(password)
        };

        _db.Login_users.Add(myUser);
        await _db.SaveChangesAsync();

        return myUser;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        Console.WriteLine($"[LoginAsync] Attempting login with username: {username}");
        
        var user = await _db.Login_users
            .FirstOrDefaultAsync(u => u.username == username);

        if (user == null)
        {
            Console.WriteLine($"[LoginAsync] User '{username}' not found in database");
            return null;
        }

        Console.WriteLine($"[LoginAsync] User found. Verifying password...");
        Console.WriteLine($"[LoginAsync] Stored password hash: {user.password}");
        
        bool isPasswordValid = VerifyPassword(password, user.password);
        Console.WriteLine($"[LoginAsync] Password verification result: {isPasswordValid}");
        
        if (!isPasswordValid)
        {
            Console.WriteLine($"[LoginAsync] Password mismatch for user '{username}'");
            return null;
        }

        Console.WriteLine($"[LoginAsync] Authentication successful for user '{username}'");
        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(loginUser user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.username),
            new Claim("userId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
