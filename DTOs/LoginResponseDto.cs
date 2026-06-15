using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApi.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
}