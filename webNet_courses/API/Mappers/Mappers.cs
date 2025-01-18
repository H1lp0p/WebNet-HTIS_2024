using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using webNet_courses.API.DTO;
using webNet_courses.Domain.Entities;

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
			return new CoursePreviewModel
			{
				Id = course.Id,
				Name = course.Name,
				StartYear = course.StartYear,
				MaximumStudentsCount = course.MaximumStidetsCount,
				ReaminingSlotsCount = course.MaximumStidetsCount - course.Students.Count,
				Status = course.Status,
				Semester = course.Semester
			};
		}
	}
}
