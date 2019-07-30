using System.IO;
using ImageUploader.Application.Contract;
using ImageUploader.Application;
using ImageUploader.Persistence;
using ImageUploader.Persistence.Contract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageUploader
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
            //var executionDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //var config = new ConfigurationBuilder()
            //    .AddEnvironmentVariables()
            //    .Build();

            var a = Configuration.GetValue<string>("FilestoragePath");

            services.AddMvc();
            services
                .AddSingleton<IImageStorage>(c => new ImageLocalStorage(a))
                .AddSingleton<IImageUploader, ImageUploadService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
