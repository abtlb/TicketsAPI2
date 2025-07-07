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
    public class PerformerController : ControllerBase
    {
        private readonly TicketsProjectContext context;

        public PerformerController(TicketsProjectContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddPerformerInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Performer performer = new Performer()
            {
                performer_name = input.performer_name,
                description = input.description
            };
            context.Performers.Add(performer);
            await context.SaveChangesAsync();

            return Ok(new { message = "Performer added successfuly" });
        }

        [Authorize]
        [HttpGet("Performers")]
        public async Task<ActionResult<ICollection<Performer>>> Performers()
        {
            var performers = await context.Performers.ToListAsync();
            return performers;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Performer>> GetPerformer(int id)
        {
            var performer = await context.Performers.FirstOrDefaultAsync(x => x.performer_id == id);

            if (performer == null)//add this to everything
            {
                return NotFound();
            }

            return performer;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, Performer updated)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            var performer = await context.Performers.FirstOrDefaultAsync(x => x.performer_id == id);

            performer.performer_name = updated.performer_name;
            performer.description = updated.description;
            context.Update(performer);
            await context.SaveChangesAsync();
            return Ok("Updated successfuly");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var performer = await context.Performers.FirstOrDefaultAsync(x => x.performer_id == id);
            context.Performers.Remove(performer);//remove from Performers

            var eventPerformers = await context.Event_Performers.Select(x => x).ToListAsync();

            context.Event_Performers.RemoveRange(await context.Event_Performers.Where(x => x.perfomer_id == id).ToListAsync());//remove from Event_Performers
            foreach (var eventPerformer in eventPerformers)
            {
                context.Events.RemoveRange(await context.Events.Where(x => eventPerformer.event_id == x.event_id).ToListAsync());
                context.Event_Tickets.RemoveRange(await context.Event_Tickets.Where(x => eventPerformer.event_id == x.event_id).ToListAsync());
            }

            await context.SaveChangesAsync();
            return Ok("Deleted successfuly");
        }
    }
}
