using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();


app.UseRouting();

// Configure WebSocket options
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
});

// Store all connected clients
var clients = new ConcurrentDictionary<WebSocket, string>();

// WebSocket handler
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws") // WebSocket request
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                clients.TryAdd(webSocket, string.Empty);
                await HandleWebSocket(context, webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
    else
    {
        await next(); 
    }
});

// WebSocket handling logic
async Task HandleWebSocket(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
        // Decode the incoming message
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Broadcast the message to all connected clients
        await BroadcastMessage(receivedMessage);

        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    // Remove the client when disconnected
    clients.TryRemove(webSocket, out _);

    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}

// Broadcast a message to all clients
async Task BroadcastMessage(string message)
{
    foreach (var client in clients.Keys)
    {
        if (client.State == WebSocketState.Open)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

app.Run();
