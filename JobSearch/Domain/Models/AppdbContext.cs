using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class AppdbContext : DbContext
{
    public AppdbContext()
    {
    }

    public AppdbContext(DbContextOptions<AppdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ChatSection> ChatSections { get; set; }

    public virtual DbSet<Connect> Connects { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<JobPost> JobPosts { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UserCredential> UserCredentials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=ELTP-LAP-0531\\SQLEXPRESS; database=Appdb; Trusted_Connection=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Applicat__C93A4C994841574B");

            entity.ToTable("Application");

            entity.Property(e => e.ApplicationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApplicationDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.JobPost).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Applicati__JobPo__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Applicati__UserI__49C3F6B7");
        });

        modelBuilder.Entity<ChatSection>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__ChatSect__A9FBE7C64C7175E3");

            entity.ToTable("ChatSection");

            entity.Property(e => e.ChatId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.Connection).WithMany(p => p.ChatSections)
                .HasForeignKey(d => d.ConnectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatSecti__Conne__59063A47");
        });

        modelBuilder.Entity<Connect>(entity =>
        {
            entity.HasKey(e => e.ConnectionId).HasName("PK__Connect__404A649315F183B8");

            entity.ToTable("Connect");

            entity.Property(e => e.ConnectionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ConnectionDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Receiver).WithMany(p => p.ConnectReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Connect__Receive__5441852A");

            entity.HasOne(d => d.Sender).WithMany(p => p.ConnectSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Connect__SenderI__534D60F1");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__C97C58525E3F6D2B");

            entity.ToTable("Interview");

            entity.Property(e => e.InterviewId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.InterviewDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Interview__Appli__4E88ABD4");
        });

        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.HasKey(e => e.JobPostId).HasName("PK__JobPost__57689C3A00C183FD");

            entity.ToTable("JobPost");

            entity.Property(e => e.JobPostId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Deadline).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JobPost__UserId__440B1D61");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__Profile__290C88E446870B77");

            entity.ToTable("Profile");

            entity.Property(e => e.ProfileId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Profile__UserId__3F466844");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AA0F8E32A");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserCred__1788CC4C1FB18BD2");

            entity.ToTable("UserCredential");

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserCredentials)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserCrede__RoleI__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
