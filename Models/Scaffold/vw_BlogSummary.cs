using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class vw_BlogSummary
{
    public int blog_id { get; set; }

    public string title { get; set; } = null!;

    public string slug { get; set; } = null!;

    public DateTime created_at { get; set; }

    public bool is_published { get; set; }

    public int user_id { get; set; }

    public string username { get; set; } = null!;

    public int like_count { get; set; }

    public int comment_count { get; set; }

    public int saved_count { get; set; }
}
