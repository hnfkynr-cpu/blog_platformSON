namespace İÇERİK_YÖNETİMİ_VE_BLOG_1.Models
{
    public class ChannelYaziModel
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string Ozet { get; set; }
        public string Yazar { get; set; }
        public string YazarResimUrl { get; set; } // Yazarın küçük yuvarlak resmi
        public string YayinAdi { get; set; } // Örn: "In The Medium Handbook"
        public string Tarih { get; set; }
        public string OkunmaSuresi { get; set; } // Örn: "4 min read"
        public string KapakResmiUrl { get; set; } // Sağdaki büyük resim
        public string BegenmeSayisi { get; set; } // "1.98K" gibi
        public int YorumSayisi { get; set; }
    }
}