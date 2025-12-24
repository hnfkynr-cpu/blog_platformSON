using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class Like
{
    public int user_id { get; set; }

    public int blog_id { get; set; }

    public DateTime liked_at { get; set; }

    public virtual Blog blog { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
