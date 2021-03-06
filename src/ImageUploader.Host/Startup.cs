﻿using System.Net.Http;
using ImageUploader.Application.Contract;
using ImageUploader.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImageUploader.Controllers;

namespace ImageUploader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services
                .AddHttpClient()
                .AddScoped<IImagesService>(c =>
                    new ImagesService(
                        path: Configuration.GetValue<string>("FILE_PATH"),
                        client: c.GetRequiredService<IHttpClientFactory>().CreateClient()));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
