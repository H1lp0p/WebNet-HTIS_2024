using webNet_courses.Domain.Enumerations;

namespace webNet_courses.Domain.Entities
{
	public class CampusCourseStudent
	{
		public Guid Id { get; set; }

		public virtual User User { get; set; }
		public virtual CampusCourse Course { get; set; }

		public StudentStatuses StudentStatus { get; set; } = StudentStatuses.InQueue;

		public StudentMarks MidtermResult { get; set; } = StudentMarks.NotDefined;
		public StudentMarks FinalResult { get; set; } = StudentMarks.NotDefined;
	}
}
