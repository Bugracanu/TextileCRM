using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DashboardApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkLogService _workLogService;

        public DashboardApiController(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            IEmployeeService employeeService,
            IWorkLogService workLogService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _employeeService = employeeService;
            _workLogService = workLogService;
        }

        /// <summary>
        /// Genel istatistikleri getirir
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetStatistics()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var customers = await _customerService.GetAllCustomersAsync();
            var products = await _productService.GetAllProductsAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();

            var totalOrders = orders.Count();
            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.New);
            var completedOrders = orders.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);
            var totalCustomers = customers.Count();
            var totalProducts = products.Count();
            var lowStockProducts = products.Count(p => p.StockQuantity <= 10);
            var activeEmployees = employees.Count(e => e.TerminationDate == null);

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                pendingOrders,
                completedOrders,
                totalCustomers,
                totalProducts,
                lowStockProducts,
                activeEmployees,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Satış özeti raporunu getirir
        /// </summary>
        [HttpGet("sales-summary")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetSalesSummary(
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var orders = await _orderService.GetAllOrdersAsync();

            // Tarih filtreleme
            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= endDate.Value);
            }

            var totalSales = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count();
            var averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;

            var salesByStatus = orders.GroupBy(o => o.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count(),
                    totalAmount = g.Sum(o => o.TotalAmount)
                });

            return Ok(new
            {
                totalSales,
                totalOrders,
                averageOrderValue,
                salesByStatus,
                startDate,
                endDate,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Aylık gelir raporunu getirir
        /// </summary>
        [HttpGet("monthly-revenue")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetMonthlyRevenue([FromQuery] int? year = null)
        {
            var targetYear = year ?? DateTime.Now.Year;
            var orders = await _orderService.GetAllOrdersAsync();

            var monthlyData = orders
                .Where(o => o.OrderDate.Year == targetYear)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    month = g.Key,
                    monthName = new DateTime(targetYear, g.Key, 1).ToString("MMMM"),
                    revenue = g.Sum(o => o.TotalAmount),
                    orderCount = g.Count()
                })
                .OrderBy(x => x.month)
                .ToList();

            return Ok(new
            {
                year = targetYear,
                totalYearlyRevenue = monthlyData.Sum(m => m.revenue),
                totalYearlyOrders = monthlyData.Sum(m => m.orderCount),
                monthlyBreakdown = monthlyData,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// En iyi müşterileri listeler
        /// </summary>
        [HttpGet("top-customers")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetTopCustomers([FromQuery] int limit = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var customers = await _customerService.GetAllCustomersAsync();

            var topCustomers = orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    customerId = g.Key,
                    customerName = customers.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Unknown",
                    totalOrders = g.Count(),
                    totalSpent = g.Sum(o => o.TotalAmount),
                    lastOrderDate = g.Max(o => o.OrderDate)
                })
                .OrderByDescending(x => x.totalSpent)
                .Take(limit)
                .ToList();

            return Ok(new
            {
                topCustomers,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Ürün performans raporunu getirir
        /// </summary>
        [HttpGet("product-performance")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetProductPerformance()
        {
            var products = await _productService.GetAllProductsAsync();
            var orders = await _orderService.GetAllOrdersAsync();

            var productStats = products.Select(p => new
            {
                productId = p.Id,
                productName = p.Name,
                code = p.Code,
                currentStock = p.StockQuantity,
                unitPrice = p.UnitPrice,
                // Not: OrderItem bilgisi için tam bir analiz yapabilmek için OrderItem'lara erişim gerekli
                totalOrders = orders.Count(o => o.OrderItems.Any(oi => oi.ProductId == p.Id))
            })
            .OrderByDescending(p => p.totalOrders)
            .ToList();

            return Ok(new
            {
                products = productStats,
                lowStockProducts = productStats.Where(p => p.currentStock <= 10),
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Sipariş trendlerini analiz eder
        /// </summary>
        [HttpGet("order-trends")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetOrderTrends([FromQuery] int days = 30)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var startDate = DateTime.Now.AddDays(-days);

            var recentOrders = orders.Where(o => o.OrderDate >= startDate);

            var dailyOrders = recentOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    date = g.Key,
                    orderCount = g.Count(),
                    totalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.date)
                .ToList();

            var totalOrders = recentOrders.Count();
            var totalRevenue = recentOrders.Sum(o => o.TotalAmount);
            var averageDailyOrders = dailyOrders.Any() ? dailyOrders.Average(d => d.orderCount) : 0;
            var averageDailyRevenue = dailyOrders.Any() ? dailyOrders.Average(d => d.totalRevenue) : 0;

            return Ok(new
            {
                period = $"Son {days} gün",
                totalOrders,
                totalRevenue,
                averageDailyOrders,
                averageDailyRevenue,
                dailyBreakdown = dailyOrders,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Üretim durumu raporunu getirir
        /// </summary>
        [HttpGet("production-status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetProductionStatus()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var productionOrders = orders.Where(o => 
                o.Status == OrderStatus.Processing || 
                o.Status == OrderStatus.InProduction);

            var statusBreakdown = productionOrders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count(),
                    totalAmount = g.Sum(o => o.TotalAmount),
                    orders = g.Select(o => new
                    {
                        orderId = o.Id,
                        customerId = o.CustomerId,
                        orderDate = o.OrderDate,
                        totalAmount = o.TotalAmount,
                        deliveryDate = o.DeliveryDate
                    }).ToList()
                });

            return Ok(new
            {
                totalProductionOrders = productionOrders.Count(),
                statusBreakdown,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Çalışan performans raporunu getirir
        /// </summary>
        [HttpGet("employee-performance")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetEmployeePerformance(
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            // Tarih filtreleme
            if (startDate.HasValue)
            {
                workLogs = workLogs.Where(w => w.CheckInTime >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                workLogs = workLogs.Where(w => w.CheckInTime <= endDate.Value);
            }

            var employeeStats = employees.Select(e => new
            {
                employeeId = e.Id,
                employeeName = $"{e.FirstName} {e.LastName}",
                department = e.Department.ToString(),
                position = e.Position,
                totalHours = workLogs.Where(w => w.EmployeeId == e.Id).Sum(w => w.WorkHours ?? 0),
                workDays = workLogs.Where(w => w.EmployeeId == e.Id).Count(),
                isActive = e.TerminationDate == null
            })
            .Where(e => e.totalHours > 0 || e.isActive)
            .OrderByDescending(e => e.totalHours)
            .ToList();

            return Ok(new
            {
                employeePerformance = employeeStats,
                startDate,
                endDate,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Dashboard için KPI (Key Performance Indicators) kartları
        /// </summary>
        [HttpGet("kpi-cards")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetKPICards()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var customers = await _customerService.GetAllCustomersAsync();
            var products = await _productService.GetAllProductsAsync();

            var today = DateTime.Now.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            // Bu ay ve geçen ay verileri
            var thisMonthOrders = orders.Where(o => o.OrderDate >= thisMonth);
            var lastMonthOrders = orders.Where(o => o.OrderDate >= lastMonth && o.OrderDate < thisMonth);

            var thisMonthRevenue = thisMonthOrders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                .Sum(o => o.TotalAmount);
            var lastMonthRevenue = lastMonthOrders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                .Sum(o => o.TotalAmount);

            var revenueChange = lastMonthRevenue > 0 
                ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 
                : 0;

            var thisMonthOrderCount = thisMonthOrders.Count();
            var lastMonthOrderCount = lastMonthOrders.Count();
            var orderChange = lastMonthOrderCount > 0 
                ? ((double)(thisMonthOrderCount - lastMonthOrderCount) / lastMonthOrderCount) * 100 
                : 0;

            var thisMonthCustomers = customers.Count(c => c.CreatedDate >= thisMonth);
            var lastMonthCustomers = customers.Count(c => c.CreatedDate >= lastMonth && c.CreatedDate < thisMonth);
            var customerChange = lastMonthCustomers > 0 
                ? ((double)(thisMonthCustomers - lastMonthCustomers) / lastMonthCustomers) * 100 
                : 0;

            var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.New);
            var totalPendingValue = orders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.New)
                .Sum(o => o.TotalAmount);

            var cards = new List<object>
            {
                new
                {
                    title = "Aylık Gelir",
                    value = thisMonthRevenue,
                    change = Math.Round(revenueChange, 1),
                    trend = revenueChange >= 0 ? "up" : "down",
                    icon = "currency",
                    color = "success"
                },
                new
                {
                    title = "Toplam Sipariş",
                    value = (decimal)thisMonthOrderCount,
                    change = Math.Round(orderChange, 1),
                    trend = orderChange >= 0 ? "up" : "down",
                    icon = "shopping-cart",
                    color = "primary"
                },
                new
                {
                    title = "Yeni Müşteriler",
                    value = (decimal)thisMonthCustomers,
                    change = Math.Round(customerChange, 1),
                    trend = customerChange >= 0 ? "up" : "down",
                    icon = "users",
                    color = "info"
                },
                new
                {
                    title = "Bekleyen Siparişler",
                    value = totalPendingValue,
                    change = Math.Round((double)pendingOrders, 1),
                    trend = "neutral",
                    icon = "clock",
                    color = "warning"
                }
            };

            return Ok(new
            {
                cards,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Grafik için satış trendi verisi (Chart.js uyumlu)
        /// </summary>
        [HttpGet("sales-chart-data")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetSalesChartData([FromQuery] int days = 30)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var startDate = DateTime.Now.AddDays(-days).Date;

            var dailySales = Enumerable.Range(0, days)
                .Select(i =>
                {
                    var date = startDate.AddDays(i);
                    var dayOrders = orders.Where(o => o.OrderDate.Date == date);
                    var revenue = dayOrders
                        .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                        .Sum(o => o.TotalAmount);
                    
                    return new
                    {
                        date = date.ToString("yyyy-MM-dd"),
                        label = date.ToString("dd MMM"),
                        revenue,
                        orderCount = dayOrders.Count()
                    };
                })
                .ToList();

            var datasets = new List<object>
            {
                new
                {
                    label = "Gelir",
                    data = dailySales.Select(d => d.revenue).ToArray(),
                    borderColor = "rgb(75, 192, 192)",
                    backgroundColor = "rgba(75, 192, 192, 0.2)",
                    tension = 0.1
                },
                new
                {
                    label = "Sipariş Sayısı",
                    data = dailySales.Select(d => (decimal)d.orderCount).ToArray(),
                    borderColor = "rgb(255, 99, 132)",
                    backgroundColor = "rgba(255, 99, 132, 0.2)",
                    tension = 0.1,
                    yAxisID = "y1"
                }
            };

            return Ok(new
            {
                labels = dailySales.Select(d => d.label).ToArray(),
                datasets,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Sipariş durumu dağılımı (Pie/Doughnut Chart için)
        /// </summary>
        [HttpGet("order-status-distribution")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetOrderStatusDistribution()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var statusDistribution = orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count(),
                    value = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(s => s.count)
                .ToList();

            var colors = new Dictionary<string, string>
            {
                { "New", "#17a2b8" },
                { "Pending", "#ffc107" },
                { "Confirmed", "#20c997" },
                { "Processing", "#fd7e14" },
                { "InProduction", "#6f42c1" },
                { "Completed", "#28a745" },
                { "Delivered", "#007bff" },
                { "Cancelled", "#dc3545" }
            };

            return Ok(new
            {
                labels = statusDistribution.Select(s => s.status).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        data = statusDistribution.Select(s => s.count).ToArray(),
                        backgroundColor = statusDistribution
                            .Select(s => colors.ContainsKey(s.status) ? colors[s.status] : "#6c757d")
                            .ToArray()
                    }
                },
                details = statusDistribution,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Kategori bazlı ürün stok dağılımı
        /// </summary>
        [HttpGet("category-stock-distribution")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCategoryStockDistribution()
        {
            var products = await _productService.GetAllProductsAsync();

            var categoryDistribution = products
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    category = g.Key.ToString(),
                    productCount = g.Count(),
                    totalStock = g.Sum(p => p.StockQuantity),
                    totalValue = g.Sum(p => p.StockQuantity * p.UnitPrice)
                })
                .OrderByDescending(c => c.totalValue)
                .ToList();

            return Ok(new
            {
                labels = categoryDistribution.Select(c => c.category).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Stok Değeri",
                        data = categoryDistribution.Select(c => c.totalValue).ToArray(),
                        backgroundColor = "rgba(54, 162, 235, 0.6)"
                    },
                    new
                    {
                        label = "Stok Miktarı",
                        data = categoryDistribution.Select(c => (decimal)c.totalStock).ToArray(),
                        backgroundColor = "rgba(255, 206, 86, 0.6)"
                    }
                },
                details = categoryDistribution,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// En çok satan ürünler (Top 10)
        /// </summary>
        [HttpGet("top-selling-products")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetTopSellingProducts([FromQuery] int limit = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var products = await _productService.GetAllProductsAsync();

            var productSales = products.Select(p =>
            {
                var totalQuantity = orders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductId == p.Id)
                    .Sum(oi => oi.Quantity);

                var totalRevenue = orders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductId == p.Id)
                    .Sum(oi => oi.TotalPrice);

                return new
                {
                    productId = p.Id,
                    productName = p.Name,
                    code = p.Code,
                    category = p.Category.ToString(),
                    quantitySold = totalQuantity,
                    revenue = totalRevenue,
                    currentStock = p.StockQuantity
                };
            })
            .Where(p => p.quantitySold > 0)
            .OrderByDescending(p => p.revenue)
            .Take(limit)
            .ToList();

            return Ok(new
            {
                topProducts = productSales,
                labels = productSales.Select(p => p.productName).ToArray(),
                revenue = productSales.Select(p => p.revenue).ToArray(),
                quantity = productSales.Select(p => (decimal)p.quantitySold).ToArray(),
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Real-time istatistikler (cache edilebilir, sık güncellenen veriler için)
        /// </summary>
        [HttpGet("realtime-stats")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetRealtimeStats()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var today = DateTime.Now.Date;

            var todayOrders = orders.Where(o => o.OrderDate.Date == today);
            var todayRevenue = todayOrders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                .Sum(o => o.TotalAmount);

            var activeOrders = orders.Count(o => 
                o.Status != OrderStatus.Completed && 
                o.Status != OrderStatus.Delivered && 
                o.Status != OrderStatus.Cancelled);

            var urgentOrders = orders.Count(o => 
                o.DeliveryDate.HasValue && 
                o.DeliveryDate.Value <= DateTime.Now.AddDays(3) &&
                o.Status != OrderStatus.Completed && 
                o.Status != OrderStatus.Delivered);

            return Ok(new
            {
                todayOrders = todayOrders.Count(),
                todayRevenue,
                activeOrders,
                urgentOrders,
                lastUpdated = DateTime.Now
            });
        }

        /// <summary>
        /// Departman bazlı iş yükü dağılımı
        /// </summary>
        [HttpGet("department-workload")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetDepartmentWorkload()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var workLogs = await _workLogService.GetAllWorkLogsAsync();
            var thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var monthlyWorkLogs = workLogs.Where(w => w.CheckInTime >= thisMonth);

            var departmentWorkload = employees
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    department = g.Key.ToString(),
                    employeeCount = g.Count(e => e.TerminationDate == null),
                    totalHours = monthlyWorkLogs
                        .Where(w => g.Select(e => e.Id).Contains(w.EmployeeId))
                        .Sum(w => w.WorkHours ?? 0),
                    avgHoursPerEmployee = g.Count(e => e.TerminationDate == null) > 0
                        ? monthlyWorkLogs.Where(w => g.Select(e => e.Id).Contains(w.EmployeeId))
                                        .Sum(w => w.WorkHours ?? 0) / g.Count(e => e.TerminationDate == null)
                        : 0
                })
                .OrderByDescending(d => d.totalHours)
                .ToList();

            return Ok(new
            {
                labels = departmentWorkload.Select(d => d.department).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Toplam Çalışma Saati",
                        data = departmentWorkload.Select(d => d.totalHours).ToArray(),
                        backgroundColor = "rgba(75, 192, 192, 0.6)"
                    }
                },
                details = departmentWorkload,
                generatedAt = DateTime.Now
            });
        }
    }
}

