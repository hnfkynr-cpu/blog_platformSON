using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class vw_UserProfileStat
{
    public int user_id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public DateTime created_at { get; set; }

    public int? blog_count { get; set; }

    public int? follower_count { get; set; }

    public int? following_count { get; set; }
}
