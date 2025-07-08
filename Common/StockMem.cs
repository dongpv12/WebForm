using Newtonsoft.Json;
using Skender.Stock.Indicators;
using System.Collections.Concurrent;
using WebForm.Common;

namespace WebForm
{
    public class StockMem
    {
        public static ConcurrentQueue<string> c_queueMessage = new ConcurrentQueue<string>();

        private static object c_lockStocks = new object();
        public static Dictionary<string, StockMemInfo> c_dicStocks = new Dictionary<string, StockMemInfo>();

        public static object c_lockStockMatchStatistics = new object();
        public static Dictionary<string, Dictionary<long, StockMatchStatisticInfo>> c_dicStockMatchStatistics = new Dictionary<string, Dictionary<long, StockMatchStatisticInfo>>();

        public static StockMemInfo GetBySymbol(string symbol)
        {
            try
            {
                if (c_dicStocks.ContainsKey(symbol))
                {
                    return c_dicStocks[symbol];
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
            return null;
        }

        public static List<StockMatchStatisticInfo> GetStockMatchStatisticsBySymbol(string symbol)
        {
            if (c_dicStockMatchStatistics.ContainsKey(symbol))
            {
                return c_dicStockMatchStatistics[symbol].Values?.ToList();
            }
            return null;
        }

        public static List<StockMemInfo> GetAll()
        {
            if (c_dicStocks != null && c_dicStocks.Count > 0)
            {
                return c_dicStocks.Values.ToList();
            }
            return null;
        }

        public static List<StockHistoryInfo> LoadData_StockFrom_VPS(string p_symbol, DateTime fromDate, DateTime toDate)
        {
            try
            {
                //DateTime _dt_1 = DateTime.Now;
                //DateTime _dt_2 = DateTime.Now.AddMonths(-6);
                long from = Utils.DateTimeToTimeStamp(fromDate);
                long to = Utils.DateTimeToTimeStamp(toDate);

                string _url = string.Format("https://histdatafeed.vps.com.vn/tradingview/history?symbol={0}&resolution=1D&from={1}&to={2}", p_symbol, from, to); // Thay đổi đường dẫn file cần download ở đây
                string ContentCK = Utils.CallAPI(_url);
                StockData_TradingView _StockData_VPS = Newtonsoft.Json.JsonConvert.DeserializeObject<StockData_TradingView>(ContentCK);
                List<StockHistoryInfo> _lst_convert = Utils.ConvertToRecords(_StockData_VPS);
                return _lst_convert;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new List<StockHistoryInfo>();
            }
        }

        public static List<Quote> LoadData_StockFrom_VPS_Quote(string p_symbol, DateTime fromDate, DateTime toDate)
        {
            try
            {
                //DateTime _dt_1 = DateTime.Now;
                //DateTime _dt_2 = DateTime.Now.AddMonths(-6);
                long from = Utils.DateTimeToTimeStamp(fromDate);
                long to = Utils.DateTimeToTimeStamp(toDate);

                string _url = string.Format("https://histdatafeed.vps.com.vn/tradingview/history?symbol={0}&resolution=1D&from={1}&to={2}", p_symbol, from, to); // Thay đổi đường dẫn file cần download ở đây
                string ContentCK = Utils.CallAPI(_url);
                StockData_TradingView _StockData_VPS = Newtonsoft.Json.JsonConvert.DeserializeObject<StockData_TradingView>(ContentCK);
                List<Quote> _lst_convert = Utils.ConvertTo_Quote(_StockData_VPS);
                return _lst_convert;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new List<Quote>();
            }
        }

        public static void Read_SymbolFile()
        {
            try
            {
                string _accountFile = System.IO.Path.Combine(ConfigInfo.ContentRootPath, "Data", "Symbol_Info.json");
                string jsonString = File.ReadAllText(_accountFile);
                List<StockMemInfo> _lstAccount = JsonConvert.DeserializeObject<List<StockMemInfo>>(jsonString);
                foreach (var item in _lstAccount)
                {
                    c_dicStocks[item.Symbol] = item;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }

    }

    public class IndexMem
    {
        private static object c_lock = new object();
        private static Dictionary<string, List<TTIndexInfo>> c_dicIndexesHist = new Dictionary<string, List<TTIndexInfo>>();

        // Thong ke chi so theo 1p
        private static Dictionary<string, Dictionary<long, IndexStatisticInfo>> c_dicIndexStatistics = new Dictionary<string, Dictionary<long, IndexStatisticInfo>>();

        private static Dictionary<string, IndexMemInfo> c_dicIndexes = new Dictionary<string, IndexMemInfo>();

        public static List<IndexStatisticInfo> GetIndexStatisticsByCode(string indexCode)
        {
            if (!string.IsNullOrEmpty(indexCode) && c_dicIndexStatistics.ContainsKey(indexCode))
            {
                return c_dicIndexStatistics[indexCode].Values?.ToList();
            }
            return null;
        }

        public static IndexMemInfo GetByCode(string indexCode)
        {
            if (c_dicIndexes != null && c_dicIndexes.ContainsKey(indexCode))
            {
                return c_dicIndexes[indexCode];
            }
            return null;
        }

        public static List<TTIndexInfo> GetIndexHistByIndexCode_TradingView(string indexCode, DateTime fromDate, DateTime toDate)
        {
            return GetIndexHistByIndexCode(indexCode, fromDate, toDate, true)?.Where(x => x.NM_TotalTradedQtty > 0)?.ToList();
        }

        public static List<TTIndexInfo> GetIndexHistByIndexCode(string indexCode, DateTime fromDate, DateTime toDate, bool includeInday = false)
        {
            try
            {
                string strFromDate = fromDate.ToString("yyyyMMdd");
                string strToDate = toDate.ToString("yyyyMMdd");
                List<TTIndexInfo> lstReturn = new List<TTIndexInfo>();

                // Lay qua khu
                if (c_dicIndexesHist != null && c_dicIndexesHist.ContainsKey(indexCode))
                {
                    lstReturn = c_dicIndexesHist[indexCode]?.Where(x => string.Compare(x.TradingDate, strFromDate) >= 0 && string.Compare(x.TradingDate, strToDate) <= 0).ToList();
                }

                // Lay trong ngay
                if (includeInday)
                {
                    if (c_dicIndexes != null && c_dicIndexes.ContainsKey(indexCode))
                    {
                        lock (c_lock)
                        {
                            IndexMemInfo obj = c_dicIndexes[indexCode];
                            if (obj != null && string.Compare(obj.TradingDate, strFromDate) >= 0 && string.Compare(obj.TradingDate, strToDate) <= 0
                                && lstReturn?.Any(x => x.TradingDate == obj.TradingDate) == false)
                            {
                                lstReturn.Add(new TTIndexInfo()
                                {
                                    TradingDate = obj.TradingDate,
                                    IndexCode = obj.IndexCode,
                                    IndexName = obj.IndexName,
                                    TypeIndex = obj.TypeIndex,
                                    MarketCode = obj.MarketCode,
                                    PriorIndexVal = obj.PriorIndex,
                                    Value = obj.CurrentIndex,
                                    //
                                    Change = obj.ChangeIndex,
                                    RatioChange = obj.PctChangeIndex,
                                    OpenIndex = obj.OpenIndex != 0 ? obj.OpenIndex : obj.CurrentIndex,
                                    CloseIndex = obj.CloseIndex != 0 ? obj.CloseIndex : obj.CurrentIndex,
                                    LowestIndex = obj.LowestIndex,
                                    //
                                    HighestIndex = obj.HighestIndex,
                                    TotalQtty = obj.TotalTradedQtty,
                                    NM_TotalTradedQtty = obj.TotalTradedQttyNM,
                                    NM_TotalTradedValue = obj.TotalTradedValueNM,
                                    PT_TotalTradedQtty = obj.TotalTradedQttyPT,
                                    //
                                    PT_TotalTradedValue = obj.TotalTradedValuePT,
                                    BuyForeignQtty = obj.BuyForeignQtty,
                                    BuyForeignValue = obj.BuyForeignValue,
                                    SellForeignQtty = obj.SellForeignQtty,
                                    SellForeignValue = obj.SellForeignValue,
                                    //
                                    TotalStock = obj.TotalStock,
                                    NumSymbolAdvances = obj.Up,
                                    NumUpCeiling = obj.UpCeil,
                                    NumSymbolNochange = obj.NoChange,
                                    NumSymbolDeclines = obj.Down,
                                    NumDownFloor = obj.DownFloor,
                                });
                            }
                        }
                    }
                }

                return lstReturn;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
            return null;
        }
        public static List<IndexMemInfo> GetAll()
        {
            if (c_dicIndexes != null && c_dicIndexes.Count > 0)
            {
                return c_dicIndexes.Values.ToList();
            }
            return null;
        }
    }

    public class Config_Chart
    {
        private static object _lockdicTVChartUserConfig = new object();
        private static Dictionary<int, TVChartUserConfig> c_dicTVChartUserConfig = new Dictionary<int, TVChartUserConfig>();

        public static void InitTVChartUserConfig()
        {
            try
            {
                lock (_lockdicTVChartUserConfig)
                {
                    c_dicTVChartUserConfig.Clear();
                }

                List<TVChartUserConfig> tVChartUserConfigs = CBO<TVChartUserConfig>.FillCollectionFromDataSet((new TVChartUserConfigDA()).TVChartUserConfig_GetAll());
                if (tVChartUserConfigs != null && tVChartUserConfigs.Count > 0)
                {
                    lock (_lockdicTVChartUserConfig)
                    {
                        for (int i = 0; i < tVChartUserConfigs.Count; i++)
                        {
                            c_dicTVChartUserConfig[tVChartUserConfigs[i].id] = tVChartUserConfigs[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }

        public static List<TVChartUserConfig> TVChartUserConfig_GetByUser(string username)
        {
            try
            {
                if (c_dicTVChartUserConfig != null && c_dicTVChartUserConfig.Count > 0)
                {
                    return c_dicTVChartUserConfig.Values.Where(x => !string.IsNullOrEmpty(x.username) && x.username.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return new List<TVChartUserConfig>();
        }

        public static TVChartUserConfig TVChartUserConfig_GetById(int id)
        {
            try
            {
                c_dicTVChartUserConfig.TryGetValue(id, out TVChartUserConfig tVChartUserConfig);
                return tVChartUserConfig;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return null;
        }
    }
}
