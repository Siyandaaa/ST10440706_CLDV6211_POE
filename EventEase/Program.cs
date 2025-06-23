using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventEase.Data;
using Azure.Storage.Blobs;
using System.Configuration;
using EventEase.Services;
using EventEase.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EventEaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventEaseContext") ?? throw new InvalidOperationException("Connection string 'EventEaseContext' not found.")));

// Register the BlobStorageService
builder.Services.AddSingleton(new BlobStorageService(builder.Configuration.GetConnectionString("BlobStorage") ?? throw new InvalidOperationException("Connection string 'BlobStorage' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
