using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.Models;
using TicketsAPI.Data;
using TicketsAPI.RequestInput;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TicketsProjectContext _context;

        public AccountController(TicketsProjectContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Person person;
            if (input.IsAdmin)
            {
                person = (Person)await _context.Admins.FirstOrDefaultAsync(x => x.username == input.Username && x.password == input.Password);
            }
            else
            {
                person = (Person) await _context.Users.FirstOrDefaultAsync(x => x.username == input.Username && x.password == input.Password);
            }

            if (person == null)
            {
                return Unauthorized("Username or password is wrong");
            }
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, input.IsAdmin? "Admin" : "User"),
                new Claim("Username", person.username),
                new Claim("Password", person.password)
            };

            string token = CreateToken(claims);

            return Ok(new { token = token });
        }

        private string CreateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("My very very veryyyyyyyyyyyyyyyyyyy secret key"));//change this
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int years = DateTime.Now.Year - input.dateOfBirth.Year;

            if (DateTime.Now.Month < input.dateOfBirth.Month || (DateTime.Now.Month == input.dateOfBirth.Month && DateTime.Now.Day < input.dateOfBirth.Day))
            {
                years--;
            }

            if (years < 18)
            {
                return Unauthorized(new {message = "You must be 18 or older to register." });
            }

            User duplicate = await _context.Users.FirstOrDefaultAsync(x => x.username == input.username);
            if (duplicate != null)
            {
                return BadRequest(new { message = "Username already exists, please choose another one." });
            }

            User user = new User()
            {
                first_name = input.first_name,
                last_name = input.last_name,
                username = input.username,
                password = input.password,
                dateOfBirth = DateOnly.FromDateTime(input.dateOfBirth),
                phone_number = input.phone_number
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Register successful" });
        }

        [HttpGet("Role")]
        public IActionResult Role()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirstValue(ClaimTypes.Role);
                if (role == "Admin")
                {
                    return Ok(new { message = "Admin"});
                }
                else
                {
                    return Ok(new { message = "User" });
                }
            }

            return Ok(new { message = "Not authenticated" });
        }

        [HttpGet("Current")]
        public async Task<IActionResult> Current()
        {
            var username = HttpContext.User.FindFirstValue("Username");
            var role = User.FindFirstValue(ClaimTypes.Role);
            if(role == "Admin")
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(x => x.username == username);
                return Ok(new { firstName = admin?.first_name});
            }
            if(role == "User")
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.username == username);
                return Ok(new { firstName = user?.first_name, lastName = user?.last_name, username = user?.username });
            }
            return Unauthorized();
        }
    }
}
