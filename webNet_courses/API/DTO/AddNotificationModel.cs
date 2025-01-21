using System.ComponentModel.DataAnnotations;

namespace webNet_courses.API.DTO
{
	public class AddNotificationModel
	{
		[Required]
		[MinLength(1)]
		public string Text { get; set; }

		[Required]
		public bool IsImportant { get; set; }
	}
}
