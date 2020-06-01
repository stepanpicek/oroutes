using Microsoft.EntityFrameworkCore;
using OBPostupy.Models;
using OBPostupy.Models.Races;
using OBPostupy.Models.Courses;
using OBPostupy.Models.Maps;
using OBPostupy.Models.Results;
using OBPostupy.Models.People;
using OBPostupy.Models.GPS;

namespace OBPostupy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
            
        }

        public DbSet<Race> Races { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Control> Controls { get; set; }
        public DbSet<CourseControl> CourseControls { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<Split> Splits { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PersonResult> PersonResults { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<SplitTime> SplitTimes { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Path> Paths { get; set; }
        public DbSet<Location> Locations { get; set; } 
        public DbSet<CourseSplit> CourseSplits { get; set; }
        public DbSet<CourseData> CourseData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if ( modelBuilder != null)
            {
                modelBuilder.Entity<Split>()
                    .HasOne(s => s.FirstControl)
                    .WithMany(c => c.SplitsFirstControl)
                    .HasForeignKey(s => s.FirstControlID)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Split>()
                    .HasOne(s => s.SecondControl)
                    .WithMany(c => c.SplitsSecondControl)
                    .HasForeignKey(s => s.SecondControlID)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<SplitTime>()
                    .HasOne(st => st.Split)
                    .WithMany(s => s.SplitTimes)
                    .HasForeignKey(st => st.SplitID)
                    .OnDelete(DeleteBehavior.SetNull);

                modelBuilder.Entity<Category>()
                    .HasOne(c => c.Course)
                    .WithMany(c => c.Categories)
                    .HasForeignKey(c => c.CourseID)
                    .OnDelete(DeleteBehavior.SetNull);

                modelBuilder.Entity<CourseControl>()
                .HasKey(cc => new { cc.CourseID, cc.ControlID });

                modelBuilder.Entity<CourseSplit>()
                .HasKey(cs => new { cs.CourseID, cs.SplitID });                

                modelBuilder.Entity<Person>()
                    .HasIndex(p => p.RegNumber);

                modelBuilder.Entity<PersonResult>()
                    .HasOne(pr => pr.Race)
                    .WithMany(r => r.PersonResults)
                    .HasForeignKey(pr => pr.RaceID)
                    .OnDelete(DeleteBehavior.Restrict);
            }

        }
    }
}
