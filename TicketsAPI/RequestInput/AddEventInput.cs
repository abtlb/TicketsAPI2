using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TicketsAPI.Models;

namespace TicketsAPI.RequestInput
{
	public class AddEventInput
	{
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string type { get; set; }

        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string name { get; set; }

        public DateOnly event_date { get; set; }

        public TimeOnly event_time { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string description { get; set; }

        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string location { get; set; }

        [Required]
        public List<int> performers { get; set; }

        [Required]
        public Dictionary<int, AddEventTicketInput> tickets { get; set; }

        public int id;

        public AddEventInput()
        {

        }

        public AddEventInput(Event _event)
        {
            this.type = _event.type;
            this.name = _event.name;
            this.event_time = _event.event_time;
            this.event_date = _event.event_date;
            this.location = _event.location;
            this.id = _event.event_id;
            this.description = _event.description;
            double d = 3;
            Console.WriteLine((((int)d)));
        }
    }
}
