using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class ReadingProgress
{
    public int user_id { get; set; }

    public int blog_id { get; set; }

    public int progress { get; set; }

    public DateTime updated_at { get; set; }

    public virtual Blog blog { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
