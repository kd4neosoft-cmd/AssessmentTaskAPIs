using EmployeeManagement.BLL.Services;
using EmployeeManagement.Common.Interfaces;
using EmployeeManagement.Common.Models;
using EmployeeManagement.DAL.Infrastructure;
using EmployeeManagement.DAL.Repository;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add Controllers
builder.Services.AddControllers();

// 🔹 Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// 🔹 Register DbConnectionFactory as Singleton
builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection")
        ?? "Server=DESKTOP-8U6HV7O\\SQLEXPRESS;Database=NeoSoft_KaranDave;Trusted_Connection=True;TrustServerCertificate=True;";
    return new DbConnectionFactory(connStr);
});

// 🔹 Register Repository (Scoped per request)
builder.Services.AddScoped<IRepository<EmployeeDto, int>, EmployeeRepository>();

// 🔹 Register Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFileService, FileService>();

// 🔹 Configure JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

// 🔹 Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Serve static files from wwwroot (default)
app.UseStaticFiles();

// 🔹 Serve uploaded files from /Uploads folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseCors("AllowAngular");

// ❌ REMOVED: No authentication/authorization needed
// app.UseAuthorization(); 

// 🔹 Map controller routes
app.MapControllers();

app.Run();
