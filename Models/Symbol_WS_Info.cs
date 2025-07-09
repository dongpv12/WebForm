namespace WebForm.Models
{

    public class Symbol_WS_Info
    {
        public Symbol_WS_Info() { }

        public Symbol_WS_Info(Symbol_WS_Info p_Obj)
        {
            Symbol = p_Obj.Symbol;
            Name = p_Obj.Name;
            IndustryName = p_Obj.IndustryName;
            GroupCode = p_Obj.GroupCode;
            TradingDate = p_Obj.TradingDate;
            MarketCode = p_Obj.MarketCode;
            Is_Over_Range_Price = p_Obj.Is_Over_Range_Price;
            Is_Over_MA10_Qtty = p_Obj.Is_Over_MA10_Qtty;
            Is_Over_50Percent_Qtty = p_Obj.Is_Over_50Percent_Qtty;
            Basic_Price = p_Obj.Basic_Price;
            Floor_Price = p_Obj.Floor_Price;
            Ceiling_Price = p_Obj.Ceiling_Price;

            MA20_Price = p_Obj.MA20_Price;
            MA10_Qtty = p_Obj.MA10_Qtty;
            Current_Price = p_Obj.Current_Price;
            PriceRangePercent = p_Obj.PriceRangePercent;
            ChangePrice = p_Obj.ChangePrice;
            ChangePricePercent = p_Obj.ChangePricePercent;
            ChangeQttyPercent = p_Obj.ChangeQttyPercent;
            Open = p_Obj.Open;
            Close = p_Obj.Close;
            Low = p_Obj.Low;
            Hight = p_Obj.Hight;
            Price_Over_Range = p_Obj.Price_Over_Range;
            Volume = p_Obj.Volume;
            Color = p_Obj.Color;

            StrMarketCode = p_Obj.StrMarketCode;
            IndustryCode = p_Obj.IndustryCode;
            Point = p_Obj.Point;
            Point_Qtty = p_Obj.Point_Qtty;
            Point_Price = p_Obj.Point_Price;
            PercentChangeBienDo = p_Obj.PercentChangeBienDo;
            SecurityType = p_Obj.SecurityType;
            Is_Choke_BolingBand = p_Obj.Is_Choke_BolingBand;
        }

        public int ChangeType { get; set; }

        /// <summary>
        /// Mã TP
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Tên TP
        /// </summary>
        public string? Name { get; set; }

        public string SecurityType { get; set; }

        /// <summary>
        /// Nhóm ngành
        /// </summary>
        public string? IndustryName { get; set; }
        public string? IndustryCode { get; set; }

        public string? GroupCode { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public string? TradingDate { get; set; }

        /// <summary>
        /// Mã thị trường
        /// </summary>
        public string? MarketCode { get; set; }

        string _StrMarketCode;
        public string StrMarketCode
        {
            get
            {
                if (MarketCode == "STX")
                {
                    return "HNX Niêm Yết";
                }
                else if (MarketCode == "UPX")
                {
                    return "HNX UpCom";

                }
                else if (MarketCode == "STO")
                {
                    return "Hose";

                }
                else if (MarketCode == "HCX")
                {
                    return "Bond";

                }
                else if (MarketCode == "DVX")
                {
                    return "Phái Sinh";

                }
                else
                    return MarketCode;
            }
            set
            {
                _StrMarketCode = value;
            }
        }

        //public string? StrMarketCode { get; set; }

        public string Change_Data_Code { get; set; }

        public int Point { get; set; }

        public int Point_Price { get; set; }
        public int Point_Qtty { get; set; }

        /// <summary>
        /// Có vượt ngưỡng 50% biên độ giá
        /// </summary>
        public int Is_Over_Range_Price { get; set; }

        /// <summary>
        /// Vượt ngưỡng MA10 về khối lượng giao dịch
        /// </summary>
        public int Is_Over_MA10_Qtty { get; set; }

        /// <summary>
        /// Vượt ngưỡng 50% KL giao dịch so với MA10 
        /// </summary>
        public int Is_Over_50Percent_Qtty { get; set; }

        /// <summary>
        /// Giá tham chiếu
        /// </summary>
        public decimal Basic_Price { get; set; }

        /// <summary>
        /// Giá sàn
        /// </summary>
        public decimal Floor_Price { get; set; }

        /// <summary>
        /// Giá trần
        /// </summary>
        public decimal Ceiling_Price { get; set; }

        /// <summary>
        /// Giá tại MA20
        /// </summary>
        public decimal MA20_Price { get; set; }

        /// <summary>
        /// Khối lượng MA10
        /// </summary>
        public decimal MA10_Qtty { get; set; }

        /// <summary>
        /// Giá khớp hiện tại
        /// </summary>
        public decimal Current_Price { get; set; }
        public decimal Match_Qtty { get; set; }


        public decimal PriceRangePercent { get; set; }

        /// <summary>
        /// Thay đổi so với giá tham chiếu
        /// </summary>
        public decimal ChangePrice { get; set; }

        /// <summary>
        /// % thay đổi kl so với giá tham chiếu
        /// </summary>
        public decimal ChangePricePercent { get; set; }

        /// <summary>
        /// % thay đổi kl so với giá tham chiếu
        /// </summary>
        public decimal PercentChangeBienDo { get; set; }

        /// <summary>
        /// % thay đổi kl so với MA10
        /// </summary>
        public decimal ChangeQttyPercent { get; set; }

        /// <summary>
        /// Mở cửa
        /// </summary>
        public decimal Open { get; set; }
        public decimal Open_Qtty { get; set; }

        public string Color { get; set; }


        /// <summary>
        /// Đóng cửa
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// Thấp nhất
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// Cao nhất
        /// </summary>
        public decimal Hight { get; set; }

        /// <summary>
        /// Tổng khối lượng giao dịch
        /// </summary>
        public decimal Volume { get; set; }

        public decimal TotalValue { get; set; }


        /// <summary>
        /// Giá tại 50% biên độ
        /// </summary>
        public decimal Price_Over_Range { get; set; }

        /// <summary>
        /// Kiểm tra xem boling band có bị thắt lại hay không
        /// </summary>
        public int Is_Choke_BolingBand { get; set; }
    }
}
