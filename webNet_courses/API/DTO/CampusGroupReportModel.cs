namespace webNet_courses.API.DTO
{
	public class CampusGroupReportModel
	{
		public string Name { get; set; }
		public Guid Id { get; set; }
		public double AveragePassed { get; set; }
		public double AverageFailed { get; set; }
	}
}
