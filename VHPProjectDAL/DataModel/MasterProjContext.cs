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

    public virtual DbSet<MemberDetails> MemberDetails { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtoken { get; set; }

    public virtual DbSet<Satsang> Satsang { get; set; }

    public virtual DbSet<Talukamaster> Talukamaster { get; set; }

    public virtual DbSet<Villagemaster> Villagemaster { get; set; }

    public virtual DbSet<Wall> Wall { get; set; }

    public virtual DbSet<WallPostComments> WallPostComments { get; set; }

    public virtual DbSet<WallPostImages> WallPostImages { get; set; }

    public virtual DbSet<WallPostLike> WallPostLike { get; set; }

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

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);

            entity.HasOne(d => d.TalukaMaster).WithMany(p => p.Member)
                .HasForeignKey(d => d.TalukaMasterId)
                .HasConstraintName("FK_Member_Taluka");

            entity.HasOne(d => d.VillageMaster).WithMany(p => p.Member)
                .HasForeignKey(d => d.VillageMasterId)
                .HasConstraintName("FK_Member_Village");
        });

        modelBuilder.Entity<MemberDetails>(entity =>
        {
            entity.HasKey(e => e.IdMember).HasName("PRIMARY");

            entity.ToTable("member_details");

            entity.Property(e => e.IdMember).HasColumnName("id_member");
            entity.Property(e => e.Active)
                .HasColumnType("bit(2)")
                .HasColumnName("active");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.EmailId)
                .HasMaxLength(255)
                .HasColumnName("email_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20)
                .HasColumnName("mobile_number");
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

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.TalukaName).HasMaxLength(100);
        });

        modelBuilder.Entity<Villagemaster>(entity =>
        {
            entity.HasKey(e => e.VillageMasterId).HasName("PRIMARY");

            entity.ToTable("villagemaster");

            entity.HasIndex(e => e.TalukaMasterId, "TalukaMasterId");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.VillageName).HasMaxLength(100);

            entity.HasOne(d => d.TalukaMaster).WithMany(p => p.Villagemaster)
                .HasForeignKey(d => d.TalukaMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("villagemaster_ibfk_1");
        });

        modelBuilder.Entity<Wall>(entity =>
        {
            entity.HasKey(e => e.IdWall).HasName("PRIMARY");

            entity.ToTable("wall");

            entity.HasIndex(e => e.MemberDetailsId, "member_details_id");

            entity.Property(e => e.IdWall).HasColumnName("id_wall");
            entity.Property(e => e.Active)
                .HasColumnType("bit(2)")
                .HasColumnName("active");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.MemberDetailsId).HasColumnName("member_details_id");
            entity.Property(e => e.PostMessages)
                .HasMaxLength(255)
                .HasColumnName("post_messages");

            entity.HasOne(d => d.MemberDetails).WithMany(p => p.Wall)
                .HasForeignKey(d => d.MemberDetailsId)
                .HasConstraintName("wall_ibfk_1");
        });

        modelBuilder.Entity<WallPostComments>(entity =>
        {
            entity.HasKey(e => e.IdwallPostComments).HasName("PRIMARY");

            entity.ToTable("wall_post_comments");

            entity.HasIndex(e => e.MemberDetailsId, "member_details_id");

            entity.HasIndex(e => e.WallId, "wall_id");

            entity.Property(e => e.IdwallPostComments).HasColumnName("idwall_post_comments");
            entity.Property(e => e.Active)
                .HasColumnType("bit(2)")
                .HasColumnName("active");
            entity.Property(e => e.CommentText)
                .HasMaxLength(255)
                .HasColumnName("comment_text");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.InsertDateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("insert_date_time");
            entity.Property(e => e.MemberDetailsId).HasColumnName("member_details_id");
            entity.Property(e => e.WallId).HasColumnName("wall_id");

            entity.HasOne(d => d.MemberDetails).WithMany(p => p.WallPostComments)
                .HasForeignKey(d => d.MemberDetailsId)
                .HasConstraintName("wall_post_comments_ibfk_1");

            entity.HasOne(d => d.Wall).WithMany(p => p.WallPostComments)
                .HasForeignKey(d => d.WallId)
                .HasConstraintName("wall_post_comments_ibfk_2");
        });

        modelBuilder.Entity<WallPostImages>(entity =>
        {
            entity.HasKey(e => e.IdwallPostImages).HasName("PRIMARY");

            entity.ToTable("wall_post_images");

            entity.HasIndex(e => e.WallId, "wall_id");

            entity.Property(e => e.IdwallPostImages).HasColumnName("idwall_post_images");
            entity.Property(e => e.Active)
                .HasColumnType("bit(2)")
                .HasColumnName("active");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(1500)
                .HasColumnName("image_url");
            entity.Property(e => e.WallId).HasColumnName("wall_id");

            entity.HasOne(d => d.Wall).WithMany(p => p.WallPostImages)
                .HasForeignKey(d => d.WallId)
                .HasConstraintName("wall_post_images_ibfk_1");
        });

        modelBuilder.Entity<WallPostLike>(entity =>
        {
            entity.HasKey(e => e.IdwallPostLike).HasName("PRIMARY");

            entity.ToTable("wall_post_like");

            entity.HasIndex(e => new { e.MemberDetailsId, e.WallId }, "unique_like").IsUnique();

            entity.HasIndex(e => e.WallId, "wall_id");

            entity.Property(e => e.IdwallPostLike).HasColumnName("idwall_post_like");
            entity.Property(e => e.Active)
                .HasColumnType("bit(2)")
                .HasColumnName("active");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Like)
                .HasColumnType("bit(1)")
                .HasColumnName("like");
            entity.Property(e => e.MemberDetailsId).HasColumnName("member_details_id");
            entity.Property(e => e.WallId).HasColumnName("wall_id");

            entity.HasOne(d => d.MemberDetails).WithMany(p => p.WallPostLike)
                .HasForeignKey(d => d.MemberDetailsId)
                .HasConstraintName("wall_post_like_ibfk_1");

            entity.HasOne(d => d.Wall).WithMany(p => p.WallPostLike)
                .HasForeignKey(d => d.WallId)
                .HasConstraintName("wall_post_like_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
