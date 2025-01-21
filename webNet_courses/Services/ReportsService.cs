using Microsoft.EntityFrameworkCore;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.API.Mappers;
using webNet_courses.Domain.Entities;
using webNet_courses.Domain.Enumerations;
using webNet_courses.Persistence;

namespace webNet_courses.Services
{
	public class ReportsService : IReportsService
	{
		private readonly CourseContext _context;

		public ReportsService(CourseContext context)
		{
			_context = context;
		}

		public async Task<ICollection<TeacherReportRecordModel>> getReport(Semester? semester, List<Guid> campusGroupId)
		{
			List<CampusGroup> groups = await _context.Groups
				.Where(g => campusGroupId.Count == 0 || campusGroupId.Contains(g.Id)).ToListAsync();

			List<TeacherReportCountDto> result = [];

			foreach (var group in groups)
			{
				var courses = group.Courses.Where(c => semester == null || c.Semester == semester).ToList();

				foreach (var course in courses)
				{
					var students = course.Students.Where(r => r.StudentStatus == StudentStatuses.Accepted).ToList();

					int passed = students.Where(r => r.FinalResult == StudentMarks.Passsed).Count();
					int failed = students.Where(r => r.FinalResult == StudentMarks.Failed).Count();
					int studentsCount = students.Count;

					var mainTeacher = course.Teachers.Where(t => t.isMain).First();

					var teacherToUpdate = result.FirstOrDefault(r => r.Id == mainTeacher.User.Id);
					if (teacherToUpdate != null)
					{
						var groupToUpdate = teacherToUpdate.CampusGroupReports.First(g => g.Id == group.Id);
						if (groupToUpdate != null) 
						{
							groupToUpdate.Failed += failed;
							groupToUpdate.Passed += passed;
							groupToUpdate.AllStudentsCount += studentsCount;
						}
						else
						{
							teacherToUpdate.CampusGroupReports.Add(new CampusGroupReportCountDto
							{
								Id = group.Id,
								Name = group.Name,
								Passed = passed,
								Failed = failed,
								AllStudentsCount = studentsCount
							});
						}
					}
					else
					{
						result.Add(new TeacherReportCountDto
						{
							Id = mainTeacher.User.Id,
							Name = mainTeacher.User.FullName,
							CampusGroupReports = new List<CampusGroupReportCountDto> { new CampusGroupReportCountDto
							{
								Id = group.Id,
								Name = group.Name,
								Passed = passed,
								Failed = failed,
								AllStudentsCount = studentsCount
							} }
						});
					}
				}
			}

			List<TeacherReportRecordModel> finalResult = [];
			result.ForEach(el => finalResult.Add(el.toModel()));

			return finalResult;
		}
	}
}
