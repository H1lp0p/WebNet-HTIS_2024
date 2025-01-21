using System.ComponentModel.DataAnnotations;

namespace webNet_courses.API.DTO
{
	public class EditCourseRAModel
	{
		[Required]
		[MinLength(1)]
		public string Requirements { get; set; }

		[Required]
		[MinLength(1)]
		public string Annotations { get; set; }
	}
}
