using System.ComponentModel.DataAnnotations;

namespace webNet_courses.Domain.Entities
{
	public class Notification
	{
		public Guid Id { get; set; }

		[Required]
		public string Text { get; set; }

		[Required]
		public bool IsImportant { get; set; } = false;

		public Guid CampusCourseId { get; set; }
	}
}
