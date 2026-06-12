 ECommerceApp - Gelişmiş Test Otomasyonu ve Raporlama Projesi

Bu proje, bir e-ticaret sisteminin temel iş akışlarını (Ürün Seçimi, Sepet İşlemleri, Sipariş Verme, Ödeme) ve sisteme eklenen kritik iş kurallarını (Stok Kontrolü, İndirim Uygulamaları, Minimum Sipariş Tutarı Kontrolü) doğrulamak amacıyla **C#** ve **NUnit** kullanılarak geliştirilmiş kapsamlı bir test otomasyon ve kalite güvence çalışmasıdır.

Yazılım kalitesini ve test süreçlerinin derinliğini simüle etmek adına sistem içerisine **bilerek belirli hatalar (bug/defect) bırakılmış** ve bu hatalar yazılan test senaryoları ile dinamik olarak tespit edilerek raporlanmıştır.

---

 Proje Dosya Yapısı

```text
ECommerceApp/
 ├── Core/
 │    ├── Product.cs          # Ürün modelini ve stok bilgisini tutar.
 │    ├── Cart.cs             # Sepet işlemlerini ve indirim mantığını yönetir.
 │    └── OrderService.cs     # Sipariş doğrulama, minimum tutar ve ödeme akışını yönetir.
 │
 ├── Tests/
 │    ├── UnitTests/          # White Box: Metot seviyesindeki mantıksal testler.
 │    ├── IntegrationTests/   # Bileşenlerin bir arada çalışma uyumunu test eder.
 │    ├── BlackBoxTests/      # Arayüz/Girdi-Çıktı odaklı davranışsal testler.
 │    └── GrayBoxTests/       # İç yapı bilgisiyle girdi-çıktı analizini birleştiren testler.
 │
 └── Program.cs               # Uygulamanın ana giriş noktası.
