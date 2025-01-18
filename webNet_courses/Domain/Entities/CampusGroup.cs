using System.ComponentModel.DataAnnotations;

namespace webNet_courses.Domain.Entities
{
	public class CampusGroup
	{
		public Guid Id { get; set; }

		[Required]
		public string Name {  get; set; }

		public ICollection<CampusCourse> Courses { get; set; } = new List<CampusCourse>();
	}
}
