using System.ComponentModel.DataAnnotations;

namespace webNet_courses.API.DTO
{
	public class UserEditModel
	{
		[Required]
		[MinLength(1)]
		public string FullName { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }
	}
}
