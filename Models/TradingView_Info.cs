namespace WebForm
{
    public class StockData_TradingView
    {
        public string symbol { get; set; }
        public string s { get; set; }
        public List<long> t { get; set; }      // Unix timestamps
        public List<double> c { get; set; }    // Close
        public List<double> o { get; set; }    // Open
        public List<double> h { get; set; }    // High
        public List<double> l { get; set; }    // Low
        public List<long> v { get; set; }      // Volume
    }
}
