using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.DTO
{
	public class CampusCourseStudentDto
	{
		public Guid Id { get; set; }

		public string? Name { get; set; }

		public string? Email { get; set; }

		public StudentStatuses StudentStatus { get; set; }

		public StudentMarks MidTermResult { get; set; }

		public StudentMarks FinalResult { get; set; }
	}
}
