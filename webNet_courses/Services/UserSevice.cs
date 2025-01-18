using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
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

		public Task<bool> addAdmin(User user)
		{
			throw new NotImplementedException();
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

		public string GenerateToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_config["JWT:key"]!);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity([
				new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				]),
				Issuer = _config["JWT:issuer"],
				Audience = _config["JWT:audience"],
				Expires = DateTime.UtcNow.AddHours(Int32.Parse(_config["JWT:expireTime"]!)),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
