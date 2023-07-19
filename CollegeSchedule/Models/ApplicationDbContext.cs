using Microsoft.EntityFrameworkCore;

namespace CollegeSchedule.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<PracticeSchedule> PracticeSchedules { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ExamsSchedule> ExamsSchedules { get; set; }
        public DbSet<ConsultationSchedule> ConsultationsSchedules { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, userName = "Admin", password = "Adm1n$Pa$$2018+-" });
        }
    }
}
