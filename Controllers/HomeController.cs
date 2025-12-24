using İÇERİK_YÖNETİMİ_VE_BLOG_1.Models;
using İÇERİK_YÖNETİMİ_VE_BLOG_1.Models.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogPlatformDbContext _db;

        public HomeController(ILogger<HomeController> logger, BlogPlatformDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        // ✅ Blog listesi: DB + arama + stats (like/comment sayıları BlogStats’tan)
        public IActionResult Index(string query)
        {
            var q = _db.Blogs
                .AsNoTracking()
                .Include(b => b.user)
                .Include(b => b.BlogStat)     // like_count/comment_count buradan
                .Include(b => b.categories)   // N-N kategori
                .Where(b => b.is_published);

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();
                q = q.Where(b =>
                    b.title.Contains(query) ||
                    b.content.Contains(query) ||
                    b.user.username.Contains(query)
                );
            }

            var blogs = q.OrderByDescending(b => b.created_at).ToList();

            var tumYazilar = blogs.Select(MapToYaziModeliSummary).ToList();
            return View(tumYazilar);
        }

        // ✅ Blog detay: içerik + kategoriler + yorumlar + stats
        public IActionResult Detay(int id)
        {
            var blog = _db.Blogs
                .AsNoTracking()
                .Include(b => b.user)
                .Include(b => b.BlogStat)
                .Include(b => b.categories)
                .Include(b => b.Comments)
                // Eğer Comment içinde user navigation varsa aç:
                // .ThenInclude(c => c.user)
                .FirstOrDefault(b => b.blog_id == id && b.is_published);

            if (blog == null) return NotFound();

            var vm = MapToYaziModeliDetail(blog);
            return View("içerikdetayblog", vm);

        }

        // --- LISTE (Index) için mapping: özet + sayılar + kategoriler ---
        private YaziModeli MapToYaziModeliSummary(Blog b)
        {
            var ozet = (b.content ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
            if (ozet.Length > 140) ozet = ozet.Substring(0, 140) + "...";

            return new YaziModeli
            {
                Id = b.blog_id,
                Baslik = b.title,
                Yazar = b.user?.username ?? "Bilinmiyor",
                Tarih = b.created_at.ToString("dd MMM"),
                Kategori = b.categories?.FirstOrDefault()?.category_name ?? "Genel",

                BegeniSayisi = b.BlogStat?.like_count ?? 0,
                YorumSayisi = b.BlogStat?.comment_count ?? 0,
                KaydetSayisi = b.BlogStat?.saved_count ?? 0,
                IsSaved = _db.SavedBlogs.AsNoTracking().Any(s => s.user_id == (HttpContext.Session.GetInt32("UserId") ?? 1) && s.blog_id == b.blog_id),


                IcerikOzet = ozet,
                ResimUrl = "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=500&q=80",

                Kategoriler = b.categories?.Select(c => c.category_name).ToList() ?? new List<string>()
            };
        }

        // --- DETAY için mapping: tam içerik + yorumlar + kategoriler ---
        private YaziModeli MapToYaziModeliDetail(Blog b)
        {
            var vm = MapToYaziModeliSummary(b);
            vm.Icerik = b.content ?? "";

            // Yorumlar (Comment modelinde alan adları: comment_text, created_at)
            vm.Yorumlar = (b.Comments ?? new List<Comment>())
                .OrderByDescending(c => c.created_at)
                .Select(c => new YaziModeli.YorumVM
                {
                    // Eğer Comment içinde user navigation varsa: c.user.username
                    // Yoksa şimdilik “Kullanıcı #id” gösterelim
                    Yazar = c.user?.username ?? $"Kullanıcı #{c.user_id}",
                    Metin = c.comment_text ?? "",
                    Tarih = c.created_at
                })
                .ToList();

            return vm;
        }

        // ✅ Like toggle (stored procedure): BlogApp.sp_ToggleLike
        // Not: gerçek kullanıcı id için login gerekir. Şimdilik Session’dan alıyoruz.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleLike(int blogId)
        {
            // Sen login yapana kadar geçici çözüm:
            // Session’a user_id koymuyorsan default 1 kullanır.
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            _db.Database.ExecuteSqlInterpolated(
                $"EXEC BlogApp.sp_ToggleLike @user_id={userId}, @blog_id={blogId}"
            );

            return RedirectToAction("Detay", new { id = blogId });
        }

        // ✅ Save toggle (stored procedure): BlogApp.sp_ToggleSave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleSave(int blogId, string returnUrl)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            _db.Database.ExecuteSqlInterpolated(
                $"EXEC BlogApp.sp_ToggleSave @user_id={userId}, @blog_id={blogId}"
            );

            // aynı sayfada kalmak için
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Detay", new { id = blogId });
        }


        // ✅ Yorum ekleme (stored procedure): BlogApp.sp_AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int blogId, string commentText)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            commentText = (commentText ?? "").Trim();
            if (commentText.Length == 0) return RedirectToAction("Detay", new { id = blogId });

            _db.Database.ExecuteSqlInterpolated(
                $"EXEC BlogApp.sp_AddComment @user_id={userId}, @blog_id={blogId}, @comment_text={commentText}"
            );

            return RedirectToAction("Detay", new { id = blogId });
        }

        // --- Sende var olan diğer action’lar (bozmuyoruz) ---
        public IActionResult Library(string tab = "your")
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            ViewBag.ActiveTab = tab;

            // === YOUR LISTS (kendi yazdıkları) ===
            if (tab == "your")
            {
                var blogs = _db.Blogs
                    .AsNoTracking()
                    .Include(b => b.user)
                    .Include(b => b.BlogStat)
                    .Include(b => b.categories)
                    .Where(b => b.user_id == userId && b.is_published)
                    .OrderByDescending(b => b.created_at)
                    .ToList();

                return View(blogs.Select(MapToYaziModeliSummary).ToList());
            }

            // === SAVED LISTS ===
            if (tab == "saved")
            {
                var blogs = _db.SavedBlogs
                    .AsNoTracking()
                    .Where(s => s.user_id == userId)
                    .Include(s => s.blog)
                        .ThenInclude(b => b.user)
                    .Include(s => s.blog)
                        .ThenInclude(b => b.BlogStat)
                    .Include(s => s.blog)
                        .ThenInclude(b => b.categories)
                    .OrderByDescending(s => s.saved_at)
                    .Select(s => s.blog)
                    .ToList();

                return View(blogs.Select(MapToYaziModeliSummary).ToList());
            }

            // === LIKES ===
            if (tab == "likes")
            {
                var blogs = _db.Likes
                    .AsNoTracking()
                    .Where(l => l.user_id == userId)
                    .Include(l => l.blog)
                        .ThenInclude(b => b.user)
                    .Include(l => l.blog)
                        .ThenInclude(b => b.BlogStat)
                    .Include(l => l.blog)
                        .ThenInclude(b => b.categories)
                    .Select(l => l.blog)
                    .ToList();

                return View(blogs.Select(MapToYaziModeliSummary).ToList());
            }

            // === COMMENTS ===
            // === COMMENTS (yorum + blog bilgisi) ===
            if (tab == "comments")
            {
                var yorumlar = _db.Comments
                    .AsNoTracking()
                    .Where(c => c.user_id == userId)
                    .Include(c => c.blog)
                        .ThenInclude(b => b.user)
                    .Include(c => c.blog)
                        .ThenInclude(b => b.BlogStat)
                    .Include(c => c.blog)
                        .ThenInclude(b => b.categories)
                    .OrderByDescending(c => c.created_at)
                    .ToList();

                // Yorum + Blog bilgisi birlikte taşınacak
                var model = yorumlar.Select(c =>
                {
                    var yazi = MapToYaziModeliSummary(c.blog);
                    yazi.YorumMetni = c.comment_text;
                    yazi.YorumTarihi = c.created_at;
                    return yazi;
                }).ToList();

                return View(model);
            }


            return View(new List<YaziModeli>());
        }



        public IActionResult Profile()
        {
            // Session yoksa 1 kullanmak yerine: var olan ilk kullanıcıyı seçelim
            var userId = HttpContext.Session.GetInt32("UserId");

            var u = _db.Users.AsNoTracking().FirstOrDefault(x => x.user_id == userId);

            if (u == null)
            {
                // DB’de ilk kullanıcıyı al
                u = _db.Users.AsNoTracking().OrderBy(x => x.user_id).FirstOrDefault();

                // DB tamamen boşsa -> açıklayıcı mesaj
                if (u == null)
                    return Content("Users tablosu boş. Önce kullanıcı eklemelisin.");

                // Session’ı düzelt
                HttpContext.Session.SetInt32("UserId", u.user_id);
            }

            ViewBag.DisplayName = string.IsNullOrWhiteSpace(u.username) ? "Kullanıcı" : u.username;
            ViewBag.AboutText = u.about ?? "";
            ViewBag.IsOwner = true;

            // View adı senin dosyana göre:
            // Eğer dosyan Views/Home/Profile.cshtml ise:
            return View("Profile");

            // Eğer dosyan Views/Home/Profil.cshtml ise:
            // return View("Profil");
        }







        public IActionResult Stories() => View();
        

        // ✅ WRITE (GET) -> kategorileri göster
        public IActionResult Write()
        {
            var kategoriler = _db.Categories
                .AsNoTracking()
                .Where(c => c.is_active)
                .OrderBy(c => c.category_name)
                .ToList();

            ViewBag.Categories = kategoriler;
            return View();
        }

        // ✅ PUBLISH (POST) -> blog + kategori ilişkileri kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Publish(string title, string content, int[] categoryIds)
        {
            // geçici kullanıcı (login yoksa)
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            title = (title ?? "").Trim();
            content = (content ?? "").Trim();

            if (title.Length < 3 || content.Length < 10)
            {
                // kategorileri tekrar yükleyip geri dön
                ViewBag.Categories = _db.Categories.AsNoTracking().Where(c => c.is_active).OrderBy(c => c.category_name).ToList();
                ViewBag.Error = "Başlık en az 3 karakter, içerik en az 10 karakter olmalı.";
                return View("Write");
            }

            // slug üret (benzersiz olsun)
            var baseSlug = Slugify(title);
            var slug = baseSlug;
            var i = 1;
            while (_db.Blogs.Any(b => b.slug == slug))
            {
                slug = $"{baseSlug}-{i++}";
            }

            // seçilen kategorileri DB’den çek
            var selectedCategories = _db.Categories
                .Where(c => categoryIds.Contains(c.category_id) && c.is_active)
                .ToList();

            // blog oluştur
            var blog = new Blog
            {
                user_id = userId,
                title = title,
                slug = slug,
                content = content,
                is_published = true,
                categories = selectedCategories
            };

            _db.Blogs.Add(blog);
            _db.SaveChanges(); // blog_id oluşur + BlogCategories satırları EF ile oluşur

            return RedirectToAction("Detay", new { id = blog.blog_id });
        }

        // ✅ basit slug helper (HomeController içine koy)
        private static string Slugify(string text)
        {
            text = (text ?? "").Trim().ToLowerInvariant();

            // Türkçe karakterleri sadeleştir
            text = text.Replace("ş", "s").Replace("ç", "c").Replace("ğ", "g")
                       .Replace("ı", "i").Replace("ö", "o").Replace("ü", "u");

            // harf/rakam/boşluk/dash dışını temizle
            var cleaned = new string(text.Select(ch =>
                char.IsLetterOrDigit(ch) ? ch :
                char.IsWhiteSpace(ch) || ch == '-' ? '-' : '\0'
            ).Where(ch => ch != '\0').ToArray());

            // çoklu - düzelt
            while (cleaned.Contains("--")) cleaned = cleaned.Replace("--", "-");
            cleaned = cleaned.Trim('-');

            return string.IsNullOrWhiteSpace(cleaned) ? "post" : cleaned;
        }


        public IActionResult Channels()
        {
            var yazilar = new List<ChannelYaziModel>();
            // (senin örnek verilerin kalsın)
            return View(yazilar);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAbout(string aboutText)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            aboutText = (aboutText ?? "").Trim();
            if (aboutText.Length > 900)
                aboutText = aboutText.Substring(0, 900);

            var u = _db.Users.FirstOrDefault(x => x.user_id == userId);
            if (u == null) return NotFound();

            // ✅ ARTIK DB’YE YAZIYOR
            u.about = aboutText;
            _db.SaveChanges();

            return RedirectToAction("Profile");
        }


        // ✅ EditProfile: formu DB’den doldur
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            var u = _db.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.user_id == userId);

            if (u == null) return NotFound();

            ViewBag.DisplayName = u.username ?? "Kullanıcı";
            ViewBag.AboutText = u.about ?? "";

            return View();
        }

        // ✅ UpdateProfile: hem username hem about DB’ye kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(string displayName, string aboutText)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            displayName = (displayName ?? "").Trim();
            aboutText = (aboutText ?? "").Trim();

            if (displayName.Length < 2) displayName = "Kullanıcı";
            if (displayName.Length > 40) displayName = displayName.Substring(0, 40);

            if (aboutText.Length > 900) aboutText = aboutText.Substring(0, 900);

            var u = _db.Users.FirstOrDefault(x => x.user_id == userId);
            if (u == null) return NotFound();

            u.username = displayName;
            u.about = aboutText;

            _db.SaveChanges();

            // İstersen session’da da tut (UI’da anında kullanıyorsan işine yarar)
            HttpContext.Session.SetString("DisplayName", displayName);
            HttpContext.Session.SetString("AboutText", aboutText);

            return RedirectToAction("Profile");
        }

        // ✅ STATS: follower/following/blog sayıları
        // ✅ STATS (aynı sayfa içinde followers/following listeleme)
        public IActionResult Stats(string tab = "stories")
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            // Sayılar
            var blogCount = _db.Blogs.AsNoTracking()
                .Count(b => b.user_id == userId && b.is_published);

            var followerCount = _db.Followers.AsNoTracking()
                .Count(f => f.following_id == userId);

            var followingCount = _db.Followers.AsNoTracking()
                .Count(f => f.follower_id == userId);

            ViewBag.BlogCount = blogCount;
            ViewBag.FollowerCount = followerCount;
            ViewBag.FollowingCount = followingCount;

            // Aktif sekme
            tab = (tab ?? "stories").Trim().ToLowerInvariant();
            if (tab != "stories" && tab != "followers" && tab != "following")
                tab = "stories";

            ViewBag.ActiveTab = tab;

            // Liste verisi (sadece followers/following seçilince doldur)
            if (tab == "followers")
            {
                var followers = _db.Followers
                    .AsNoTracking()
                    .Where(f => f.following_id == userId)
                    .Join(_db.Users.AsNoTracking(),
                          f => f.follower_id,
                          u => u.user_id,
                          (f, u) => new
                          {
                              u.user_id,
                              u.username,
                              u.about,
                              f.follow_date
                          })
                    .OrderByDescending(x => x.follow_date)
                    .ToList();

                ViewBag.People = followers;
            }
            else if (tab == "following")
            {
                var following = _db.Followers
                    .AsNoTracking()
                    .Where(f => f.follower_id == userId)
                    .Join(_db.Users.AsNoTracking(),
                          f => f.following_id,
                          u => u.user_id,
                          (f, u) => new
                          {
                              u.user_id,
                              u.username,
                              u.about,
                              f.follow_date
                          })
                    .OrderByDescending(x => x.follow_date)
                    .ToList();

                ViewBag.People = following;
            }

            return View();
        }



    }
}
