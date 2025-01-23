using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.API.Mappers;
using webNet_courses.Domain.Entities;
using webNet_courses.Persistence;

namespace webNet_courses.Services
{
	public class UserSevice : IUserService
	{
		private readonly CourseContext _context;
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		private readonly IHttpContextAccessor _httpcontextAccessor;

		public UserSevice(
			CourseContext context,
			IConfiguration config,
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			_context = context;
			_config = config;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<bool> edit(Guid userId, string newFullName, DateTime newBirthDate)
		{
			User? user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
			{
				return false;
			}

			user.FullName = newFullName;
			user.BirthDate = newBirthDate;

			var editResult = await _userManager.UpdateAsync(user);

			return editResult.Succeeded;
		}

		public async Task<string> GenerateToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var key = Encoding.UTF8.GetBytes(_config["JWT:key"]!);

			List<Claim> claimsList = [];

			var roles = await _userManager.GetRolesAsync(user);

			claimsList.Add(new Claim(ClaimTypes.Email, user.Email!));
			claimsList.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

			foreach (var role in roles)
			{
				claimsList.Add(new Claim(ClaimTypes.Role, role));
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claimsList),
				Issuer = _config["JWT:issuer"],
				Audience = _config["JWT:audience"],
				Expires = DateTime.UtcNow.AddHours(Int32.Parse(_config["JWT:expireTime"]!)),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public async Task<ICollection<UserShortDto>> GetUserList()
		{
			var result = new List<UserShortDto>();
			await _userManager.Users.ForEachAsync(el => result.Add(el.toShortDto()));
			return result;
		}

		public async Task<UserRolesDto> getRoles(Guid id)
		{
			User? user = await _userManager.FindByIdAsync(id.ToString());
			if (user == null)
			{
				throw new Exception("Not found");
			}
			var result = user.toRolesDto(await _userManager.IsInRoleAsync(user, "Admin"));
			return result; 
		}
	}
}
