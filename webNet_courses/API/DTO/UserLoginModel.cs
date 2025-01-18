using System.ComponentModel.DataAnnotations;

namespace webNet_courses.API.DTO
{
	public class UserLoginModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[MinLength(6)]
		[MaxLength(32)]
		public string Password { get; set; }
	}
}
