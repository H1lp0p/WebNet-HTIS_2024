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

		public virtual ICollection<CampusCourseTeacher> TeachingCourses { get; set; } = new List<CampusCourseTeacher>();

		public virtual ICollection<CampusCourseStudent> LearningCourses { get; set; } = new List<CampusCourseStudent>();
	}
}
