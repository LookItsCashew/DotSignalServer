using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotSignalServer.Server.Models;

public class ConnectionLog
{
    [Key]
    public int Id { get; set; }
    
    public string? Message { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime Timestamp { get; set; }
}