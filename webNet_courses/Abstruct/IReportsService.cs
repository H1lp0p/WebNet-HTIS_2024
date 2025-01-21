using System.Net;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.Abstruct
{
	public interface IReportsService
	{
		public Task<ICollection<TeacherReportRecordModel>> getReport(Semester? semester, List<Guid> campusGroupId);
	}
}
