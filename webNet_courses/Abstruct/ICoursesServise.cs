using webNet_courses.API.DTO;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.Abstruct
{
	public interface ICoursesServise
	{
		public Task<ICollection<CoursePreviewModel>> deleteCourse(Guid id);

		public Task<CampusCourseDetailsModel> editCourse(Guid courseId, EditCampusCourseModel editData);

		public Task<CampusCourseDetailsModel> getDetails(Guid id);

		public Task<bool> signUp (Guid courseId, User user);

		public Task<CampusCourseDetailsModel> setStatus(Guid courseId, EditCourseStatusModel newStatus, User currentUser, bool isAdmin);

		public Task<CampusCourseDetailsModel> editStudentStatus(Guid studentId, Guid courseId, EditCourseStudentStatusModel newStatus, User currentUser, bool isAdmin);

		public Task<CampusCourseDetailsModel> addNotification(Guid courseId, AddNotificationModel newNotification, User currentUser, bool isAdmin);

		public Task<CampusCourseDetailsModel> editStudentMark(Guid courseId, Guid studentId, EditCourseStudentMarkModel mark, User currentUser, bool isAdmin);

		public Task<CampusCourseDetailsModel> editRA(Guid courseId, EditCourseRAModel edit, User currentUser, bool isAdmin);

		public Task<CampusCourseDetailsModel> addTeacher(Guid courseId, AddTeacherModel newTeacher,User currentUser, bool isAdmin);

		public Task<ICollection<CoursePreviewModel>> myCourses(User user);

		public Task<ICollection<CoursePreviewModel>> teachingCourses(User user);

		public Task<ICollection<CoursePreviewModel>> getCourses(
			string? search,
			bool? hasPlaceAndOpen,
			Semester? semester,
			CourseSorting? sorting,
			int page = 1,
			int pageSize = 10
			);
	}
}
