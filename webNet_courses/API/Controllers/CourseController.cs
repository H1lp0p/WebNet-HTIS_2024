using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.Controllers
{
	[Route("courses")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ProducesResponseType(typeof(Response), 500)]
	public class CourseController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole<Guid>> _roleManager;

		private readonly ICoursesServise _cousesServise;

		public CourseController(
			UserManager<User> userManager,
			RoleManager<IdentityRole<Guid>> roleManager,
			ICoursesServise coursesService
			)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_cousesServise = coursesService;
		}

		///<summary>Get course details</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		[Route("{id}/details")]
		public async Task<ActionResult<CampusCourseDetailsModel>> GetDetails([FromRoute] Guid id)
		{
			var curUser = await _userManager.GetUserAsync(User);
			return Ok(await _cousesServise.getDetails(id, curUser!.Id));
		}

		///<summary>Get list of user's courses</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		[Route("my")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> GetMy()
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.myCourses(nowUser));
		}

		///<summary>Get list of teaching courses</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		[Route("teaching")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> GetTeaching()
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;
			return Ok(await _cousesServise.teachingCourses(nowUser));
		}

		///<summary>Get list of all courses</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		[Route("list")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> GetList(
			[FromQuery] string? search,
			[FromQuery] bool? hasPlaceAndOpen,
			[FromQuery] Semester? semester,
			[FromQuery] CourseSorting? sorting,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 10
			)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;
			return Ok(await _cousesServise.getCourses(
					search,
					hasPlaceAndOpen,
					semester,
					sorting,
					page,
					pageSize
					));
		}

		///<summary>Sign-up to the course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("{id}/sign-up")]
		public async Task<IActionResult> SignUp([FromRoute] Guid id)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			await _cousesServise.signUp(id, nowUser);
			return Ok();
		}

		///<summary>Edit status of the course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpPost]
		[Route("{courseId}/status")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editStatus([FromRoute] Guid courseId, EditCourseStatusModel edit)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.setStatus(courseId, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
		}

		///<summary>Edit student's status</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("{id}/student-status/{studentId}")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editStudentStatus([FromRoute] Guid id, [FromRoute] Guid studentId, EditCourseStudentStatusModel edit)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.editStudentStatus(studentId, id, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
		}

		///<summary>Add notification to the course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("{id}/notifications")]
		public async Task<ActionResult<CampusCourseDetailsModel>> addNotification([FromRoute] Guid id, AddNotificationModel addNotif)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.addNotification(id, addNotif, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
		}

		///<summary>Edit student's mark</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("{id}/marks/{studentId}")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editStudentMark([FromRoute] Guid id, [FromRoute] Guid studentId, EditCourseStudentMarkModel edit)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.editStudentMark(id, studentId, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
		}

		///<summary>Add teacher to the course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("{id}/teachers")]
		public async Task<ActionResult<CampusCourseDetailsModel>> AddTeacher([FromRoute] Guid id, AddTeacherModel addTeacher)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.addTeacher(id, addTeacher, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
		}

		///<summary>Edit requirements and annotations</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPut]
		[Route("{id}/requirements-and-annotations")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editRA([FromRoute] Guid id, EditCourseRAModel edit)
		{
			User nowUser = (await _userManager.GetUserAsync(User))!;

			return Ok(await _cousesServise.editRA(id, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
		}

		///<summary>Edit course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpPut]
		[Route("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editCourse([FromRoute] Guid id, EditCampusCourseModel edit)
		{
			return Ok(await _cousesServise.editCourse(id, edit));
		}

		///<summary>Delete course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpDelete]
		[Route("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> deleteCourse([FromRoute] Guid id)
		{
			try
			{
				return Ok(await _cousesServise.deleteCourse(id));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}
}
