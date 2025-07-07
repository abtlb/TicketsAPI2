using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketsAPI.Data;
using TicketsAPI.Models;
using TicketsAPI.RequestInput;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly TicketsProjectContext context;

        public EventController(TicketsProjectContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public IActionResult Add(AddEventInput input)
        {
            if (input.performers == null || input.performers.Count == 0)
            {
                return BadRequest(new { message = "Choose at least one performer." });
            }
            if (input.tickets == null || input.tickets.Count == 0)
            {
                ModelState.AddModelError("Tickets", "Add at least one ticket type");
                return BadRequest(new { message = "Add at least one ticket type" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var username = User.FindFirstValue("Username");
            Admin admin = context.Admins.FirstOrDefault(x => x.username == username);
            Event event1 = new()
            {
                adminNavigation = admin,
                type = input.type,
                name = input.name,
                event_date = input.event_date,
                event_time = input.event_time,
                description = input.description,
                location = input.location
            };
            foreach (var entry in input.tickets)
            {
                Event_Ticket ticket = new()
                {
                    ticket_type = entry.Value.Type,
                    number_of_tickets = entry.Value.TicketsNum,
                    price = entry.Value.Price,
                    _event = event1
                };
                event1.Event_Tickets.Add(ticket);
            }
            foreach (var entry in input.performers)
            {
                Event_Performer event_performer = new()
                {
                    perfomer_id = entry,
                    _event = event1
                };
                event1.Event_Performers.Add(event_performer);
            }

            context.Events.Add(event1);
            context.SaveChanges();

            return Ok(new { message = "Event added successfuly" });
        }

        [Authorize]
        [HttpGet("Events")]
        public async Task<ActionResult<ICollection<Event>>> GetEvents()
        {
            var events = await context.Events.ToListAsync();
            foreach (var _event in events)
            {
                _event.adminNavigation = await context.Admins.FirstOrDefaultAsync(x => x.username == _event.admin);
            }

            return Ok(events);
        }

        [Authorize]
        [HttpGet("EventsByPerformer/{id}")]
        public async Task<ActionResult<ICollection<Event>>> GetEventsByPerformer(int id)//performer id
        {
            var events = await context.Event_Performers
             .Where(ep => ep.perfomer_id == id)
             .Join(context.Events,
             ep => ep.event_id,
             e => e.event_id,
             (ep, e) => e)
             .ToListAsync();

            foreach (var _event in events)
            {
                _event.adminNavigation = context.Admins.FirstOrDefault(x => x.username == _event.admin);
            }
            return events;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            Event _event = await context.Events.Include(e => e.Event_Performers).ThenInclude(ep => ep.perfomer).Include(e => e.Event_Tickets).ThenInclude(et => et.Tickets).FirstOrDefaultAsync(x => x.event_id == id);
            return _event;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEdit(int id, AddEventInput input)
        {
            if (input.performers == null || input.performers.Count == 0)
            {
                return BadRequest("Select at least one performer");
            }
            if (input.tickets == null || input.tickets.Count == 0)
            {
                return BadRequest("Add at least one ticket type");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var username = User.FindFirstValue("Username");
            Admin admin = await context.Admins.FirstOrDefaultAsync(x => x.username == username);
            Event event1 = await context.Events.FirstOrDefaultAsync(x => x.event_id == id);
            event1.adminNavigation = admin;
            event1.type = input.type;
            event1.name = input.name;
            event1.event_date = input.event_date;
            event1.event_time = input.event_time;
            event1.description = input.description;
            event1.location = input.location;

            context.Event_Tickets.RemoveRange(await context.Event_Tickets.Where(x => x.event_id == id).ToListAsync());
            event1.Event_Tickets = new List<Event_Ticket>();
            foreach (var entry in input.tickets)
            {
                Event_Ticket ticket = new()
                {
                    ticket_type = entry.Value.Type,
                    number_of_tickets = entry.Value.TicketsNum,
                    price = entry.Value.Price,
                    _event = event1
                };
                event1.Event_Tickets.Add(ticket);
            }

            context.Event_Performers.RemoveRange(context.Event_Performers.Where(x => x.event_id == id).ToList());
            event1.Event_Performers = new List<Event_Performer>();
            foreach (var entry in input.performers)
            {
                Event_Performer event_performer = new()
                {
                    perfomer_id = entry,
                    _event = event1

                };
                event1.Event_Performers.Add(event_performer);
            }

            context.Events.Update(event1);
            await context.SaveChangesAsync();

            return Ok("Event updated successfuly");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            context.Events.RemoveRange(context.Events.Where(x => x.event_id == id).ToList());
            context.Event_Performers.RemoveRange(context.Event_Performers.Where(x => x.event_id == id).ToList());
            context.Event_Tickets.RemoveRange(context.Event_Tickets.Where(x => x.event_id == id).ToList());

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            Event _event = context.Events.FirstOrDefault(x => x.event_id == id);
            var ticketTypes = context.Event_Tickets.Where(x => x.event_id == id).ToList();
            var performers = context.Event_Performers
            .Where(ep => ep.event_id == id)            // Filter by event_id
            .Join(context.Performers,                  // Join with Performers table
            ep => ep.perfomer_id,                      // Match performer_id from Event_Performers
            p => p.performer_id,                       // Match performer_id in Performers
            (ep, p) => p)                              // Select the performer
            .ToListAsync();                            // Execute the query and return the list

            return Ok(new {resultEvent = _event, resultPerformers = performers, resultTicketTypes = ticketTypes});
        }
    }
}
