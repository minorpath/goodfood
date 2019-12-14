using Backend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var storageAccount = CloudStorageAccount.Parse(
                Configuration.GetConnectionString("StorageConnectionString"));

            var cloudBlobContainer = new CloudBlobContainer(
                new Uri(Configuration["BlobStorageUriWithSAS"]));

            services.AddSingleton<StorageManager>(new StorageManager(storageAccount));
            services.AddSingleton<IFileStorage>(new BlobFileStorage(cloudBlobContainer));

            services.AddControllers();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // app.UseStaticFiles();

            app.Run(async (context) =>
            {
                // Default handler for requests not handled elsewhere
                await context.Response.WriteAsync("Could Not Find Anything");
            });
        }
    }
}
