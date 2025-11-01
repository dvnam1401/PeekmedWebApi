using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PeekmedWebApi.Models;

namespace PeekmedWebApi.Data;

public partial class PeekMedDbContext : DbContext
{
    public PeekMedDbContext()
    {
    }

    public PeekMedDbContext(DbContextOptions<PeekMedDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Queue> Queues { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:peekmeddb-server-123.database.windows.net,1433;Initial Catalog=peekmeddb;Persist Security Info=False;User ID=sqladmin;Password=Tien221274;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC2BEEB8BDE");

            entity.HasIndex(e => e.AppointmentDateTime, "idx_appointment_datetime");

            entity.HasIndex(e => e.Status, "idx_appointment_status");

            entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Depar__403A8C7D");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Appointme__Docto__3F466844");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Hospi__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Appointme__UserI__3E52440B");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BED4A1D0A06");

            entity.Property(e => e.DepartmentName).HasMaxLength(150);

            entity.HasOne(d => d.Hospital).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HospitalId)
                .HasConstraintName("FK__Departmen__Hospi__300424B4");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EBF5948DF80");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Doctors__85FB4E38D798AA3E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Doctors__A9D105347517D56F").IsUnique();

            entity.HasIndex(e => new { e.LastName, e.FirstName }, "idx_doctor_name");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Sex).HasMaxLength(10);
            entity.Property(e => e.Specialty).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Doctors__Departm__37A5467C");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.HospitalId).HasName("PK__Hospital__38C2E5AFDEBB5AF0");

            entity.HasIndex(e => e.Email, "UQ__Hospital__A9D1053486B69544").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1258F259DE");

            entity.HasIndex(e => e.UserId, "idx_notification_user");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__4D94879B");
        });

        modelBuilder.Entity<Queue>(entity =>
        {
            entity.HasKey(e => e.QueueId).HasName("PK__Queue__8324E7155E542CD4");

            entity.ToTable("Queue");

            entity.HasIndex(e => e.AppointmentId, "UQ__Queue__8ECDFCC3EC7B0B76").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Waiting");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Appointment).WithOne(p => p.Queue)
                .HasForeignKey<Queue>(d => d.AppointmentId)
                .HasConstraintName("FK__Queue__Appointme__48CFD27E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2B7DDD97");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Users__85FB4E38CCAFF09F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053443879AD9").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Sex).HasMaxLength(10);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
