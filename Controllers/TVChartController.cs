using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebForm.Common;

namespace WebForm.Controllers
{
    public class TVChartController : Controller
    {
        private readonly object[] c_exchanges = new object[] {
            new { value = "", name = "Tất cả", desc = "" },
            new { value = "HNX", name = "HNX", desc = "HNX" },
            new { value = "UPCOM", name = "UPCOM", desc = "UPCOM" },
            new { value = "HSX", name = "HSX", desc = "HSX" }
        };
        private readonly string[] c_resolutions = { "1", "5", "15", "30", "60", "D", "W", "M" };
        private readonly string[] c_indayResolutions = { "1", "5", "15", "30", "60" };
        private readonly Dictionary<string, string> c_dicMapIndex = new Dictionary<string, string> {
            { "HSX", "VNINDEX" },
            { "HOSE", "VNINDEX" },
            { "VNINDEX", "VNINDEX" },
            { "HNX", "HNXINDEX" },
            { "HNXINDEX", "HNXINDEX" },
            { "UPCOM", "HNXUPCOMINDEX" },
            { "HNXUPCOMINDEX", "HNXUPCOMINDEX" }
        };

        private const string c_Type_Stock = "Stock";
        private const string c_Type_Index = "Index";

        //
        [HttpGet, Route("config")]
        public object Config()
        {
            return new
            {
                supports_search = true,
                supports_group_request = false,
                supports_marks = true,
                supports_timescale_marks = true,
                supports_time = true,
                exchanges = c_exchanges,
                symbols_types = new object[] { new { name = "Tất cả", value = "" }, new { name = "Cổ phiếu", value = c_Type_Stock }, new { name = "Chỉ số", value = c_Type_Index } },
                supported_resolutions = c_resolutions
            };
        }

        [HttpGet, Route("search")]
        public IEnumerable<object> Search([FromQuery] int limit, [FromQuery] string query = "", [FromQuery] string? type = "", [FromQuery] string? exchange = "")
        {
            try
            {
                var _results = new object[limit];
                int _countAdded = 0;

                // Lay thong tin chi so trc
                if (string.IsNullOrEmpty(type) || type.Equals(c_Type_Index) && _countAdded < limit)
                {
                    foreach (var item in IndexMem.GetAll()?.Where(x => x.IndexCode.Contains(query, StringComparison.OrdinalIgnoreCase)))
                    {
                        _results[_countAdded] = new { symbol = item.IndexCode, full_name = item.IndexCode, description = item.IndexCode, exchange = GetExchangeByMarketCode(item.MarketCode), type = c_Type_Index };

                        _countAdded++;
                        if (_countAdded >= limit)
                        {
                            break;
                        }
                    }
                }

                // Lay thong tin chung khoan
                if (string.IsNullOrEmpty(type) || type.Equals(c_Type_Stock) && _countAdded < limit)
                {
                    foreach (var item in StockMem.GetAll()?.Where(x => x.Symbol.Contains(query, StringComparison.OrdinalIgnoreCase)))
                    {
                        _results[_countAdded] = new { symbol = item.Symbol, full_name = item.Symbol, description = item.Symbol, exchange = GetExchangeByMarketCode(item.MarketCode), type = c_Type_Stock };

                        _countAdded++;
                        if (_countAdded >= limit)
                        {
                            break;
                        }
                    }
                }

                //
                return _results.Where(x => x != null).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return new object[0];
        }

        [HttpGet, Route("symbols")]
        public object Symbol_Info(string symbol)
        {
            string _symboCode = "", _symboName = "", _type = "";
            try
            {
                if (!string.IsNullOrEmpty(symbol))
                {
                    if (c_dicMapIndex?.ContainsKey(symbol.ToUpper()) == true)
                    {
                        symbol = c_dicMapIndex[symbol.ToUpper()];
                    }

                    var _indexInfo = IndexMem.GetByCode(symbol);
                    if (_indexInfo != null)
                    {
                        _symboCode = _indexInfo.IndexCode;
                        _symboName = _indexInfo.IndexName;
                        _type = c_Type_Index;
                    }
                    else
                    {
                        //dangtq tạm thời chưa có -> sẽ lấy ở thằng VPS
                        //var _stockInfo = ServiceTVSI.GetObjStockInfo(symbol);

                        var _stockInfo = StockMem.GetBySymbol(symbol);
                        if (_stockInfo != null)
                        {
                            _symboCode = _stockInfo.SymbolName;
                            _symboName = _stockInfo.SymbolName;
                            _type = c_Type_Stock;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return new
            {
                name = _symboCode,
                symbol = _symboCode,
                timezone = "Asia/Bangkok",
                minmov = 1,
                minmov2 = 0,
                pricescale = 100,
                pointvalue = 1,
                session = "0900-1500",
                has_intraday = true,
                has_no_volume = false,
                ticker = _symboCode,
                description = _symboCode,
                type = _type,
                supported_resolutions = c_resolutions
            };
        }

        [HttpGet, Route("time")]
        public string Time()
        {
            return Utils.DateTimeToTimeStamp(DateTime.UtcNow).ToString();
        }

        [HttpGet, Route("marks")]
        public IEnumerable<object> Marks(string symbol, string resolution, long from, long to)
        {
            return new string[0];
        }

        [HttpGet, Route("timescale_marks")]
        public IEnumerable<object> Timescale_Marks(string symbol, string resolution, long from, long to)
        {
            return new string[0];
        }

        [HttpGet, Route("history")]
        public object History(string symbol, string resolution, long from, long to)
        {
            //history?symbol=ticker&resolution=D&from=1605413135&to=1636517195

            long[] arrTime = new long[0];
            decimal[] arrOpen = new decimal[0];
            decimal[] arrClose = new decimal[0];
            decimal[] arrHigh = new decimal[0];
            decimal[] arrLow = new decimal[0];
            decimal[] arrVolumn = new decimal[0];


            if (from < 0)
            {
                return new
                {
                    t = arrTime,
                    //
                    o = arrOpen,
                    c = arrClose,
                    h = arrHigh,
                    l = arrLow,
                    //
                    v = arrVolumn,
                    s = arrTime?.Length > 0 ? "ok" : "no_data"
                };
            }

            DateTime fromDateUTC = Utils.UnixTimeStampToDateTime(from);
            DateTime toDateUTC = Utils.UnixTimeStampToDateTime(to);

            try
            {
                if (!string.IsNullOrEmpty(symbol) && !string.IsNullOrEmpty(resolution))
                {
                    var _indexInfo = IndexMem.GetByCode(symbol);
                    bool isIndex = _indexInfo != null && !string.IsNullOrEmpty(_indexInfo.IndexCode);

                    // Lay dl trong ngay
                    if (c_indayResolutions.Contains(resolution))
                    {
                        if (isIndex)
                        {
                            // Lay dl cua chi so
                            var lstIndexOnline = GetIndexData_Inday_ByResolution(symbol, resolution, from, to);
                            if (lstIndexOnline?.Count > 0)
                            {
                                arrTime = lstIndexOnline.Select(x => (long)Math.Floor((decimal)x.TimestampUTC / 1000)).ToArray();
                                arrOpen = lstIndexOnline.Select(x => x.OpenIndex).ToArray();
                                arrClose = lstIndexOnline.Select(x => x.CloseIndex).ToArray();
                                arrHigh = lstIndexOnline.Select(x => x.HighestIndex).ToArray();
                                arrLow = lstIndexOnline.Select(x => x.LowestIndex).ToArray();
                                arrVolumn = lstIndexOnline.Select(x => x.ChangeTradedQtty).ToArray();
                            }
                        }
                        else
                        {
                            // Lay du lieu cua ck
                            var lstOnline = GetDataStock_Inday_ByResolution(symbol, resolution, from, to);
                            if (lstOnline?.Count > 0)
                            {
                                arrTime = lstOnline.Select(x => (long)Math.Floor((decimal)x.TimestampUTC / 1000)).ToArray();
                                arrOpen = lstOnline.Select(x => x.OpenPrice).ToArray();
                                arrClose = lstOnline.Select(x => x.ClosePrice).ToArray();
                                arrHigh = lstOnline.Select(x => x.HighestPrice).ToArray();
                                arrLow = lstOnline.Select(x => x.LowestPrice).ToArray();
                                arrVolumn = lstOnline.Select(x => x.TotalTradedQtty).ToArray();
                            }
                        }
                    }
                    else
                    {
                        if (isIndex)
                        {
                            // Lay dl cua chi so
                            var lstIndexHist = IndexMem.GetIndexHistByIndexCode_TradingView(symbol, fromDateUTC.ToLocalTime(), toDateUTC.ToLocalTime())?.OrderBy(x => x.TradingDate).ToList();
                            if (lstIndexHist?.Count > 0)
                            {
                                arrTime = lstIndexHist.Select(x => Utils.DateTimeToTimeStamp(Utils.StringToDateTime(x.TradingDate))).ToArray();
                                arrOpen = lstIndexHist.Select(x => x.OpenIndex).ToArray();
                                arrClose = lstIndexHist.Select(x => x.CloseIndex).ToArray();
                                arrHigh = lstIndexHist.Select(x => x.HighestIndex).ToArray();
                                arrLow = lstIndexHist.Select(x => x.LowestIndex).ToArray();
                                arrVolumn = lstIndexHist.Select(x => x.NM_TotalTradedQtty).ToArray();
                            }
                        }
                        else
                        {

                            // lấy dữ liệu từ fiin
                            var lstHist = GetStockChartData_Hist(symbol, fromDateUTC.ToLocalTime(), toDateUTC.ToLocalTime());

                            // Lay du lieu cua ck
                            //var lstHist = MemoryData.StockMem.GetStockHistBySymbol_TradingView(symbol, fromDateUTC.ToLocalTime(), toDateUTC.ToLocalTime())?.OrderBy(x => x.TradingDate).ToList();
                            if (lstHist?.Count > 0)
                            {
                                return new
                                {
                                    //t = lstHist.Select(x => x.historydate_timestamp).ToArray(),
                                    t = lstHist.Select(x => Utils.DateTimeToTimeStamp(x.historydate)).ToArray(),
                                    //
                                    o = lstHist.Select(x => x.OpenPrice).ToArray(),
                                    c = lstHist.Select(x => x.ClosePrice).ToArray(),
                                    h = lstHist.Select(x => x.Max).ToArray(),
                                    l = lstHist.Select(x => x.Min).ToArray(),
                                    //
                                    v = lstHist.Select(x => x.MatchQtty).ToArray(),
                                    s = lstHist?.Count > 0 ? "ok" : "no_data"
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return new
            {
                t = arrTime,
                //
                o = arrOpen,
                c = arrClose,
                h = arrHigh,
                l = arrLow,
                //
                v = arrVolumn,
                s = arrTime?.Length > 0 ? "ok" : "no_data"
            };
        }

        [Route("charts/{apiVersion}/charts")]
        public object SaveCharts([FromQuery] string client, [FromQuery] string user, [FromQuery] int chart = 0)
        {
            string username = user;
            if (Request.Method.ToUpper() == "GET")
            {
                if (chart > 0)
                {
                    // Get by id
                    TVChartUserConfig data = TVChartUserConfig_GetById(chart);
                    if (data != null && data.id > 0)
                    {
                        return new
                        {
                            status = "ok",
                            data = new
                            {
                                id = data.id,
                                name = data.name,
                                content = data.content,
                                symbol = data.symbol,
                                resolution = data.resolution,
                                timestamp = data.timestamp,
                            }
                        };
                    }
                }
                else
                {
                    List<TVChartUserConfig> memDatas = TVChartUserConfig_GetByUser(username);

                    // Get alls
                    if (memDatas != null && memDatas.Count > 0)
                    {
                        // Get saved charts 
                        var datas = memDatas.Select(x => new
                        {
                            id = x.id,
                            name = x.name,
                            //content = x.content,
                            symbol = x.symbol,
                            resolution = x.resolution,
                            timestamp = x.timestamp
                        }).ToList();

                        return new { status = "ok", data = datas };

                    }
                    else
                    {
                        return new { status = "ok", data = new object[0] };
                    }
                }
            }
            else if (Request.Method.ToUpper() == "DELETE")
            {
                // Delete by id
                int result = TVChartUserConfig_Remove(chart, username);
                if (result > 0)
                {
                    return new { status = "ok", id = chart };
                }
            }
            else if (Request.Method.ToUpper() == "POST")
            {
                IFormCollection form = Request.Form;
                // Add or Update (chart > 0)
                if (chart == 0)
                {
                    int result = TVChartUserConfig_Add(form["name"], form["content"], form["symbol"], form["resolution"],
                        Utils.DateTimeToUnixTimeStamp(DateTime.UtcNow), 0, username);

                    if (result > 0)
                    {
                        return new { status = "ok", id = result };
                    }
                }
                else
                {
                    int result = TVChartUserConfig_Update(chart, form["name"], form["content"], form["symbol"], form["resolution"],
                        Utils.DateTimeToUnixTimeStamp(DateTime.UtcNow), 0, username);
                    if (result > 0)
                    {
                        return new { status = "ok", id = result };
                    }
                }
            }

            return new { status = "error" };
        }

        private string GetExchangeByMarketCode(string marketCode)
        {
            marketCode ??= "";
            string exchange = "";

            if (marketCode == MarketCode.HNX)
            {
                exchange = "HNX";
            }
            else if (marketCode == MarketCode.HoSE)
            {
                exchange = "HSX";
            }
            else if (marketCode == MarketCode.UpCOM)
            {
                exchange = "UPCOM";
            }
            return exchange;
        }

        private string GetMarketCodeByExchange(string exchange)
        {
            exchange ??= "";
            string marketCode = "";

            if (exchange == "HNX")
            {
                marketCode = MarketCode.HNX;
            }
            else if (exchange == "HSX")
            {
                marketCode = MarketCode.HoSE;
            }
            else if (exchange == "UPCOM")
            {
                marketCode = MarketCode.UpCOM;
            }
            return marketCode;
        }

        private List<IndexStatisticInfo> GetIndexData_Inday_ByResolution(string indexCode, string resolution, long from, long to)
        {
            try
            {
                int.TryParse(resolution, out int _resolution);
                var lstIndexOnline = IndexMem.GetIndexStatisticsByCode(indexCode)?.Where(x => Math.Floor((decimal)x.TimestampUTC / 1000) >= from && Math.Floor((decimal)x.TimestampUTC / 1000) <= to).OrderBy(x => x.TimestampUTC).Select(x => (IndexStatisticInfo)x.CloneObject()).ToList();
                if (c_indayResolutions.Contains(resolution) && _resolution > 0 && lstIndexOnline?.Count > 0)
                {
                    if (_resolution == 1)
                    {
                        return lstIndexOnline;
                    }
                    else
                    {
                        lstIndexOnline.ForEach(x =>
                        {
                            x.IndexTime = Utils.UnixTimeStampToDateTime(x.TimestampUTC);
                        });
                        //
                        return lstIndexOnline.GroupBy(x => Math.Floor(Convert.ToDecimal(x.IndexTime.TimeOfDay.TotalMinutes) / _resolution)).Select(g =>
                        {
                            DateTime _tradeTime = g.Min(x => x.IndexTime.Date).AddMinutes((int)g.Key * _resolution);
                            return new IndexStatisticInfo()
                            {
                                IndexTime = _tradeTime,
                                TimestampUTC = Utils.DateTimeToTimeStampMillisecond(_tradeTime),
                                IndexCode = g.Min(x => x.IndexCode),
                                OpenIndex = g.OrderBy(x => x.TimestampUTC).First().OpenIndex,
                                CloseIndex = g.OrderBy(x => x.TimestampUTC).Last().CloseIndex,
                                HighestIndex = g.Max(x => x.HighestIndex),
                                LowestIndex = g.Min(x => x.LowestIndex),
                                TotalTradedQtty = g.Max(x => x.TotalTradedQtty),
                                TotalTradedValue = g.Max(x => x.TotalTradedValue),
                                ChangeTradedQtty = g.Sum(x => x.ChangeTradedQtty),
                                ChangeTradedValue = g.Sum(x => x.ChangeTradedValue),
                            };
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
            return null;
        }


        private List<StockMatchStatisticInfo> GetDataStock_Inday_ByResolution(string symbol, string resolution, long from, long to)
        {
            try
            {
                int.TryParse(resolution, out int _resolution);
                var lstOnline = StockMem.GetStockMatchStatisticsBySymbol(symbol)?.Where(x => Math.Floor((decimal)x.TimestampUTC / 1000) >= from && Math.Floor((decimal)x.TimestampUTC / 1000) <= to).OrderBy(x => x.TimestampUTC).ToList();
                if (c_indayResolutions.Contains(resolution) && _resolution > 0 && lstOnline?.Count > 0)
                {
                    if (_resolution == 1)
                    {
                        return lstOnline;
                    }
                    else
                    {
                        lstOnline.ForEach(x =>
                        {
                            x.TradeTime = Utils.UnixTimeStampToDateTime(x.TimestampUTC);
                        });
                        //
                        return lstOnline.GroupBy(x => Math.Floor(Convert.ToDecimal(x.TradeTime.TimeOfDay.TotalMinutes) / _resolution)).Select(g =>
                        {
                            DateTime _tradeTime = g.Min(x => x.TradeTime.Date).AddMinutes((int)g.Key * _resolution);
                            return new StockMatchStatisticInfo()
                            {
                                TradeTime = _tradeTime,
                                TimestampUTC = Utils.DateTimeToTimeStampMillisecond(_tradeTime),
                                Symbol = g.Min(x => x.Symbol),
                                OpenPrice = g.OrderBy(x => x.TimestampUTC).First().OpenPrice,
                                OpenQtty = g.OrderBy(x => x.TimestampUTC).First().OpenQtty,
                                ClosePrice = g.OrderBy(x => x.TimestampUTC).Last().ClosePrice,
                                CloseQtty = g.OrderBy(x => x.TimestampUTC).Last().CloseQtty,
                                HighestPrice = g.Max(x => x.HighestPrice),
                                LowestPrice = g.Min(x => x.LowestPrice),
                                TotalTradedQtty = g.Sum(x => x.TotalTradedQtty),
                                TotalTradedValue = g.Sum(x => x.TotalTradedValue),
                            };
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Lấy dữ liệu quá khứ từ fiin (chưa lấy ngày hiện tại -> phải viết thêm vào)
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        private List<StockHistoryInfo> GetStockChartData_Hist(string symbol, DateTime fromDate, DateTime toDate)
        {
            var lstStockHistory = new List<StockHistoryInfo>();
            try
            {
                lstStockHistory = StockMem.LoadData_StockFrom_VPS(symbol, fromDate, toDate);
                var epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                if (toDate.Date == DateTime.Now.Date)
                {
                    // Lấy thêm trong ngày vào đây
                    StockHistoryInfo _inday = Get_Stock_Inday(symbol);
                    if (_inday != null && _inday.OpenPrice > 0)
                    {
                        _inday.historydate_timestamp = Convert.ToDecimal((_inday.historydate.ToUniversalTime() - epochDate).Ticks / 10000000);
                        lstStockHistory.Add(_inday);
                    }
                }

                lstStockHistory.ForEach(o => o.historydate_timestamp = Convert.ToDecimal((o.historydate.ToUniversalTime() - epochDate).Ticks / 10000000));

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return lstStockHistory;
        }

        private StockHistoryInfo Get_Stock_Inday(string p_symbol)
        {
            try
            {
                // Lấy thêm trong ngày vào đây
                StockMemInfo _stock_inday = StockMem.GetBySymbol(p_symbol);
                if (_stock_inday != null)
                {
                    StockHistoryInfo _inday = new StockHistoryInfo();
                    _inday.historydate = DateTime.Now; // xem lại -> phải lấy ngày hiện tại
                    _inday.symbol = p_symbol;
                    _inday.OpenPrice = _stock_inday.OpenPrice != 0 ? _stock_inday.OpenPrice : _stock_inday.MatchPrice;
                    _inday.OpenPrice = _inday.OpenPrice > 0 ? _inday.OpenPrice / 1000 : 0;

                    _inday.ClosePrice = _stock_inday.ClosePrice != 0 ? _stock_inday.ClosePrice : _stock_inday.MatchPrice;
                    _inday.ClosePrice = _inday.ClosePrice > 0 ? _inday.ClosePrice / 1000 : 0;


                    _inday.Max = _stock_inday.HighestPrice > 0 ? _stock_inday.HighestPrice / 1000 : 0;
                    _inday.Min = _stock_inday.LowestPrice > 0 ? _stock_inday.LowestPrice / 1000 : 0;
                    _inday.MatchQtty = _stock_inday.TotalTradedQttyNM;

                    return _inday;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return null;
        }

        private int TVChartUserConfig_Add(string name, string content, string symbol, string resolution, long timestamp, int userid, string username)
        {
            var rs = -1;

            try
            {
                rs = (new TVChartUserConfigDA()).TVChartUserConfig_Add(new TVChartUserConfig()
                {
                    name = name,
                    content = content,
                    symbol = symbol,
                    resolution = resolution,
                    timestamp = timestamp,
                    userid = userid,
                    username = username,
                    createddate = DateTime.Now,
                    createdby = username,
                });

                if (rs > 0)
                {
                    Config_Chart.InitTVChartUserConfig();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }

            return rs;
        }

        private int TVChartUserConfig_Update(int id, string name, string content, string symbol, string resolution, long timestamp, int userid, string username)
        {
            var rs = -1;

            try
            {
                rs = (new TVChartUserConfigDA()).TVChartUserConfig_Update(new TVChartUserConfig()
                {
                    id = id,
                    name = name,
                    content = content,
                    symbol = symbol,
                    resolution = resolution,
                    timestamp = timestamp,
                    userid = userid,
                    username = username,
                    modifieddate = DateTime.Now,
                    modifiedby = username,
                });

                if (rs > 0)
                {
                    Config_Chart.InitTVChartUserConfig();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }

            return rs;
        }

        private int TVChartUserConfig_Remove(int id, string username)
        {
            var rs = -1;

            try
            {
                rs = (new TVChartUserConfigDA()).TVChartUserConfig_Remove(id, username);
                if (rs > 0)
                {
                    Config_Chart.InitTVChartUserConfig();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }

            return rs;
        }

        private TVChartUserConfig TVChartUserConfig_GetById(int id)
        {
            try
            {
                TVChartUserConfig userConfig = Config_Chart.TVChartUserConfig_GetById(id);
                return userConfig;
            }
            catch (Exception)
            {
            }
            return null;
        }

        private List<TVChartUserConfig> TVChartUserConfig_GetByUser(string username)
        {
            try
            {
                List<TVChartUserConfig> _lst = Config_Chart.TVChartUserConfig_GetByUser(username);
                return _lst;
            }
            catch (Exception)
            {
            }
            return null;
        }

    }
}
