using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class User
{
    public int user_id { get; set; }

    public string email { get; set; } = null!;

    public string username { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public DateTime created_at { get; set; }

    public bool is_active { get; set; }

    public string? about { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Follower> Followerfollowers { get; set; } = new List<Follower>();

    public virtual ICollection<Follower> Followerfollowings { get; set; } = new List<Follower>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual ICollection<SavedBlog> SavedBlogs { get; set; } = new List<SavedBlog>();
}
