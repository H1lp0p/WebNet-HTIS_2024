namespace webNet_courses.API.DTO
{
	public class CampusGroupReportCountDto
	{
		public string Name { get; set; }
		public Guid Id { get; set; }
		public int Passed { get; set; }
		public int Failed { get; set; }
		public int AllStudentsCount { get; set; }
	}
}
