// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Configures the ASP.NET Core application, services, middleware, and database connection.
//
// References:
// Microsoft (2025) ASP.NET Core fundamentals.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-10.0
// (Accessed: 14 April 2026)
//
// Microsoft (2025) Dependency injection in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
// (Accessed: 14 April 2026)
//
// Microsoft (2025) Configuration in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration
// (Accessed: 15 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on ASP.NET Core setup.
// The Independent Institute of Education.using EventEase.Models;

//PART 2:
// References:
// Microsoft (2026) Dependency injection in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
// (Accessed: 02 May 2026)
//
// Microsoft (2026) Configuration in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration
// (Accessed: 03 May 2026)

using EventEase.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using EventEase.Services;

namespace EventEase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create builder for the application
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC services (controllers + views)
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<BlobService>();

            // Configure database connection using appsettings.json
            builder.Services.AddDbContext<EventEaseDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Build the application
            var app = builder.Build();

            // Configure error handling for production
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                // Enable HTTP Strict Transport Security
                app.UseHsts();
            }

            // Redirect HTTP to HTTPS
            app.UseHttpsRedirection();

            // Enable routing
            app.UseRouting();

            // Enable authorization (if needed later)
            app.UseAuthorization();

            // Enable static files (CSS, JS, images)
            app.MapStaticAssets();

            // Default route configuration
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Run the application
            app.Run();

            //Registereing Blob Storage Service
            builder.Services.AddScoped<BlobService>();
            builder.Services.AddSingleton(x =>
            new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

            builder.Services.AddScoped<BlobService>();
        }
    }
}