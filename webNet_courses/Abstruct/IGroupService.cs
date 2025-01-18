using Microsoft.AspNetCore.Mvc;
using webNet_courses.API.DTO;

namespace webNet_courses.Abstruct
{
	public interface IGroupService
	{
		public Task<CampusGroupModel> create(CUGroupModel model);

		public Task<CampusGroupModel> update(Guid id, CUGroupModel model);

		public Task<bool> delete(Guid id);

		public Task<ICollection<CampusGroupModel>> groupList();

		public Task<ICollection<CoursePreviewModel>> courseList(Guid groupId);

		public Task<ICollection<CoursePreviewModel>> addCourse(Guid id, CreateCourseModel newCoures);
	}
}
