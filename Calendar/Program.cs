using Microsoft.EntityFrameworkCore;
using Calendar.Models;
using Calendar.Services;

namespace Calendar
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Voeg controller ondersteuning toe
            builder.Services.AddControllersWithViews();

            // Configureer distributed cache voor sessies
            builder.Services.AddDistributedMemoryCache();

            // Configureer sessie-instellingen
            builder.Services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
            });

            // Registreren van services
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<EventAttendanceService>(); // Voeg deze regel toe

            // Configureer de databasecontext met SQLite
            builder.Services.AddDbContext<DatabaseContext>(
                options => options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteDb")));

            var app = builder.Build();

            // Configureer de HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.Urls.Add("http://localhost:3000");

            app.UseAuthorization();

            app.UseSession();

            // Stel de standaard route in
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
