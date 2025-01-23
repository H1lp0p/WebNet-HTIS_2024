namespace webNet_courses.Domain.Entities
{
	public class CampusCourseTeacher
	{
		public Guid Id { get; set; }

		public virtual User User { get; set; }
		public virtual CampusCourse Course { get; set; }

		public bool isMain { get; set; } = false;
	}
}
