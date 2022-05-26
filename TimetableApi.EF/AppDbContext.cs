using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApi.Converters.DateOnly;
using TimetableApi.EF.Models;

namespace TimetableApi.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>(builder =>
            {
                //builder.HasKey(a => new { a.ProjectId, a.EmployeeId });

                builder.Property(a => a.Date)
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            });
                
            modelBuilder.Entity<Project>(builder =>
            {
                builder.Property(p => p.StartDate)
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();

                builder.Property(p => p.EndDate)
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            });

            modelBuilder.Entity<Employee>()
                .Property(e => e.Birthday)
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();

            modelBuilder.Entity<ActivityType>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(a => a.Name)
                .IsUnique();
        }
    }
}
