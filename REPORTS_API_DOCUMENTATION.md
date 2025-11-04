# Textile CRM - Gelişmiş Raporlama API Dokümantasyonu

## 📊 Yeni Eklenen Raporlama API'leri

### 🔗 Erişim
- **Reports API Base**: `/api/reportsapi`
- **Dashboard API Base**: `/api/dashboardapi`

---

## 1. 💰 Finansal Raporlar (`/api/reportsapi/financial/...`)

### GET `/api/reportsapi/financial/summary`
**Detaylı Finansal Özet Raporu**

Query Parameters:
- `startDate` (DateTime, optional): Başlangıç tarihi
- `endDate` (DateTime, optional): Bitiş tarihi

Response:
```json
{
  "period": { "startDate": "2024-01-01", "endDate": "2024-12-31" },
  "summary": {
    "totalRevenue": 500000.00,
    "totalOrders": 250,
    "averageOrderValue": 2000.00,
    "pendingValue": 50000.00,
    "cancelledValue": 10000.00,
    "completionRate": 85.5
  },
  "monthlyTrend": [...],
  "weeklyTrend": [...]
}
```

### GET `/api/reportsapi/financial/profit-loss`
**Kar/Zarar Raporu**

İşçilik maliyeti dahil temel kar/zarar hesaplaması.

### GET `/api/reportsapi/financial/cash-flow`
**Nakit Akışı Raporu**

Query Parameters:
- `months` (int, default=6): Kaç aylık veri

---

## 2. 👥 Müşteri Analiz Raporları (`/api/reportsapi/customers/...`)

### GET `/api/reportsapi/customers/segmentation`
**Müşteri Segmentasyon Analizi**

Müşterileri VIP, Premium, Standard ve New olarak kategorize eder.

Response:
```json
{
  "totalCustomers": 150,
  "activeCustomers": 120,
  "inactiveCustomers": 30,
  "segmentSummary": [
    {
      "segment": "VIP",
      "customerCount": 15,
      "totalRevenue": 250000.00,
      "averageOrderValue": 5000.00
    }
  ],
  "customerDetails": [...]
}
```

### GET `/api/reportsapi/customers/churn-analysis`
**Müşteri Kayıp (Churn) Analizi**

Query Parameters:
- `inactiveDays` (int, default=90): Aktif olmama eşiği (gün)

Müşterileri "Active", "Warning", "At Risk" olarak sınıflandırır.

### GET `/api/reportsapi/customers/lifetime-value`
**Customer Lifetime Value (CLV) Analizi**

3 yıllık tahmin ile müşteri yaşam boyu değerini hesaplar.

---

## 3. 🏭 Üretim ve Operasyonel Raporlar (`/api/reportsapi/operations/...`)

### GET `/api/reportsapi/operations/production-efficiency`
**Üretim Verimliliği Raporu**

Query Parameters:
- `startDate` (DateTime, optional)
- `endDate` (DateTime, optional)

Response:
```json
{
  "period": {...},
  "summary": {
    "totalOrders": 200,
    "completedOrders": 180,
    "completionRate": 90.00,
    "totalWorkHours": 1600.50,
    "ordersPerHour": 0.1125,
    "averageCompletionTimeDays": 15.5
  },
  "statusDistribution": [...]
}
```

### GET `/api/reportsapi/operations/delivery-performance`
**Sipariş Teslim Performansı**

Zamanında ve geç teslimat analizi.

---

## 4. 👨‍💼 Çalışan ve İK Raporları (`/api/reportsapi/hr/...`)

### GET `/api/reportsapi/hr/employee-productivity`
**Çalışan Verimlilik Raporu**

Query Parameters:
- `startDate` (DateTime, optional)
- `endDate` (DateTime, optional)

Response:
```json
{
  "period": {...},
  "summary": {
    "totalEmployees": 50,
    "totalWorkHours": 8000.00,
    "avgWorkHoursPerEmployee": 160.00
  },
  "departmentStats": [...],
  "topPerformers": [...],
  "allEmployees": [...]
}
```

### GET `/api/reportsapi/hr/department-comparison`
**Departman Performans Karşılaştırması**

Tüm departmanları karşılaştırır.

---

## 5. 📦 Stok ve Envanter Raporları (`/api/reportsapi/inventory/...`)

### GET `/api/reportsapi/inventory/stock-status`
**Detaylı Stok Durumu Raporu**

Response:
```json
{
  "summary": {
    "totalProducts": 500,
    "totalStockValue": 1000000.00,
    "outOfStockCount": 25,
    "lowStockCount": 50
  },
  "categoryBreakdown": [...],
  "stockByStatus": [...],
  "criticalProducts": [...]
}
```

### GET `/api/reportsapi/inventory/stock-movement`
**Stok Hareket Raporu**

Query Parameters:
- `startDate` (DateTime, optional)
- `endDate` (DateTime, optional)

En çok satan ve yavaş hareket eden ürünleri gösterir.

---

## 6. 📊 Karşılaştırmalı Analizler (`/api/reportsapi/comparison/...`)

### GET `/api/reportsapi/comparison/period-over-period`
**Dönemsel Karşılaştırma**

Query Parameters:
- `period` (string, default="month"): "month", "quarter" veya "year"

Mevcut dönem ile bir önceki dönemi karşılaştırır.

### GET `/api/reportsapi/comparison/year-over-year`
**Yıl Bazlı Karşılaştırma**

Bu yıl ile geçen yılı aylık olarak karşılaştırır.

---

## 7. 📈 Dashboard Widget'ları (`/api/dashboardapi/...`)

### GET `/api/dashboardapi/kpi-cards`
**KPI (Key Performance Indicators) Kartları**

Dashboard için 4 temel KPI kartı:
```json
{
  "cards": [
    {
      "title": "Aylık Gelir",
      "value": 125000.00,
      "change": 15.5,
      "trend": "up",
      "icon": "currency",
      "color": "success"
    },
    {...}
  ]
}
```

### GET `/api/dashboardapi/sales-chart-data`
**Satış Trendi Grafik Verisi** (Chart.js uyumlu)

Query Parameters:
- `days` (int, default=30): Kaç günlük veri

Response:
```json
{
  "labels": ["01 Kas", "02 Kas", ...],
  "datasets": [
    {
      "label": "Gelir",
      "data": [1500.00, 2000.00, ...],
      "borderColor": "rgb(75, 192, 192)",
      "backgroundColor": "rgba(75, 192, 192, 0.2)"
    }
  ]
}
```

### GET `/api/dashboardapi/order-status-distribution`
**Sipariş Durumu Dağılımı** (Pie/Doughnut Chart)

Sipariş durumlarının renk kodlu dağılımı.

### GET `/api/dashboardapi/category-stock-distribution`
**Kategori Bazlı Stok Dağılımı**

Ürün kategorilerine göre stok analizi.

### GET `/api/dashboardapi/top-selling-products`
**En Çok Satan Ürünler**

Query Parameters:
- `limit` (int, default=10): Kaç ürün

### GET `/api/dashboardapi/realtime-stats`
**Real-time İstatistikler**

Sık güncellenen, cache edilebilir veriler:
```json
{
  "todayOrders": 5,
  "todayRevenue": 7500.00,
  "activeOrders": 35,
  "urgentOrders": 8,
  "lastUpdated": "2024-11-04T14:30:00"
}
```

### GET `/api/dashboardapi/department-workload`
**Departman İş Yükü Dağılımı**

Departmanların bu ayki çalışma saati analizi.

---

## 🎨 Frontend Entegrasyonu

### Chart.js Örneği
```javascript
// Satış trendi grafiği
fetch('/api/dashboardapi/sales-chart-data?days=30')
  .then(response => response.json())
  .then(data => {
    const ctx = document.getElementById('salesChart').getContext('2d');
    new Chart(ctx, {
      type: 'line',
      data: data,
      options: {
        responsive: true,
        interaction: {
          mode: 'index',
          intersect: false,
        }
      }
    });
  });
```

### KPI Card Örneği
```javascript
// KPI kartlarını göster
fetch('/api/dashboardapi/kpi-cards')
  .then(response => response.json())
  .then(data => {
    data.cards.forEach(card => {
      renderKPICard(card);
    });
  });
```

---

## 📊 Rapor Tipleri Özeti

| Kategori | Endpoint Sayısı | Özellikler |
|----------|----------------|------------|
| **Finansal** | 3 | Gelir, kar/zarar, nakit akışı |
| **Müşteri** | 3 | Segmentasyon, churn, CLV |
| **Operasyon** | 2 | Verimlilik, teslim performansı |
| **İK** | 2 | Çalışan verimliliği, departman karşılaştırma |
| **Stok** | 2 | Stok durumu, hareket analizi |
| **Karşılaştırma** | 2 | Dönemsel, yıllık karşılaştırma |
| **Dashboard** | 7 | Widget'lar, grafikler, real-time |

**Toplam**: 21 yeni gelişmiş rapor endpoint'i

---

## 🚀 Kullanım Senaryoları

### 1. Yönetici Dashboard'u
```
GET /api/dashboardapi/kpi-cards
GET /api/dashboardapi/sales-chart-data
GET /api/dashboardapi/order-status-distribution
GET /api/dashboardapi/realtime-stats
```

### 2. Satış Raporları
```
GET /api/reportsapi/financial/summary
GET /api/reportsapi/customers/segmentation
GET /api/reportsapi/customers/lifetime-value
GET /api/dashboardapi/top-selling-products
```

### 3. Üretim Raporları
```
GET /api/reportsapi/operations/production-efficiency
GET /api/reportsapi/operations/delivery-performance
GET /api/dashboardapi/department-workload
```

### 4. İK Raporları
```
GET /api/reportsapi/hr/employee-productivity
GET /api/reportsapi/hr/department-comparison
GET /api/dashboardapi/employee-performance
```

### 5. Analitik & Karşılaştırma
```
GET /api/reportsapi/comparison/period-over-period
GET /api/reportsapi/comparison/year-over-year
GET /api/reportsapi/customers/churn-analysis
```

---

## 📝 Notlar

1. **Performans**: Büyük veri setleri için cache mekanizması eklenebilir
2. **Yetkilendirme**: Tüm endpoint'ler `[Authorize]` attribute'u ile korunmuştur
3. **Tarih Filtreleri**: Çoğu rapor tarih aralığı parametreleri kabul eder
4. **Chart.js Uyumlu**: Grafik verileri doğrudan Chart.js'e bağlanabilir
5. **Real-time**: `/realtime-stats` endpoint'i cache için uygundur (örn: 30 saniye)

---

## ✅ Tamamlanan Özellikler

- ✅ 21 gelişmiş rapor endpoint'i
- ✅ Chart.js uyumlu veri formatları
- ✅ KPI kartları ve widget'lar
- ✅ Finansal analizler (gelir, kar/zarar, nakit akışı)
- ✅ Müşteri analizi (segmentasyon, churn, CLV)
- ✅ Operasyonel metrikler (verimlilik, teslim performansı)
- ✅ İK raporları (çalışan verimliliği, departman analizi)
- ✅ Stok ve envanter raporları
- ✅ Karşılaştırmalı analizler (dönemsel, yıllık)
- ✅ Real-time istatistikler

---

**Swagger UI'da tüm yeni endpoint'leri test edebilirsiniz**: `/api-docs`

