using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class BlogPlatformDbContext : DbContext
{
    public BlogPlatformDbContext()
    {
    }

    public BlogPlatformDbContext(DbContextOptions<BlogPlatformDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogStat> BlogStats { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<ReadingProgress> ReadingProgresses { get; set; }

    public virtual DbSet<SavedBlog> SavedBlogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<vw_BlogSummary> vw_BlogSummaries { get; set; }

    public virtual DbSet<vw_UserProfileStat> vw_UserProfileStats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-NAJ0IAL;Database=blog_platform;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.blog_id).HasName("PK__Blogs__2975AA28C2B9D394");

            entity.ToTable("Blogs", "BlogApp", tb => tb.HasTrigger("trg_Blogs_Insert_Stats"));

            entity.HasIndex(e => new { e.is_published, e.created_at }, "IX_Blogs_IsPubDate").IsDescending(false, true);

            entity.HasIndex(e => e.user_id, "IX_Blogs_User");

            entity.HasIndex(e => e.slug, "UQ_Blogs_Slug").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.slug).HasMaxLength(250);
            entity.Property(e => e.title).HasMaxLength(200);

            entity.HasOne(d => d.user).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Blogs_Users");

            entity.HasMany(d => d.categories).WithMany(p => p.blogs)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("category_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlogCategories_Categories"),
                    l => l.HasOne<Blog>().WithMany()
                        .HasForeignKey("blog_id")
                        .HasConstraintName("FK_BlogCategories_Blogs"),
                    j =>
                    {
                        j.HasKey("blog_id", "category_id").HasName("PK__BlogCate__742144B31F4B36B9");
                        j.ToTable("BlogCategories", "BlogApp");
                    });
        });

        modelBuilder.Entity<BlogStat>(entity =>
        {
            entity.HasKey(e => e.blog_id).HasName("PK__BlogStat__2975AA28BFA5634E");

            entity.ToTable("BlogStats", "BlogApp");

            entity.Property(e => e.blog_id).ValueGeneratedNever();
            entity.Property(e => e.last_updated).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.blog).WithOne(p => p.BlogStat)
                .HasForeignKey<BlogStat>(d => d.blog_id)
                .HasConstraintName("FK_BlogStats_Blogs");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.category_id).HasName("PK__Categori__D54EE9B435319527");

            entity.ToTable("Categories", "BlogApp");

            entity.HasIndex(e => e.category_name, "UQ__Categori__5189E255C4C16542").IsUnique();

            entity.Property(e => e.category_name).HasMaxLength(100);
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.comment_id).HasName("PK__Comments__E7957687534796A8");

            entity.ToTable("Comments", "BlogApp", tb => tb.HasTrigger("trg_Comments_Change_Stats"));

            entity.HasIndex(e => e.blog_id, "IX_Comments_Blog");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.blog_id)
                .HasConstraintName("FK_Comments_Blogs");

            entity.HasOne(d => d.user).WithMany(p => p.Comments)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(e => new { e.follower_id, e.following_id }).HasName("PK__Follower__CAC186A73808D0B4");

            entity.ToTable("Followers", "BlogApp");

            entity.Property(e => e.follow_date).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.follower).WithMany(p => p.Followerfollowers)
                .HasForeignKey(d => d.follower_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followers_Follower");

            entity.HasOne(d => d.following).WithMany(p => p.Followerfollowings)
                .HasForeignKey(d => d.following_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followers_Following");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.blog_id }).HasName("PK__Likes__2B296DAD66685301");

            entity.ToTable("Likes", "BlogApp", tb => tb.HasTrigger("trg_Likes_Change_Stats"));

            entity.HasIndex(e => e.blog_id, "IX_Likes_Blog");

            entity.HasIndex(e => e.user_id, "IX_Likes_User");

            entity.Property(e => e.liked_at).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.blog).WithMany(p => p.Likes)
                .HasForeignKey(d => d.blog_id)
                .HasConstraintName("FK_Likes_Blogs");

            entity.HasOne(d => d.user).WithMany(p => p.Likes)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Users");
        });

        modelBuilder.Entity<ReadingProgress>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.blog_id }).HasName("PK__ReadingP__2B296DAD7DACDA7C");

            entity.ToTable("ReadingProgress", "BlogApp");

            entity.HasIndex(e => e.blog_id, "IX_Reading_Blog");

            entity.HasIndex(e => e.user_id, "IX_Reading_User");

            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.blog).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.blog_id)
                .HasConstraintName("FK_ReadingProgress_Blogs");

            entity.HasOne(d => d.user).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReadingProgress_Users");
        });

        modelBuilder.Entity<SavedBlog>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.blog_id }).HasName("PK__SavedBlo__2B296DADA8A95B46");

            entity.ToTable("SavedBlogs", "BlogApp", tb => tb.HasTrigger("trg_SavedBlogs_Change_Stats"));

            entity.HasIndex(e => e.blog_id, "IX_SavedBlogs_Blog");

            entity.HasIndex(e => e.user_id, "IX_SavedBlogs_User");

            entity.Property(e => e.saved_at).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.blog).WithMany(p => p.SavedBlogs)
                .HasForeignKey(d => d.blog_id)
                .HasConstraintName("FK_SavedBlogs_Blogs");

            entity.HasOne(d => d.user).WithMany(p => p.SavedBlogs)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedBlogs_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PK__Users__B9BE370F60190ABE");

            entity.ToTable("Users", "BlogApp");

            entity.HasIndex(e => e.email, "IX_Users_Email");

            entity.HasIndex(e => e.username, "IX_Users_Username");

            entity.HasIndex(e => e.email, "UQ__Users__AB6E6164D0C4876A").IsUnique();

            entity.HasIndex(e => e.username, "UQ__Users__F3DBC5721128ABDA").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.password_hash).HasMaxLength(300);
            entity.Property(e => e.username).HasMaxLength(100);
        });

        modelBuilder.Entity<vw_BlogSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_BlogSummary", "BlogApp");

            entity.Property(e => e.slug).HasMaxLength(250);
            entity.Property(e => e.title).HasMaxLength(200);
            entity.Property(e => e.username).HasMaxLength(100);
        });

        modelBuilder.Entity<vw_UserProfileStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_UserProfileStats", "BlogApp");

            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.user_id).ValueGeneratedOnAdd();
            entity.Property(e => e.username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
