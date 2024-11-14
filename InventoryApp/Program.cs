using AutoMapper;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Mappings;
using InventoryApp.Models.Context;
using InventoryApp.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
                .AddScoped<IProductTypeRepository, ProductTypeRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
