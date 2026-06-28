namespace DotSignalServer.Domain;

public class Peer
{
    public required string Id { get; set; }
    public string Name { get; set; } = null!;
    public required System.Net.IPAddress Address { get; set; }
}