using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;

namespace webNet_courses.API.Mappers
{
	public static class Mappers
	{
		public static ProfileModel Profile(this User user)
		{
			return new ProfileModel
			{
				FullName = user.FullName,
				Email = user.Email,
				BirthDate = user.BirthDate,
			};
		}

		public static CampusGroupModel toDTO(this CampusGroup group)
		{
			return new CampusGroupModel
			{
				Id = group.Id,
				Name = group.Name
			};
		}

		public static CoursePreviewModel toPreview(this CampusCourse course)
		{
			int remaining = course.MaximumStidetsCount -
				course.Students.ToList().FindAll(s => s.StudentStatus == StudentStatuses.Accepted).Count;

			return new CoursePreviewModel
			{
				Id = course.Id,
				Name = course.Name,
				StartYear = course.StartYear,
				MaximumStudentsCount = course.MaximumStidetsCount,
				ReaminingSlotsCount = remaining,
				Status = course.Status,
				Semester = course.Semester
			};
		}

		public static UserShortDto toShortDto(this User user)
		{
			return new UserShortDto
			{
				Id = user.Id,
				FullName = user.FullName
			};
		}

		public static UserRolesDto toRolesDto(this User user, bool isAdmin)
		{
			return new UserRolesDto
			{
				IsAdmin = isAdmin,
				IsStudent = user.LearningCourses.Count > 0,
				IsTeacher = user.TeachingCourses.Count > 0
			};
		}

		public static CampusCourseStudentDto toDto(this CampusCourseStudent courseStudent)
		{
			return new CampusCourseStudentDto
			{
				Id = courseStudent.User.Id,
				Name = courseStudent.User.FullName,
				Email = courseStudent.User.Email,
				StudentStatus = courseStudent.StudentStatus,
				MidTermResult = courseStudent.MidtermResult,
				FinalResult = courseStudent.FinalResult
			};
		}

		public static CampusCourseTeacherDto toDto(this CampusCourseTeacher courseTeacher)
		{
			return new CampusCourseTeacherDto
			{
				Email = courseTeacher.User.Email,
				Name = courseTeacher.User.FullName,
				IsMain = courseTeacher.isMain
			};
		}

		public static NotificationDto toDto(this Notification notification)
		{
			return new NotificationDto
			{
				Text = notification.Text,
				IsImportant = notification.IsImportant
			};
		}

		public static CampusCourseDetailsModel getDetails(
			this CampusCourse course,
			courseDetailsPermission permission, 
			Guid? nowStudentId = null)
		{
			var teachersDtoList = new List<CampusCourseTeacherDto>();
			course.Teachers.ToList().ForEach(t => teachersDtoList.Add(t.toDto()));

			var studentsDtoList = new List<CampusCourseStudentDto>();
			course.Students.ToList().ForEach(s => studentsDtoList.Add(s.toDto()));

			switch (permission)
			{
				case courseDetailsPermission.standart:
					foreach (var student in studentsDtoList)
					{
						student.FinalResult = StudentMarks.NotDefined;
						student.MidTermResult = StudentMarks.NotDefined;
					}
					studentsDtoList = studentsDtoList
						.Where(s => s.StudentStatus == StudentStatuses.Accepted)
						.ToList();
					break;
				case courseDetailsPermission.courseStudent:
					if (nowStudentId == null)
					{
						throw new Exception("Id of current student is null");
					}
					foreach (var student in studentsDtoList)
					{
						if (student.Id != nowStudentId)
						{
							student.FinalResult = StudentMarks.NotDefined;
							student.MidTermResult = StudentMarks.NotDefined;
						}
					}
					studentsDtoList = studentsDtoList
						.Where(s => s.StudentStatus == StudentStatuses.Accepted)
						.ToList();
					break;
				default:

					break;
			}

			var notificationsDtoList = new List<NotificationDto>();
			course.Notifications.ToList().ForEach(n => notificationsDtoList.Add(n.toDto()));

			return new CampusCourseDetailsModel
			{
				Id = course.Id,
				Name = course.Name,
				StartYear = course.StartYear,
				MaximumStidetsCount = course.MaximumStidetsCount,
				Requirements = course.Requirements,
				Annotations = course.Annotations,
				Semester = course.Semester,
				Status = course.Status,
				Teachers = teachersDtoList,
				Students = studentsDtoList,
				Notifications = notificationsDtoList
			};
		}

		public static CampusGroupReportModel toModel(this CampusGroupReportCountDto count)
		{
			return new CampusGroupReportModel
			{
				Id = count.Id,
				Name = count.Name,
				AveragePassed = count.AllStudentsCount != 0 ? ((double)count.Passed / count.AllStudentsCount) : 0.0,
				AverageFailed = count.AllStudentsCount != 0 ? ((double)count.Failed / count.AllStudentsCount) : 0.0
			};
		}

		public static TeacherReportRecordModel toModel(this TeacherReportCountDto count)
		{
			var groupModelList = new List<CampusGroupReportModel>();
			count.CampusGroupReports.ForEach(el => groupModelList.Add(el.toModel()));

			return new TeacherReportRecordModel 
			{
				Id = count.Id,
				FullName = count.Name,
				CampusGroupReports = groupModelList
			};
		}
	}
}
