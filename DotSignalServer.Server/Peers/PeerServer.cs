using System.Net.WebSockets;
using System.Text;
using DotSignalServer.Server.Messages;

namespace DotSignalServer.Server.Peers;

public class PeerServer
{
    private readonly List<Peer> _connectedPeers = [];
    private readonly MessageSender _globalMessageSender = new();

    public async Task OnPeerConnected(Peer peer)
    {
        if (!_connectedPeers.Contains(peer))
        {
            _connectedPeers.Add(peer);
            _globalMessageSender.SubscribedPeers.Add(peer);
            ArraySegment<byte> data = new(Encoding.UTF8.GetBytes($"Peer connected. Total peers: {_connectedPeers.Count}"));
            await _globalMessageSender.SendMessageToSubscribedPeersAsync(
                peer,
                data,
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