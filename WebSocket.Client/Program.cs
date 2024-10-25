using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Ask the user for their name
        Console.WriteLine("Enter your name: ");
        string userName = Console.ReadLine();
        if (string.IsNullOrEmpty(userName))
        {
            Console.WriteLine("Name cannot be empty. Exiting...");
            return;
        }

        Console.WriteLine("Press enter to connect...");
        Console.ReadLine();

        using (ClientWebSocket client = new ClientWebSocket())
        {
            Uri serviceUri = new Uri("ws://localhost:5248/send");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(120));

            try
            {
                // Connect to the WebSocket server
                await client.ConnectAsync(serviceUri, cts.Token);
                Console.WriteLine("Connected to the server.");

                // Start a task to receive messages asynchronously
                var receiveTask = ReceiveMessages(client, cts.Token);

                while (client.State == WebSocketState.Open)
                {
                    Console.WriteLine("Enter message to send: ");
                    string message = Console.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        // Append the username to the message
                        string messageWithName = $"{userName}: {message}";

                        // Sending message to the server
                        ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageWithName));
                        await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cts.Token);
                    }
                    else
                    {
                        Console.WriteLine("Message cannot be empty.");
                    }
                }

                await receiveTask; // Ensure the receiving task finishes
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
                    Console.WriteLine("Connection closed.");
                }
            }
        }

        Console.ReadLine();
    }

    // Task for receiving messages from the server
    static async Task ReceiveMessages(ClientWebSocket client, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024];

        try
        {
            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine(message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket receive error: {ex.Message}");
        }
    }
}
