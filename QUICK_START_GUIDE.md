# 🚀 Textile CRM - Hızlı Başlangıç Rehberi

## ✅ Uygulama Çalışıyor!

### 🌐 Erişim Adresleri

**Ana Sayfa:**
```
https://localhost:7000
```

**Swagger API Dokümantasyonu:**
```
https://localhost:7000/api-docs
```

---

## 🔐 Giriş Bilgileri

```
Kullanıcı Adı: admin
Şifre: admin123
```

```
Kullanıcı Adı: manager
Şifre: manager123
```

```
Kullanıcı Adı: user
Şifre: user123
```

---

## 📋 Sol Menüde Bulunan Sayfalar

### 📊 Ana Menü
- ✅ **Dashboard** - KPI kartları, grafikler, real-time veriler
- ✅ **Müşteriler** - Müşteri yönetimi
- ✅ **Siparişler** - Sipariş takibi
- ✅ **Ürünler** - Ürün katalog
- ✅ **Çalışanlar** - Personel yönetimi
- ✅ **Mesai Kayıtları** - Çalışma saatleri

### 💰 Finans
- ✅ **Faturalar** (`/Invoice`) - API'den canlı veri
- ✅ **Ödemeler** (`/Payment`) - Ödeme işlemleri

### ⚙️ Sistem
- ✅ **Bildirimler** (`/Notifications`) - Real-time bildirimler
- ✅ **Stok Uyarıları** (`/StockAlerts`) - Otomatik stok kontrolü
- ✅ **Dosyalar** (`/Files`) - Dosya yükleme/indirme

### 📈 Raporlar
- ✅ **Satış Raporları** (`/Reports/Sales`) - Satış analizleri
- ✅ **Finansal Raporlar** (`/Reports/Financial`) - Kar/zarar, nakit akışı
- ✅ **Müşteri Analizleri** (`/Reports/Customers`) - Segmentasyon, CLV, churn

### 🔗 API
- ✅ **API Dokümantasyonu** - Swagger UI

---

## 🎯 Dashboard Özellikleri

### 1. **Dinamik KPI Kartları** (API'den Gerçek Zamanlı)
- 💰 Aylık Gelir (değişim % ile)
- 🛒 Toplam Sipariş (trend göstergesi)
- 👥 Yeni Müşteriler (aylık)
- ⏰ Bekleyen Siparişler

### 2. **İnteraktif Grafikler** (Chart.js)
- 📈 Satış Trendi (30 günlük line chart)
- 🥧 Sipariş Durumu Dağılımı (doughnut chart)

### 3. **Real-time Widget'lar**
- ⚠️ Aktif Stok Uyarıları (son 5)
- 🏆 En Çok Satan Ürünler (top 5)
- 📋 Son Siparişler

### 4. **Otomatik Güncellemeler**
- Badge'ler her 30 saniyede güncellenir
- KPI kartları her 60 saniyede güncellenir

---

## 📊 API Kullanım Örnekleri

### Dashboard API'leri
```javascript
// KPI Kartları
GET /api/dashboardapi/kpi-cards

// Satış Trendi
GET /api/dashboardapi/sales-chart-data?days=30

// Sipariş Durumu
GET /api/dashboardapi/order-status-distribution

// En Çok Satanlar
GET /api/dashboardapi/top-selling-products?limit=5

// Real-time Stats
GET /api/dashboardapi/realtime-stats
```

### Fatura & Ödeme API'leri
```javascript
// Fatura Listesi
GET /api/invoicesapi

// Ödeme Kaydet
POST /api/paymentsapi
{
  "invoiceId": 1,
  "amount": 5000,
  "paymentMethod": 2,
  "paymentDate": "2024-11-04"
}

// Fatura Email Gönder
POST /api/invoicesapi/1/send-email
```

### Bildirim API'leri
```javascript
// Okunmamış Bildirimler
GET /api/notificationsapi/my-notifications/unread

// Okunmamış Sayısı
GET /api/notificationsapi/my-notifications/unread-count

// Tümünü Okundu İşaretle
POST /api/notificationsapi/mark-all-as-read
```

### Stok Uyarı API'leri
```javascript
// Aktif Uyarılar
GET /api/stockalertsapi/active

// Tüm Stokları Kontrol Et
POST /api/stockalertsapi/check-all

// Uyarıyı Çöz
POST /api/stockalertsapi/1/resolve
```

---

## 🎨 Yeni Eklenen Özellikler

### 1. Sidebar Navigasyon
- ✅ Kategorize edilmiş menü yapısı
- ✅ Real-time badge'ler (bildirim, stok uyarısı)
- ✅ Hover efektleri
- ✅ Aktif sayfa göstergesi

### 2. API Entegrasyonlu Sayfalar
- ✅ Faturalar sayfası (CRUD işlemleri)
- ✅ Ödemeler sayfası (işleme alma özelliği)
- ✅ Bildirimler sayfası (okundu işaretleme)
- ✅ Stok Uyarıları sayfası (otomatik kontrol)
- ✅ Dosyalar sayfası (yükleme/indirme)
- ✅ 3 farklı rapor sayfası

### 3. Dashboard İyileştirmeleri
- ✅ API'den dinamik KPI kartları
- ✅ 2 farklı interaktif grafik
- ✅ Real-time stok uyarıları
- ✅ En çok satan ürünler listesi
- ✅ Otomatik güncellemeler

### 4. Real-time Özellikler
- ✅ Bildirim badge'i (30 saniyede bir güncellenir)
- ✅ Stok uyarı badge'i (30 saniyede bir güncellenir)
- ✅ KPI kartları (60 saniyede bir güncellenir)

---

## 🎯 Test Senaryoları

### Senaryo 1: Dashboard'u İnceleyin
1. Ana sayfaya gidin: `https://localhost:7000`
2. Üstteki 4 KPI kartını görün (API'den gelir)
3. Satış trendi grafiğini inceleyin
4. Sipariş durumu dağılımını görün
5. Stok uyarılarını kontrol edin

### Senaryo 2: Stok Uyarılarını Test Edin
1. Sol menüden "Stok Uyarıları"na tıklayın
2. "Tüm Stokları Kontrol Et" butonuna basın
3. Oluşan uyarıları görün
4. Bir uyarıyı "Çöz" butonuyla çözün

### Senaryo 3: Bildirimleri Görün
1. Sol menüden "Bildirimler"e tıklayın
2. Varsa okunmamış bildirimleri görün
3. "Tümünü Okundu İşaretle" butonunu deneyin
4. Badge'in kaybolduğunu görün

### Senaryo 4: Raporları İnceleyin
1. Sol menüden "Satış Raporları"na gidin
2. Tarih filtresi ile rapor oluşturun
3. "Finansal Raporlar"a gidin
4. Kar/zarar analizini görün
5. "Müşteri Analizleri"nde segmentasyonu inceleyin

---

## 🔧 Sorun Giderme

### Uygulama Çalışmıyorsa:
```bash
cd TextileCRM.WebUI
dotnet run --launch-profile https
```

### Uygulama Yavaş Çalışıyorsa:
```bash
# Uygulamayı durdur
Ctrl+C

# Temizle ve yeniden başlat
dotnet clean
dotnet build
dotnet run --launch-profile https
```

### API Hata Veriyorsa:
- Swagger UI'da API'leri test edin: `/api-docs`
- Browser Console'da hataları kontrol edin (F12)
- Giriş yapmayı unutmayın!

---

## 📚 Kullanılan Teknolojiler

- ✅ ASP.NET Core 8.0 (MVC + Web API)
- ✅ Entity Framework Core 9.0
- ✅ SQL Server
- ✅ Swagger/OpenAPI
- ✅ Chart.js 4.4.0
- ✅ Bootstrap 5
- ✅ Bootstrap Icons
- ✅ jQuery

---

## 🎊 Tamamlanan Özellikler

### Backend (API)
- ✅ 113+ API Endpoint
- ✅ 12 Entity (Domain Model)
- ✅ 13 Service (Business Logic)
- ✅ 12 API Controller
- ✅ Repository Pattern
- ✅ Swagger Dokümantasyonu

### Frontend (Web UI)
- ✅ Sidebar navigasyon
- ✅ Dashboard (API entegrasyonlu)
- ✅ 8 yeni sayfa (Fatura, Ödeme, Bildirim vs.)
- ✅ Chart.js grafikleri
- ✅ Real-time güncellemeler
- ✅ Responsive tasarım

### Özellikler
- ✅ Fatura & Ödeme yönetimi
- ✅ Dosya yükleme/indirme
- ✅ Bildirim sistemi
- ✅ Otomatik stok uyarıları
- ✅ Kapsamlı raporlama
- ✅ Email servisi (mock)
- ✅ Real-time badge güncellemeleri

---

## 🎉 Her Şey Hazır!

Textile CRM projeniz **kurumsal seviyede** bir sistem haline geldi!

### Öne Çıkan Özellikler:
1. 🎨 Modern ve responsive UI
2. 📊 Canlı grafikler ve KPI kartları
3. 🔔 Real-time bildirimler
4. 📦 Otomatik stok takibi
5. 💰 Tam finansal yönetim
6. 📈 Gelişmiş raporlama
7. 📁 Dosya yönetimi
8. 🔗 113+ API endpoint

**İyi çalışmalar! 🚀**

---

## 💡 İpuçları

1. **API Dokümantasyonu**: Swagger UI'da tüm endpoint'leri test edebilirsiniz
2. **Real-time Güncellemeler**: Sayfa açık kaldığı sürece veriler otomatik güncellenir
3. **Stok Kontrolü**: Dashboard'da veya Stok Uyarıları sayfasında toplu kontrol yapabilirsiniz
4. **Grafikler**: Chart.js ile oluşturulmuş, interaktif ve responsive
5. **Email Servisi**: Şu anda console'a yazdırıyor, production'da SMTP entegrasyonu yapılabilir

---

**Projenizin tadını çıkarın! 🎊**

