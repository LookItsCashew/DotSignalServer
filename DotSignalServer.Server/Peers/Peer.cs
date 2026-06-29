using System.Net.WebSockets;

namespace DotSignalServer.Server.Peers;

public class Peer
{
    public required string Id { get; set; }
    public string Name { get; set; } = null!;
    public WebSocket Socket { get; set; } = null!;
}