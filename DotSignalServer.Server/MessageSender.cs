using System.Net.WebSockets;

namespace DotSignalServer.Server;

public class MessageSender
{
    public List<Peer> SubscribedPeers { get; } = [];

    public async Task SendMessageToSubscribedPeersAsync(
        Peer sender,
        ArraySegment<byte> message,
        WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken
        )
    {
        try
        {
            if (!sender.Socket.CloseStatus.HasValue)
            {
                foreach (var peer in SubscribedPeers)
                    await peer.Socket.SendAsync(message, messageType, endOfMessage, cancellationToken);
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}