using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.API.Mappers;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;
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
				throw new Exception("Not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new Exception("You can't add notifications to this course");
			}

			Notification newNot = new Notification
			{
				Text = newNotification.Text,
				IsImportant = newNotification.IsImportant
			};

			course.Notifications.Add( newNot );
			await _context.SaveChangesAsync();

			return course.getDetails();

		}

		public async Task<CampusCourseDetailsModel> addTeacher(Guid courseId, AddTeacherModel newTeacher, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			User? teacher = await _userManager.FindByIdAsync(newTeacher.UserId.ToString());

			if (course == null)
			{
				throw new Exception("Course not found");
			}

			if (teacher == null)
			{
				throw new Exception("User not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id && r.isMain))
			{
				throw new Exception("You can't add teachers");
			}

			bool isStudent = course.Students.Any(s => s.User.Id == teacher.Id);

			if (isStudent) 
			{
				throw new Exception("User is student");
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

			return course.getDetails();
		}

		public async Task<ICollection<CoursePreviewModel>> deleteCourse(Guid id)
		{
			CampusCourse? course = await _context.Courses.FindAsync(id);

			if (course == null)
			{
				throw new Exception("Not found");
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
				throw new Exception("Not found");
			}

			course.Name = editData.Name;
			course.StartYear = editData.StartYear;
			course.MaximumStidetsCount = editData.MaximumStidetsCount;
			course.Requirements = editData.Requirements;
			course.Annotations = editData.Annotations;
			course.Semester = editData.Semester;

			await _context.SaveChangesAsync();

			return course.getDetails();
			
		}

		public async Task<CampusCourseDetailsModel> editRA(Guid courseId, EditCourseRAModel edit, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new Exception("Not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new Exception("You can't edit requirements and annotations of this course");
			}

			course.Requirements = edit.Requirements;
			course.Annotations = edit.Annotations;

			await _context.SaveChangesAsync();

			return course.getDetails();
		}

		public async Task<CampusCourseDetailsModel> editStudentMark(Guid courseId, Guid studentId, EditCourseStudentMarkModel mark,User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new Exception("Not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new Exception("You can't edit student's marks in this course");
			}

			CampusCourseStudent? relationship = course.Students.FirstOrDefault(r => r.User.Id == studentId);

			if (relationship == null)
			{
				throw new Exception("Can't find student in course");
			}

			if (relationship.StudentStatus == StudentStatuses.InQueue)
			{
				throw new Exception("Student currently in queue");
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

			return course.getDetails();
		}

		public async Task<CampusCourseDetailsModel> editStudentStatus(Guid studentId, Guid courseId, EditCourseStudentStatusModel newStatus, User currentUser, bool isAdmin)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new Exception("Not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new Exception("You can't edit student's statuses in this course");
			}

			CampusCourseStudent? relationship = course.Students.FirstOrDefault(r => r.User.Id == studentId);

			if (relationship == null)
			{
				throw new Exception("Can't find student in course");
			}

			int remaining = course.MaximumStidetsCount -
				course.Students.ToList().FindAll(s => s.StudentStatus == StudentStatuses.Accepted).Count;

			if (newStatus.Status == StudentStatuses.Accepted)
			{
				if (relationship.StudentStatus == StudentStatuses.Declined)
				{
					throw new Exception("Student prewiously was declined");
				}
				else if (relationship.StudentStatus == StudentStatuses.Accepted)
				{
					return course.getDetails();
				}

				if (remaining == 0)
				{
					throw new Exception("Course is fullfilled with students");
				}
			}
			else if (newStatus.Status == StudentStatuses.Declined)
			{
				if (relationship.StudentStatus == StudentStatuses.Accepted)
				{
					throw new Exception("Student prewiously was accepted");
				}
			}

			relationship.StudentStatus = newStatus.Status;
			await _context.SaveChangesAsync();

			return course.getDetails();

		}

		public async Task<ICollection<CoursePreviewModel>> getCourses(string? search, bool? hasPlaceAndOpen, Semester? semester, CourseSorting? sorting, int page = 1, int pageSize = 10)
		{
			List<CoursePreviewModel> result = new List<CoursePreviewModel>();

			List<CampusCourse> list = [];

			list = await _context.Courses
				.Where(c => search == null || c.Name.Contains(search))
				.Where(c =>
					hasPlaceAndOpen == null ||
					(c.Status == CourseStatuses.OpenForAssignig && (c.Students.Where(
							el => el.StudentStatus == StudentStatuses.Accepted).Count() < c.MaximumStidetsCount)))
				.Where(c => semester == null || c.Semester == semester).ToListAsync();

			if (sorting != null)
			{
				if (sorting == CourseSorting.CreatedAsc)
				{
					list = list.OrderBy(c => c.CreatedTime).Skip((page - 1) * pageSize).Take(page).ToList();
				}
				else
				{
					list = list.OrderByDescending(c => c.CreatedTime).Skip((page - 1) * pageSize).Take(page).ToList();
				}
			}
			else
			{
				list = list.Skip((page - 1) * pageSize).Take(page).ToList();
			}

			list.ForEach(el => result.Add(el.toPreview()));

			return result;
		}

		public async Task<CampusCourseDetailsModel> getDetails(Guid id)
		{
			CampusCourse? course = await _context.Courses.FindAsync(id);

			if (course == null)
			{
				throw new Exception("Not found");
			}

			return course.getDetails();
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
				throw new Exception("Not found");
			}

			if (!isAdmin && !course.Teachers.Any(r => r.User.Id == currentUser.Id))
			{
				throw new Exception("You can't edit course status");
			}

			if (course.Status > newStatus.Status)
			{
				throw new Exception("Can't set course status to prewious one");
			}

			course.Status = newStatus.Status;
			await _context.SaveChangesAsync();

			return course.getDetails();
		}

		public async Task<bool> signUp(Guid courseId, User user)
		{
			CampusCourse? course = await _context.Courses.FindAsync(courseId);

			if (course == null)
			{
				throw new Exception("Not found");
			}

			bool isTeacher = course.Teachers.Any(t => t.User.Id == user.Id);

			if (isTeacher)
			{
				throw new Exception("User is teacher");
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
