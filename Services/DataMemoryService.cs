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
        //Nếu dùng source là NAVISOFT thì dùng hàm này để xử lý dữ liệu từ WebSocket của Navisoft 
        if (ConfigInfo.Source_WS == "NAVISOFT")
        {
            Task.Run(Thread_ProcessWSRequest, cancellationToken);
        }
        else
        {
            // không thì xử lý trực tiếp từ TVSI
            Task.Run(Thread_ProcessWSRequest_TVSI, cancellationToken);
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    void Write_Symbol_File()
    {
        try
        {
            Logger.Log.Info("Begin Write Symbol File");
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
            Logger.Log.Info("Done Write Symbol File");
        }
        catch (Exception ex)
        {
            Logger.Log.Error(ex.ToString());
        }
    }

    private async Task Thread_ProcessWSRequest()
    {
        DateTime lastWrite = DateTime.MinValue;

        while (true)
        {
            try
            {
                bool _dequeueSuccess = StockMem.c_queueMessage.TryDequeue(out string requestMessage);
                SymbolDA _da = new SymbolDA();
                if (_dequeueSuccess && requestMessage != null)
                {
                    Notify_WebSocket_Info _Notify_WebSocket_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<Notify_WebSocket_Info>(requestMessage);
                    if (_Notify_WebSocket_Info != null && _Notify_WebSocket_Info.message_type != "")
                    {
                        Symbol_WS_Info _Symbol_WS_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<Symbol_WS_Info>(_Notify_WebSocket_Info.data);
                        if (_Symbol_WS_Info != null && _Symbol_WS_Info.Symbol != null && _Symbol_WS_Info.Symbol != "")
                        {
                            // update de lai trong mem cua DataMemory
                            Symbol_Notify_Info _Symbol_mem = DataMemory.c_dicSymbol.ContainsKey(_Symbol_WS_Info.Symbol) ? DataMemory.c_dicSymbol[_Symbol_WS_Info.Symbol] : null;
                            if (_Symbol_mem != null)
                            {
                                if (_Symbol_WS_Info.Current_Price != _Symbol_mem.Current_Price)
                                {
                                    Logger.Log.Debug("Symbol " + _Symbol_mem.Symbol + " Current_Price " + _Symbol_WS_Info.Current_Price + " Total message in queue " + StockMem.c_queueMessage.Count);
                                }
                                _Symbol_mem.Current_Price = _Symbol_WS_Info.Current_Price;

                                // chi xu ly cac ma cua he thong finart
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

                                    // xem co phai symbol thuoc finart khai bao khong
                                    if (_Symbol_mem != null)
                                    {
                                        stockMemInfo.Is_FinArt = true;
                                    }
                                    else
                                    {
                                        stockMemInfo.Is_FinArt = false;
                                    }

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

                                    if (_Symbol_mem != null)
                                    {
                                        stockMemInfo.Is_FinArt = true;
                                    }
                                    else
                                    {
                                        stockMemInfo.Is_FinArt = false;
                                    }

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
                            }
                        }

                        // Nếu đã qua ít nhất 1 phút từ lần ghi trước đó
                        if ((DateTime.Now - lastWrite).TotalMinutes >= 1)
                        {
                            Write_Symbol_File();
                            lastWrite = DateTime.Now;
                        }
                    }
                    else
                    {
                        // Nếu đã qua ít nhất 1 phút từ lần ghi trước đó
                        if ((DateTime.Now - lastWrite).TotalMinutes >= 1)
                        {
                            Write_Symbol_File();
                            lastWrite = DateTime.Now;
                        }

                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                }
                else
                {
                    // update vào db lúc 15h30
                    if (StockMem.c_update_price == false && DateTime.Now.ToString("HH:mm") == "15:30")
                    {
                        // ghi vao file
                        Write_Symbol_File();
                        lastWrite = DateTime.Now;

                        foreach (var item in DataMemory.c_lstSymbolData)
                        {
                            if (item.Status != "2")
                            {
                                _da.UpdateCurrenPrice(item);
                            }

                        }
                        StockMem.c_update_price = true;
                    }

                    // reset mem nếu qua ngày
                    if (DateTime.Now.Date != StockMem.c_trading_date.Date)
                    {
                        StockMem.c_trading_date = DateTime.Now;

                        // XÓA DỮ liệu giá khớp từng mã chứng khoán
                        StockMem.c_dicStockMatchStatistics = new Dictionary<string, Dictionary<long, StockMatchStatisticInfo>>();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
    }

    /// <summary>
    /// Nếu dùng source là TVSI thì dùng hàm này để xử lý dữ liệu từ WebSocket của TVSI 
    /// </summary>
    private async Task Thread_ProcessWSRequest_TVSI()
    {
        DateTime lastWrite = DateTime.MinValue;

        while (true)
        {
            try
            {
                bool _dequeueSuccess = StockMem.c_queueMessage.TryDequeue(out string requestMessage);
                SymbolDA _da = new SymbolDA();
                if (_dequeueSuccess && requestMessage != null)
                {
                    string _data_decrypt = LZStringCSharp.LZString.DecompressFromBase64(requestMessage);
                    var _info = Newtonsoft.Json.JsonConvert.DeserializeObject<Data_Socket>(_data_decrypt);
                    if (_info == null)
                    {
                        return;
                    }

                    if (_info.msgType == 2)
                    {
                        string[] _arr_ck = _info.message.Split('$');
                        foreach (var item_ck in _arr_ck)
                        {
                            string _symbol = Utils.Get_Symbol(item_ck);
                            if (_symbol != "")
                            {
                                // nếu có trong dữ liệu của quản trị thì xử lý
                                Symbol_Notify_Info _Symbol_mem = DataMemory.c_dicSymbol.ContainsKey(_symbol) ? DataMemory.c_dicSymbol[_symbol] : null;
                                if (_Symbol_mem != null)
                                {
                                    Symbol_WS_Info _Symbol_WS_Info = Utils.Init_Symbol_FromSocket(item_ck);

                                    if (_Symbol_WS_Info.Current_Price != _Symbol_mem.Current_Price)
                                    {
                                        Logger.Log.Debug("Symbol " + _Symbol_mem.Symbol + " Current_Price " + _Symbol_WS_Info.Current_Price + " Total message in queue " + StockMem.c_queueMessage.Count);
                                    }

                                    if (_Symbol_WS_Info != null && _Symbol_WS_Info.Symbol != null && _Symbol_WS_Info.Symbol != "")
                                    {
                                        if (_Symbol_WS_Info.Current_Price > 0)
                                        {
                                            _Symbol_mem.Current_Price = _Symbol_WS_Info.Current_Price;
                                            _Symbol_mem.Close = _Symbol_WS_Info.Current_Price;
                                            if (_Symbol_mem.Low < _Symbol_WS_Info.Current_Price)
                                            {
                                                _Symbol_mem.Low = _Symbol_WS_Info.Current_Price;
                                            }

                                            if (_Symbol_mem.Hight < _Symbol_WS_Info.Current_Price)
                                            {
                                                _Symbol_mem.Hight = _Symbol_WS_Info.Current_Price;
                                            }
                                        }

                                        if (_Symbol_WS_Info.Match_Qtty > 0)
                                        {
                                            _Symbol_mem.Match_Qtty = _Symbol_WS_Info.Match_Qtty;
                                            if (_Symbol_mem.Open_Qtty <= 0)
                                            {
                                                _Symbol_mem.Open_Qtty = _Symbol_WS_Info.Match_Qtty;
                                            }
                                        }

                                        if (_Symbol_WS_Info.Volume > 0)
                                        {
                                            _Symbol_mem.Volume = _Symbol_WS_Info.Volume;
                                        }
                                        if (_Symbol_WS_Info.TotalValue > 0)
                                        {
                                            _Symbol_mem.TotalValue = _Symbol_WS_Info.TotalValue;
                                        }
                                        if (_Symbol_WS_Info.Open > 0)
                                        {
                                            _Symbol_mem.Open = _Symbol_WS_Info.Open;
                                        }
                                        if (_Symbol_WS_Info.Name != null && _Symbol_WS_Info.Name != "")
                                        {
                                            _Symbol_mem.Name = _Symbol_WS_Info.Name;
                                        }

                                        if (StockMem.c_dicStocks.ContainsKey(_Symbol_WS_Info.Symbol) == false)
                                        {
                                            StockMemInfo stockMemInfo = new StockMemInfo
                                            {
                                                Symbol = _Symbol_mem.Symbol,
                                                SymbolName = _Symbol_mem.Name,
                                                MarketCode = _Symbol_mem.MarketCode,
                                                OpenPrice = _Symbol_mem.Open,
                                                ClosePrice = _Symbol_mem.Close,
                                                HighestPrice = _Symbol_mem.Hight,
                                                LowestPrice = _Symbol_mem.Low,
                                                TotalTradedQttyNM = _Symbol_mem.Volume,
                                                TotalTradedValueNM = _Symbol_mem.TotalValue,
                                                MatchPrice = _Symbol_mem.Current_Price
                                            };

                                            StockMem.c_dicStocks[_Symbol_mem.Symbol] = stockMemInfo;
                                        }
                                        else
                                        {
                                            StockMemInfo stockMemInfo = StockMem.c_dicStocks[_Symbol_mem.Symbol];
                                            stockMemInfo.SymbolName = _Symbol_mem.Name;
                                            stockMemInfo.MarketCode = _Symbol_mem.MarketCode;
                                            stockMemInfo.OpenPrice = _Symbol_mem.Open;
                                            stockMemInfo.ClosePrice = _Symbol_mem.Close;
                                            stockMemInfo.HighestPrice = _Symbol_mem.Hight;
                                            stockMemInfo.LowestPrice = _Symbol_mem.Low;
                                            stockMemInfo.TotalTradedQttyNM = _Symbol_mem.Volume;
                                            stockMemInfo.TotalTradedValueNM = _Symbol_mem.TotalValue;
                                            stockMemInfo.MatchPrice = _Symbol_mem.Current_Price;
                                            StockMem.c_dicStocks[_Symbol_mem.Symbol] = stockMemInfo;
                                        }

                                        // update vào bảng mem dữ liệu trong ngày
                                        Utils.AddOrUpdateStockMatchStatistic(new StockMatchStatisticInfo()
                                        {
                                            TradeTime = DateTime.Now,
                                            TimestampUTC = Utils.DateTimeToTimeStampMillisecond(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).ToUniversalTime()),
                                            Symbol = _Symbol_mem.Symbol,
                                            OpenPrice = _Symbol_mem.Open,
                                            OpenQtty = _Symbol_mem.Open_Qtty,
                                            HighestPrice = _Symbol_mem.Hight,
                                            LowestPrice = _Symbol_mem.Low,
                                            ClosePrice = _Symbol_mem.Close,
                                            CloseQtty = 0,
                                            TotalTradedQtty = _Symbol_mem.Volume,
                                            TotalTradedValue = _Symbol_mem.TotalValue,
                                        }); 
                                    }
                                }
                            }
                        }
                    }

                    // Nếu đã qua ít nhất 1 phút từ lần ghi trước đó
                    if ((DateTime.Now - lastWrite).TotalMinutes >= 1)
                    {
                        Write_Symbol_File();
                        lastWrite = DateTime.Now;
                    }
                }
                else
                {
                    // update vào db lúc 15h30
                    if (StockMem.c_update_price == false && DateTime.Now.ToString("HH:mm") == "15:30")
                    {
                        foreach (var item in DataMemory.c_lstSymbolData)
                        {
                            if (item.Status != "2")
                            {
                                _da.UpdateCurrenPrice(item);
                            }

                        }
                        StockMem.c_update_price = true;

                        Write_Symbol_File();
                        lastWrite = DateTime.Now;
                    }

                    // reset mem nếu qua ngày
                    if (DateTime.Now.Date != StockMem.c_trading_date.Date)
                    {
                        StockMem.c_trading_date = DateTime.Now;

                        // XÓA DỮ liệu giá khớp từng mã chứng khoán
                        StockMem.c_dicStockMatchStatistics = new Dictionary<string, Dictionary<long, StockMatchStatisticInfo>>();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
    }

}
