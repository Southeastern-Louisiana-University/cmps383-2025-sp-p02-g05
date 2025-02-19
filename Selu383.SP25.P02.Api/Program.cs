
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Models;

namespace Selu383.SP25.P02.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext") ?? throw new InvalidOperationException("Connection string 'DataContext' not found.")));

            builder.Services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            // ? Configure Cookie Authentication
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/api/authentication/login"; 
                options.AccessDeniedPath = "/api/authentication/accessdenied";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
                options.SlidingExpiration = true; 
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401; 
                    return Task.CompletedTask;
                };
            });

            // ? Add Authentication & Authorization Middleware
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            builder.Services.AddAuthorization();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                await db.Database.MigrateAsync();

                SeedTheaters.Initialize(scope.ServiceProvider);

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                await SeedUsers.Initialize(userManager, roleManager); 

            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(x =>
                 {
                     x.MapControllers();
                 });

            app.MapControllers();

            app.Run();
        }
    }
}
