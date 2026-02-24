# HBT Fatura

Fatura yönetim uygulaması: ASP.NET Core 8 (backend) + Angular 17 (frontend) + MSSQL (Code First).

## Gereksinimler

- .NET 8 SDK
- Node.js 18+
- SQL Server LocalDB (Windows) veya SQL Server / SQL Express

## Çalıştırma

### Backend (API)

```bash
cd src/Backend/HbtFatura.Api
dotnet run
```

API varsayılan olarak https://localhost:7270 üzerinde çalışır. İlk çalıştırmada veritabanı migration ile oluşturulur.

### Frontend (Angular)

```bash
cd src/frontend
npm install
npm start
```

Tarayıcıda http://localhost:4200 açın. API URL'i `src/frontend/src/environments/environment.development.ts` içinde `apiUrl: 'https://localhost:7270'` olarak ayarlıdır.

### İlk kullanım

1. **Kayıt ol**: http://localhost:4200/register — e-posta, şifre ve ad soyad ile hesap oluşturun.
2. **Giriş**: Login sayfasından giriş yapın.
3. **Firma bilgileri**: Firma menüsünden fatura başlığında kullanılacak firma bilgilerini girin.
4. **Müşteriler**: Müşteri ekleyin; fatura oluştururken listeden seçebilir veya manuel girebilirsiniz.
5. **Faturalar**: Yeni fatura ekleyin; kalemlerde sadece miktar, birim fiyat ve KDV % girin — toplamlar backend'de hesaplanır. PDF indirebilirsiniz.

## Proje yapısı

- `src/Backend/HbtFatura.Api` — ASP.NET Core Web API, EF Core Code First, JWT + refresh token, Customers, Invoices, CompanySettings, PDF export
- `src/frontend` — Angular 17 SPA, auth guard, HTTP interceptor, fatura/müşteri/firma sayfaları

## API özeti

- `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/revoke`
- `GET/PUT /api/companysettings` (Authorization: Bearer)
- `GET/POST/GET id/PUT id/DELETE id /api/customers`, `GET /api/customers/dropdown`
- `GET/POST/GET id/PUT id/PATCH id/status /api/invoices`, `GET /api/invoices/{id}/pdf`
