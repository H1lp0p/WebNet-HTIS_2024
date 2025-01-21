using webNet_courses.Domain.Entities;
using webNet_courses.API.DTO;

namespace webNet_courses.Abstruct
{
	public interface IUserService
	{
		public Task<bool> edit(
			Guid userId,
			string newFullName,
			DateTime newBirthDate
			);

		public string GenerateToken(User user);

		public Task<ICollection<UserShortDto>> GetUserList();

		public Task<UserRolesDto> getRoles(Guid id);


	}
}
