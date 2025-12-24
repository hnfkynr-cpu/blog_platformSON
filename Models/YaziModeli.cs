
namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models
{
    public class YaziModeli
    {
        public int Id { get; set; }

        // Öneri sistemi için gerekli kategori
        public string Kategori { get; set; }

        public string Baslik { get; set; }      // Blog Başlığı
        public string Yazar { get; set; }       // Yazar Adı
        public string IcerikOzet { get; set; }  // Kısa Açıklama
        public string Tarih { get; set; }       // Tarih
        public int BegeniSayisi { get; set; }   // Beğeni Sayısı
        public int YorumSayisi { get; set; }    // Yorum Sayısı

        public int KaydetSayisi { get; set; }   // 📥 Save sayısı (BlogStats.saved_count)
        public bool IsSaved { get; set; }       // Kullanıcı bu blogu kaydetti mi?


        // YENİ EKLENEN: Her yazıya özel resim linki için
        public string ResimUrl { get; set; }
        public string Icerik { get; set; } = "";                // Detayda tam içerik
        public List<string> Kategoriler { get; set; } = new();   // BlogApp.Categories
        public List<YorumVM> Yorumlar { get; set; } = new();     // BlogApp.Comments
        public string? YorumMetni { get; set; }
        public DateTime? YorumTarihi { get; set; }

        public class YorumVM
        {
            public string Yazar { get; set; } = "";
            public string Metin { get; set; } = "";
            public DateTime Tarih { get; set; }
        }

    }
}
