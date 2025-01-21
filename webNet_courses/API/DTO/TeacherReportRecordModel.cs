namespace webNet_courses.API.DTO
{
	public class TeacherReportRecordModel
	{
		public string FullName { get; set; }
		public Guid Id { get; set; }
		public List<CampusGroupReportModel> CampusGroupReports { get; set; }
	}
}
