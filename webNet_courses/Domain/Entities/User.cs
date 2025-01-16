using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webNet_courses.Domain.Entities
{
	public class Role
	{
		public string Name { get; set; }
	}

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

		public ICollection<Role> Roles { get; set; } = new List<Role>();
	}
}
