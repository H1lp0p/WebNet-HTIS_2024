using System.ComponentModel.DataAnnotations;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.Domain.Entities
{
	public class CampusCourse
	{
		public Guid Id { get; set; }

		public Guid CampusGroupId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public DateTime CreatedTime { get; set; }

		[Required]
		[Range(2000, 2029, ErrorMessage = "Year must be in range [2000, 2029]")] //TODO: test maxValue
		public int StartYear { get; set; }

		[Required]
		[Range(1, 200, ErrorMessage = "Maximum students count must be in range [1, 200]")] //TODO: also
		public int MaximumStidetsCount { get; set; }

		[Required]
		[MinLength(1)]
		public string Requirements { get; set; }

		[Required]
		[MinLength(1)]
		public string Annotations { get; set; }

		[Required]
		public Semester Semester { get; set; }

		[Required]
		public CourseStatuses Status { get; set; } = CourseStatuses.Created;

		public virtual ICollection<CampusCourseTeacher> Teachers { get; set; } = new List<CampusCourseTeacher>();

		public virtual ICollection<CampusCourseStudent> Students { get; set; } = new List<CampusCourseStudent>();

		public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
	}
}
