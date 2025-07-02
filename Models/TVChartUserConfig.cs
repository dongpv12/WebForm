namespace WebForm
{
    public class TVChartUserConfig
    {
        public int id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public string symbol { get; set; }
        public string resolution { get; set; }
        public long timestamp { get; set; }
        public int userid { get; set; }
        public string username { get; set; }
        public DateTime createddate { get; set; }
        public string createdby { get; set; }
        public DateTime modifieddate { get; set; }
        public string modifiedby { get; set; }
    }

    public class StockHistoryInfo
    {
        public string symbol { get; set; }
        public DateTime historydate { get; set; }

        public decimal historydate_timestamp { get; set; }

        public decimal refer { get; set; }
        public decimal ceil { get; set; }
        public decimal floor { get; set; }

        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }

        public decimal MatchQtty { get; set; }

        //
        public string CompanyNameVi { get; set; }
        public string _COMPANY_NAME_EN { get; set; }
        public string ItemName { get; set; }
        public string _FLOORCODE { get; set; }
        public string StockName { get; set; }
    }

    public class StockMemInfo
    {
        public string Symbol { get; set; }

        public string SymbolName { get; set; }
        public int SymbolID { get; set; }
        public string MarketCode { get; set; }

        public decimal OpenPrice { get; set; } // Giá mở cửa
        public decimal ClosePrice { get; set; } // Giá đóng cửa
        public decimal HighestPrice { get; set; } // giá cao nhất
        public decimal LowestPrice { get; set; } // giá thấp nhất
        public decimal TotalTradedQttyNM { get; set; } // Tổng khối lượng giao dịch
        public decimal TotalTradedValueNM { get; set; } // Tổng khối lượng giao dịch
        public decimal MatchPrice { get; set; } // Giá giao dịch
    }

    public class StockMatchStatisticInfo
    {
        public DateTime TradeTime { get; set; }
        public long TimestampUTC { get; set; }
        public string Symbol { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal OpenQtty { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal CloseQtty { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal TotalTradedQtty { get; set; }
        public decimal TotalTradedValue { get; set; }
    }

    public class IndexStatisticInfo
    {
        public DateTime IndexTime { get; set; }
        public long TimestampUTC { get; set; }
        public string IndexCode { get; set; }
        public decimal OpenIndex { get; set; }
        public decimal CloseIndex { get; set; }
        public decimal HighestIndex { get; set; }
        public decimal LowestIndex { get; set; }
        public decimal TotalTradedQtty { get; set; }
        public decimal TotalTradedValue { get; set; }
        public decimal ChangeTradedQtty { get; set; }
        public decimal ChangeTradedValue { get; set; }
    }

    public class IndexMemInfo
    {
        public string IndexCode { get; set; }
        public string IndexName { get; set; }
        public string TypeIndex { get; set; }
        public string MarketCode { get; set; }
        public string TradingDate { get; set; }

        public decimal PriorIndex { get; set; }
        public decimal CurrentIndex { get; set; }
        public decimal ChangeIndex { get; set; }
        public decimal PctChangeIndex { get; set; }

        public decimal HighestIndex { get; set; } // Giá trị chỉ số, hoặc TRI cao nhất. Không đẩy ra với DPI
        public decimal LowestIndex { get; set; } // Giá trị chỉ số hoặc TRI thấp nhất. Không đẩy ra với DPI
        public decimal OpenIndex { get; set; }
        public decimal CloseIndex { get; set; } // Giá trị chỉ số hoặc TRI khi đóng cửa. Không đẩy ra với DPI

        public decimal TotalTradedQtty { get; set; }
        public decimal TotalTradedValue { get; set; }
        public decimal TotalTradedQttyNM { get; set; }
        public decimal TotalTradedValueNM { get; set; }
        public decimal TotalTradedQttyPT { get; set; }
        public decimal TotalTradedValuePT { get; set; }

        public int TotalStock { get; set; }
        public int Up { get; set; }
        public int UpCeil { get; set; }
        public int NoChange { get; set; }
        public int Down { get; set; }
        public int DownFloor { get; set; }

        public string Status { get; set; }
        public string StatusStr { get; set; }
        public string StatusStrEn { get; set; }

        public decimal BuyForeignQtty { get; set; }
        public decimal BuyForeignValue { get; set; }
        public decimal SellForeignQtty { get; set; }
        public decimal SellForeignValue { get; set; }

        public decimal SubForeignQtty { get; set; }
        public decimal SubForeignValue { get; set; }

        public decimal TotalTradedQttyUp { get; set; }
        public decimal TotalTradedValueUp { get; set; }
        public decimal TotalTradedQttyDown { get; set; }
        public decimal TotalTradedValueDown { get; set; }
        public decimal TotalTradedQttyNoChange { get; set; }
        public decimal TotalTradedValueNoChange { get; set; }

        public decimal PriceRatioChange { get; set; }

    }

    public static class MarketCode
    {
        public const string HNX = "STX";
        public const string UpCOM = "UPX";
        public const string HoSE = "STO";
        public const string Bond = "HCX";
        public const string Derivative = "DVX";
        //
        public static string GetDisplayName(string marketCode, bool isEn = false)
        {
            string marketName = string.Empty;

            if (!string.IsNullOrEmpty(marketCode))
            {
                if (marketCode == MarketCode.HNX)
                {
                    marketName = isEn ? "HNX" : "HNX";
                }
                else if (marketCode == MarketCode.UpCOM)
                {
                    marketName = isEn ? "UpCOM" : "UpCOM";
                }
                else if (marketCode == MarketCode.HoSE)
                {
                    marketName = isEn ? "HSX" : "HSX";
                }
                else if (marketCode == MarketCode.Bond)
                {
                    marketName = isEn ? "Bond" : "Trái phiếu";
                }
                else if (marketCode == MarketCode.Derivative)
                {
                    marketName = isEn ? "Derivative" : "Phái sinh";
                }
            }

            return marketName;
        }
    }

    public class TTIndexInfo
    {
        public long TickUpdate { get; set; } = 0;
        public int c_PreCloseIndex { get; set; } = 0;

        //

        public string IndexCode { get; set; } // Mã chỉ số
        public string IndexName { get; set; } //Tên của chỉ số, tên TRI hoặc tên DPI
        public string TypeIndex { get; set; } // Loại chỉ số:
        public string MarketCode { get; set; }
        public decimal Value { get; set; } //Giá trị chỉ số tại thời điểm hiện tại;  Giá trị TRI tại thời điểm hiện tại; Giá trị DPI trong ngày
        public string CalTime { get; set; } //Thời gian tính theo định dạng HH:mm:ss
        public decimal Change { get; set; } //Giá trị thay đổi chỉ số hoặc TRI so với ngày hôm trước.Không đẩy ra với DPI
        public decimal RatioChange { get; set; } //Tỷ lệ thay đổi chỉ số hoặc TRI. Không đẩy ra với DPI
        public decimal TotalQtty { get; set; } //Tổng khối lượng giao dịch của khớp lệnh (lô chẵn+ thỏa thuận). Không đẩy ra với DPI và TRI
        public decimal TotalValue { get; set; } //Tổng giá trị giao dịch của khớp lệnh (lô chẵn+ thỏa thuận). Không đẩy ra với DPI và TRI
        public string TradingDate { get; set; } //Ngày giao dịch hiện tại theo định dạng YYYYMMdd
        public decimal PriorIndexVal { get; set; } //Giá trị index, TRI hoặc DPI ngày hôm trước
        public decimal HighestIndex { get; set; } //Giá trị chỉ số, hoặc TRI cao nhất. Không đẩy ra với DPI
        public decimal LowestIndex { get; set; } //Giá trị chỉ số hoặc TRI thấp nhất. Không đẩy ra với DPI
        public decimal OpenIndex { get; set; } //Giá trị chỉ số hoặc TRI khi đóng cửa. Không đẩy ra với DPI
        public decimal CloseIndex { get; set; } //Giá trị chỉ số hoặc TRI khi đóng cửa. Không đẩy ra với DPI
        public int TotalStock { get; set; } //Tổng số chứng khoán trong rổ. Không đẩy ra với DPI và TRI
        public int NumSymbolAdvances { get; set; } //Số chứng khoán tăng giá (khớp lệnh, lô chẵn)
        public int NumSymbolNochange { get; set; } //Sô chứng khoán ko thay đổi giá (khớp lệnh, lô chẵn)
        public int NumSymbolDeclines { get; set; } //Số chứng khoán giảm giá (khớp lệnh, lô chẵn)
        public int NumUpCeiling { get; set; }
        public int NumDownFloor { get; set; }
        public decimal PT_TotalTradedQtty { get; set; } //KL giao dịch thỏa thuận (chẵn, lẻ)
        public decimal PT_TotalTradedValue { get; set; } //Giá trị giao dịch thỏa thuận (chẵn, lẻ)
        public decimal NM_TotalTradedQtty { get; set; } //KL giao dịch khớp lệnh lô chẵn
        public decimal NM_TotalTradedValue { get; set; } //Giá trị giao dịch khớp lệnh lô chẵn

        public decimal BuyForeignQtty { get; set; }
        public decimal BuyForeignValue { get; set; }
        public decimal SellForeignQtty { get; set; }
        public decimal SellForeignValue { get; set; }
        public decimal SubForeignQtty { get { return BuyForeignQtty - SellForeignQtty; } }
        public decimal SubForeignValue { get { return SellForeignQtty - SellForeignValue; } }

        //
        public decimal TTTradeValueUp { get; set; }
        public decimal TTTradeValueNochange { get; set; }
        public decimal TTTradeValueDown { get; set; }
        public decimal TTTradeQttyUp { get; set; }
        public decimal TTTradeQttyNochange { get; set; }
        public decimal TTTradeQttyDown { get; set; }
        public decimal PriceRatioChange { get; set; }
    }

}
