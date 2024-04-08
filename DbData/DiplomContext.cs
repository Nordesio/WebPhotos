using System;
using System.Collections.Generic;
using DbData.Models;
using Microsoft.EntityFrameworkCore;

namespace DbData;

public partial class DiplomContext : DbContext
{
    public DiplomContext()
    {
    }

    public DiplomContext(DbContextOptions<DiplomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Entity> Entities { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vkuser> Vkusers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=diplom;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Entities_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Geo).HasColumnName("geo");
            entity.Property(e => e.Money).HasColumnName("money");
            entity.Property(e => e.Namedentity).HasColumnName("namedentity");
            entity.Property(e => e.Organization).HasColumnName("organization");
            entity.Property(e => e.Person).HasColumnName("person");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Street).HasColumnName("street");

            entity.HasOne(d => d.Request).WithMany(p => p.Entities)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("Enity_request");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Requests_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Author).HasColumnName("author");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.AuthorLink).HasColumnName("author_link");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ImageByte).HasColumnName("image_byte");
            entity.Property(e => e.ImageLink).HasColumnName("image_link");
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
