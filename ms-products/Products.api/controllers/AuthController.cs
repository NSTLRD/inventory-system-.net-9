
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Products.Api.Common.DTOs;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser>   _userManager;
        private readonly RoleManager<IdentityRole>   _roleManager;
        private readonly IConfiguration               _config;

        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config      = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            var user = new IdentityUser { UserName = dto.UserName, Email = dto.Email };
            var res  = await _userManager.CreateAsync(user, dto.Password);
            if (!res.Succeeded)
                return BadRequest(res.Errors);

            // Assign role
            await _userManager.AddToRoleAsync(user, dto.Role);
            return Ok(new { user.Id, user.UserName, dto.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            // build claims
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // create token
            var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires= DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return Ok(new AuthResponseDto {
                Token   = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires
            });
        }
    }
}
