# HBT Fatura & Ön Muhasebe Yönetimi

Modern, web tabanlı e-fatura ve ön muhasebe yönetim sistemi. ASP.NET Core 8 Web API ve Angular 17 kullanılarak geliştirilmiştir.

## 🚀 Öne Çıkan Özellikler

### 📄 Gelişmiş Fatura Yönetimi
-   **Alış ve Satış Faturaları:** Hem alış hem satış süreçlerini yönetebilme.
-   **e-Fatura Standartları:** GİB standartlarına uygun PDF tasarımı.
-   **ETTN Üretimi:** Her fatura için otomatik benzersiz GUID (ETTN) üretimi.
-   **QR Kod Desteği:** Fatura numarasına özel otomatik QR kod üretimi ve PDF'e yerleşimi.
-   **Senaryo Yönetimi:** GİB'e gönderim öncesi **Temel Fatura** veya **Ticari Fatura** seçimi.
-   **Otomatik Toplamlar:** KDV, İskonto ve Genel Toplamların backend tarafında hassas hesaplanması.
-   **Yazı ile Tutar:** Toplam tutarın faturada otomatik olarak yazıya çevrilmesi.

## 🖼 Örnek Fatura Görünümü

Sistem tarafından üretilen; GİB standartlarına uygun, QR kodlu ve ETTN imzalı örnek fatura tasarımı aşağıdadır:

![Örnek Fatura Tasarımı](assets/invoice-sample.png)

### 📦 Stok ve Ürün Yönetimi
-   Ürün kartları tanımlama ve takip.
-   Fatura onaylandığında otomatik stok düşümü/artırımı (esnek yapı).
-   Stok hareketleri geçmişi ve güncel stok durumu raporları.

### 💰 Finans ve Kasa Modülleri
-   **Banka Hesapları:** Banka bazlı hesap takibi ve IBAN yönetimi.
-   **Kasa Yönetimi:** Nakit giriş-çıkış takibi için birden fazla kasa tanımlama.
-   **Çek ve Senet:** Müşteri/Firma çek ve senetlerinin vade takibi.
-   **Cari Hesaplar:** Müşteri bazlı borç-alacak takibi ve otomatik işlem kayıtları.

### 🚚 Lojistik ve Satış
-   **Siparişler:** Proforma veya resmi siparişlerin yönetimi.
-   **İrsaliyeler:** Sevkiyat takibi ve irsaliyenin tek tıkla faturaya dönüştürülmesi.

### 📊 Raporlama
-   Stok durum raporları.
-   Satış ve finansal durum analizleri.

## 🛠 Teknoloji Yığını

-   **Backend:** .NET 8, Entity Framework Core (Code First), SQL Server.
-   **Frontend:** Angular 17, Reactive Forms, Ngx-Toastr.
-   **PDF Engine:** QuestPDF (Yüksek performanslı ve pixel-perfect tasarım).
-   **QR Engine:** QRCoder kütüphanesi.
-   **Güvenlik:** JWT Authentication & Refresh Token mekanizması.

## 🏗 Proje Yapısı

-   **Backend (`src/backend/HbtFatura.Api`):**
    -   `Entities`: Veritabanı modelleri (Invoice, Stock, Cash, etc.).
    -   `Services`: İş mantığı, PDF üretimi ve hesaplama servisleri.
    -   `Controllers`: RESTful API uç noktaları.
    -   `Data`: EF Core DbContext ve Migration dosyaları.
-   **Frontend (`src/frontend`):**
    -   `app/services`: API servisleri.
    -   `app/invoices`, `app/customers`, vb.: Modüler sayfa bileşenleri.
    -   `app/core`: Guard'lar ve interceptor'lar.

## ⚙️ Kurulum ve Çalıştırma

### Gereksinimler
-   .NET 8 SDK
-   Node.js 18+
-   SQL Server (Express veya LocalDB)

### Backend (API)

1.  `src/backend/HbtFatura.Api/appsettings.json` dosyasındaki Connection String bilgisini kendi SQL Server yapınıza göre düzenleyin.
2.  Migration işlemlerini uygulayarak veritabanını oluşturun:
    -   **Visual Studio (Package Manager Console):**
        ```powershell
        Update-Database
        ```
    -   **Terminal / dotnet CLI:**
        ```bash
        cd src/backend/HbtFatura.Api
        dotnet ef database update
        ```
3.  Projeyi çalıştırın:
    ```bash
    dotnet run
    ```
API: `https://localhost:7270` üzerinden çalışır. Swagger arayüzüne `/swagger` adresinden erişebilirsiniz.

### Frontend (Angular)
```bash
cd src/frontend
npm install
npm start
```
Uygulama: `http://localhost:4200` adresinde açılır.

## 💡 İlk Kullanım ve Simülasyon Rehberi

Bu proje, her firmanın sadece kendi verilerine erişebildiği **Çoklu Kiracı (Multi-tenant)** yapısını simüle etmek amacıyla tasarlanmıştır.

### 1. Sisteme İlk Giriş (Super Admin)
Veritabanı migration işleminden sonra sistemde otomatik olarak bir **Super Admin** hesabı oluşturulur:
- **E-posta:** `admin@test.com`
- **Şifre:** `Admin123!`

**Super Admin ile Neler Yapılabilir?**
- Sistemdeki tüm işlemleri içeren **Log Kayıtlarını** kontrol edebilir.
- **Firmalar** sekmesinden yeni kurumsal firmalar tanımlayabilir. Yeni bir firma oluşturduğunuzda, o firmaya ait bir **Yönetici Hesabı** da otomatik olarak oluşturulur.

### 2. Firma Yönetimi (Firma Admin)
Oluşturduğunuz fimanın yönetici bilgileri ile giriş yaptıktan sonra:
- **Çalışanlar:** Firmanıza ait yeni çalışan hesapları ekleyebilir ve yetkilendirebilirsiniz.
- **Firma Ayarları:** Fatura başlığında görünecek Logo, Banka/IBAN ve adres bilgilerini düzenleyebilirsiniz.
- **Muhasebe İşlemleri:** Sadece kendi firmanıza ait fatura, stok, cari, kasa ve banka işlemlerini yürütebilirsiniz.

### 3. E-Fatura Süreci
1.  **Cari & Ürün Tanımla:** Fatura kesmeden önce ilgili kartları oluşturun.
2.  **Fatura Oluştur:** Taslak olarak faturayı kaydedin.
3.  **GİB Gönderimi:** Fatura detayından **Gönderim Şekli** (Temel/Ticari) seçip "GİB'e Gönder" butonuna basın. Kayıt resmileşecek, ETTN ve QR kod otomatik üretilecektir.

## 🔌 API Özeti

-   **Kimlik Doğrulama:** `POST /api/auth/login`, `/register`, `/refresh`
-   **Firma Ayarları:** `GET/PUT /api/companysettings`
-   **Cari Yönetimi:** `GET/POST/PUT/DELETE /api/customers`
-   **Ürün/Stok:** `GET/POST/PUT/DELETE /api/products`
-   **Faturalar:** `GET/POST/PUT /api/invoices`, `PATCH /api/invoices/{id}/status`
-   **e-Fatura İşlemleri:** `POST /api/invoices/{id}/send-to-gib`, `GET /api/invoices/{id}/pdf`
-   **Finansal Modüller:** `/api/bankaccounts`, `/api/cashregisters`, `/api/orders`, `/api/deliverynotes`

---
*Geliştirme süreci devam etmektedir.*