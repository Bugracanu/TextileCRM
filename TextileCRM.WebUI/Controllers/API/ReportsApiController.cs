using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)] // Swagger'dan geçici olarak gizle
    public class ReportsApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkLogService _workLogService;

        public ReportsApiController(
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

        #region Finansal Raporlar

        /// <summary>
        /// Detaylı finansal rapor - Gelir, gider ve kar analizi
        /// </summary>
        [HttpGet("financial/summary")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetFinancialSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-12);
            var end = endDate ?? DateTime.Now;

            var orders = await _orderService.GetAllOrdersAsync();
            var filteredOrders = orders.Where(o => o.OrderDate >= start && o.OrderDate <= end);

            // Sipariş durumuna göre gelir analizi
            var completedOrders = filteredOrders.Where(o => 
                o.Status == OrderStatus.Completed || 
                o.Status == OrderStatus.Delivered);

            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            var totalOrders = completedOrders.Count();
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Aylık gelir trendi
            var monthlyRevenue = completedOrders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    monthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    revenue = g.Sum(o => o.TotalAmount),
                    orderCount = g.Count(),
                    averageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(x => x.year).ThenBy(x => x.month)
                .ToList();

            // Haftalık gelir
            var weeklyRevenue = completedOrders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30))
                .GroupBy(o => o.OrderDate.Date.AddDays(-(int)o.OrderDate.DayOfWeek))
                .Select(g => new
                {
                    weekStart = g.Key,
                    revenue = g.Sum(o => o.TotalAmount),
                    orderCount = g.Count()
                })
                .OrderBy(x => x.weekStart)
                .ToList();

            // Bekleyen ve iptal edilen siparişler
            var pendingValue = filteredOrders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.New)
                .Sum(o => o.TotalAmount);

            var cancelledValue = filteredOrders
                .Where(o => o.Status == OrderStatus.Cancelled)
                .Sum(o => o.TotalAmount);

            return Ok(new
            {
                period = new { startDate = start, endDate = end },
                summary = new
                {
                    totalRevenue,
                    totalOrders,
                    averageOrderValue,
                    pendingValue,
                    cancelledValue,
                    completionRate = filteredOrders.Count() > 0 
                        ? (double)completedOrders.Count() / filteredOrders.Count() * 100 
                        : 0
                },
                monthlyTrend = monthlyRevenue,
                weeklyTrend = weeklyRevenue,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Kar/Zarar raporu (temel hesaplama)
        /// </summary>
        [HttpGet("financial/profit-loss")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetProfitLossReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;

            var orders = await _orderService.GetAllOrdersAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            var completedOrders = orders.Where(o => 
                (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered) &&
                o.OrderDate >= start && o.OrderDate <= end);

            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);

            // İşçilik maliyeti hesaplama
            var laborCost = workLogs
                .Where(w => w.CheckInTime >= start && w.CheckInTime <= end)
                .Sum(w => 
                {
                    var employee = employees.FirstOrDefault(e => e.Id == w.EmployeeId);
                    var hourlyRate = employee != null ? employee.Salary / 160 : 0; // 160 saat/ay
                    return (w.WorkHours ?? 0) * hourlyRate;
                });

            var grossProfit = totalRevenue - laborCost;
            var profitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;

            return Ok(new
            {
                period = new { startDate = start, endDate = end },
                revenue = totalRevenue,
                costs = new
                {
                    labor = laborCost,
                    total = laborCost // Diğer maliyetler eklenebilir
                },
                grossProfit,
                profitMargin = Math.Round(profitMargin, 2),
                orderCount = completedOrders.Count(),
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Nakit akışı raporu
        /// </summary>
        [HttpGet("financial/cash-flow")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCashFlowReport(
            [FromQuery] int months = 6)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var startDate = DateTime.Now.AddMonths(-months);

            var cashFlow = orders
                .Where(o => o.OrderDate >= startDate)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    monthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    inflow = g.Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                             .Sum(o => o.TotalAmount),
                    pending = g.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.New)
                             .Sum(o => o.TotalAmount),
                    cancelled = g.Where(o => o.Status == OrderStatus.Cancelled)
                               .Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.year).ThenBy(x => x.month)
                .ToList();

            var totalInflow = cashFlow.Sum(cf => cf.inflow);
            var totalPending = cashFlow.Sum(cf => cf.pending);

            return Ok(new
            {
                period = $"Son {months} ay",
                totalInflow,
                totalPending,
                monthlyCashFlow = cashFlow,
                generatedAt = DateTime.Now
            });
        }

        #endregion

        #region Müşteri Analiz Raporları

        /// <summary>
        /// Müşteri segmentasyon analizi
        /// </summary>
        [HttpGet("customers/segmentation")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCustomerSegmentation()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var orders = await _orderService.GetAllOrdersAsync();

            var customerAnalysis = customers.Select(c =>
            {
                var customerOrders = orders.Where(o => o.CustomerId == c.Id);
                var totalSpent = customerOrders.Sum(o => o.TotalAmount);
                var orderCount = customerOrders.Count();
                var lastOrderDate = customerOrders.Any() ? customerOrders.Max(o => o.OrderDate) : (DateTime?)null;
                var avgOrderValue = orderCount > 0 ? totalSpent / orderCount : 0;

                return new
                {
                    customerId = c.Id,
                    customerName = c.Name,
                    companyName = c.CompanyName,
                    totalSpent,
                    orderCount,
                    averageOrderValue = avgOrderValue,
                    lastOrderDate,
                    daysSinceLastOrder = lastOrderDate.HasValue 
                        ? (DateTime.Now - lastOrderDate.Value).Days 
                        : (int?)null,
                    segment = totalSpent > 50000 ? "VIP" :
                             totalSpent > 20000 ? "Premium" :
                             totalSpent > 5000 ? "Standard" : "New"
                };
            }).ToList();

            var segmentSummary = customerAnalysis
                .GroupBy(c => c.segment)
                .Select(g => new
                {
                    segment = g.Key,
                    customerCount = g.Count(),
                    totalRevenue = g.Sum(c => c.totalSpent),
                    averageOrderValue = g.Average(c => c.averageOrderValue)
                })
                .OrderByDescending(s => s.totalRevenue)
                .ToList();

            return Ok(new
            {
                totalCustomers = customers.Count(),
                activeCustomers = customerAnalysis.Count(c => c.daysSinceLastOrder <= 90),
                inactiveCustomers = customerAnalysis.Count(c => c.daysSinceLastOrder > 90),
                segmentSummary,
                customerDetails = customerAnalysis.OrderByDescending(c => c.totalSpent).Take(50),
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Müşteri churn (kayıp) analizi
        /// </summary>
        [HttpGet("customers/churn-analysis")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetChurnAnalysis([FromQuery] int inactiveDays = 90)
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var orders = await _orderService.GetAllOrdersAsync();

            var customerActivity = customers.Select(c =>
            {
                var customerOrders = orders.Where(o => o.CustomerId == c.Id);
                var lastOrderDate = customerOrders.Any() ? customerOrders.Max(o => o.OrderDate) : (DateTime?)null;
                var daysSinceLastOrder = lastOrderDate.HasValue 
                    ? (DateTime.Now - lastOrderDate.Value).Days 
                    : int.MaxValue;

                return new
                {
                    customerId = c.Id,
                    customerName = c.Name,
                    lastOrderDate,
                    daysSinceLastOrder,
                    totalOrders = customerOrders.Count(),
                    status = daysSinceLastOrder > inactiveDays ? "At Risk" :
                            daysSinceLastOrder > inactiveDays / 2 ? "Warning" : "Active"
                };
            }).ToList();

            var atRiskCustomers = customerActivity.Where(c => c.status == "At Risk").ToList();
            var warningCustomers = customerActivity.Where(c => c.status == "Warning").ToList();
            var activeCustomers = customerActivity.Where(c => c.status == "Active").ToList();

            return Ok(new
            {
                summary = new
                {
                    totalCustomers = customers.Count(),
                    activeCount = activeCustomers.Count,
                    warningCount = warningCustomers.Count,
                    atRiskCount = atRiskCustomers.Count,
                    churnRate = customers.Count() > 0 
                        ? (double)atRiskCustomers.Count / customers.Count() * 100 
                        : 0
                },
                atRiskCustomers = atRiskCustomers.OrderByDescending(c => c.totalOrders).Take(20),
                warningCustomers = warningCustomers.OrderByDescending(c => c.totalOrders).Take(20),
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Müşteri yaşam boyu değeri (Customer Lifetime Value) analizi
        /// </summary>
        [HttpGet("customers/lifetime-value")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCustomerLifetimeValue()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var orders = await _orderService.GetAllOrdersAsync();

            var clvAnalysis = customers.Select(c =>
            {
                var customerOrders = orders.Where(o => o.CustomerId == c.Id);
                var totalSpent = customerOrders.Sum(o => o.TotalAmount);
                var orderCount = customerOrders.Count();
                var firstOrderDate = customerOrders.Any() ? customerOrders.Min(o => o.OrderDate) : DateTime.Now;
                var lastOrderDate = customerOrders.Any() ? customerOrders.Max(o => o.OrderDate) : DateTime.Now;
                var customerLifetime = (lastOrderDate - firstOrderDate).Days + 1;
                var avgOrderValue = orderCount > 0 ? totalSpent / orderCount : 0;
                var avgDaysBetweenOrders = orderCount > 1 
                    ? customerLifetime / (orderCount - 1) 
                    : 0;

                // CLV Hesaplama (basitleştirilmiş - 3 yıllık tahmin)
                var estimatedOrdersPerYear = avgDaysBetweenOrders > 0 
                    ? 365 / avgDaysBetweenOrders 
                    : 0;
                var clv = avgOrderValue * estimatedOrdersPerYear * 3;

                return new
                {
                    customerId = c.Id,
                    customerName = c.Name,
                    totalSpent,
                    orderCount,
                    averageOrderValue = avgOrderValue,
                    customerLifetimeDays = customerLifetime,
                    averageDaysBetweenOrders = avgDaysBetweenOrders,
                    estimatedCLV = clv,
                    firstOrderDate,
                    lastOrderDate
                };
            })
            .Where(c => c.orderCount > 0)
            .OrderByDescending(c => c.estimatedCLV)
            .ToList();

            var totalCLV = clvAnalysis.Sum(c => c.estimatedCLV);
            var averageCLV = clvAnalysis.Any() ? clvAnalysis.Average(c => c.estimatedCLV) : 0;

            return Ok(new
            {
                totalCustomers = clvAnalysis.Count,
                totalEstimatedCLV = totalCLV,
                averageCLV = averageCLV,
                topCustomers = clvAnalysis.Take(20),
                generatedAt = DateTime.Now
            });
        }

        #endregion

        #region Üretim ve Operasyonel Raporlar

        /// <summary>
        /// Üretim verimliliği raporu
        /// </summary>
        [HttpGet("operations/production-efficiency")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetProductionEfficiency(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;

            var orders = await _orderService.GetAllOrdersAsync();
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            var productionOrders = orders.Where(o =>
                o.OrderDate >= start && o.OrderDate <= end &&
                (o.Status == OrderStatus.Processing ||
                 o.Status == OrderStatus.InProduction ||
                 o.Status == OrderStatus.Completed ||
                 o.Status == OrderStatus.Delivered));

            var totalOrders = productionOrders.Count();
            var completedOrders = productionOrders.Count(o => 
                o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);

            var totalWorkHours = workLogs
                .Where(w => w.CheckInTime >= start && w.CheckInTime <= end)
                .Sum(w => w.WorkHours ?? 0);

            var completionRate = totalOrders > 0 
                ? (double)completedOrders / totalOrders * 100 
                : 0;

            var ordersPerHour = totalWorkHours > 0 
                ? completedOrders / totalWorkHours 
                : 0;

            // Ortalama tamamlanma süresi
            var averageCompletionTime = completedOrders > 0
                ? productionOrders
                    .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                    .Where(o => o.DeliveryDate.HasValue)
                    .Average(o => (o.DeliveryDate!.Value - o.OrderDate).TotalDays)
                : 0;

            // Durum bazlı dağılım
            var statusDistribution = productionOrders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count(),
                    percentage = totalOrders > 0 ? (double)g.Count() / totalOrders * 100 : 0
                })
                .ToList();

            return Ok(new
            {
                period = new { startDate = start, endDate = end },
                summary = new
                {
                    totalOrders,
                    completedOrders,
                    completionRate = Math.Round(completionRate, 2),
                    totalWorkHours = Math.Round(totalWorkHours, 2),
                    ordersPerHour = Math.Round(ordersPerHour, 4),
                    averageCompletionTimeDays = Math.Round(averageCompletionTime, 1)
                },
                statusDistribution,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Sipariş teslim performansı
        /// </summary>
        [HttpGet("operations/delivery-performance")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetDeliveryPerformance(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-3);
            var end = endDate ?? DateTime.Now;

            var orders = await _orderService.GetAllOrdersAsync();
            var relevantOrders = orders.Where(o => 
                o.OrderDate >= start && o.OrderDate <= end &&
                o.DeliveryDate.HasValue);

            var onTimeDeliveries = relevantOrders.Count(o => 
                o.DeliveryDate!.Value <= o.OrderDate.AddDays(30)); // 30 gün standart

            var lateDeliveries = relevantOrders.Count(o => 
                o.DeliveryDate!.Value > o.OrderDate.AddDays(30));

            var onTimeRate = relevantOrders.Count() > 0 
                ? (double)onTimeDeliveries / relevantOrders.Count() * 100 
                : 0;

            var averageDeliveryTime = relevantOrders.Any()
                ? relevantOrders.Average(o => (o.DeliveryDate!.Value - o.OrderDate).TotalDays)
                : 0;

            var monthlyPerformance = relevantOrders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    monthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    totalOrders = g.Count(),
                    onTime = g.Count(o => o.DeliveryDate!.Value <= o.OrderDate.AddDays(30)),
                    late = g.Count(o => o.DeliveryDate!.Value > o.OrderDate.AddDays(30)),
                    onTimeRate = g.Count() > 0 
                        ? (double)g.Count(o => o.DeliveryDate!.Value <= o.OrderDate.AddDays(30)) / g.Count() * 100 
                        : 0,
                    avgDeliveryDays = g.Average(o => (o.DeliveryDate!.Value - o.OrderDate).TotalDays)
                })
                .OrderBy(x => x.year).ThenBy(x => x.month)
                .ToList();

            return Ok(new
            {
                period = new { startDate = start, endDate = end },
                summary = new
                {
                    totalOrders = relevantOrders.Count(),
                    onTimeDeliveries,
                    lateDeliveries,
                    onTimeRate = Math.Round(onTimeRate, 2),
                    averageDeliveryTimeDays = Math.Round(averageDeliveryTime, 1)
                },
                monthlyPerformance,
                generatedAt = DateTime.Now
            });
        }

        #endregion

        #region Çalışan ve İK Raporları

        /// <summary>
        /// Çalışan verimlilik raporu
        /// </summary>
        [HttpGet("hr/employee-productivity")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetEmployeeProductivity(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;

            var employees = await _employeeService.GetAllEmployeesAsync();
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            var filteredWorkLogs = workLogs.Where(w => 
                w.CheckInTime >= start && w.CheckInTime <= end);

            var employeeStats = employees
                .Where(e => e.TerminationDate == null)
                .Select(e =>
                {
                    var employeeLogs = filteredWorkLogs.Where(w => w.EmployeeId == e.Id);
                    var totalHours = employeeLogs.Sum(w => w.WorkHours ?? 0);
                    var workDays = employeeLogs.Count();
                    var avgHoursPerDay = workDays > 0 ? totalHours / workDays : 0;

                    return new
                    {
                        employeeId = e.Id,
                        employeeName = e.FullName,
                        department = e.Department.ToString(),
                        position = e.Position,
                        totalHours = Math.Round(totalHours, 2),
                        workDays,
                        avgHoursPerDay = Math.Round(avgHoursPerDay, 2),
                        monthlySalary = e.Salary,
                        costPerHour = totalHours > 0 ? e.Salary / totalHours : 0
                    };
                })
                .Where(e => e.workDays > 0)
                .OrderByDescending(e => e.totalHours)
                .ToList();

            var departmentStats = employeeStats
                .GroupBy(e => e.department)
                .Select(g => new
                {
                    department = g.Key,
                    employeeCount = g.Count(),
                    totalHours = g.Sum(e => e.totalHours),
                    avgHoursPerEmployee = g.Average(e => e.totalHours),
                    totalCost = g.Sum(e => e.monthlySalary)
                })
                .OrderByDescending(d => d.totalHours)
                .ToList();

            return Ok(new
            {
                period = new { startDate = start, endDate = end },
                summary = new
                {
                    totalEmployees = employeeStats.Count,
                    totalWorkHours = employeeStats.Sum(e => e.totalHours),
                    avgWorkHoursPerEmployee = employeeStats.Any() 
                        ? employeeStats.Average(e => e.totalHours) 
                        : 0
                },
                departmentStats,
                topPerformers = employeeStats.Take(10),
                allEmployees = employeeStats,
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Departman performans karşılaştırması
        /// </summary>
        [HttpGet("hr/department-comparison")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetDepartmentComparison()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            var departmentAnalysis = employees
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    department = g.Key.ToString(),
                    totalEmployees = g.Count(),
                    activeEmployees = g.Count(e => e.TerminationDate == null),
                    totalSalary = g.Sum(e => e.Salary),
                    avgSalary = g.Average(e => e.Salary),
                    totalWorkHours = workLogs
                        .Where(w => g.Select(e => e.Id).Contains(w.EmployeeId))
                        .Sum(w => w.WorkHours ?? 0),
                    avgWorkHoursPerEmployee = g.Count() > 0
                        ? workLogs.Where(w => g.Select(e => e.Id).Contains(w.EmployeeId))
                                  .Sum(w => w.WorkHours ?? 0) / g.Count()
                        : 0
                })
                .OrderByDescending(d => d.totalEmployees)
                .ToList();

            return Ok(new
            {
                totalDepartments = departmentAnalysis.Count,
                departmentComparison = departmentAnalysis,
                generatedAt = DateTime.Now
            });
        }

        #endregion

        #region Stok ve Envanter Raporları

        /// <summary>
        /// Detaylı stok durumu raporu
        /// </summary>
        [HttpGet("inventory/stock-status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetStockStatus()
        {
            var products = await _productService.GetAllProductsAsync();

            var stockAnalysis = products.Select(p => new
            {
                productId = p.Id,
                productName = p.Name,
                code = p.Code,
                category = p.Category.ToString(),
                currentStock = p.StockQuantity,
                unitPrice = p.UnitPrice,
                stockValue = p.StockQuantity * p.UnitPrice,
                status = p.StockQuantity == 0 ? "Out of Stock" :
                        p.StockQuantity <= 10 ? "Low Stock" :
                        p.StockQuantity <= 50 ? "Medium Stock" : "Good Stock"
            }).ToList();

            var categoryBreakdown = stockAnalysis
                .GroupBy(p => p.category)
                .Select(g => new
                {
                    category = g.Key,
                    productCount = g.Count(),
                    totalStock = g.Sum(p => p.currentStock),
                    totalValue = g.Sum(p => p.stockValue),
                    outOfStock = g.Count(p => p.status == "Out of Stock"),
                    lowStock = g.Count(p => p.status == "Low Stock")
                })
                .OrderByDescending(c => c.totalValue)
                .ToList();

            var stockByStatus = stockAnalysis
                .GroupBy(p => p.status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count(),
                    totalValue = g.Sum(p => p.stockValue)
                })
                .ToList();

            return Ok(new
            {
                summary = new
                {
                    totalProducts = products.Count(),
                    totalStockValue = stockAnalysis.Sum(p => p.stockValue),
                    outOfStockCount = stockAnalysis.Count(p => p.status == "Out of Stock"),
                    lowStockCount = stockAnalysis.Count(p => p.status == "Low Stock")
                },
                categoryBreakdown,
                stockByStatus,
                criticalProducts = stockAnalysis
                    .Where(p => p.status == "Out of Stock" || p.status == "Low Stock")
                    .OrderBy(p => p.currentStock)
                    .ToList(),
                allProducts = stockAnalysis.OrderBy(p => p.currentStock),
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Stok hareket raporu
        /// </summary>
        [HttpGet("inventory/stock-movement")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetStockMovement(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;

            var orders = await _orderService.GetAllOrdersAsync();
            var products = await _productService.GetAllProductsAsync();

            var relevantOrders = orders.Where(o => o.OrderDate >= start && o.OrderDate <= end);

            var productMovement = products.Select(p =>
            {
                var productOrders = relevantOrders
                    .Where(o => o.OrderItems.Any(oi => oi.ProductId == p.Id));

                var totalQuantitySold = productOrders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductId == p.Id)
                    .Sum(oi => oi.Quantity);

                var totalRevenue = productOrders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductId == p.Id)
                    .Sum(oi => oi.TotalPrice);

                return new
                {
                    productId = p.Id,
                    productName = p.Name,
                    code = p.Code,
                    currentStock = p.StockQuantity,
                    quantitySold = totalQuantitySold,
                    revenue = totalRevenue,
                    turnoverRate = p.StockQuantity > 0 
                        ? (double)totalQuantitySold / p.StockQuantity 
                        : 0
                };
            })
            .Where(p => p.quantitySold > 0)
            .OrderByDescending(p => p.quantitySold)
            .ToList();

            return Ok(new
            {
                period = new { startDate = start, endDate = end },
                topSellingProducts = productMovement.Take(20),
                slowMovingProducts = productMovement
                    .Where(p => p.turnoverRate < 0.5)
                    .OrderBy(p => p.turnoverRate)
                    .Take(20),
                totalRevenue = productMovement.Sum(p => p.revenue),
                generatedAt = DateTime.Now
            });
        }

        #endregion

        #region Karşılaştırmalı Analizler

        /// <summary>
        /// Dönemsel karşılaştırma raporu
        /// </summary>
        [HttpGet("comparison/period-over-period")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetPeriodComparison(
            [FromQuery] string period = "month") // month, quarter, year
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var customers = await _customerService.GetAllCustomersAsync();

            DateTime currentStart, currentEnd, previousStart, previousEnd;

            switch (period.ToLower())
            {
                case "quarter":
                    var currentQuarter = (DateTime.Now.Month - 1) / 3;
                    currentStart = new DateTime(DateTime.Now.Year, currentQuarter * 3 + 1, 1);
                    currentEnd = currentStart.AddMonths(3).AddDays(-1);
                    previousStart = currentStart.AddMonths(-3);
                    previousEnd = currentStart.AddDays(-1);
                    break;
                case "year":
                    currentStart = new DateTime(DateTime.Now.Year, 1, 1);
                    currentEnd = new DateTime(DateTime.Now.Year, 12, 31);
                    previousStart = new DateTime(DateTime.Now.Year - 1, 1, 1);
                    previousEnd = new DateTime(DateTime.Now.Year - 1, 12, 31);
                    break;
                default: // month
                    currentStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    currentEnd = currentStart.AddMonths(1).AddDays(-1);
                    previousStart = currentStart.AddMonths(-1);
                    previousEnd = currentStart.AddDays(-1);
                    break;
            }

            var currentOrders = orders.Where(o => o.OrderDate >= currentStart && o.OrderDate <= currentEnd);
            var previousOrders = orders.Where(o => o.OrderDate >= previousStart && o.OrderDate <= previousEnd);

            var currentRevenue = currentOrders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                .Sum(o => o.TotalAmount);
            var previousRevenue = previousOrders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                .Sum(o => o.TotalAmount);

            var revenueGrowth = previousRevenue > 0 
                ? ((currentRevenue - previousRevenue) / previousRevenue) * 100 
                : 0;

            var currentOrderCount = currentOrders.Count();
            var previousOrderCount = previousOrders.Count();
            var orderGrowth = previousOrderCount > 0 
                ? ((double)(currentOrderCount - previousOrderCount) / previousOrderCount) * 100 
                : 0;

            var currentNewCustomers = customers.Count(c => 
                c.CreatedDate >= currentStart && c.CreatedDate <= currentEnd);
            var previousNewCustomers = customers.Count(c => 
                c.CreatedDate >= previousStart && c.CreatedDate <= previousEnd);
            var customerGrowth = previousNewCustomers > 0 
                ? ((double)(currentNewCustomers - previousNewCustomers) / previousNewCustomers) * 100 
                : 0;

            return Ok(new
            {
                periodType = period,
                currentPeriod = new
                {
                    startDate = currentStart,
                    endDate = currentEnd,
                    revenue = currentRevenue,
                    orderCount = currentOrderCount,
                    newCustomers = currentNewCustomers,
                    avgOrderValue = currentOrderCount > 0 ? currentRevenue / currentOrderCount : 0
                },
                previousPeriod = new
                {
                    startDate = previousStart,
                    endDate = previousEnd,
                    revenue = previousRevenue,
                    orderCount = previousOrderCount,
                    newCustomers = previousNewCustomers,
                    avgOrderValue = previousOrderCount > 0 ? previousRevenue / previousOrderCount : 0
                },
                growth = new
                {
                    revenueGrowth = Math.Round(revenueGrowth, 2),
                    orderGrowth = Math.Round(orderGrowth, 2),
                    customerGrowth = Math.Round(customerGrowth, 2)
                },
                generatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Yıl bazlı karşılaştırma (Year over Year)
        /// </summary>
        [HttpGet("comparison/year-over-year")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetYearOverYearComparison()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var currentYear = DateTime.Now.Year;
            var lastYear = currentYear - 1;

            var monthlyComparison = Enumerable.Range(1, 12).Select(month =>
            {
                var currentYearOrders = orders.Where(o => 
                    o.OrderDate.Year == currentYear && o.OrderDate.Month == month);
                var lastYearOrders = orders.Where(o => 
                    o.OrderDate.Year == lastYear && o.OrderDate.Month == month);

                var currentRevenue = currentYearOrders
                    .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                    .Sum(o => o.TotalAmount);
                var lastYearRevenue = lastYearOrders
                    .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                    .Sum(o => o.TotalAmount);

                var growth = lastYearRevenue > 0 
                    ? ((currentRevenue - lastYearRevenue) / lastYearRevenue) * 100 
                    : 0;

                return new
                {
                    month,
                    monthName = new DateTime(currentYear, month, 1).ToString("MMMM"),
                    currentYearRevenue = currentRevenue,
                    lastYearRevenue = lastYearRevenue,
                    growth = Math.Round(growth, 2),
                    currentYearOrders = currentYearOrders.Count(),
                    lastYearOrders = lastYearOrders.Count()
                };
            }).ToList();

            var currentYearTotal = monthlyComparison.Sum(m => m.currentYearRevenue);
            var lastYearTotal = monthlyComparison.Sum(m => m.lastYearRevenue);
            var yearlyGrowth = lastYearTotal > 0 
                ? ((currentYearTotal - lastYearTotal) / lastYearTotal) * 100 
                : 0;

            return Ok(new
            {
                currentYear,
                lastYear,
                yearlyComparison = new
                {
                    currentYearTotal,
                    lastYearTotal,
                    growth = Math.Round(yearlyGrowth, 2)
                },
                monthlyBreakdown = monthlyComparison,
                generatedAt = DateTime.Now
            });
        }

        #endregion
    }
}

