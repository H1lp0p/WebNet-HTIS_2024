using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Entities;
using webNet_courses.Persistence;

namespace webNet_courses.API.Controllers
{
	
	[Route("")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ProducesResponseType(typeof(Response), 500)]
	public class UserController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IUserService _userService;

		private readonly RoleManager<IdentityRole<Guid>> _roleManager;

		public UserController(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			IUserService userService,
			RoleManager<IdentityRole<Guid>> roleManager
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_userService = userService;
			_roleManager = roleManager;
		}

		///<summary>Get list of all users</summary>
		[HttpGet]
		[Route("/users")]
		public async Task<ActionResult<ICollection<UserShortDto>>> GetUsers()
		{
			try
			{
				return Ok(await _userService.GetUserList());
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("/roles")]
		public async Task<ActionResult<UserRolesDto>> GetRoles()
		{
			User? user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return Unauthorized();
			}

			return Ok(await _userService.getRoles(user.Id));
		}
	}
}
