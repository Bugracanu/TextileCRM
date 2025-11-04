# Textile CRM - API Dokümantasyonu

## 🚀 API Erişimi

Projeyi çalıştırdıktan sonra aşağıdaki adreslerden API'lere erişebilirsiniz:

- **Swagger UI**: `https://localhost:{port}/api-docs`
- **Swagger JSON**: `https://localhost:{port}/swagger/v1/swagger.json`
- **API Base URL**: `https://localhost:{port}/api/`

## 🔐 Kimlik Doğrulama

Tüm API endpoint'leri `[Authorize]` attribute'u ile korunmaktadır. API'leri kullanmak için önce giriş yapmanız gerekmektedir.

---

## 📋 API Endpoint'leri

### 1. Customers API (`/api/customersapi`)

#### GET `/api/customersapi`
- **Açıklama**: Tüm müşterileri listeler
- **Response**: `200 OK` - Müşteri listesi

#### GET `/api/customersapi/{id}`
- **Açıklama**: ID'ye göre müşteri getirir
- **Response**: 
  - `200 OK` - Müşteri bilgisi
  - `404 Not Found` - Müşteri bulunamadı

#### GET `/api/customersapi/search?searchTerm={term}`
- **Açıklama**: İsme, şirket adına veya email'e göre müşteri arar
- **Query Parameters**: 
  - `searchTerm` (string): Arama terimi
- **Response**: `200 OK` - Filtrelenmiş müşteri listesi

#### POST `/api/customersapi`
- **Açıklama**: Yeni müşteri oluşturur
- **Request Body**: Customer objesi
- **Response**: 
  - `201 Created` - Oluşturulan müşteri
  - `400 Bad Request` - Geçersiz veri

#### PUT `/api/customersapi/{id}`
- **Açıklama**: Müşteri bilgilerini günceller
- **Request Body**: Customer objesi
- **Response**: 
  - `204 No Content` - Başarılı güncelleme
  - `400 Bad Request` - ID uyuşmazlığı
  - `404 Not Found` - Müşteri bulunamadı

#### DELETE `/api/customersapi/{id}`
- **Açıklama**: Müşteri siler
- **Response**: 
  - `204 No Content` - Başarılı silme
  - `404 Not Found` - Müşteri bulunamadı

---

### 2. Products API (`/api/productsapi`)

#### GET `/api/productsapi`
- **Açıklama**: Tüm ürünleri listeler
- **Response**: `200 OK` - Ürün listesi

#### GET `/api/productsapi/{id}`
- **Açıklama**: ID'ye göre ürün getirir
- **Response**: 
  - `200 OK` - Ürün bilgisi
  - `404 Not Found` - Ürün bulunamadı

#### GET `/api/productsapi/low-stock?threshold={value}`
- **Açıklama**: Düşük stok seviyesindeki ürünleri listeler
- **Query Parameters**: 
  - `threshold` (int, default=10): Stok eşik değeri
- **Response**: `200 OK` - Düşük stoklu ürün listesi

#### GET `/api/productsapi/search?searchTerm={term}`
- **Açıklama**: İsme, açıklamaya veya koda göre ürün arar
- **Query Parameters**: 
  - `searchTerm` (string): Arama terimi
- **Response**: `200 OK` - Filtrelenmiş ürün listesi

#### POST `/api/productsapi`
- **Açıklama**: Yeni ürün oluşturur
- **Request Body**: Product objesi
- **Response**: 
  - `201 Created` - Oluşturulan ürün
  - `400 Bad Request` - Geçersiz veri

#### PUT `/api/productsapi/{id}`
- **Açıklama**: Ürün bilgilerini günceller
- **Request Body**: Product objesi
- **Response**: 
  - `204 No Content` - Başarılı güncelleme
  - `400 Bad Request` - ID uyuşmazlığı
  - `404 Not Found` - Ürün bulunamadı

#### PATCH `/api/productsapi/{id}/stock`
- **Açıklama**: Stok miktarını günceller
- **Request Body**: integer (yeni stok miktarı)
- **Response**: 
  - `204 No Content` - Başarılı güncelleme
  - `404 Not Found` - Ürün bulunamadı

#### DELETE `/api/productsapi/{id}`
- **Açıklama**: Ürün siler
- **Response**: 
  - `204 No Content` - Başarılı silme
  - `404 Not Found` - Ürün bulunamadı

---

### 3. Orders API (`/api/ordersapi`)

#### GET `/api/ordersapi`
- **Açıklama**: Tüm siparişleri listeler
- **Response**: `200 OK` - Sipariş listesi

#### GET `/api/ordersapi/{id}`
- **Açıklama**: ID'ye göre sipariş getirir
- **Response**: 
  - `200 OK` - Sipariş bilgisi
  - `404 Not Found` - Sipariş bulunamadı

#### GET `/api/ordersapi/customer/{customerId}`
- **Açıklama**: Müşteriye ait siparişleri listeler
- **Response**: 
  - `200 OK` - Sipariş listesi
  - `404 Not Found` - Müşteri bulunamadı

#### GET `/api/ordersapi/status/{status}`
- **Açıklama**: Duruma göre siparişleri filtreler
- **Path Parameters**: 
  - `status` (OrderStatus): Sipariş durumu (New, Pending, Confirmed, Processing, InProduction, Completed, Delivered, Cancelled)
- **Response**: `200 OK` - Filtrelenmiş sipariş listesi

#### GET `/api/ordersapi/date-range?startDate={start}&endDate={end}`
- **Açıklama**: Tarih aralığına göre siparişleri filtreler
- **Query Parameters**: 
  - `startDate` (DateTime): Başlangıç tarihi
  - `endDate` (DateTime): Bitiş tarihi
- **Response**: `200 OK` - Filtrelenmiş sipariş listesi

#### POST `/api/ordersapi`
- **Açıklama**: Yeni sipariş oluşturur
- **Request Body**: Order objesi
- **Response**: 
  - `201 Created` - Oluşturulan sipariş
  - `400 Bad Request` - Geçersiz veri

#### PUT `/api/ordersapi/{id}`
- **Açıklama**: Sipariş bilgilerini günceller
- **Request Body**: Order objesi
- **Response**: 
  - `204 No Content` - Başarılı güncelleme
  - `400 Bad Request` - ID uyuşmazlığı
  - `404 Not Found` - Sipariş bulunamadı

#### PATCH `/api/ordersapi/{id}/status`
- **Açıklama**: Sipariş durumunu günceller
- **Request Body**: OrderStatus (enum)
- **Response**: 
  - `200 OK` - Güncellenmiş sipariş
  - `404 Not Found` - Sipariş bulunamadı

#### DELETE `/api/ordersapi/{id}`
- **Açıklama**: Sipariş siler
- **Response**: 
  - `204 No Content` - Başarılı silme
  - `404 Not Found` - Sipariş bulunamadı

---

### 4. Employees API (`/api/employeesapi`)

#### GET `/api/employeesapi`
- **Açıklama**: Tüm çalışanları listeler
- **Response**: `200 OK` - Çalışan listesi

#### GET `/api/employeesapi/{id}`
- **Açıklama**: ID'ye göre çalışan getirir
- **Response**: 
  - `200 OK` - Çalışan bilgisi
  - `404 Not Found` - Çalışan bulunamadı

#### GET `/api/employeesapi/active`
- **Açıklama**: Aktif çalışanları listeler (TerminationDate null olanlar)
- **Response**: `200 OK` - Aktif çalışan listesi

#### GET `/api/employeesapi/department/{department}`
- **Açıklama**: Departmana göre çalışanları filtreler
- **Path Parameters**: 
  - `department` (Department): Departman (Management, Sales, Production, Cutting, Sewing, Packaging, Warehouse, Accounting, HumanResources)
- **Response**: `200 OK` - Filtrelenmiş çalışan listesi

#### GET `/api/employeesapi/search?searchTerm={term}`
- **Açıklama**: İsme, soyisme, email'e veya departmana göre çalışan arar
- **Query Parameters**: 
  - `searchTerm` (string): Arama terimi
- **Response**: `200 OK` - Filtrelenmiş çalışan listesi

#### POST `/api/employeesapi`
- **Açıklama**: Yeni çalışan oluşturur
- **Request Body**: Employee objesi
- **Response**: 
  - `201 Created` - Oluşturulan çalışan
  - `400 Bad Request` - Geçersiz veri

#### PUT `/api/employeesapi/{id}`
- **Açıklama**: Çalışan bilgilerini günceller
- **Request Body**: Employee objesi
- **Response**: 
  - `204 No Content` - Başarılı güncelleme
  - `400 Bad Request` - ID uyuşmazlığı
  - `404 Not Found` - Çalışan bulunamadı

#### DELETE `/api/employeesapi/{id}`
- **Açıklama**: Çalışan siler
- **Response**: 
  - `204 No Content` - Başarılı silme
  - `404 Not Found` - Çalışan bulunamadı

---

### 5. WorkLogs API (`/api/worklogsapi`)

#### GET `/api/worklogsapi`
- **Açıklama**: Tüm çalışma kayıtlarını listeler
- **Response**: `200 OK` - Çalışma kaydı listesi

#### GET `/api/worklogsapi/{id}`
- **Açıklama**: ID'ye göre çalışma kaydı getirir
- **Response**: 
  - `200 OK` - Çalışma kaydı bilgisi
  - `404 Not Found` - Çalışma kaydı bulunamadı

#### GET `/api/worklogsapi/employee/{employeeId}`
- **Açıklama**: Çalışana ait kayıtları listeler
- **Response**: 
  - `200 OK` - Çalışma kaydı listesi
  - `404 Not Found` - Çalışan bulunamadı

#### GET `/api/worklogsapi/date-range?startDate={start}&endDate={end}`
- **Açıklama**: Tarih aralığına göre çalışma kayıtlarını filtreler
- **Query Parameters**: 
  - `startDate` (DateTime): Başlangıç tarihi
  - `endDate` (DateTime): Bitiş tarihi
- **Response**: `200 OK` - Filtrelenmiş çalışma kaydı listesi

#### GET `/api/worklogsapi/employee/{employeeId}/total-hours?startDate={start}&endDate={end}`
- **Açıklama**: Çalışanın toplam çalışma saatini hesaplar
- **Query Parameters**: 
  - `startDate` (DateTime, optional): Başlangıç tarihi
  - `endDate` (DateTime, optional): Bitiş tarihi
- **Response**: 
  - `200 OK` - Toplam saat bilgisi
  - `404 Not Found` - Çalışan bulunamadı

#### POST `/api/worklogsapi`
- **Açıklama**: Yeni çalışma kaydı oluşturur
- **Request Body**: WorkLog objesi
- **Response**: 
  - `201 Created` - Oluşturulan çalışma kaydı
  - `400 Bad Request` - Geçersiz veri

#### PUT `/api/worklogsapi/{id}`
- **Açıklama**: Çalışma kaydını günceller
- **Request Body**: WorkLog objesi
- **Response**: 
  - `204 No Content` - Başarılı güncelleme
  - `400 Bad Request` - ID uyuşmazlığı
  - `404 Not Found` - Çalışma kaydı bulunamadı

#### DELETE `/api/worklogsapi/{id}`
- **Açıklama**: Çalışma kaydını siler
- **Response**: 
  - `204 No Content` - Başarılı silme
  - `404 Not Found` - Çalışma kaydı bulunamadı

---

### 6. Dashboard API (`/api/dashboardapi`)

#### GET `/api/dashboardapi/statistics`
- **Açıklama**: Genel istatistikleri getirir
- **Response**: `200 OK` - İstatistik verisi
  ```json
  {
    "totalOrders": 150,
    "totalRevenue": 250000.00,
    "pendingOrders": 25,
    "completedOrders": 100,
    "totalCustomers": 50,
    "totalProducts": 200,
    "lowStockProducts": 15,
    "activeEmployees": 30,
    "generatedAt": "2024-11-04T12:00:00"
  }
  ```

#### GET `/api/dashboardapi/sales-summary?startDate={start}&endDate={end}`
- **Açıklama**: Satış özeti raporunu getirir
- **Query Parameters**: 
  - `startDate` (DateTime, optional): Başlangıç tarihi
  - `endDate` (DateTime, optional): Bitiş tarihi
- **Response**: `200 OK` - Satış özeti

#### GET `/api/dashboardapi/monthly-revenue?year={year}`
- **Açıklama**: Aylık gelir raporunu getirir
- **Query Parameters**: 
  - `year` (int, optional): Yıl (default: geçerli yıl)
- **Response**: `200 OK` - Aylık gelir raporu

#### GET `/api/dashboardapi/top-customers?limit={limit}`
- **Açıklama**: En iyi müşterileri listeler
- **Query Parameters**: 
  - `limit` (int, default=10): Listeleme limiti
- **Response**: `200 OK` - En iyi müşteri listesi

#### GET `/api/dashboardapi/product-performance`
- **Açıklama**: Ürün performans raporunu getirir
- **Response**: `200 OK` - Ürün performans verisi

#### GET `/api/dashboardapi/order-trends?days={days}`
- **Açıklama**: Sipariş trendlerini analiz eder
- **Query Parameters**: 
  - `days` (int, default=30): Geriye dönük gün sayısı
- **Response**: `200 OK` - Sipariş trend verisi

#### GET `/api/dashboardapi/production-status`
- **Açıklama**: Üretim durumu raporunu getirir
- **Response**: `200 OK` - Üretim durumu verisi

#### GET `/api/dashboardapi/employee-performance?startDate={start}&endDate={end}`
- **Açıklama**: Çalışan performans raporunu getirir
- **Query Parameters**: 
  - `startDate` (DateTime, optional): Başlangıç tarihi
  - `endDate` (DateTime, optional): Bitiş tarihi
- **Response**: `200 OK` - Çalışan performans verisi

---

## 📝 Veri Modelleri

### OrderStatus Enum
- `New` - Yeni
- `Pending` - Beklemede
- `Confirmed` - Onaylandı
- `Processing` - İşleniyor
- `InProduction` - Üretimde
- `Completed` - Tamamlandı
- `Delivered` - Teslim Edildi
- `Cancelled` - İptal Edildi

### Department Enum
- `Management` - Yönetim
- `Sales` - Satış
- `Production` - Üretim
- `Cutting` - Kesim
- `Sewing` - Dikiş
- `Packaging` - Paketleme
- `Warehouse` - Depo
- `Accounting` - Muhasebe
- `HumanResources` - İnsan Kaynakları

### ProductCategory Enum
- `Fabric` - Kumaş
- `Thread` - İplik
- `Button` - Düğme
- `Zipper` - Fermuar
- `Accessory` - Aksesuar
- `FinishedProduct` - Bitmiş Ürün
- `Other` - Diğer

---

## 🧪 Örnek Kullanım

### cURL ile Müşteri Ekleme
```bash
curl -X POST "https://localhost:5001/api/customersapi" \
  -H "Content-Type: application/json" \
  -H "Cookie: .AspNetCore.Cookies=YOUR_AUTH_COOKIE" \
  -d '{
    "name": "Ahmet Yılmaz",
    "companyName": "ABC Tekstil",
    "email": "ahmet@abctekstil.com",
    "phone": "0555 123 4567",
    "address": "İstanbul, Türkiye"
  }'
```

### JavaScript ile İstatistik Çekme
```javascript
fetch('https://localhost:5001/api/dashboardapi/statistics', {
  method: 'GET',
  credentials: 'include',
  headers: {
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

---

## ✅ Sonraki Adımlar

Şu anda aşağıdaki API'ler tamamen kullanıma hazır:
- ✅ RESTful CRUD API'leri (Customers, Products, Orders, Employees, WorkLogs)
- ✅ Dashboard ve Raporlama API'leri
- ✅ Swagger/OpenAPI Dokümantasyonu

### Gelecekte Eklenebilecek API'ler:
- 📧 Email Bildirimleri API'si
- 📄 Fatura ve Ödeme API'si
- 📁 Dosya Yükleme/İndirme API'si
- 📊 Gelişmiş Analitik API'si
- 🔔 Gerçek Zamanlı Bildirim API'si

---

**Not**: Swagger UI'da tüm endpoint'leri test edebilir ve detaylı request/response örnekleri görebilirsiniz.

