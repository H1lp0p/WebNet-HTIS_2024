using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.API.Mappers;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Excpetions;
using webNet_courses.Persistence;

namespace webNet_courses.API.Controllers
{
	[Route("")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ProducesResponseType(typeof(Response), 500)]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IUserService _userService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly RoleManager<IdentityRole<Guid>> _roleManager;

		public AccountController(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			IUserService userService,
			RoleManager<IdentityRole<Guid>> roleManager,
			IHttpContextAccessor httpContextAccessor
			)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_userService = userService;
			_roleManager = roleManager;
			_httpContextAccessor = httpContextAccessor;
		}

		///<summary>Register new user</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="400">BadRequest</responce>>
		[HttpPost]
		[AllowAnonymous]
		[Route("/registration")]
		public async Task<ActionResult<TokenDto>> Register(UserRegisterModel register)
		{
			if (ModelState.IsValid)
			{
				if (register.Password != register.ConfirmPassword)
				{
					throw new BLException("password and it's confirmations doesn't match");
				}

				User newUser = new User
				{
					UserName = register.FullName,
					FullName = register.FullName,
					Email = register.Email,
					BirthDate = register.BirthDate
				};

				var result = await _userManager.CreateAsync(newUser, register.Password);
				if (!result.Succeeded)
				{
					throw new Exception(result.ToString());
				}
				var nowUser = await _userManager.FindByEmailAsync(register.Email);
				var loginResult = await _signInManager.PasswordSignInAsync(nowUser, register.Password, true, false);

				if (!loginResult.Succeeded)
				{
					throw new Exception(loginResult.ToString());
				}

				var jwt = await _userService.GenerateToken(newUser);
				return Ok(new TokenDto { token = jwt });
			}
			return BadRequest(ModelState);
		}

		///<summary>Login</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="400">BadRequest</responce>>
		[HttpPost]
		[Route("/login")]
		[AllowAnonymous]
		public async Task<ActionResult<TokenDto>> Login(UserLoginModel login)
		{
			if (ModelState.IsValid)
			{
				User? user = await _userManager.FindByEmailAsync(login.Email);
				if (user == null)
				{
					throw new BLException("User not found");
				}

				var loginResult = await _signInManager.PasswordSignInAsync(user, login.Password, true, false);

				var jwt = await _userService.GenerateToken(user);

				return Ok(new TokenDto { token = jwt});
			}
			return BadRequest(ModelState);
		}

		//TODO: 
		///<summary>Logout</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("logout")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return Ok();
		}

		///<summary>Edit user's data</summary>
		/// <responce code="200">Succeded</responce>>
		/// <response code="400">BadRequest</response>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPut]
		[Route("/profile")]
		public async Task<ActionResult<ProfileModel>> Edit(UserEditModel edit)
		{
			if (ModelState.IsValid)
			{
				User user = (await _userManager.GetUserAsync(User))!;
				bool editRes = await _userService.edit(user.Id, edit.FullName, edit.BirthDate);
				user = (await _userManager.GetUserAsync(User))!;
				return Ok(user.Profile());
			}
			return BadRequest(ModelState);
		}

		///<summary>Get user's profile</summary>
		/// <responce code="200">Succeded</responce>>
		/// <response code="400">BadRequest</response>
		/// <responce code="401">Unauthorized</responce>>
		[HttpGet]
		[Route("/profile")]
		public async Task<ActionResult<ProfileModel>> Profile()
		{
			User user = (await _userManager.GetUserAsync(User))!;
			return user.Profile();
		}

		///<summary>Set user as admin [for dev]</summary>
		/// <responce code="200">Succeded</responce>>
		/// <responce code="400">BadRequest</responce>>
		/// <responce code="401">Unauthorized</responce>>
		[HttpPost]
		[Route("/setAdmin")]
		public async Task<IActionResult> SetAdmin()
		{
			User user = (await _userManager.GetUserAsync(User))!;

			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
			}

			var result = await _userManager.AddToRoleAsync(user, "Admin");
			if (result.Succeeded)
			{
				return Ok();
			}

			return BadRequest(result.ToString());
		}
	}
}
