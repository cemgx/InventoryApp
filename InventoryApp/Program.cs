using AutoMapper;
using FluentValidation.AspNetCore;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Mappings;
using InventoryApp.Application.MiddleWares;
using InventoryApp.Models.Context;
using InventoryApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options => options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
{
    Indented = true
});

var connectionString = builder.Configuration.GetConnectionString("Default Connection");

var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn("UserName", System.Data.SqlDbType.NVarChar) { DataLength = 256 }
    }
};

builder.Services.AddRedaction(x =>
{
    x.SetRedactor<StarRedactor>(new DataClassificationSet(DataTaxonomy.SensitiveData));
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.With(new UserNameEnricher(new StarRedactor()))
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
            SchemaName = "dbo",
            BatchPostingLimit = 1
        },
        columnOptions: new ColumnOptions
        {
            Exception = { AllowNull = true }
        })
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<SanitizeFilter>();
}).AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddMemoryCache();
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


app.UseStaticFiles();

app.UseMiddleware<LoggingMiddleware>();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowFrontend");

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
