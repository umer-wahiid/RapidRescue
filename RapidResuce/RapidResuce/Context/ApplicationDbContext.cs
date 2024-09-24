using Microsoft.EntityFrameworkCore;
using RapidResuce.Models;

namespace RapidResuce.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<FirstAid> FirstAids { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MedicalProfile> MedicalProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ambulance> Ambulances { get; set; }
        public DbSet<EmergencyRequest> EmergencyRequests{ get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Ambulance>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<EmergencyRequest>().HasQueryFilter(er => !er.IsDeleted);
            modelBuilder.Entity<Feedback>().HasQueryFilter(f => !f.IsDeleted);
            modelBuilder.Entity<Contact>().HasQueryFilter(c => !c.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }
    }
}
