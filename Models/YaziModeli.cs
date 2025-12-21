
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

        // YENİ EKLENEN: Her yazıya özel resim linki için
        public string ResimUrl { get; set; }
    }
}
