using System.Net.WebSockets;
using DotSignalServer.Server.Peers;

namespace DotSignalServer.Server.Messages;

public class MessageSender
{
    public List<PeerServerConnection> SubscribedPeers { get; } = [];

    public async Task SendMessageToSubscribedPeersAsync(
        PeerServerConnection sender,
        ArraySegment<byte> message,
        WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken)
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