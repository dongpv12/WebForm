using RestSharp;
using System.Globalization;
using WebForm.Common;

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
                    OpenPrice = (decimal)(data.o[i] * 1000),
                    ClosePrice = (decimal)(data.c[i] * 1000),
                    Max = (decimal)(data.h[i] * 1000),
                    Min = (decimal)(data.l[i] * 1000),
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
    }
}
