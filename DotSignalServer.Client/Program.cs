using System.Net.WebSockets;
using System.Text;

namespace DotSignalServer.Client;

class Program
{
    public static async Task Main()
    {
        var buffer = new byte[1024 * 4];
        using var socket = new ClientWebSocket();
        
        // Get user input before connecting
        Console.WriteLine("Enter a nickname: ");
        var name = Console.ReadLine();
        
        await socket.ConnectAsync(new Uri("ws://localhost:5015/"), CancellationToken.None);
        WebSocketReceiveResult receiveResult = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer), 
            CancellationToken.None);
        if (receiveResult.Count > 0)
        {
            var data = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            Console.WriteLine($"{data}");
        }
        while (!socket.CloseStatus.HasValue)
        {
            Console.WriteLine("Enter a message: ");
            var msg = Console.ReadLine();

            await socket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg!)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var data = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            Console.WriteLine($"Server says: {data}");
        }
        await socket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            "",
            CancellationToken.None);
    }
}