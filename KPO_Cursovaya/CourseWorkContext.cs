using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DB.Models;
namespace DB;

public partial class CourseWorkContext : DbContext
{
    public CourseWorkContext()
    {
    }

    public CourseWorkContext(DbContextOptions<CourseWorkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=course_work;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roles_pkey");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasMany(d => d.Users).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("UserRoles_Users_Id"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("UserRoles_Roles_Id"),
                    j =>
                    {
                        j.HasKey("RoleId", "UserId").HasName("UserRoles_pkey");
                        j.ToTable("UserRoles");
                        j.IndexerProperty<string>("RoleId").HasMaxLength(128);
                        j.IndexerProperty<string>("UserId").HasMaxLength(128);
                    });
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tariffs_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.QueryCount).HasColumnName("query_count");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Patronymic).HasColumnName("patronymic");
            entity.Property(e => e.Surname).HasColumnName("surname");
            entity.Property(e => e.UserGroupId).HasColumnName("user_group_id");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasOne(d => d.UserGroup).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserGroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Users_UserGroup_Id");
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserClaims_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserClaims_Users_Id");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserGroups_pkey");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FinishDate).HasColumnName("finish_date");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Subscription).HasColumnName("subscription");
            entity.Property(e => e.TariffId).HasColumnName("tariff_id");

            entity.HasOne(d => d.Tariff).WithMany(p => p.UserGroups)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("UserGroups_Tariff_Id");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProviderKey, e.LoginProvider }).HasName("UserLogins_pkey");

            entity.Property(e => e.UserId).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.LoginProvider).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserLogins_Users_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
