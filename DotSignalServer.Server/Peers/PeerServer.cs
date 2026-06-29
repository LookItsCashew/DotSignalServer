using System.Net.WebSockets;
using System.Text;
using DotSignalServer.Server.Messages;

namespace DotSignalServer.Server.Peers;

public class PeerServer
{
    public List<Peer> ConnectedPeers = [];
    public MessageSender GlobalMessageSender { get; } =  new();

    public async Task OnPeerConnected(Peer peer)
    {
        if (!ConnectedPeers.Contains(peer))
        {
            ConnectedPeers.Add(peer);
            GlobalMessageSender.SubscribedPeers.Add(peer);
            await GlobalMessageSender.SendMessageToSubscribedPeersAsync(
                peer,
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(
                    $"There are a total of {ConnectedPeers.Count} peers connected.")
                ),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }
    
    public async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}