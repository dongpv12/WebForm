namespace WebForm
{
    public class StockAnalysis
    {
        public double CurrentPrice { get; set; }
        public double Change20Days { get; set; }
        public string Trend { get; set; }
        public double RSI { get; set; }
        public string RSISignal { get; set; }
        public double MACD { get; set; }
        public double MACDSignal { get; set; }
        public string MACDCrossSignal { get; set; }
        public string BollingerSignal { get; set; }
        public double Volatility { get; set; }
        public double RecentVolatility { get; set; }
        public double AverageVolume { get; set; }
        public double RecentAverageVolume { get; set; }
        public double VolumeChange { get; set; }
        public string? PredictedDirection { get; set; }
        public double PredictedProbability { get; set; }
        public double PredictedPriceChange { get; set; }
        public string Analysis_Info { get; set; }
        public List<string> Observations { get; set; }
    }
}
