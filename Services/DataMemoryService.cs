﻿using Newtonsoft.Json;
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
                            catch (Exception ex)
                            {
                                Logger.Log.Error(ex.ToString());
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
                    // Nếu đã qua ít nhất 1 phút từ lần ghi trước đó
                    if ((DateTime.Now - lastWrite).TotalMinutes >= 1)
                    {

                        Write_Symbol_File();
                        lastWrite = DateTime.Now;
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
                                Symbol_Notify_Info _Symbol_Notify_Info = DataMemory.c_dicSymbol.ContainsKey(_symbol) ? DataMemory.c_dicSymbol[_symbol] : null;
                                if (_Symbol_Notify_Info != null)
                                {
                                    Symbol_WS_Info _Symbol_WS_Info = Utils.Init_Symbol_FromSocket(item_ck);

                                    if (_Symbol_WS_Info != null && _Symbol_WS_Info.Symbol != null && _Symbol_WS_Info.Symbol != "")
                                    {
                                        if (_Symbol_WS_Info.Current_Price > 0)
                                        {
                                            _Symbol_Notify_Info.Current_Price = _Symbol_WS_Info.Current_Price;
                                            _Symbol_Notify_Info.Close = _Symbol_WS_Info.Current_Price;
                                            if (_Symbol_Notify_Info.Low < _Symbol_WS_Info.Current_Price)
                                            {
                                                _Symbol_Notify_Info.Low = _Symbol_WS_Info.Current_Price;
                                            }

                                            if (_Symbol_Notify_Info.Hight < _Symbol_WS_Info.Current_Price)
                                            {
                                                _Symbol_Notify_Info.Hight = _Symbol_WS_Info.Current_Price;
                                            }
                                        }

                                        if (_Symbol_WS_Info.Match_Qtty > 0)
                                        {
                                            _Symbol_Notify_Info.Match_Qtty = _Symbol_WS_Info.Match_Qtty;
                                            if (_Symbol_Notify_Info.Open_Qtty <= 0)
                                            {
                                                _Symbol_Notify_Info.Open_Qtty = _Symbol_WS_Info.Match_Qtty;
                                            }
                                        }

                                        if (_Symbol_WS_Info.Volume > 0)
                                        {
                                            _Symbol_Notify_Info.Volume = _Symbol_WS_Info.Volume;
                                        }
                                        if (_Symbol_WS_Info.TotalValue > 0)
                                        {
                                            _Symbol_Notify_Info.TotalValue = _Symbol_WS_Info.TotalValue;
                                        }
                                        if (_Symbol_WS_Info.Open > 0)
                                        {
                                            _Symbol_Notify_Info.Open = _Symbol_WS_Info.Open;
                                        }
                                        if (_Symbol_WS_Info.Name != null && _Symbol_WS_Info.Name !=  "")
                                        {
                                            _Symbol_Notify_Info.Name = _Symbol_WS_Info.Name;
                                        }

                                        if (StockMem.c_dicStocks.ContainsKey(_Symbol_WS_Info.Symbol) == false)
                                        {
                                            StockMemInfo stockMemInfo = new StockMemInfo
                                            {
                                                Symbol = _Symbol_Notify_Info.Symbol,
                                                SymbolName = _Symbol_Notify_Info.Name,
                                                MarketCode = _Symbol_Notify_Info.MarketCode,
                                                OpenPrice = _Symbol_Notify_Info.Open,
                                                ClosePrice = _Symbol_Notify_Info.Close,
                                                HighestPrice = _Symbol_Notify_Info.Hight,
                                                LowestPrice = _Symbol_Notify_Info.Low,
                                                TotalTradedQttyNM = _Symbol_Notify_Info.Volume,
                                                TotalTradedValueNM = _Symbol_Notify_Info.TotalValue,
                                                MatchPrice = _Symbol_Notify_Info.Current_Price
                                            };

                                            StockMem.c_dicStocks[_Symbol_Notify_Info.Symbol] = stockMemInfo;
                                        }
                                        else
                                        {
                                            StockMemInfo stockMemInfo = StockMem.c_dicStocks[_Symbol_Notify_Info.Symbol];
                                            stockMemInfo.SymbolName = _Symbol_Notify_Info.Name;
                                            stockMemInfo.MarketCode = _Symbol_Notify_Info.MarketCode;
                                            stockMemInfo.OpenPrice = _Symbol_Notify_Info.Open;
                                            stockMemInfo.ClosePrice = _Symbol_Notify_Info.Close;
                                            stockMemInfo.HighestPrice = _Symbol_Notify_Info.Hight;
                                            stockMemInfo.LowestPrice = _Symbol_Notify_Info.Low;
                                            stockMemInfo.TotalTradedQttyNM = _Symbol_Notify_Info.Volume;
                                            stockMemInfo.TotalTradedValueNM = _Symbol_Notify_Info.TotalValue;
                                            stockMemInfo.MatchPrice = _Symbol_Notify_Info.Current_Price;
                                            StockMem.c_dicStocks[_Symbol_Notify_Info.Symbol] = stockMemInfo;
                                        }

                                        // update vào bảng mem dữ liệu trong ngày
                                        Utils.AddOrUpdateStockMatchStatistic(new StockMatchStatisticInfo()
                                        {
                                            TradeTime = DateTime.Now,
                                            TimestampUTC = Utils.DateTimeToTimeStampMillisecond(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).ToUniversalTime()),
                                            Symbol = _Symbol_Notify_Info.Symbol,
                                            OpenPrice = _Symbol_Notify_Info.Open,
                                            OpenQtty = _Symbol_Notify_Info.Open_Qtty,
                                            HighestPrice = _Symbol_Notify_Info.Hight,
                                            LowestPrice = _Symbol_Notify_Info.Low,
                                            ClosePrice = _Symbol_Notify_Info.Close,
                                            CloseQtty = 0,
                                            TotalTradedQtty = _Symbol_Notify_Info.Volume,
                                            TotalTradedValue = _Symbol_Notify_Info.TotalValue,
                                        });

                                        // update vao DB  
                                        try
                                        {
                                            //Symbol_Notify_Info info = new Symbol_Notify_Info();
                                            //info.Symbol = _Symbol_Notify_Info.Symbol;
                                            //info.Current_Price = _Symbol_Notify_Info.Current_Price;
                                            //_ck = _da.UpdateCurrenPrice(info);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Log.Error(ex.ToString());
                                        }
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
                    // Nếu đã qua ít nhất 1 phút từ lần ghi trước đó
                    if ((DateTime.Now - lastWrite).TotalMinutes >= 1)
                    {
                        Write_Symbol_File();
                        lastWrite = DateTime.Now;
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
