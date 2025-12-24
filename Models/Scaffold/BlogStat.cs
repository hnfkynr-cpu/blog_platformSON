using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class BlogStat
{
    public int blog_id { get; set; }

    public int like_count { get; set; }

    public int comment_count { get; set; }

    public int saved_count { get; set; }

    public DateTime last_updated { get; set; }

    public virtual Blog blog { get; set; } = null!;
}
