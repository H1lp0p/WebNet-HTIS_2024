using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.API.Mappers;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;
using webNet_courses.Domain.Excpetions;
using webNet_courses.Persistence;

namespace webNet_courses.Services
{
	public class CourseService : ICoursesServise
	{
		private readonly CourseContext _context;
		private readonly UserManager<User> _userManager;
		private readonly IGroupService _groupService;

		public CourseService(
			CourseContext context,
			UserManager<User> userManager,
			IGroupService groupService
			)
		{
			_context = context;
			_userManager = userManager;
			_groupService = groupService;
		}

		public async Task<CampusCourseDetailsModel> addNotification(Guid courseId, AddNotificationModel newNotification, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync( courseId );
			if ( course == null )
			{
				throw new FileNotFoundException("Course not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new BLException("You can't add notifications to this course");
			}

			Notification newNot = new Notification
			{
				Text = newNotification.Text,
				IsImportant = newNotification.IsImportant
			};

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);

		}

		public async Task<CampusCourseDetailsModel> addTeacher(Guid courseId, AddTeacherModel newTeacher, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			User? teacher = await _userManager.FindByIdAsync(newTeacher.UserId.ToString());

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			if (teacher == null)
			{
				throw new FileNotFoundException("User not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id && r.isMain))
			{
				throw new BLException("You can't add teachers");
			}

			bool isStudent = course.Students.Any(s => s.User.Id == teacher.Id);

			bool isAlreadyTeacher = course.Teachers.Any(t => t.User.Id == teacher.Id);

			if (isStudent) 
			{
				throw new BLException("User is student");
			}

			if (isAlreadyTeacher)
			{
				throw new BLException("User is already teacher");
			}

			CampusCourseTeacher newRelationship = new CampusCourseTeacher
			{
				User = teacher,
				Course = course,
				isMain = false
			};

			course.Teachers.Add(newRelationship);
			teacher.TeachingCourses.Add(newRelationship);
			await _context.SaveChangesAsync();

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);
		}

		public async Task<ICollection<CoursePreviewModel>> deleteCourse(Guid id)
		{
			CampusCourse? course = await _context.Courses.FindAsync(id);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}
			var delResult = _context.Courses.Remove(course);

			await _context.SaveChangesAsync();

			return await _groupService.courseList(course.CampusGroupId);
		}

		public async Task<CampusCourseDetailsModel> editCourse(Guid courseId, EditCampusCourseModel editData)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			course.Name = editData.Name;
			course.StartYear = editData.StartYear;
			course.MaximumStidetsCount = editData.MaximumStidetsCount;
			course.Requirements = editData.Requirements;
			course.Annotations = editData.Annotations;
			course.Semester = editData.Semester;

			await _context.SaveChangesAsync();

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);
			
		}

		public async Task<CampusCourseDetailsModel> editRA(Guid courseId, EditCourseRAModel edit, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new BLException("You can't edit requirements and annotations of this course");
			}

			course.Requirements = edit.Requirements;
			course.Annotations = edit.Annotations;

			await _context.SaveChangesAsync();

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);
		}

		public async Task<CampusCourseDetailsModel> editStudentMark(Guid courseId, Guid studentId, EditCourseStudentMarkModel mark,User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new BLException("You can't edit student's marks in this course");
			}

			CampusCourseStudent? relationship = course.Students.FirstOrDefault(r => r.User.Id == studentId);

			if (relationship == null)
			{
				throw new BLException("Can't find student in course");
			}

			if (relationship.StudentStatus == StudentStatuses.InQueue)
			{
				throw new BLException("Student currently in queue");
			}

			if (mark.MarkType == MarkType.Midterm)
			{
				relationship.MidtermResult = mark.Mark;
			}
			else
			{
				relationship.FinalResult = mark.Mark;
			}

			await _context.SaveChangesAsync();

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);
		}

		public async Task<CampusCourseDetailsModel> editStudentStatus(Guid studentId, Guid courseId, EditCourseStudentStatusModel newStatus, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new BLException("You can't edit student's statuses in this course");
			}

			CampusCourseStudent? relationship = course.Students.FirstOrDefault(r => r.User.Id == studentId);

			if (relationship == null)
			{
				throw new FileNotFoundException("Can't find student in course");
			}

			int remaining = course.MaximumStidetsCount -
				course.Students.ToList().FindAll(s => s.StudentStatus == StudentStatuses.Accepted).Count;

			if (newStatus.Status == StudentStatuses.Accepted)
			{
				if (relationship.StudentStatus == StudentStatuses.Declined)
				{
					throw new BLException("Student prewiously was declined");
				}
				else if (relationship.StudentStatus == StudentStatuses.Accepted)
				{
					return course.getDetails(courseDetailsPermission.TeacherOrAdmin);
				}

				if (remaining == 0)
				{
					throw new BLException("Course is fullfilled with students");
				}
			}
			else if (newStatus.Status == StudentStatuses.Declined)
			{
				if (relationship.StudentStatus == StudentStatuses.Accepted)
				{
					throw new BLException("Student prewiously was accepted");
				}
			}

			relationship.StudentStatus = newStatus.Status;
			await _context.SaveChangesAsync();

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);

		}

		public async Task<ICollection<CoursePreviewModel>> getCourses(string? search, bool? hasPlaceAndOpen, Semester? semester, CourseSorting? sorting, int page = 1, int pageSize = 10)
		{
			List<CoursePreviewModel> result = new List<CoursePreviewModel>();

			List<CampusCourse> list = [];

			list = await _context.Courses
				.Where(c => search == null || c.Name.IndexOf(search) == 0)
				.Where(c =>
					hasPlaceAndOpen != true ||
					(c.Status == CourseStatuses.OpenForAssignig && (c.Students.Where(
							el => el.StudentStatus == StudentStatuses.Accepted).Count() < c.MaximumStidetsCount)))
				.Where(c => semester == null || c.Semester == semester).ToListAsync();

			if (sorting != null)
			{
				if (sorting == CourseSorting.CreatedAsc)
				{
					list = list.OrderBy(c => c.CreatedTime).ToList();
				}
				else
				{
					list = list.OrderByDescending(c => c.CreatedTime).ToList();
				}
			}

			list = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			list.ForEach(el => result.Add(el.toPreview()));

			return result;
		}

		public async Task<CampusCourseDetailsModel> getDetails(Guid id, Guid curUserId)
		{
			CampusCourse? course = await _context.Courses.FindAsync(id);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			User nowUser = (await _userManager.FindByIdAsync(curUserId.ToString()))!;

			var isAdmin = await _userManager.IsInRoleAsync(nowUser, "Admin");
			
			var isTeacher = course.Teachers
				.Any(t => t.User.Id == curUserId);

			var isStudent = course.Students
				.Any(s => s.User.Id == curUserId && s.StudentStatus == StudentStatuses.Accepted);

			courseDetailsPermission permission = (
				isAdmin || isTeacher ? courseDetailsPermission.TeacherOrAdmin :
				(isStudent ? courseDetailsPermission.courseStudent :
						courseDetailsPermission.standart));

			return course.getDetails(permission, curUserId);
		}

		public async Task<ICollection<CoursePreviewModel>> myCourses(User user)
		{
			List<CoursePreviewModel> result = new List<CoursePreviewModel>();
			user.LearningCourses.ToList().ForEach(c => result.Add(c.Course.toPreview()));

			return result;
		}

		public async Task<CampusCourseDetailsModel> setStatus(Guid courseId, EditCourseStatusModel newStatus, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new BLException("You can't edit course status");
			}

			if (course.Status > newStatus.Status)
			{
				throw new BLException("Can't set course status to prewious one");
			}

			course.Status = newStatus.Status;
			await _context.SaveChangesAsync();

			return course.getDetails(courseDetailsPermission.TeacherOrAdmin);
		}

		public async Task<bool> signUp(Guid courseId, User user)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new FileNotFoundException("Course not found");
			}

			if (course.Status != CourseStatuses.OpenForAssignig)
			{
				throw new BLException("You can't sign up to the course that is not open for assigning");
			}

			bool isTeacher = course.Teachers.Any(t => t.User.Id == user.Id);
			bool isAlreadyStudent = course.Students.Any(s => s.User.Id == user.Id);

			int studentsCount = course.Students
				.Where(s => s.StudentStatus == StudentStatuses.Accepted).Count();

			if (studentsCount >= course.MaximumStidetsCount)
			{
				throw new BLException("Course is fullfilled with students");
			}

			if (isTeacher)
			{
				throw new BLException("User is teacher");
			}

			if (isAlreadyStudent)
			{
				throw new BLException("User is already student");
			}

			CampusCourseStudent newRelationship = new CampusCourseStudent
			{
				User = user,
				Course = course,
			};

			user.LearningCourses.Add(newRelationship);
			course.Students.Add(newRelationship);
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<ICollection<CoursePreviewModel>> teachingCourses(User user)
		{
			List<CoursePreviewModel> result = new List<CoursePreviewModel>();
			user.TeachingCourses.ToList().ForEach(c => result.Add(c.Course.toPreview()));

			return result;
		}
	}
}
