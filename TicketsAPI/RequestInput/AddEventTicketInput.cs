using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.RequestInput
{
    public class AddEventTicketInput
    {
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string Type { get; set; }

        [Required]
        public int TicketsNum { get; set; }

        [Required]
        public decimal Price { get; set; }


    }
}
