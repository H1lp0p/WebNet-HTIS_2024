using System.ComponentModel.DataAnnotations;

namespace webNet_courses.API.DTO
{
	public class AddTeacherModel
	{
		[Required]
		public Guid UserId { get; set; }
	}
}
