using M12_HW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace M12_HW.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Authorizate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                return Ok($"User {newUser.UserName} is registered successfully!");
            }
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Authorizate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                return Ok($"User {user.UserName} logged in successfully!");
            }
            return Unauthorized("Invalid email or password.");
        }

        [HttpPost("createrole")]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("Error RoleName ...");
            }
            var existRoleName = await _roleManager.RoleExistsAsync(role.RoleName);
            if (existRoleName)
            {
                return BadRequest("This RoleName alredy exist ...");
            }
            var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            if (result.Succeeded)
            {
                return Ok($"Role: {role.RoleName} created ...");
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole(Role role)
        {
            if (
                string.IsNullOrEmpty(role.UserId) &&
                string.IsNullOrEmpty(role.RoleId) &&
                string.IsNullOrEmpty(role.RoleName)
                )
            {
                return BadRequest("Error assign role ...");
            }
            var existRole = await _roleManager.FindByIdAsync(role.RoleId);
            if (existRole == null)
            {
                return BadRequest("Not found Role ...");
            }
            var existUser = await _userManager.FindByIdAsync(role.UserId);
            if (existUser == null)
            {
                return BadRequest("Not found User ...");
            }
            var result = await _userManager.AddToRoleAsync(existUser, role.RoleName);
            if (result.Succeeded)
            {
                return Ok($"Role {role.RoleName} assigned to {existUser.UserName}...");
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string userName = User.Identity?.Name ?? "Unknown user";
            await _signInManager.SignOutAsync();
            return Ok($"User {userName} logged out successfully!");
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] Authorizate model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid login attempt 1" });
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    var token_jwt = GenerateJwtToken(user);
                    return Ok(new
                    {
                        message = "User logged in",
                        status = 200,
                        token = token_jwt
                    });
                }
                return Unauthorized(new { message = "Invalid login attempt 2" });
            }
            else
            {
                return BadRequest(new { message = "Error model" });
            }
        }
        private string GenerateJwtToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); ;
        }
    }
}
