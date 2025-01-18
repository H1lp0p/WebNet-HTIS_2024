using webNet_courses.Domain.Entities;

namespace webNet_courses.Abstruct
{
	public interface IUserService
	{
		public Task<bool> edit(
			Guid userId,
			string newFullName,
			DateTime newBirthDate
			);

		public Task<bool> addAdmin(User user);

		public string GenerateToken(User user);
	}
}
