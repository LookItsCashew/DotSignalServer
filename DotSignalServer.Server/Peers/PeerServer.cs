using System.Net.WebSockets;
using System.Text;
using DotSignalServer.Server.Messages;

namespace DotSignalServer.Server.Peers;

public class PeerServer
{
    private readonly List<PeerServerConnection> _connectedPeers = [];
    private readonly MessageSender _globalMessageSender = new();
    private static PeerServer? _instance;
    
    public static PeerServer Instance => _instance ??= new PeerServer();

    public async Task OnPeerConnected(PeerServerConnection peer)
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
}