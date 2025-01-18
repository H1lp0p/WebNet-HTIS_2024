using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webNet_courses.Domain.Entities
{
	public class User : IdentityUser<Guid>
	{
		[Required]
		public string FullName { get; set; }

		[Required]
		public DateTime BirthDate {  get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public ICollection<CampusCourseTeacher> TeachingCourses { get; set; } = new List<CampusCourseTeacher>();

		public ICollection<CampusCourseStudent> LearningCourses { get; set; } = new List<CampusCourseStudent>();
	}
}
