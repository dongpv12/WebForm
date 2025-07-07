using RestSharp;
using System.Globalization;
using System.Text.Json;
using WebForm.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebForm
{
    public class Utils
    {
        static readonly DateTime s_UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static readonly double s_MaxUnixSeconds = (DateTime.MaxValue - s_UnixEpoch).TotalSeconds;
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return unixTimeStamp > s_MaxUnixSeconds ? s_UnixEpoch.AddMilliseconds(unixTimeStamp) : s_UnixEpoch.AddSeconds(unixTimeStamp);
        }
        public static long DateTimeToTimeStamp(DateTime utcDateTime)
        {
            return (utcDateTime - s_UnixEpoch).Ticks / TimeSpan.TicksPerSecond;
        }
        public static long DateTimeToTimeStampMillisecond(DateTime utcDateTime)
        {
            return (utcDateTime - s_UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        }
        public static DateTime FIXUTCTimestampToDateTime(string s)
        {
            DateTime _result = DateTime.MinValue;
            if (!string.IsNullOrEmpty(s) && s.Length <= 17)
            {
                DateTime.TryParseExact(s, "yyyyMMdd-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _result);
            }
            else
            {
                DateTime.TryParseExact(s, "yyyyMMdd-HH:mm:ss.ffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _result);
            }
            return _result;
        }
        //
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //
        public static DateTime StringToDateTime(string value, string format = "yyyyMMdd")
        {
            DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _date);
            return _date;
        }
        //
        public static string DateStringToDisplay(string dateStr)
        {
            if (dateStr?.Length == 8)
            {
                return dateStr.Substring(6, 2) + "/" + dateStr.Substring(4, 2) + "/" + dateStr.Substring(0, 4);
            }
            return dateStr;
        }

        public static long DateTimeToUnixTimeStamp(DateTime utcDateTime)
        {
            return (utcDateTime - s_UnixEpoch).Ticks / 10000000;
        }

        public static string CallAPI(string url)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(url, Method.Get);
                RestResponse response = client.Execute(request);
                if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "";
                }

                return response.Content == null ? "" : response.Content;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("URL err:" + url);
                Logger.Log.Error(ex.ToString());
                return "";
            }
        }

        public static List<StockHistoryInfo> ConvertToRecords(StockData_TradingView data)
        {
            var records = new List<StockHistoryInfo>();

            for (int i = 0; i < data.t.Count; i++)
            {
                records.Add(new StockHistoryInfo
                {
                    symbol = data.symbol,
                    historydate = DateTimeOffset.FromUnixTimeSeconds(data.t[i]).DateTime,
                    OpenPrice = (decimal)(data.o[i] ),
                    ClosePrice = (decimal)(data.c[i]),
                    Max = (decimal)(data.h[i] ),
                    Min = (decimal)(data.l[i]),
                    MatchQtty = data.v[i]
                });
            }

            return records;
        }

        public static void AppendMessageToFile(string json, string path = "DataSymbol/data.txt")
        {
            // Tạo thư mục nếu chưa tồn tại
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            using StreamWriter writer = new StreamWriter(path, append: true);
            writer.WriteLine(json);
        }
        //TimestampUTC = Utils.DateTimeToTimeStampMillisecond(DateTimeOffset.FromUnixTimeSeconds(data.t[i]).DateTime),

        public static long ConvertLocalTimeToUnixTimestamp()
        {
            DateTime localTime = DateTime.Now;
            DateTime utcTime = localTime.ToUniversalTime();
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timestamp = (long)(utcTime - epoch).TotalSeconds;
            return timestamp;
        }

        public static void AddOrUpdateStockMatchStatistic(StockMatchStatisticInfo matchStatisticInfo)
        {
            try
            {
                if (matchStatisticInfo != null && !string.IsNullOrEmpty(matchStatisticInfo.Symbol))
                {
                    lock (StockMem.c_lockStockMatchStatistics)
                    {
                        string symbol = matchStatisticInfo.Symbol;
                        long timestampInMinute = matchStatisticInfo.TimestampUTC;

                        Dictionary<long, StockMatchStatisticInfo> dic = new Dictionary<long, StockMatchStatisticInfo>();
                        if (StockMem.c_dicStockMatchStatistics.ContainsKey(symbol))
                        {
                            dic = StockMem.c_dicStockMatchStatistics[symbol] ?? new Dictionary<long, StockMatchStatisticInfo>();
                        }
                        //
                        if (dic.ContainsKey(timestampInMinute))
                        {
                            var obj = dic[timestampInMinute];
                            //
                            if (obj == null)
                            {
                                obj = new StockMatchStatisticInfo();
                                //
                                obj.TradeTime = matchStatisticInfo.TradeTime;
                                obj.TimestampUTC = matchStatisticInfo.TimestampUTC;
                                obj.Symbol = matchStatisticInfo.Symbol;
                                obj.OpenPrice = matchStatisticInfo.OpenPrice;
                                obj.OpenQtty = matchStatisticInfo.OpenQtty;
                            }

                            if (matchStatisticInfo.HighestPrice > obj.HighestPrice)
                            {
                                obj.HighestPrice = matchStatisticInfo.HighestPrice;
                            }
                            if (matchStatisticInfo.LowestPrice < obj.LowestPrice)
                            {
                                obj.LowestPrice = matchStatisticInfo.LowestPrice;
                            }

                            obj.ClosePrice = matchStatisticInfo.ClosePrice;
                            obj.CloseQtty = matchStatisticInfo.CloseQtty;
                            obj.TotalTradedQtty += matchStatisticInfo.TotalTradedQtty;
                            obj.TotalTradedValue += matchStatisticInfo.TotalTradedQtty;
                        }
                        else
                        {
                            dic.Add(timestampInMinute, matchStatisticInfo);
                        }
                        //
                        StockMem.c_dicStockMatchStatistics[symbol] = dic;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static StockAnalysis Get_technical_analysis(string p_symbol)
        {
            try
            {
                var client = new RestClient(ConfigInfo.ApiUrl_Analysis);
                var request = new RestRequest("api/analysis-symbol?p_symbol=" + p_symbol, Method.Get);
                RestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonSerializer.Deserialize<StockAnalysis>(response.Content ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return new StockAnalysis();
        }

        /// <summary>
        /// Hàm ví dụ phân tích mã chứng khoán
        /// </summary>
        /// <param name="p_symbol"></param>
        public static void Save_technical_analysis(string p_symbol)
        {
            try
            {
                if (string.IsNullOrEmpty(p_symbol))
                {
                    return;
                }
                StockAnalysis stockAnalysis = Get_technical_analysis(p_symbol);
                if (stockAnalysis == null || stockAnalysis.CurrentPrice <= 0)
                {
                    return;
                }

                string _msg_analysis = $"📈 PHÂN TÍCH CHỨNG KHOÁN **{p_symbol}**" + "\n";
                _msg_analysis += $"Giá hiện tại: {stockAnalysis.CurrentPrice:F0}" + "\n";
                _msg_analysis += $"Xu hướng: **{stockAnalysis.Trend}**" + "\n";
                _msg_analysis += $"Thay đổi giá 20 ngày: {stockAnalysis.Change20Days:F2}%" + "\n";
                _msg_analysis += $"RSI: {stockAnalysis.RSI:F2} - {stockAnalysis.RSISignal}" + "\n";
                _msg_analysis += $"MACD: {stockAnalysis.MACDCrossSignal}" + "\n";
                _msg_analysis += $"Bollinger Bands: {stockAnalysis.BollingerSignal}" + "\n";
                _msg_analysis += $"Biến động giá: {stockAnalysis.Volatility:F2}%" + "\n";
                _msg_analysis += $"Thay đổi khối lượng 30 phiên: {stockAnalysis.VolumeChange * 100:F2}%" + "\n";

                if (stockAnalysis.Observations != null)
                {
                    _msg_analysis += "\n📝 Nhận định:" + "\n";
                    foreach (var obs in stockAnalysis.Observations)
                    {
                        _msg_analysis += obs + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
    }
}
