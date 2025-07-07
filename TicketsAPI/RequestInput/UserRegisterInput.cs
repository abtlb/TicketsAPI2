using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.RequestInput
{
	public class UserRegisterInput
	{
		public string username { get; set; }

		[Required]
		[StringLength(50)]
		[Unicode(false)]
		public string password { get; set; }

		[Required]
		[StringLength(50)]
		[Unicode(false)]
		public string first_name { get; set; }

		[Required]
		[StringLength(50)]
		[Unicode(false)]
		public string last_name { get; set; }

		public DateTime dateOfBirth { get; set; }

		[Required]
		[StringLength(50)]
		[Unicode(false)]
		public string phone_number { get; set; }
	}
}
