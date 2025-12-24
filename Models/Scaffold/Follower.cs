using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class Follower
{
    public int follower_id { get; set; }

    public int following_id { get; set; }

    public DateTime follow_date { get; set; }

    public virtual User follower { get; set; } = null!;

    public virtual User following { get; set; } = null!;
}
