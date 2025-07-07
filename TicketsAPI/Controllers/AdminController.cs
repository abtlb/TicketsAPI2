using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketsAPI.Models;
using TicketsAPI.Data;
using TicketsAPI.RequestInput;
using Microsoft.EntityFrameworkCore;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly TicketsProjectContext _context;

        public AdminController(TicketsProjectContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpGet("LoggedInAdmin")]
        public async Task<ActionResult<Admin>> LoggedInAdmin()
        {
            var username = HttpContext.User.FindFirstValue("Username");
            var claims = HttpContext.User.Claims.ToList();
            Admin admin = await _context.Admins.FirstOrDefaultAsync(x => x.username == username);

            return Ok(admin);
        }
    }
}
