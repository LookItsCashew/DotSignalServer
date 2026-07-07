using System.Net.WebSockets;

namespace DotSignalServer.Server.Peers;

public class Peer
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; set; } = null!;
    public WebSocket Socket { get; init; } = null!;
}