using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DotSignalServer.Server.Peers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotSignalServer.Server;

class Program
{
    private static readonly PeerServer PeerServer = new();
    
    public static void Main(string[] args)
    {
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

        #region HTTP Mappings
        
        // Add websocket middleware to request pipeline
        app.Use(async (HttpContext context, Func<Task>next) =>
        {
            if (context.Request.Path.StartsWithSegments((PathString)"/ws"))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    //await Echo(webSocket);
                }
                else context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                await next();
            }
        });
        
        app.MapGet("/",  async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var socket = await context.WebSockets.AcceptWebSocketAsync();
                var peer = new Peer() { Id = Guid.NewGuid().ToString(), Socket =  socket };
                await PeerServer.OnPeerConnected(peer);
                app.Logger.LogInformation($"Peer Connected. Id: {peer.Id + Environment.NewLine}");
                await PeerServer.Echo(socket);
            }
        });
        
        #endregion

        app.Run();
    }
}