using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtDbApi.DTOs;
using JwtDbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtDbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly ApplicationDbContext _context;

        public TokenController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserModel userModel)
        {
            if (userModel != null && userModel.Email != null && userModel.Password != null)
            {
                var user = await GetUser(userModel.Email);
                
                if (user != null)
                {
                    if (!BCrypt.Net.BCrypt.Verify(userModel.Password, user.Password))
                    {
                        return BadRequest("Wrong password.");
                    }


                    var claims = new[] 
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("id", user.UserId.ToString()),
                        new Claim("Email", user.Email),
                        //new Claim("Password", user.Password), //ency
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        // [HttpGet]
        // public async Task<UserInfo> GetUser(string userName, string pass)
        // {
        //     return await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == pass);
        // }

        private async Task<UserInfo> GetUser(string Email)
        {
            // return await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == pass);
            return await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == Email);
        }
    }
}