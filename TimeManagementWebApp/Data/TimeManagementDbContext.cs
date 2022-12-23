using Microsoft.EntityFrameworkCore;
using TimeManagementClassLibrary;

namespace TimeManagementWebApp.Data
{
    public class TimeManagementDbContext : DbContext
    {
        public TimeManagementDbContext(DbContextOptions options) : base(options)
        {
        }

        //Maybe put models in class library?
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Module> Modules { get; set; } = null!;
        public DbSet<StudyHours> StudyHours { get; set; } = null!;
    }
}
