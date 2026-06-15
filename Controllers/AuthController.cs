using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthApi.DTOs;
using AuthApi.Services;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(new { message = "User registered successfully" });
    }


    [HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    var token = await _authService.LoginAsync(dto);

    if (token == null)
    {
        return Unauthorized(new
        {
            message = "Invalid Credentials"
        });
    }

    return Ok(new
    {
        token
    });
}
}