using Azure.Identity;
using Azure.Storage.Blobs;
using BlobStorageSample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BlobStorageSample
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
            services.AddControllers();
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationCredential"));
            var settings = Configuration.GetSection("ApplicationCredential").Get<ApplicationSettings>();
            var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
            services.AddScoped(_ => new BlobContainerClient(new Uri(settings.StorageAccountUrl, settings.RootContainer), credential));
            services.AddScoped(_ => new BlobServiceClient(settings.StorageAccountUrl, credential));

            services.AddScoped<IBlobStorageService, BlobStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
