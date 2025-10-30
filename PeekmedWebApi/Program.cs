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
        // Fix tiếng Việt và ký tự đặc biệt
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 4️⃣ Swagger để test API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

// ⚙️ Tắt gzip / chunked compression (nếu có cấu hình)
app.Use((context, next) =>
{
    context.Response.Headers["Accept-Encoding"] = "identity";
    return next();
});

app.UseAuthorization();
app.MapControllers();
app.Run();
