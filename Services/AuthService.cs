using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthApi.DTOs;
using AuthApi.Models;
using AuthApi.Data;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppdbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
        AppdbContext context,
        IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow

            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return "User registered successfully";
        }

        public async Task<string?> LoginAsync(LoginDto dto)
{
    var user = _context.Users
        .FirstOrDefault(x => x.Email == dto.Email);

    if (user == null)
    {
        return null;
    }

    var validPassword = BCrypt.Net.BCrypt.Verify(
        dto.Password,
        user.PasswordHash
    );

    if (!validPassword)
    {
        return null;
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"]!
        ));

    var creds = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256
    );

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler()
        .WriteToken(token);
}
    }
}