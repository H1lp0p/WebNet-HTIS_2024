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
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ProducesResponseType(typeof(Response), 500)]
	public class CourseController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		private readonly ICoursesServise _cousesServise;

		public CourseController(
			UserManager<User> userManager,
			RoleManager<IdentityRole> roleManager,
			ICoursesServise coursesService
			)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_cousesServise = coursesService;
		}

		[HttpGet]
		[Route("{id}/details")]
		public async Task<ActionResult<CampusCourseDetailsModel>> GetDetails([FromQuery] Guid id)
		{
			try
			{
				return Ok(await _cousesServise.getDetails(id));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("my")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> GetMy()
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(await _cousesServise.myCourses(nowUser));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("teaching")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> GetTeaching()
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(await _cousesServise.teachingCourses(nowUser));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

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
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(await _cousesServise.getCourses(
					search,
					hasPlaceAndOpen,
					semester,
					sorting,
					page,
					pageSize
					));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/sign-up")]
		public async Task<IActionResult> SignUp([FromQuery] Guid id)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				await _cousesServise.signUp(id, nowUser);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/status")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editStatus([FromQuery] Guid courseId, EditCourseStatusModel edit)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(await _cousesServise.setStatus(courseId, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
			}
			catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/student-status/{studentId}")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editStudentStatus([FromQuery] Guid id, [FromQuery] Guid studentId, EditCourseStudentStatusModel edit)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(_cousesServise.editStudentStatus(studentId, id, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/notifications")]
		public async Task<ActionResult<CampusCourseDetailsModel>> addNotification([FromQuery] Guid id, AddNotificationModel addNotif)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(_cousesServise.addNotification(id, addNotif, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/marks/{studentId}")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editStudentMark([FromQuery] Guid id, [FromQuery] Guid studentId, EditCourseStudentMarkModel edit)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(_cousesServise.editStudentMark(id, studentId,edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/notifications")]
		public async Task<ActionResult<CampusCourseDetailsModel>> AddTeacher([FromQuery] Guid id, AddTeacherModel addTeacher)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(_cousesServise.addTeacher(id, addTeacher, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut]
		[Route("{id}/requirements-and-annotations")]
		public async Task<ActionResult<CampusCourseDetailsModel>> addNotification([FromQuery] Guid id, EditCourseRAModel edit)
		{
			User? nowUser = await _userManager.GetUserAsync(User);
			if (nowUser == null)
			{
				return Unauthorized();
			}

			try
			{
				return Ok(_cousesServise.editRA(id, edit, nowUser, await _userManager.IsInRoleAsync(nowUser, "Admin")));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut]
		[Route("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CampusCourseDetailsModel>> editCourse([FromQuery] Guid id, EditCampusCourseModel edit)
		{
			try
			{
				return Ok(await _cousesServise.editCourse(id, edit));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete]
		[Route("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> deleteCourse([FromQuery] Guid id)
		{
			try
			{
				return Ok(_cousesServise.deleteCourse(id));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}
}
