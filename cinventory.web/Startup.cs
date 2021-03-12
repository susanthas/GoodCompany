using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using static cinventory.web.Controllers.InventoryController;

namespace cinventory.web
{
    public class Startup
    {
        public static List<InventoryItem> _inventory;
        public static List<InventoryItemFeature> desktopFeatures = new List<InventoryItemFeature>();
        public static List<InventoryItemFeature> laptopFeatures = new List<InventoryItemFeature>();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            desktopFeatures.Add(new InventoryItemFeature() { featureName = "Tower", featureValue = "Yes" });
            desktopFeatures.Add(new InventoryItemFeature() { featureName = "Front USB", featureValue = "No" });
            laptopFeatures.Add(new InventoryItemFeature() { featureName = "Sreen Size", featureValue = "15.6" });
            laptopFeatures.Add(new InventoryItemFeature() { featureName = "Screen Resolution", featureValue = "FHD" });

            _inventory = new List<InventoryItem>();
            _inventory.Add(new InventoryItem() { id = 1, computerType = "Desktop", processor = "i3", brand = "Dell", price = 600, noOfUSBPorts =4, noOfRAMPorts=4, features = desktopFeatures });
            _inventory.Add(new InventoryItem() { id = 2, computerType = "Desktop", processor = "i5", brand = "HP", price = 1000, noOfUSBPorts = 4, noOfRAMPorts = 4, features = desktopFeatures });
            _inventory.Add(new InventoryItem() { id = 3, computerType = "Desktop", processor = "i7", brand = "Dell", price = 1100, noOfUSBPorts = 6, noOfRAMPorts = 4, features = null });
            _inventory.Add(new InventoryItem() { id = 4, computerType = "Laptop", processor = "i3", brand = "MSI", price = 1600, noOfUSBPorts = 3, noOfRAMPorts = 2, features = laptopFeatures });
            _inventory.Add(new InventoryItem() { id = 5, computerType = "Laptop", processor = "i5", brand = "MSI", price = 2000, noOfUSBPorts = 4, noOfRAMPorts = 1, features = laptopFeatures });

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

        }
    }
}
