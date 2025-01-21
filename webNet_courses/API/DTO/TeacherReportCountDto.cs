namespace webNet_courses.API.DTO
{
	public class TeacherReportCountDto
	{
		public string Name { get; set; }
		public Guid Id { get; set; }
		public List<CampusGroupReportCountDto> CampusGroupReports { get; set; }
	}
}
