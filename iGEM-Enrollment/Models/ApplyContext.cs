using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace iGEM_Enrollment.Models
{
    public class ApplyContext : DbContext
    {
        public ApplyContext(DbContextOptions<ApplyContext> options) : base(options)
        {
        }

        public DbSet<AppliForm> AppliForm { get; set; }
        public DbSet<Applicant> Applicant { get; set; }
        public DbSet<User> UserData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppliForm>().ToTable("AppliForm");
            modelBuilder.Entity<Applicant>().ToTable("Applicant");
            modelBuilder.Entity<User>().ToTable("UserData");
        }
    }
}
