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


		///<summary>Get list of group's courses</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		public async Task<ActionResult<ICollection<CampusGroupModel>>> GroupList()
		{
			return Ok(await _groupService.groupList());
		}

		///<summary>Create group</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CampusGroupModel>> CreateGroup(CUGroupModel newGroup)
		{
			return Ok(await _groupService.create(newGroup));

		}

		///<summary>Edit group</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpPut]
		[Authorize(Roles = "Admin")]
		[Route("{id}")]
		public async Task<ActionResult<CampusGroupModel>> Edit([FromRoute] Guid id, CUGroupModel edit)
		{
			return Ok(await _groupService.update(id, edit));
		}

		///<summary>Delete group</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpDelete]
		[Authorize(Roles = "Admin")]
		[Route("{id}")]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			return Ok(await _groupService.delete(id));
		}

		///<summary>Get list of all group's courses</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		[Route("{id}")]
		public async Task<ActionResult<List<CoursePreviewModel>>> GetCourseList([FromRoute] Guid id)
		{
			return Ok(await _groupService.courseList(id));
		}


		///<summary>Create course</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		/// <responce code="403">Forbidden</responce>>
		[HttpPost]
		[Route("{groupId}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ICollection<CoursePreviewModel>>> CreateCourse([FromRoute] Guid groupId, CreateCourseModel newCourse)
		{
			return Ok(await _groupService.addCourse(groupId, newCourse));
		}
	}
}
