using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Enumerations;
using webNet_courses.Persistence;

namespace webNet_courses.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
	[ProducesResponseType(typeof(Response), 500)]
	public class ReportController : ControllerBase
	{
		private readonly IReportsService _reportsService;

		public ReportController(IReportsService reportsService)
		{
			_reportsService = reportsService;
		}


		///<summary>Get report</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpGet]
		[Route("")]
		public async Task<ActionResult<ICollection<TeacherReportRecordModel>>> getReport
			(
			[FromQuery] Semester? semester,
			[FromQuery] List<Guid> campusGroupIds
			)
		{
			return Ok(await _reportsService.getReport(semester, campusGroupIds));
		}

	}
}
