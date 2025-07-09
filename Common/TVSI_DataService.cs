using Microsoft.AspNetCore.SignalR;
using WebForm.Common;
using Websocket.Client;

namespace WebForm
{
    public class TVSI_DataService : BackgroundService
    {
        private readonly Uri _uri = new Uri(ConfigInfo.WebSocket_TVSI);
        private readonly IHubContext<MessageHub> _hubContext;

        public TVSI_DataService(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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

                    // gửi thông tin cơ bản
                    Send_Data_Socket data_Socket = new Send_Data_Socket(4);
                    string _s = Newtonsoft.Json.JsonConvert.SerializeObject(data_Socket);
                    client.Send(_s);


                    // gửi thông tin thay đổi
                    Send_Data_Socket data_Socket_change = new Send_Data_Socket(2);
                    _s = Newtonsoft.Json.JsonConvert.SerializeObject(data_Socket_change);
                    client.Send(_s);

                    Send_Data_Socket data_Socket_change_3 = new Send_Data_Socket(3);
                    _s = Newtonsoft.Json.JsonConvert.SerializeObject(data_Socket_change_3);
                    client.Send(_s);

                    exitEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                Logger.Log.Error("Error connect to Websocket data Navi");
            }
        }

    }

    public class Send_Data_Socket
    {
        public Send_Data_Socket(int msgType)
        {
            this.msgType = msgType;
        }
        public int msgType { get; set; }
    }

    public class Data_Socket
    {
        public Data_Socket(int msgType, string message)
        {
            this.msgType = msgType;
            this.message = message;
        }

        public int msgType { get; set; }
        public string message { get; set; }

    }
}
