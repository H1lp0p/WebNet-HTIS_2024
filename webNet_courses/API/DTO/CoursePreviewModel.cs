using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.DTO
{
	public class CoursePreviewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int StartYear { get; set; }
		public int MaximumStudentsCount { get; set; }
		public int ReaminingSlotsCount { get; set; }
		public CourseStatuses Status { get; set; }
		public Semester Semester { get; set; }
	}
}
