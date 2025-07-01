using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForm.Models
{
    public class Notify_WebSocket_Info
    {
        public string message_type { get; set; }
        public string data { get; set; }
    }


    public class Symbol_Notify_Info
    {
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