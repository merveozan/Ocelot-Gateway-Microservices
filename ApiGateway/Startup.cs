using JwtAuthenticationManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
//using Ocelot.Tracing.Butterfly;
//using Butterfly.OpenTracing;  // Butterfly entegrasyonu

namespace ApiGateway
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

            services.AddOcelot();
            services.AddControllers();
            services.AddCustomJwtAuthentication();


            //Eğer sunucu belirli bir süre cevap vermezse o sunucuyu kapatıyor
           // services.AddOcelot().AddPolly();
            // CacheManager'i MemoryCache ile entegre ediyoruz
            services.AddCacheManager<object>(settings =>
            {
                settings.WithDictionaryHandle();  // Memory-based cache işlemi
            });

            /*
            services
            .AddOcelot()
            // This comes from Ocelot.Tracing.Butterfly package
            .AddButterfly(option =>
            {
                // This is the URL that the Butterfly collector server is running on...
                option.CollectorUrl = "http://localhost:9618";
                option.Service = "Ocelot";
            });*/




        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOcelot();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
