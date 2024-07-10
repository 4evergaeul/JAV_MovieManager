using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary;
using MovieManager.ClassLibrary.Settings;
using Serilog;
using System;

namespace MovieManager.Endpoint
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
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddOptions();
            services.AddControllers();
            services.AddTransient<MovieService>();
            services.AddTransient<PotPlayerService>();
            services.AddTransient<XmlProcessor>();
            services.AddTransient<ScrapeService>();
            services.AddTransient<ActorService>();
            services.AddTransient<GenreService>();
            services.AddTransient<TagService>();
            services.AddTransient<DirectorService>();
            services.AddSingleton<UserSettingsService>();
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

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
