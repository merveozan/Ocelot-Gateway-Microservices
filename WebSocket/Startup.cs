using System.Net.WebSockets;
using System.Text;

namespace WebSocket
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            // WebSocket'i aktif ediyoruz
            app.UseWebSockets();

            // Tüm client'ların bağlantılarını tutan bir liste
            var clients = new List<WebSocket>();

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    clients.Add(webSocket);  // Yeni bağlantıyı listeye ekliyoruz
                    await HandleWebSocketConnection(context, webSocket, clients);
                }
                else
                {
                    await next();
                }
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Chat}/{action=Chat}/{id?}");
            });
        }

        // WebSocket isteklerini yönetiyoruz
        private async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket, List<WebSocket> clients)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Diğer tüm bağlı client'lara mesajı gönderiyoruz
                foreach (var client in clients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            clients.Remove(webSocket);  // Bağlantı kapanırsa client'ı listeden çıkartıyoruz
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }

}
