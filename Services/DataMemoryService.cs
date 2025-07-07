using Newtonsoft.Json;
using System;
using System.Reflection;
using WebForm;
using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Models;

public class DataMemoryService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(LoadMemory, cancellationToken);
        Task.Run(Thread_ProcessWSRequest, cancellationToken);
        Task.Run(Write_SymbolFile, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task LoadMemory()
    {
        while (true)
        {
            try
            {
                DataMemory.LoadNews();

                DataMemory.GetAllcode();

                StockMem.Read_SymbolFile();

                DataMemory.LoadSymbol();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private async Task Thread_ProcessWSRequest()
    {
        while (true)
        {
            try
            {
                bool _dequeueSuccess = StockMem.c_queueMessage.TryDequeue(out string requestMessage);
                decimal _ck = 0;
                SymbolDA _da = new SymbolDA();
                if (_dequeueSuccess && requestMessage != null)
                {
                    Notify_WebSocket_Info _Notify_WebSocket_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<Notify_WebSocket_Info>(requestMessage);
                    if (_Notify_WebSocket_Info != null && _Notify_WebSocket_Info.message_type != "")
                    {
                        Symbol_WS_Info _Symbol_WS_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<Symbol_WS_Info>(_Notify_WebSocket_Info.data);
                        if (_Symbol_WS_Info != null && _Symbol_WS_Info.Symbol != null && _Symbol_WS_Info.Symbol != "")
                        {

                            if (StockMem.c_dicStocks.ContainsKey(_Symbol_WS_Info.Symbol) == false)
                            {
                                StockMemInfo stockMemInfo = new StockMemInfo
                                {
                                    Symbol = _Symbol_WS_Info.Symbol,
                                    SymbolName = _Symbol_WS_Info.Name,
                                    MarketCode = _Symbol_WS_Info.MarketCode,
                                    OpenPrice = _Symbol_WS_Info.Open,
                                    ClosePrice = _Symbol_WS_Info.Close,
                                    HighestPrice = _Symbol_WS_Info.Hight,
                                    LowestPrice = _Symbol_WS_Info.Low,
                                    TotalTradedQttyNM = _Symbol_WS_Info.Volume,
                                    TotalTradedValueNM = _Symbol_WS_Info.TotalValue,
                                    MatchPrice = _Symbol_WS_Info.Current_Price
                                };

                                StockMem.c_dicStocks[_Symbol_WS_Info.Symbol] = stockMemInfo;
                            }
                            else
                            {
                                StockMemInfo stockMemInfo = StockMem.c_dicStocks[_Symbol_WS_Info.Symbol];
                                stockMemInfo.SymbolName = _Symbol_WS_Info.Name;
                                stockMemInfo.MarketCode = _Symbol_WS_Info.MarketCode;
                                stockMemInfo.OpenPrice = _Symbol_WS_Info.Open;
                                stockMemInfo.ClosePrice = _Symbol_WS_Info.Close;
                                stockMemInfo.HighestPrice = _Symbol_WS_Info.Hight;
                                stockMemInfo.LowestPrice = _Symbol_WS_Info.Low;
                                stockMemInfo.TotalTradedQttyNM = _Symbol_WS_Info.Volume;
                                stockMemInfo.TotalTradedValueNM = _Symbol_WS_Info.TotalValue;
                                stockMemInfo.MatchPrice = _Symbol_WS_Info.Current_Price;
                                StockMem.c_dicStocks[_Symbol_WS_Info.Symbol] = stockMemInfo;
                            }

                            // update vào bảng mem dữ liệu trong ngày
                            Utils.AddOrUpdateStockMatchStatistic(new StockMatchStatisticInfo()
                            {
                                TradeTime = DateTime.Now,
                                TimestampUTC = Utils.DateTimeToTimeStampMillisecond(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).ToUniversalTime()),
                                Symbol = _Symbol_WS_Info.Symbol,
                                OpenPrice = _Symbol_WS_Info.Open,
                                OpenQtty = _Symbol_WS_Info.Open_Qtty,
                                HighestPrice = _Symbol_WS_Info.Hight,
                                LowestPrice = _Symbol_WS_Info.Low,
                                ClosePrice = _Symbol_WS_Info.Close,
                                CloseQtty = 0,
                                TotalTradedQtty = _Symbol_WS_Info.Volume,
                                TotalTradedValue = _Symbol_WS_Info.TotalValue,
                            });

                            // update vao DB  
                            try
                            {
                                //Symbol_Notify_Info info = new Symbol_Notify_Info();
                                //info.Symbol = _Symbol_WS_Info.Symbol;
                                //info.Current_Price = _Symbol_WS_Info.Current_Price;
                                //_ck = _da.UpdateCurrenPrice(info);
                            }
                            catch(Exception ex)
                            {
                                Logger.Log.Error(ex.ToString());
                            }
                        }
                    }
                    //Utils.AppendMessageToFile(requestMessage, "DataSymbol/data.txt");
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
    }

    public Task Write_SymbolFile()
    {
        Logger.Log.Info("RunBackgroundService Write_AccountFile");

        while (true)
        {

            try
            {
                List<StockMemInfo> _lst = new List<StockMemInfo>();
                List<string> _lst_symbol = new List<string>(StockMem.c_dicStocks.Keys);
                foreach (var item in _lst_symbol)
                {
                    if (StockMem.c_dicStocks.ContainsKey(item) == true)
                    {
                        StockMemInfo _Info = StockMem.c_dicStocks[item].CloneObjectT<StockMemInfo>();
                        _lst.Add(_Info);
                    }
                }
                string jsonString = JsonConvert.SerializeObject(_lst);

                // Ghi chuỗi JSON vào tệp
                string _accountFile = System.IO.Path.Combine(ConfigInfo.ContentRootPath, "Data", "Symbol_Info.json");
                File.WriteAllText(_accountFile, jsonString);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }

            Thread.Sleep(30000);
        }
    }
}
