using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.RequestInput
{
	public class AddPerformerInput
	{
		[Required]
		[StringLength(50)]
		[Unicode(false)]
		public string performer_name { get; set; }

		[StringLength(250)]
		[Unicode(false)]
		public string description { get; set; }
	}
}
