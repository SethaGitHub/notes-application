using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using NoteAPI.Models;
using NoteAPI.DTOs;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;

public class UserService
{
    private readonly IDbConnection _connection;
    private readonly string _jwtSecret;

    public UserService(IDbConnection connection, IConfiguration configuration)
    {
        _connection = connection;
        _jwtSecret = configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JWT secret key is missing");
    }


    public async Task<IEnumerable<User>> GetAllUsers() =>
        await _connection.QueryAsync<User>("SELECT * FROM Users");

    public async Task<User?> GetUserById(int id) =>
        await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });

    public async Task<User?> GetUserByEmail(string email) =>
        await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE email = @Email", new { Email = email });

    public async Task<bool> RegisterUser(RegisterDto registerDto)
    {
        var existingUser = await _connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email", new { registerDto.Email });

        if (existingUser != null) return false; // User already exists

        string hashedPassword = HashPassword(registerDto.Password);
        var result = await _connection.ExecuteAsync(
            "INSERT INTO Users (Email, Password, CreatedAt, UpdatedAt) VALUES (@Email, @Password, @CreatedAt, @UpdatedAt)",
            new { registerDto.Email, Password = hashedPassword, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });

        return result > 0;
    }

    public async Task<string?> LoginUser(LoginDto loginDto)
    {
        var user = await _connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email", new { loginDto.Email });

        if (user == null || !VerifyPassword(loginDto.Password, user.Password)) return null;

        return GenerateJwtToken(user);
    }

    public async Task<bool> DeleteUser(int id)
    {
        var result = await _connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
        return result > 0;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
    {
        return HashPassword(enteredPassword) == storedHashedPassword;
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
