using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using TodoApi.Models;

namespace TodoApi;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<loginUser> Login_users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=bwdhfhsxod0ovlwo6nzt-mysql.services.clever-cloud.com;user=uijd9nyojjlr1zie;password=mBQhsTwUFFLAJudjlUsK;database=bwdhfhsxod0ovlwo6nzt", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .UseCollation("utf8mb4_0900_ai_ci")
        .HasCharSet("utf8mb4");

    // Items table
    modelBuilder.Entity<Item>(entity =>
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");
        entity.ToTable("items");

        entity.Property(e => e.Id).HasColumnName("ID");
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.IsComplete);
    });
    // Users table
    modelBuilder.Entity<loginUser>(entity =>
    {
        entity.ToTable("login_users");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("ID");
        entity.Property(e => e.username).HasColumnName("USERNAME").HasMaxLength(100).IsRequired();
        entity.Property(e => e.password).HasColumnName("PASSWORD").HasMaxLength(255).IsRequired();
    });

    OnModelCreatingPartial(modelBuilder);
}

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
