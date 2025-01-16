namespace webNet_courses.Domain.Entities
{
	public class CampusCourseTeacher
	{
		public User User { get; set; }
		public CampusCourse Course { get; set; }

		public bool isMain { get; set; } = false;
	}
}
