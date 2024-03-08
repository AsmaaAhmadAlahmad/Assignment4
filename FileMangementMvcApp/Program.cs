
using Assignement3_Domain.Helper;
using FileManagement.Data;
using FileMangementMvcApp.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace FileMangementMvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession();

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                            .AddRazorRuntimeCompilation();

             // ����� ����������
            builder.Services.AddDbContext<FileManagementDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:FileManagementMvcDbConnectionString"]);
            });

            // stateRepository ����� 
            builder.Services.AddTransient<IStateRepository, SessionStateRepository>();

            // AutoMapper �����
            builder.Services.AddTransient(typeof(HelperMapper<,,>));

            // ����� ������������ ������ 
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options => 
                {
                    options.LoginPath ="/Authentication/Index" ;
                    options.ExpireTimeSpan = TimeSpan.FromDays(150);
                    options.SlidingExpiration = true;
                });

            var app = builder.Build();

            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=File}/{action=Index}/{id?}");

            app.Run();
        }
    }
}