using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

using webNet_courses.Domain.Entities;

namespace webNet_courses.Persistence
{
	public class CourseContext  : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public CourseContext(DbContextOptions<CourseContext> options) : base(options) { }
	}
}
