using System;
using WebForm;
using WebForm.Common;
using WebForm.Models;

public class DataMemoryService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(LoadMemory, cancellationToken);
        Task.Run(LoadSymbolMemory, cancellationToken);
        Task.Run(Thread_ProcessWSRequest, cancellationToken);
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
                if (_dequeueSuccess && requestMessage != null)
                {
                    Symbol_WS_Info _Symbol_WS_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<Symbol_WS_Info>(requestMessage);
                    if (_Symbol_WS_Info != null)
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
                            OpenPrice = 0,
                            OpenQtty = _Symbol_WS_Info.Open,
                            HighestPrice = _Symbol_WS_Info.Hight,
                            LowestPrice = _Symbol_WS_Info.Low,
                            ClosePrice = _Symbol_WS_Info.Close,
                            CloseQtty = 0,
                            TotalTradedQtty = _Symbol_WS_Info.Volume,
                            TotalTradedValue = _Symbol_WS_Info.TotalValue,
                        });
                    }
                    Utils.AppendMessageToFile(requestMessage, "DataSymbol/data.txt");
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





    private async Task LoadSymbolMemory()
    {
        while (true)
        {
            try
            {


                DataMemory.LoadSymbol();


            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            await Task.Delay(TimeSpan.FromSeconds(60*30));
        }
    }

}
