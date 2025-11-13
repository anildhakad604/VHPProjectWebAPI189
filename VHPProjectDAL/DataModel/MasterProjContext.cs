using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace VHPProjectDAL.DataModel;

public partial class MasterProjContext : DbContext
{
    public MasterProjContext()
    {
    }

    public MasterProjContext(DbContextOptions<MasterProjContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Designation> Designation { get; set; }

    public virtual DbSet<Member> Member { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtoken { get; set; }

    public virtual DbSet<Satsang> Satsang { get; set; }

    public virtual DbSet<Talukamaster> Talukamaster { get; set; }

    public virtual DbSet<Villagemaster> Villagemaster { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=DESKTOP-OI7KVCP;port=3306;user=root;password=Anil@6996;database=vhp_projdb", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DesignationId).HasName("PRIMARY");

            entity.ToTable("designation");

            entity.HasIndex(e => e.DesignationName, "UX_Designation_DesignationName").IsUnique();

            entity.Property(e => e.DesignationName).HasMaxLength(200);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PRIMARY");

            entity.ToTable("member");

            entity.HasIndex(e => e.TalukaMasterId, "FK_Member_Taluka");

            entity.HasIndex(e => e.VillageMasterId, "FK_Member_Village");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);

            entity.HasOne(d => d.TalukaMaster).WithMany(p => p.Member)
                .HasForeignKey(d => d.TalukaMasterId)
                .HasConstraintName("FK_Member_Taluka");

            entity.HasOne(d => d.VillageMaster).WithMany(p => p.Member)
                .HasForeignKey(d => d.VillageMasterId)
                .HasConstraintName("FK_Member_Village");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.IdRefreshToken).HasName("PRIMARY");

            entity.ToTable("refreshtoken");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsRevoked).HasDefaultValueSql("'0'");
            entity.Property(e => e.IsUsed).HasDefaultValueSql("'0'");
            entity.Property(e => e.Value).HasMaxLength(255);
        });

        modelBuilder.Entity<Satsang>(entity =>
        {
            entity.HasKey(e => e.IdSatsang).HasName("PRIMARY");

            entity.ToTable("satsang");

            entity.HasIndex(e => e.TalukaMasterId, "fk_satsang_taluka");

            entity.HasIndex(e => e.VillageMasterId, "fk_satsang_village");

            entity.Property(e => e.IdSatsang).HasColumnName("id_satsang");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("active");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.FromDate)
                .HasColumnType("datetime")
                .HasColumnName("from_date");
            entity.Property(e => e.SatsangName)
                .HasMaxLength(45)
                .HasColumnName("satsang_name");
            entity.Property(e => e.TalukaMasterId).HasColumnName("taluka_master_id");
            entity.Property(e => e.TempleAddress)
                .HasMaxLength(45)
                .HasColumnName("temple_address");
            entity.Property(e => e.TempleName)
                .HasMaxLength(45)
                .HasColumnName("temple_name");
            entity.Property(e => e.ToDate)
                .HasColumnType("datetime")
                .HasColumnName("to_date");
            entity.Property(e => e.VillageMasterId).HasColumnName("village_master_id");

            entity.HasOne(d => d.TalukaMaster).WithMany(p => p.Satsang)
                .HasForeignKey(d => d.TalukaMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_satsang_taluka");

            entity.HasOne(d => d.VillageMaster).WithMany(p => p.Satsang)
                .HasForeignKey(d => d.VillageMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_satsang_village");
        });

        modelBuilder.Entity<Talukamaster>(entity =>
        {
            entity.HasKey(e => e.TalukaMasterId).HasName("PRIMARY");

            entity.ToTable("talukamaster");

            entity.HasIndex(e => new { e.IsActive, e.TalukaName }, "IX_Taluka_IsActive_Name");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.TalukaName).HasMaxLength(200);
        });

        modelBuilder.Entity<Villagemaster>(entity =>
        {
            entity.HasKey(e => e.VillageMasterId).HasName("PRIMARY");

            entity.ToTable("villagemaster");

            entity.HasIndex(e => new { e.IsActive, e.VillageName }, "IX_Village_IsActive_Name");

            entity.HasIndex(e => e.TalukaMasterId, "IX_Village_Taluka");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.VillageName).HasMaxLength(200);

            entity.HasOne(d => d.TalukaMaster).WithMany(p => p.Villagemaster)
                .HasForeignKey(d => d.TalukaMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("villagemaster_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
