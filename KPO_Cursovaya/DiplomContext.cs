using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KPO_Cursovaya.Models;
namespace KPO_Cursovaya;

public partial class DiplomContext : DbContext
{
    public DiplomContext()
    {
    }

    public DiplomContext(DbContextOptions<DiplomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vkuser> Vkusers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=diplom;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Requests_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Author).HasColumnName("author");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.AuthorLink).HasColumnName("author_link");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ImageByte).HasColumnName("image_byte");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.VkuserId).HasColumnName("vkuser_id");

            entity.HasOne(d => d.Vkuser).WithMany(p => p.Requests)
                .HasForeignKey(d => d.VkuserId)
                .HasConstraintName("request_id_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("Roles_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.EmailConfirmed).HasColumnName("Email_confirmed");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("role_name");
        });

        modelBuilder.Entity<Vkuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Request_pkey");

            entity.ToTable("VKuser");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Vkusers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_id_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
