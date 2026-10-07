using Microsoft.EntityFrameworkCore;
using VendorMngSystem.DbData;
using VendorMngSystem.IRepositorys;
using VendorMngSystem.IServices;
using VendorMngSystem.MappingClass;
using VendorMngSystem.Repositorys;
using VendorMngSystem.Services;
namespace VendorMngSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<DataDbContext>(sqlo=>sqlo.UseSqlServer(builder.Configuration.GetConnectionString("VendorAppDbConString")));

            builder.Services.AddScoped<IVendorsRepository,VendorsRepository>();
            builder.Services.AddScoped<IVendorDocRepository,VendorDocRepository>();
            builder.Services.AddScoped<IVendorService,VendorServives>();
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            // Add services to the container.

            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingModels).Assembly);
            var app = builder.Build();


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
                pattern: "{controller=Vendors}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
