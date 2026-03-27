using EmployeeManagement.BLL.Services;
using EmployeeManagement.Common.Interfaces;
using EmployeeManagement.Common.Models;
using EmployeeManagement.DAL.Infrastructure;
using EmployeeManagement.DAL.Repository;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection")
        ?? "Server=DESKTOP-8U6HV7O\\SQLEXPRESS;Database=NeoSoft_KaranDave;Trusted_Connection=True;TrustServerCertificate=True;";
    return new DbConnectionFactory(connStr);
});

builder.Services.AddScoped<IRepository<EmployeeDto, int>, EmployeeRepository>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();
app.UseCors("AllowAngular");
app.MapControllers();
app.Run();
