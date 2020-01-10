using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace attendance.Entities
{
    public partial class attendanceContext : DbContext
    {
        public attendanceContext()
        {
        }

        public attendanceContext(DbContextOptions<attendanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Login> Login { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employee");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("character varying");

                entity.Property(e => e.Dateout)
                    .HasColumnName("dateout")
                    .HasColumnType("character varying");

                entity.Property(e => e.Employeeid)
                    .HasColumnName("employeeid")
                    .HasColumnType("character varying");

                entity.Property(e => e.Timein)
                    .HasColumnName("timein")
                    .HasColumnType("character varying");

                entity.Property(e => e.Timeout)
                    .HasColumnName("timeout")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("login");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasColumnType("character varying");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasColumnType("character varying");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
