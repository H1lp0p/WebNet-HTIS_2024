using webNet_courses.Domain.Enumerations;

namespace webNet_courses.Domain.Entities
{
	public class CampusCourseStudent
	{
		public User User { get; set; }
		public CampusCourse Course { get; set; }

		public StudentStatuses StudentStatus { get; set; } = StudentStatuses.InQueue;

		public StudentMarks MidtermResult { get; set; } = StudentMarks.NotDefined;
		public StudentMarks FinalResult { get; set; } = StudentMarks.NotDefined;
	}
}
