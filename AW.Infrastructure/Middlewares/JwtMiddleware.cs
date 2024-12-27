using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Infrastructure.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var authToken = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(authToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    tokenHandler.ValidateToken(
                                        authToken,
                                        new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                        {
                                            ValidateIssuer = true,
                                            ValidateAudience = true,
                                            ValidAudience = _configuration["JWT:Audience"],
                                            ValidateLifetime = true,
                                            ValidateIssuerSigningKey = true,
                                            ValidIssuer = _configuration["JWT:Issuer"],
                                            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration[key: "JWT:SigningKey"] ?? "")),
                                            ClockSkew = new TimeSpan(0, 1, 5),
                                        },
                                        out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);

                    context.Request.Headers["Authorization"] = $"Bearer {authToken}";
                    //context.Request.Headers.Add("Authorization", $"Bearer {authToken}");
                    Console.WriteLine(context.Request.Headers["Authorization"]);
                }
                catch 
                {
                    // Token tidak valid, kembalikan 401
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }                
            }
            await _next(context);
        }
    }
}
