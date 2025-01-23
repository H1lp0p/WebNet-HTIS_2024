using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Entities;

namespace webNet_courses.API.Controllers
{
	[Route("/groups")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ProducesResponseType(typeof(Response), 500)]
	public class GroupController : Controller
	{
		private readonly IGroupService _groupService;

		public GroupController(
			IGroupService groupService
			)
		{
			_groupService = groupService;
		}

		[HttpGet]
		public async Task<ActionResult<ICollection<CampusGroupModel>>> GroupList()
		{
			try
			{
				return Ok(await _groupService.groupList());
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CampusGroupModel>> CreateGroup(CUGroupModel newGroup)
		{
			try
			{
				return Ok(await _groupService.create(newGroup));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
			
		}

		[HttpPut]
		[Authorize(Roles = "Admin")]
		[Route("{id}")]
		public async Task<ActionResult<CampusGroupModel>> Edit([FromRoute] Guid id, CUGroupModel edit)
		{
			try
			{
				return Ok(await _groupService.update(id, edit));
			}
			catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete]
		[Authorize(Roles = "Admin")]
		[Route("{id}")]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			try
			{
				return Ok(await _groupService.delete(id));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<ActionResult<List<CoursePreviewModel>>> GetCourseList([FromRoute] Guid id)
		{
			try
			{
				return Ok(await _groupService.courseList(id));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{groupId}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> CreateCourse([FromRoute] Guid groupId, CreateCourseModel newCourse)
		{
			return Ok(await _groupService.addCourse(groupId, newCourse));

			/*try
			{
				
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}*/
		}
	}
}
