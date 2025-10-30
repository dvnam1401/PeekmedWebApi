// Đảm bảo các using này khớp chính xác với namespace trong các file của bạn
// Ví dụ, file PeekMedDbContext.cs phải có dòng: namespace PeekmedWebApi.Data;
// và file User.cs phải có dòng: namespace PeekmedWebApi.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using PeekmedWebApi.Data;
using PeekmedWebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Cấu hình các dịch vụ (Services) ---

// 1. Đăng ký DbContext với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<PeekMedDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Xây dựng Entity Data Model (EDM) cho OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<User>("Users");
modelBuilder.EntitySet<Hospital>("Hospitals");
modelBuilder.EntitySet<Department>("Departments");
modelBuilder.EntitySet<Doctor>("Doctors");
modelBuilder.EntitySet<Appointment>("Appointments");
modelBuilder.EntitySet<Queue>("Queues");
modelBuilder.EntitySet<Notification>("Notifications");

// 3. Đăng ký dịch vụ Controller và cấu hình OData
builder.Services.AddControllers().AddOData(options => options
    .Select()  // Cho phép truy vấn $select
    .Filter()  // Cho phép truy vấn $filter
    .OrderBy() // Cho phép truy vấn $orderby
    .Expand()  // Cho phép truy vấn $expand
    .Count()   // Cho phép truy vấn $count
    .SetMaxTop(100) // Giới hạn số lượng bản ghi trả về tối đa là 100
    .AddRouteComponents("odata", modelBuilder.GetEdmModel())); // Thiết lập tiền tố "odata"

// 4. Thêm các dịch vụ cần thiết cho API Explorer và Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Xây dựng ứng dụng ---
var app = builder.Build();

// --- Cấu hình HTTP Request Pipeline ---

// Bật Swagger UI chỉ trong môi trường Development để test API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Tự động chuyển hướng từ HTTP sang HTTPS
app.UseHttpsRedirection();

// Bật cơ chế xác thực và phân quyền (nếu có)
app.UseAuthorization();

// Ánh xạ các request tới các Controller tương ứng
app.MapControllers();

// Chạy ứng dụng
app.Run();