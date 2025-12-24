using System;
using System.Collections.Generic;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;

public partial class Category
{
    public int category_id { get; set; }

    public string category_name { get; set; } = null!;

    public bool is_active { get; set; }

    public virtual ICollection<Blog> blogs { get; set; } = new List<Blog>();
}
