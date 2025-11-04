using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TextileCRM.Application.Interfaces;
using TextileCRM.Application.Services;
using TextileCRM.Infrastructure.Context;
using TextileCRM.Infrastructure.Repositories;
using TextileCRM.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add API Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Textile CRM API",
        Version = "v1",
        Description = "Tekstil CRM Sistemi RESTful API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Textile CRM"
        }
    });
    
    // JsonIgnore attribute'larını ve navigation property'leri dikkate al
    c.SchemaFilter<IgnoreNavigationPropertiesSchemaFilter>();
    
    // Döngüsel referansları görmezden gel
    c.UseAllOfToExtendReferenceSchemas();
    c.UseOneOfForPolymorphism();
    
    // Schema ID çakışmalarını önle
    c.CustomSchemaIds(type => {
        if (type.Namespace != null && type.Namespace.StartsWith("TextileCRM.Domain.Entities"))
        {
            return type.Name; // Sadece sınıf adını kullan
        }
        return type.FullName?.Replace("+", ".");
    });
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Register Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Real Services with EF Core
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IAuthService, AuthService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IUserService, UserService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.ICustomerService, CustomerService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IOrderService, OrderService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IProductService, ProductService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IEmployeeService, EmployeeService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IWorkLogService, WorkLogService>();

// Register New Services (Invoice, Payment, File, Notification, Email, StockAlert)
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IInvoiceService, TextileCRM.Application.Services.InvoiceService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IPaymentService, TextileCRM.Application.Services.PaymentService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IFileService, TextileCRM.Application.Services.FileService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.INotificationService, TextileCRM.Application.Services.NotificationService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IEmailService, TextileCRM.Application.Services.EmailService>();
builder.Services.AddScoped<TextileCRM.Application.Interfaces.IStockAlertService, TextileCRM.Application.Services.StockAlertService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "Textile CRM API v1");
    c.RoutePrefix = "api-docs"; // Swagger UI'ya /api-docs adresinden erişilecek
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map API Controllers
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
