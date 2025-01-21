using System.ComponentModel.DataAnnotations;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.DTO
{
	public class CampusCourseDetailsModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int StartYear { get; set; }
		public int MaximumStidetsCount { get; set; }

		public string Requirements { get; set; }
		public string Annotations { get; set; }

		public Semester Semester { get; set; }
		public CourseStatuses Status { get; set; }

		public ICollection<CampusCourseTeacherDto> Teachers { get; set; }

		public ICollection<CampusCourseStudentDto> Students { get; set; }

		public ICollection<NotificationDto> Notifications { get; set; }
	}
}
