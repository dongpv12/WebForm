using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WebForm;
using WebForm.Common;
using Websocket.Client;

public class WebSocketReceiverService : BackgroundService
{
    private readonly Uri _uri = new Uri(ConfigInfo.WebSocketData);
    private readonly IHubContext<MessageHub> _hubContext;

    public WebSocketReceiverService(IHubContext<MessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var ws = new ClientWebSocket();

        try
        {
            var exitEvent = new ManualResetEvent(false);
            using (var client = new WebsocketClient(_uri))
            {
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);

                client.DisconnectionHappened.Subscribe(info =>
                {
                    Logger.Log.Debug("Websocket data FinArt DisconnectionHappened happened, type: " + info.Type);

                });

                client.ReconnectionHappened.Subscribe(info =>
                {
                    Logger.Log.Debug("Websocket data FinArt Reconnection happened, type: " + info.Type);
                });

                //đăng ký hàm nhận tất cả msg gửi từ các server
                client.MessageReceived.Subscribe(msg =>
                {
                    if (msg.ToString() == "hello")
                    {
                        Logger.Log.Info("Websocket Connected => done recv message hello");
                    }
                    else
                    {
                        StockMem.c_queueMessage.Enqueue(msg.ToString());
                        _hubContext.Clients.All.SendAsync("ReceiveMessage", msg.ToString(), stoppingToken);
                    }    
                });

                await client.Start();
                
                // gửi message xin chào để test kết nối
                client.Send("hello");

                exitEvent.WaitOne();
            }

            //Logger.Log.Info("🔌 Connecting to WebSocket...");
            //Console.WriteLine("🔌 Connecting to WebSocket...");
            //await ws.ConnectAsync(_uri, stoppingToken);
            //var buffer = new byte[4096];
            //while (!stoppingToken.IsCancellationRequested && ws.State == WebSocketState.Open)
            //{
            //    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

            //    if (result.MessageType == WebSocketMessageType.Close)
            //    {
            //        Console.WriteLine("⚠️ Server closed connection.");
            //        Logger.Log.Info("⚠️ Server closed connection.");
            //        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", stoppingToken);
            //    }
            //    else
            //    {
            //        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            //        //Console.WriteLine($"📩 Received: {message}");

            //        // ✅ Gửi tới client qua SignalR
            //        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message, stoppingToken);

            //        // Ghi log nếu cần
            //        StockMem.c_queueMessage.Enqueue(message);
            //        //AppendMessageToFile(message);
            //    }
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in WebSocket: {ex.Message}");
            Logger.Log.Error($"❌ Error in WebSocket: {ex.Message}");
        }
    }


}
