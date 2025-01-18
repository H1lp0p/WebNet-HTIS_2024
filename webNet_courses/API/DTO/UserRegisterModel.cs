using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace webNet_courses.API.DTO
{
	public class UserRegisterModel
	{
		[Required]
		[MinLength(1)]
		[MaxLength(1000)]
		public required string FullName { get; set; }

		[Required]
		[MinLength(6)]
		[MaxLength(32)]
		public required string Password { get; set; }

		[Required]
		[MinLength(6)]
		[MaxLength(32)]
		public required string ConfirmPassword { get; set; }

		[Required]
		[MinLength(1)]
		[EmailAddress]
		public required string Email { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }
	}
}
