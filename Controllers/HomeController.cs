
using İÇERİK_YÖNETİMİ_VE_BLOG_1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string query)
        {
            // 1. Yazı Listesini Getir (Hem özel hem rastgele yazılar)
            var tumYazilar = GetYaziListesi();

            // 2. Arama Yapıldıysa Filtrele
            if (!string.IsNullOrEmpty(query))
            {
                tumYazilar = tumYazilar.Where(y => y.Baslik.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                            y.IcerikOzet.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                            y.Yazar.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                // Arama yoksa varsayılan akış (Burada hepsini gösteriyoruz ki sayfa dolu dursun)
                // İsterseniz buraya yine "kullaniciIlgiAlanlari" filtresi ekleyebilirsiniz.
            }

            return View(tumYazilar);
        }

        public IActionResult Detay(int id)
        {
            var yazi = GetYaziListesi().FirstOrDefault(x => x.Id == id);
            if (yazi == null) return NotFound();
            return View(yazi);
        }

        // --- GELİŞMİŞ VERİ LİSTESİ ---
        private List<YaziModeli> GetYaziListesi()
        {
            var list = new List<YaziModeli>();

            // 1. ÖZEL İÇERİKLER (Resimleri elle seçtik)
            list.Add(new YaziModeli
            {
                Id = 1,
                Kategori = "Yazılım",
                Baslik = "Yazılım Dünyasına Giriş",
                Yazar = "Ahmet Yılmaz",
                Tarih = "10 Ara",
                BegeniSayisi = 120,
                YorumSayisi = 15,
                IcerikOzet = "Yazılım öğrenmeye nereden başlamalı? İşte ipuçları...",
                ResimUrl = "https://images.unsplash.com/photo-1484417894907-623942c8ee29?w=500&q=80" // Okyanus/Doğa
            });

            list.Add(new YaziModeli
            {
                Id = 2,
                Kategori = "Yazılım",
                Baslik = "C# ve .NET Core İpuçları",
                Yazar = "Ayşe Demir",
                Tarih = "11 Ara",
                BegeniSayisi = 85,
                YorumSayisi = 42,
                IcerikOzet = "Visual Studio kullanırken hızınızı artıracak kısayollar.",
                ResimUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=500&q=80" // Kodlama Ekranı
            });

            list.Add(new YaziModeli
            {
                Id = 3,
                Kategori = "Tasarım",
                Baslik = "Tasarım Prensipleri",
                Yazar = "Mehmet Öz",
                Tarih = "12 Ara",
                BegeniSayisi = 200,
                YorumSayisi = 5,
                IcerikOzet = "Minimalist tasarımın gücü ve kullanıcı deneyimi.",
                ResimUrl = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=500&q=80" // Sanatsal/Tasarım
            });

            // 2. RASTGELE İÇERİK ÜRETİCİSİ (Sayfa dolu görünsün diye 15 tane daha ekliyoruz)
            var rnd = new Random();
            string[] isimler = { "Elif Kaya", "Can Yücel", "Selin Tekin", "Barış Manço", "Zeynep Yılmaz" };
            string[] kategoriler = { "Teknoloji", "Yazılım", "Tasarım", "Kariyer" };
            string[] resimler = {
                "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=500&q=80", // Laptop
                "https://images.unsplash.com/photo-1496171367470-9ed9a91ea931?w=500&q=80", // Masa
                "https://images.unsplash.com/photo-1550439062-609e1531270e?w=500&q=80", // Ofis
                "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=500&q=80"  // Kod
            };

            for (int i = 4; i <= 20; i++)
            {
                list.Add(new YaziModeli
                {
                    Id = i,
                    Kategori = kategoriler[rnd.Next(kategoriler.Length)],
                    Baslik = $"Blog Yazısı #{i}: Teknoloji ve Gelecek",
                    Yazar = isimler[rnd.Next(isimler.Length)],
                    Tarih = $"{rnd.Next(1, 30)} Ara",
                    BegeniSayisi = rnd.Next(10, 500),
                    YorumSayisi = rnd.Next(0, 50),
                    IcerikOzet = "Bu konuda detaylı bir inceleme yaptık. Devamını okumak için tıklayın...",
                    ResimUrl = resimler[rnd.Next(resimler.Length)]
                });
            }

            return list;
        }

        public IActionResult Library() { return View(); }
        // HomeController.cs dosyasında Library() metodunun altına şu kısmı ekle:

        // HomeController.cs içine:
        public IActionResult Profile()
        {
            return View();
        }
        // HomeController.cs dosyasına ekle:
        public IActionResult Stories()
        {
            return View();
        }
        // HomeController.cs içine ekle:
        public IActionResult Stats()
        {
            return View();
        }
        // HomeController.cs içine ekle:
        public IActionResult Write()
        {
            return View();
        }
        public IActionResult Channels()
        {
            var yazilar = new List<ChannelYaziModel>();

            // ÖRNEK VERİ 1 (Resimdeki Tasarıma Uygun)
            yazilar.Add(new ChannelYaziModel
            {
                Id = 1,
                Yazar = "Medium Staff",
                YazarResimUrl = "https://ui-avatars.com/api/?name=Medium+Staff&background=000&color=fff&rounded=true",
                YayinAdi = "The Medium Handbook",
                Baslik = "Editor Newsletter: What made these 3 featured stories reach 8x more readers?",
                Ozet = "Plus changes to the Partner Program, opportunities to share your feedback, and writing prompts.",
                Tarih = "Dec 12",
                BegenmeSayisi = "1.98K",
                YorumSayisi = 45,
                KapakResmiUrl = "https://images.unsplash.com/photo-1512314889357-e157c22f938d?w=200&h=130&fit=crop"
            });

            // ÖRNEK VERİ 2
            yazilar.Add(new ChannelYaziModel
            {
                Id = 2,
                Yazar = "Can Yücel",
                YazarResimUrl = "https://ui-avatars.com/api/?name=Can+Yucel&background=random&rounded=true",
                YayinAdi = "Yazılım Dünyası",
                Baslik = "Writer Newsletter: How to get your story featured by publication editors",
                Ozet = "Plus footnotes, writing prompts, and upcoming writing events for all developers.",
                Tarih = "Dec 10",
                BegenmeSayisi = "1.5K",
                YorumSayisi = 23,
                KapakResmiUrl = "https://images.unsplash.com/photo-1455390582262-044cdead277a?w=200&h=130&fit=crop"
            });

            return View(yazilar);
        }
        public IActionResult Privacy() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}