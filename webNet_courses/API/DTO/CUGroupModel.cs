using System.ComponentModel.DataAnnotations;

namespace webNet_courses.API.DTO
{
	public class CUGroupModel
	{
		[Required]
		[MinLength(1)]
		public string Name { get; set; }
	}
}
