using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class Blog
{
    public int blog_id { get; set; }

    public int user_id { get; set; }

    public string title { get; set; } = null!;

    public string slug { get; set; } = null!;

    public string content { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public bool is_published { get; set; }

    public virtual BlogStat? BlogStat { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual ICollection<SavedBlog> SavedBlogs { get; set; } = new List<SavedBlog>();

    public virtual User user { get; set; } = null!;

    public virtual ICollection<Category> categories { get; set; } = new List<Category>();
}
