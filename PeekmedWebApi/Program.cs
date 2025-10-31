using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using PeekmedWebApi.Data;
using PeekmedWebApi.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// ======================
// CẤU HÌNH DỊCH VỤ (SERVICES)
// ======================

// 1️⃣ Kết nối đến Azure SQL Database
builder.Services.AddDbContext<PeekMedDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Tạo mô hình OData (EDM)
var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<User>("Users");
odataBuilder.EntitySet<Hospital>("Hospitals");
odataBuilder.EntitySet<Department>("Departments");
odataBuilder.EntitySet<Doctor>("Doctors");
odataBuilder.EntitySet<Appointment>("Appointments");
odataBuilder.EntitySet<Queue>("Queues");
odataBuilder.EntitySet<Notification>("Notifications");

// 3️⃣ Cấu hình Controller + OData + JSON Unicode
builder.Services.AddControllers()
    .AddOData(options =>
        options.Select()
               .Filter()
               .OrderBy()
               .Expand()
               .Count()
               .SetMaxTop(100)
               .AddRouteComponents("odata", odataBuilder.GetEdmModel()))
    .AddJsonOptions(options =>
    {
        // ✅ Cải thiện JSON serialization cho Azure
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Giữ nguyên tên property
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// 4️⃣ Swagger để test API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Thêm logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 5️⃣ Cấu hình CORS cho frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                // ✅ Local development
                "http://localhost:5173",  // Vite dev server
                "http://localhost:3000",  // React dev server (backup)
                "http://localhost:8080",  // Vite config port
                "https://localhost:7081", // Local HTTPS
                
                // ✅ Production domains
                "https://www.peekmed.click",  // Production frontend
                "https://peekmed.click",      // Production frontend (without www)
                
                // ✅ Azure deployment URLs (nếu có)
                "https://peekmedwebapi-cvhnhxa9bpcke0b8.southeastasia-01.azurewebsites.net"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ======================
// XÂY DỰNG ỨNG DỤNG
// ======================
var app = builder.Build();

// ======================
// PIPELINE XỬ LÝ REQUEST
// ======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // ✅ Cấu hình đặc biệt cho Azure Production
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Tắt compression có thể gây vấn đề
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Remove("Accept-Encoding");
        context.Response.Headers["Cache-Control"] = "no-cache, no-store";
        await next();
    });
}

app.UseHttpsRedirection();

// 🌐 Enable CORS
app.UseCors("AllowFrontend");

// ✅ Middleware để handle errors
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred");
        
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal server error");
    }
});

app.UseAuthorization();
app.MapControllers();
app.Run();
