using Microsoft.EntityFrameworkCore;
using MiniProject.Models;

namespace MiniProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Any additional configuration, but we used attributes generally.
            // We can enforce Schema here as well if needed, but [Table] handles it.
        }
    }
}
