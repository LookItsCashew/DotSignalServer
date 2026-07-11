// Implicit usings
using DotSignalServer.Server.Models;
using DotSignalServer.Server.Peers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };
app.UseWebSockets(webSocketOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

#region API Mappings
        
// Add websocket middleware to request pipeline
app.MapGet("/connect", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var peer = new PeerServerConnection()
        {
            PeerModel = new Peer() { PeerId = Guid.NewGuid().ToString() },
            Socket =  socket
        };
        await PeerServer.Instance.OnPeerConnected(peer);
        app.Logger.LogInformation($"Peer Connected. Id: {peer.PeerModel.PeerId + Environment.NewLine}");
    }
});
        
#endregion

app.Run();