﻿using ContactBook_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace ContactBook_API.Controllers
{
 

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;



        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var user = new User{ UserName = model.Email, Email = model.Email};

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return BadRequest(ModelState);
                }
                else
                {
                    return Ok(new { Message = "User registration successful." });
                }
                
            }
           
        }


        




        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound(new { Message = "Invalid credentials." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }








        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Set a default expiration in minutes if "AccessTokenExpiration" is missing or not a valid numeric value.
            if (!double.TryParse(jwtSettings["AccessTokenExpiration"], out double accessTokenExpirationMinutes))
            {
                accessTokenExpirationMinutes = 30; // Default expiration of 30 minutes.
            }

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
