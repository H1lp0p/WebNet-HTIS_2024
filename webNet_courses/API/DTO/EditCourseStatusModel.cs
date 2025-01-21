using System.ComponentModel.DataAnnotations;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.DTO
{
	public class EditCourseStatusModel
	{
		[Required]
		public CourseStatuses Status { get; set; }
	}
}
