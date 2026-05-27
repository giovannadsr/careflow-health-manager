using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using CareFlow.Application.DTOs.Auth;
using CareFlow.Application.Interfaces;
using CareFlow.Domain.Entities;
using CareFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CareFlow.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;

        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (existingUser != null)
        {
            throw new Exception("Email já cadastrado.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,

            Email = request.Email,

            PasswordHash = passwordHash
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            throw new Exception("Usuário ou senha inválidos.");
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            throw new Exception("Usuário ou senha inválidos.");
        }

        return GenerateToken(user);
    }

    private AuthResponseDto GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(ClaimTypes.Email, user.Email),

            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(
                _configuration["Jwt:ExpiresInMinutes"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],

            audience: _configuration["Jwt:Audience"],

            claims: claims,

            expires: expires,

            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),

            ExpiresAt = expires
        };
    }
}