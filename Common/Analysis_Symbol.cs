using MathNet.Numerics.Statistics;
using Skender.Stock.Indicators;
using System.Data;
using WebForm.Common;
using WebForm.DataAccess;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebForm
{
    public class Analysis_Symbol
    {
        public static async Task<StockAnalysis> AnalyzeStockDataAsync(string p_Symbol)
        {
            try
            {
                StockMemInfo _StockMemInfo = new StockMemInfo();
                if (StockMem.c_dicStocks.ContainsKey(p_Symbol))
                {
                    _StockMemInfo = StockMem.c_dicStocks[p_Symbol];
                }
                else
                {
                    Logger.Log.Error($"Symbol {p_Symbol} not found in StockMem.");
                    return new StockAnalysis()
                    {
                        Analysis_Info = "Symbol not found in memory."
                    };
                }

                // lấy giá từ file symbol
                if (_StockMemInfo == null)
                {
                    return new StockAnalysis()
                    {
                        Analysis_Info = ""
                    };
                }
                double Current_Price = (double)_StockMemInfo.MatchPrice;

                DateTime _dt_1 = DateTime.Now;
                DateTime _dt_2 = DateTime.Now.AddMonths(-8);
                List<Quote> Lst_Quote = StockMem.LoadData_StockFrom_VPS_Quote(p_Symbol, _dt_1, _dt_2);

                if (Lst_Quote == null || Lst_Quote.Count == 0)
                {
                    return new StockAnalysis()
                    {
                        Analysis_Info = ""
                    };
                }

                Quote quote = Lst_Quote.Where(x => x.Date.Date == DateTime.Now.Date.Date).FirstOrDefault();
                if (quote == null && Current_Price > 0)
                {
                    quote = new Quote();
                    quote.Date = DateTime.Now.Date;
                    quote.Close = (decimal)Current_Price / 1000;
                    quote.Open = _StockMemInfo.OpenPrice / 1000;
                    quote.High = _StockMemInfo.HighestPrice / 1000;
                    quote.Low = _StockMemInfo.LowestPrice / 1000;
                    quote.Volume = _StockMemInfo.TotalTradedQttyNM;
                    Lst_Quote.Insert(0, quote);
                }

                var closePrices = Lst_Quote.Select(s => s.Close).ToList();
                List<double> DailyReturn = CalculateDailyReturns(closePrices);

                // kiểm tra BB
                //List<Quote> _lst = new List<Quote>(queueManagement.c_dic_symbol_info[item.Symbol].Lst_Quote);
                IEnumerable<BollingerBandsResult> _lst_bolingBand = Lst_Quote.GetBollingerBands();

                double MA20 = 0;
                double MA50 = 0;
                double MA200 = 0;
                double RSI = 0;
                double MACD_Signal = 0;
                double MACD = 0;
                double MACD_Pre = 0; MACD_Pre = 0;
                double MACD_Signal_Pre = 0;
                double BollingerUpper = 0;
                double BollingerLower = 0;

                // tinh MA20, Ma50, Ma200
                IEnumerable<EmaResult> _ema_20 = Lst_Quote.GetEma(20);
                if (_ema_20 != null && _ema_20.Count() > 0)
                {
                    MA20 = (double)_ema_20.Last().Ema;
                }

                IEnumerable<EmaResult> _ema_50 = Lst_Quote.GetEma(50);
                if (_ema_50 != null && _ema_50.Count() > 0)
                {
                    MA50 = (double)_ema_50.Last().Ema;
                }
                IEnumerable<EmaResult> _ema_200 = Lst_Quote.GetEma(200);
                if (_ema_200 != null && _ema_200.Count() > 0)
                {
                    MA200 = (double)_ema_200.Last().Ema;
                }

                // calculate RSI of OBV
                IEnumerable<RsiResult> results_rsi = Lst_Quote
                    .GetObv()
                    .GetRsi(14);
                if (results_rsi != null && results_rsi.Count() > 0)
                {
                    RSI = (double)results_rsi.Last().Rsi;
                }

                // MACD
                IEnumerable<MacdResult> results_macd = Lst_Quote.GetMacd(12, 26, 9);
                if (results_macd != null && results_macd.Count() > 0)
                {
                    MACD = (double)results_macd.Last().Macd;
                    MACD_Signal = (double)results_macd.Last().Signal;

                    int _pre = results_macd.Count() - 1;
                    MACD_Pre = (double)results_macd.ElementAt(_pre).Macd;
                    MACD_Signal_Pre = (double)results_macd.ElementAt(_pre).Signal;

                }

                // bb
                if (_lst_bolingBand != null && _lst_bolingBand.Count() > 0)
                {
                    BollingerUpper = (double)_lst_bolingBand.Last().UpperBand;
                    BollingerLower = (double)_lst_bolingBand.Last().LowerBand;
                }

                StockAnalysis analysis = new StockAnalysis();

                List<Quote> recentData = Lst_Quote.OrderByDescending(x => x.Date).Take(20).ToList();

                // Thông tin cơ bản
                analysis.CurrentPrice = Current_Price;
                analysis.Change20Days = Math.Round((Current_Price - ((double)recentData.Last().Close * 1000)) / ((double)recentData.Last().Close * 1000) * 100, 2);

                // Xác định xu hướng
                if (Current_Price / 1000 > MA200 && MA20 > MA50)
                    analysis.Trend = "TĂNG MẠNH";
                else if (Current_Price / 1000 > MA200)
                    analysis.Trend = "TĂNG";
                else if (Current_Price / 1000 < MA200 && MA20 < MA50)
                    analysis.Trend = "GIẢM MẠNH";
                else if (Current_Price / 1000 < MA200)
                    analysis.Trend = "GIẢM";
                else
                    analysis.Trend = "ĐI NGANG";

                // Phân tích RSI
                analysis.RSI = RSI;
                if (RSI > 70)
                    analysis.RSISignal = "BÁN";
                else if (RSI < 30)
                    analysis.RSISignal = "MUA";
                else
                    analysis.RSISignal = "TRUNG LẬP";

                // Phân tích MACD
                analysis.MACD = MACD;
                analysis.MACDSignal = MACD_Signal;

                if (MACD > MACD_Signal && MACD_Pre <= MACD_Signal_Pre)
                    analysis.MACDCrossSignal = "TÍN HIỆU MUA (MACD cắt lên đường tín hiệu)";
                else if (MACD < MACD_Signal && MACD_Pre >= MACD_Signal_Pre)
                    analysis.MACDCrossSignal = "TÍN HIỆU BÁN (MACD cắt xuống đường tín hiệu)";
                else
                    analysis.MACDCrossSignal = "KHÔNG CÓ TÍN HIỆU MACD MỚI";

                // Phân tích Bollinger Bands
                if (Current_Price / 1000 > BollingerUpper)
                    analysis.BollingerSignal = "VÙNG QUÁ MUA - Có thể điều chỉnh giảm";
                else if (Current_Price / 1000 < BollingerLower)
                    analysis.BollingerSignal = "VÙNG QUÁ BÁN - Có thể phục hồi tăng";
                else
                    analysis.BollingerSignal = "NẰM TRONG BIÊN ĐỘ BOLLINGER";

                // Phân tích biến động
                analysis.Volatility = DailyReturn.StandardDeviation();
                analysis.RecentVolatility = DailyReturn.Take(10).StandardDeviation();

                // Phân tích khối lượng
                analysis.AverageVolume = (double)Lst_Quote.Take(30).Select(d => d.Volume).Average();
                analysis.RecentAverageVolume = (double)Lst_Quote.Take(5).Select(d => d.Volume).Average();
                analysis.VolumeChange = (analysis.RecentAverageVolume / analysis.AverageVolume) - 1;

                // Tạo nhận xét tổng hợp
                analysis.Observations = new List<string>();
                analysis.Observations.Add($"Chứng khoán đang trong xu hướng {analysis.Trend} với giá hiện tại {analysis.CurrentPrice:F2}.");
                analysis.Observations.Add($"Thay đổi trong 20 ngày gần đây: {analysis.Change20Days:F2}%.");

                if (analysis.RSI > 70)
                    analysis.Observations.Add($"RSI đang ở mức {analysis.RSI:F2}, cho thấy cổ phiếu đang trong vùng QUÁ MUA.");
                else if (analysis.RSI < 30)
                    analysis.Observations.Add($"RSI đang ở mức {analysis.RSI:F2}, cho thấy cổ phiếu đang trong vùng QUÁ BÁN.");

                analysis.Observations.Add(analysis.MACDCrossSignal);
                analysis.Observations.Add($"Theo dải Bollinger: {analysis.BollingerSignal}");

                if (analysis.VolumeChange > 0.20)
                    analysis.Observations.Add($"Khối lượng giao dịch gần đây tăng {analysis.VolumeChange * 100:F2}% so với trung bình, cho thấy sự quan tâm đang tăng.");
                else if (analysis.VolumeChange < -0.20)
                    analysis.Observations.Add($"Khối lượng giao dịch gần đây giảm {Math.Abs(analysis.VolumeChange) * 100:F2}% so với trung bình, cho thấy sự quan tâm đang giảm.");

                if (analysis.RecentVolatility > analysis.Volatility * 1.5)
                    analysis.Observations.Add($"Biến động gần đây tăng cao ({analysis.RecentVolatility:F2}% so với trung bình {analysis.Volatility:F2}%), thể hiện rủi ro cao hơn.");

                // Thêm dự đoán từ mô hình nếu có
                if (!string.IsNullOrEmpty(analysis.PredictedDirection))
                {
                    analysis.Observations.Add($"Dự đoán từ mô hình: Xu hướng {analysis.PredictedDirection} (độ tin cậy: {analysis.PredictedProbability * 100:F2}%)");
                    if (analysis.PredictedPriceChange != 0)
                        analysis.Observations.Add($"Dự đoán thay đổi giá: {analysis.PredictedPriceChange:F2}%");
                }

                string _msg_analysis = "";
                if (analysis != null)
                {
                    _msg_analysis = "KẾT QUẢ PHÂN TÍCH CHỨNG KHOÁN" + "\n";

                    _msg_analysis += $"Giá hiện tại: {analysis.CurrentPrice:F2}" + "\n";
                    _msg_analysis += $"Xu hướng: {analysis.Trend}" + "\n";
                    _msg_analysis += $"Thay đổi giá 20 ngày: {analysis.Change20Days:F2}%" + "\n";
                    _msg_analysis += $"RSI: {analysis.RSI:F2} - {analysis.RSISignal}" + "\n";
                    _msg_analysis += $"MACD: {analysis.MACDCrossSignal}" + "\n";
                    _msg_analysis += $"Bollinger Bands: {analysis.BollingerSignal}" + "\n";
                    _msg_analysis += $"Biến động giá: {analysis.Volatility:F2}%" + "\n";
                    _msg_analysis += $"Thay đổi khối lượng 30 phiên: {analysis.VolumeChange * 100:F2}%" + "\n";
                    analysis.Analysis_Info = _msg_analysis;
                }

                return analysis;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return null;
        }

        private static List<double> CalculateDailyReturns(List<decimal> prices)
        {
            var result = new List<double>();
            for (int i = 1; i < prices.Count; i++)
            {
                if (prices[i - 1] == 0) continue;
                result.Add((double)((prices[i] / prices[i - 1])) * 100); // Phần trăm thay đổi
            }
            return result;
        }

    }
}
