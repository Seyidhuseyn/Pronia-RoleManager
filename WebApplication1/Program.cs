using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Abstractions.Services;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
                    //.AddNewtonsoftJson(options =>
                    //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL")));
            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddIdentity<AppUser, IdentityRole>(opt=>
            {
                opt.Password.RequireDigit= true;
                opt.Password.RequireNonAlphanumeric= false;
                opt.Password.RequiredLength = 5;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
                opt.Lockout.AllowedForNewUsers= true;

                opt.SignIn.RequireConfirmedEmail= true;
                opt.User.RequireUniqueEmail= true;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddScoped<IEmailService, EmailService>();
            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Account/Register";
            //    options.LogoutPath = "/Account/logout";
            //    options.AccessDeniedPath = "/Identity/Account/login";
            //});
            var app = builder.Build();

            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("Before");
            //    await next();
            //    Console.WriteLine("After");
            //});

            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
