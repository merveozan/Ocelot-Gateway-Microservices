using ApiGateway;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // ocelot.json dosyasını Ocelot'un ayarları olarak ekliyoruz.
                config.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                // Log sağlayıcılarını temizliyoruz ve loglama yapılandırmasını ekliyoruz.
                logging.ClearProviders();
                logging.AddConsole();  // Konsola loglama
                logging.AddDebug();    // Debug penceresine loglama
                logging.SetMinimumLevel(LogLevel.Debug);  // Minimum log seviyesi Debug olarak ayarlandı
            });
}
