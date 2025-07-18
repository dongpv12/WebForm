using RestSharp;
using Skender.Stock.Indicators;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebForm.Common;
using WebForm.Models;
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
                    OpenPrice = (decimal)(data.o[i]),
                    ClosePrice = (decimal)(data.c[i]),
                    Max = (decimal)(data.h[i]),
                    Min = (decimal)(data.l[i]),
                    MatchQtty = data.v[i]
                });
            }

            return records;
        }

        public static List<Quote> ConvertTo_Quote(StockData_TradingView data)
        {
            var records = new List<Quote>();

            for (int i = 0; i < data.t.Count; i++)
            {
                records.Add(new Quote
                {
                    Date = DateTimeOffset.FromUnixTimeSeconds(data.t[i]).DateTime,
                    Open = (decimal)(data.o[i]),
                    Close = (decimal)(data.c[i]),
                    High = (decimal)(data.h[i]),
                    Low = (decimal)(data.l[i]),
                    Volume = data.v[i]
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
        public static async Task Save_technical_analysisAsync(string p_symbol)
        {
            try
            {
                if (string.IsNullOrEmpty(p_symbol))
                {
                    return;
                }

                //StockAnalysis stockAnalysis = Get_technical_analysis(p_symbol);
                StockAnalysis stockAnalysis = await Analysis_Symbol.AnalyzeStockDataAsync(p_symbol);

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

        public static string Get_Symbol(string p_data)
        {
            try
            {
                string[] _arr_ck_1 = p_data.Split('|');
                return _arr_ck_1[0];
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return "";
            }
        }

        public static Symbol_WS_Info Init_Symbol_FromSocket(string p_data)
        {
            try
            {

                string[] _arr_ck_1 = p_data.Split('|');

                Symbol_WS_Info _Info = new Symbol_WS_Info();
                _Info.Symbol = _arr_ck_1[0];

                foreach (var item_2 in _arr_ck_1)
                {
                    string[] _arr_Properties = item_2.Split('*');
                    if (_arr_Properties.Length < 2)
                    {
                        continue;
                    }

                    if (_arr_Properties[0] == "1" && _arr_Properties[1] != "")
                    {
                        _Info.Ceiling_Price = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    if (_arr_Properties[0] == "2" && _arr_Properties[1] != "")
                    {
                        _Info.Floor_Price = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    if (_arr_Properties[0] == "3" && _arr_Properties[1] != "")
                    {
                        _Info.Basic_Price = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }

                    // giá khớp gần nhất
                    if (_arr_Properties[0] == "16" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Current_Price = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    // thay đổi giá
                    if (_arr_Properties[0] == "17" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.ChangePrice = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    // % thay đổi giá
                    if (_arr_Properties[0] == "18" && _arr_Properties[1] != ""&& _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.ChangePricePercent = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    // kl giao dịch
                    if (_arr_Properties[0] == "61" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Volume = Convert.ToDecimal(_arr_Properties[1]);
                    }
                    // kl giao dịch
                    if (_arr_Properties[0] == "62" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.TotalValue = Convert.ToDecimal(_arr_Properties[1]);
                    }
                    if (_arr_Properties[0] == "19" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Match_Qtty = Convert.ToDecimal(_arr_Properties[1]);
                    }
                    if (_arr_Properties[0] == "22" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Hight = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    if (_arr_Properties[0] == "24" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Low = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    if (_arr_Properties[0] == "30" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Open = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    if (_arr_Properties[0] == "31" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.Close = Convert.ToDecimal(_arr_Properties[1]) * 1000;
                    }
                    if (_arr_Properties[0] == "34" && _arr_Properties[1] != "" && _arr_Properties[1] != "ATO" && _arr_Properties[1] != "ATC")
                    {
                        _Info.MarketCode = _arr_Properties[1];
                    }
                    if (_arr_Properties[0] == "59" && _arr_Properties[1] != "")
                    {
                        _Info.Name = _arr_Properties[1];
                    }
                }

                return _Info;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return null;
            }
        }




        public static string ToSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            // Chuyển về chữ thường
            text = text.ToLowerInvariant();

            // Bỏ dấu tiếng Việt
            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            text = sb.ToString().Normalize(NormalizationForm.FormC);

            // Thay thế khoảng trắng và dấu bằng dấu "-"
            text = Regex.Replace(text, @"\s+", "-");

            // Xoá ký tự không phải a-z, 0-9, dấu "-"
            text = Regex.Replace(text, @"[^a-z0-9\-]", "");

            // Xoá dấu "-" lặp lại và ở đầu/cuối
            text = Regex.Replace(text, @"-+", "-").Trim('-');

            return text;
        }



    }
}
