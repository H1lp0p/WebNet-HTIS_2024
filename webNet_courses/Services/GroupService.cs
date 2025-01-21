using Microsoft.EntityFrameworkCore;
using webNet_courses.Abstruct;
using webNet_courses.API.DTO;
using webNet_courses.API.Mappers;
using webNet_courses.Persistence;
using webNet_courses.Domain.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace webNet_courses.Services
{
	public class GroupService : IGroupService
	{
		private readonly CourseContext _context;

		public GroupService(CourseContext courseContext)
		{
			_context = courseContext;
		}

		public async Task<ICollection<CoursePreviewModel>> addCourse(Guid id, CreateCourseModel newCoures)
		{
			CampusGroup? group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
				throw new Exception("Not found");
            }

			if (group.Courses.FirstOrDefault(course => course.Name == newCoures.Name) != null)
			{
				throw new Exception("Course with this name already exists in this group");
			}

			CampusCourse newCourse = new CampusCourse
			{
				Name = newCoures.Name,
				StartYear = newCoures.StartYear,
				MaximumStidetsCount = newCoures.MaximumStidetsCount,
				Requirements = newCoures.Requirements,
				Annotations = newCoures.Annotations,
				Semester = newCoures.Semester,
				CreatedTime = DateTime.Now,
			};

			group.Courses.Add(newCourse);
			await _context.SaveChangesAsync();

			var result = new List<CoursePreviewModel>();
			var courses = group.Courses.ToList();
			courses.ForEach(el => result.Add(el.toPreview()));
            
			return result;
        }

		public async Task<ICollection<CoursePreviewModel>> courseList(Guid groupId)
		{
			CampusGroup? group = await _context.Groups.FindAsync(groupId);

            if (group == null)
            {
				throw new Exception("Not Found");
            }

			var courses = group.Courses.ToList();

			var result = new List<CoursePreviewModel>();

			courses.ForEach(el => el.toPreview());

			return result;
        }

		public async Task<CampusGroupModel> create(CUGroupModel model)
		{
			if (await _context.Groups.FirstOrDefaultAsync(el => el.Name == model.Name) == null)
			{
				throw new Exception("Group with this name already exists");
			}

			CampusGroup newGroup = new CampusGroup {  Name = model.Name };

			var group = await _context.Groups.AddAsync(newGroup);
			_context.SaveChanges();

			return group.Entity.toDTO();
		}

		public async Task<CampusGroupModel> update(Guid id, CUGroupModel model)
		{
			CampusGroup? group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
				throw new Exception("Not found");
            }
			group.Name = model.Name;
			await _context.SaveChangesAsync();

			return group.toDTO();
        }

		public async Task<bool> delete(Guid id)
		{
			CampusGroup? groupToDelete = await _context.Groups.FindAsync(id);
			if (groupToDelete != null)
			{
				_context.Groups.Remove(groupToDelete);
				await _context.SaveChangesAsync();
				return true;
			}
			throw new Exception("Not found");
		}

		public async Task<ICollection<CampusGroupModel>> groupList()
		{
			var list = await _context.Groups.ToListAsync();
			var result = new List<CampusGroupModel>();
			list.ForEach(el => result.Add(el.toDTO()));

			return result;
		}
	}
}
