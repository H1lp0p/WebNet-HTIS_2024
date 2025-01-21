using System.ComponentModel.DataAnnotations;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.DTO
{
	public class EditCourseStudentStatusModel
	{
		[Required]
		public StudentStatuses Status { get; set; }
	}
}
