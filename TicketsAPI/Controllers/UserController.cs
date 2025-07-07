using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketsAPI.Data;
using TicketsAPI.Models;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private TicketsProjectContext context;
        public UserController(TicketsProjectContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = "User")]
        [HttpPost("BuyTicket/{eventId}/{ticketTypeStr}")]
        public async Task<IActionResult> BuyTicket(int eventId, string ticketTypeStr)
        {
            Event_Ticket ticketType = await context.Event_Tickets.FirstOrDefaultAsync(x => x.event_id == eventId && x.ticket_type == ticketTypeStr);
            if (ticketType.number_of_tickets <= 0)
            {
                throw new NotImplementedException();
            }

            var username = User.FindFirstValue("Username");
            Models.User user = await context.Users.FirstOrDefaultAsync(x => x.username == username);
            Ticket ticket = new Ticket()
            {
                owner = username,
                ownerNavigation = user,
                dateOfPurchase = DateOnly.FromDateTime(DateTime.Now),
                status = ((int)Ticket.State.Active),
                ticket_type = ticketType.ticket_type,
                event_id = ticketType.event_id,
                _event = ticketType._event,
                Event_Ticket = ticketType
            };

            ticketType.number_of_tickets--;
            context.Tickets.Add(ticket);
            context.Event_Tickets.Update(ticketType);
            await context.SaveChangesAsync();
            return Ok(new {message = "Ticket buyed", id = ticket.ticket_id});
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetReciept")]
        public async Task<IActionResult> GetReciept(int id)//ticket id
        {
            Ticket ticket = await context.Tickets.FirstOrDefaultAsync(x => id == x.ticket_id);
            User user = await context.Users.FirstOrDefaultAsync(u => u.username == ticket.owner);

            var username = User.FindFirstValue("Username");
            if (ticket.owner != username)
            {
                return BadRequest();
            }

            var ticketType = await context.Event_Tickets
                .Where(tp => tp.event_id == ticket.event_id && tp.ticket_type == ticket.ticket_type).FirstOrDefaultAsync();
            var _event = await context.Events.Where(e => e.event_id == ticket.event_id).FirstOrDefaultAsync();

            return Ok(new
            {
                event_name = _event.name,
                location = _event.location,
                event_time = _event.event_time.ToString(),
                event_date = _event.event_date.ToString(),

                ticket_id = ticket.ticket_id,
                dateOfPurchase = ticket.dateOfPurchase.ToString(),
                ticket_type = ticket.ticket_type,
                status = (Ticket.State)ticket.status,

                first_name = user.first_name,
                last_name = user.last_name,
                username = user.username,
                phone_number = user.phone_number
            });
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetHistory")]
        public async Task<IActionResult> GetHistory()
        {
            var username = User.FindFirstValue("Username");

            User user = await context.Users.FirstOrDefaultAsync(u => u.username == username);
            //event name, location, ticket type, ticked id
            var tickets = context.Tickets.Where(t => t.owner == username);
            List<int> ticketsIds = new List<int>();
            foreach (var ticket in tickets)
            {
                ticketsIds.Add(ticket.ticket_id);
            }

            return Ok(ticketsIds);
        }
    }
}
