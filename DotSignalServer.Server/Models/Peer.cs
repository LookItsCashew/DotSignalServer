using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotSignalServer.Server.Models;

public class Peer
{
    public string PeerId { get; init; } = string.Empty;
    public string Username { get; set; } = null!;
}