using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using webNet_courses.Domain.Entities;

namespace webNet_courses.Persistence
{

	public class CourseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public CourseContext(DbContextOptions<CourseContext> options) : base(options) { }

		public DbSet<User> User {  get; set; }
		public DbSet<CampusCourse> Courses { get; set; }
		public DbSet<CampusGroup> Groups { get; set; }
		public DbSet<CampusCourseStudent> Students { get; set; }
		public DbSet<CampusCourseTeacher> Teachers { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}

	
}
