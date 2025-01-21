using System.ComponentModel.DataAnnotations;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.DTO
{
	public class EditCourseStudentMarkModel
	{
		[Required]
		public MarkType MarkType { get; set; }

		[Required]
		public StudentMarks Mark { get; set; }
	}
}
