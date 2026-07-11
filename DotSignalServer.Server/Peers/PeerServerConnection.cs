using System.Net.WebSockets;

namespace DotSignalServer.Server.Peers;

public class PeerServerConnection
{
    public Models.Peer PeerModel { get; init; } = null!;
    public WebSocket Socket { get; init; } = null!;
}