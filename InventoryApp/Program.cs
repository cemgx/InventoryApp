using AutoMapper;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Mappings;
using InventoryApp.Application.MiddleWares;
using InventoryApp.Models.Context;
using InventoryApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<SanitizeFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InventoryAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default Connection")));
builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfiles(new List<Profile>()
    {
        new EmployeeProfile(),
        new InventoryProfile(),
        new ProductProfile(),
        new ProductTypeProfile(),
        new InvoiceProfile(),
    });
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped<IInvoiceRepository, InvoiceRepository>()
                .AddScoped<IEmployeeRepository, EmployeeRepository>()
                .AddScoped<IInventoryRepository, InventoryRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IProductTypeRepository, ProductTypeRepository>()
                .AddScoped<InventoryApp.Application.Hash.PasswordHasher>();

builder.Services.AddAuthentication("EmployeeCookie")
    .AddCookie("EmployeeCookie", options =>
    {
        options.Cookie.Name = "EmployeeAuthCookie";
        options.LoginPath = "/api/Auth/login";
        options.LogoutPath = "/api/Auth/logout";
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:44335")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
