using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class NumberFormat
    {
        public const string NUMBER_FORMAT_2_DECIMAL = "_(* #,##0.00_);_(* (#,##0.00);_(* \"0\"??_);_(@_)";
        public const string NUMBER_FORMAT_PERCENTAGE = "0.00%";
        public const string NUMBER_FORMAT_INT = "_(* #,##0_);_(* (#,##0);_(* \"0\"_);_(@_)";
        public const string NUMBER_FORMAT_N0_0 = "#,###";
        public const string NUMBER_FORMAT_N0_1 = "#,##0";
        public const string NUMBER_FORMAT_N1_0 = "#,###.#";
        public const string NUMBER_FORMAT_N1_1 = "#,##0.#";
        public const string NUMBER_FORMAT_N2_0 = "#,###.##";
        public const string NUMBER_FORMAT_N2_1 = "#,##0.##";
        public const string NUMBER_FORMAT_N3_0 = "#,###.###";
        public const string NUMBER_FORMAT_N3_1 = "#,##0.###";
        public const string NUMBER_FORMAT_RATE = "#,##0.00";
        public const string NUMBER_FORMAT_RATE_1 = "#,##0.000";
        public const string NUMBER_FORMAT_N10 = "#,##0.##########";

    }

    public static class NumberFormatExtension
    {
        public static string ToNumberStringN0(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N0_1) : number.ToString(NumberFormat.NUMBER_FORMAT_N0_0));
        }
        public static string ToNumberStringN01(this decimal number)
        {
            return number.ToString(NumberFormat.NUMBER_FORMAT_N0_1);
        }
        // 1 digit after decimal point
        public static string ToNumberStringN1(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N1_1) : number.ToString(NumberFormat.NUMBER_FORMAT_N1_0));
        }

        // 2 digits after decimal point
        public static string ToNumberStringN2(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N2_1) : number.ToString(NumberFormat.NUMBER_FORMAT_N2_0));
        }

        public static string ToNumberStringN21(this decimal number)
        {
            return  number.ToString(NumberFormat.NUMBER_FORMAT_N2_1);
        }

        // 3 digits after decimal point
        public static string ToNumberStringN3(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N3_1) : number.ToString(NumberFormat.NUMBER_FORMAT_N3_0));
        }
        // lấy 10 số thập phân
        public static string ToNumberStringN10(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N10) : number.ToString(NumberFormat.NUMBER_FORMAT_N10));
        }
        //
        public static string ToNumberStringN31(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N3_1) : number.ToString(NumberFormat.NUMBER_FORMAT_N3_1));
        }

        public static string ToNumberStringN32(this decimal number)
        {
            return ((number > 0 && number < 1) ? number.ToString(NumberFormat.NUMBER_FORMAT_N3_1) : number.ToString(NumberFormat.NUMBER_FORMAT_N3_0));
        }

        public static string ToNumberStringRate(this decimal number)
        {
            return ((number == 0) ? "" : number.ToString(NumberFormat.NUMBER_FORMAT_RATE));
        }
        public static string ToNumberStringRate_1(this decimal number)
        {
            return ((number == 0) ? "" : number.ToString(NumberFormat.NUMBER_FORMAT_RATE_1));
        }
        public static string ToNumberStringRate_2(this decimal number)
        {
            return number.ToString(NumberFormat.NUMBER_FORMAT_N3_1);
        }
    }
}
