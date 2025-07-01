//HungTD:26-09-2014

using WebForm.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebForm.Helpers

{
    public class HtmlControllHelpers
    {
        /// <summary>
        ///     Hàm gen ra chuỗi html dùng cho phân trang
        ///     tổng số trang = tổng số bản ghi /số bản ghi trên 1 trang
        ///     <param name="iCurrentPage">Trang hiện tại</param>
        ///     <param name="iTotalRecords">Tổng số bản ghi</param>
        ///     <param name="iPageSize">Số bản ghi trên 1 trang</param>
        ///     <returns></returns>
        public static string WritePaging(decimal iPageCount, int iCurrentPage, int iTotalRecords, int iPageSize, string pLoaiBanGhi)
        {
            try
            {

                var strPage = "";
                strPage += "<div id='d_total_rec'>" + "Có tổng " + iTotalRecords + " " + pLoaiBanGhi + "</div>";
                strPage += "<div id='d_number_of_page' class='dataTables_paginate paging_simple_numbers'><ul class='pagination'>";
                if (iPageCount <= 1)
                {
                    strPage += "</ul></div>";
                    return strPage;
                }

                if (iCurrentPage > 1)
                    strPage += "<li class='paginate_button page-item previous' onclick=\"page('prew')\"><span id=\"back\" class='page-link' href=\"\"><</span></li>";
                if (iPageCount <= 5)
                {
                    for (var j = 0; j < iPageCount; j++)
                        if (iCurrentPage == j + 1)
                            strPage += "<li class='paginate_button page-item active'  onclick=\"page(" + (j + 1) +
                                       ")\"><span class=\"a-active page-link\" id=" + (j + 1) + "  href=\"\">" + (j + 1) +
                                       "</span></li>";
                        else
                            strPage += "<li class='paginate_button page-item ' onclick=\"page(" + (j + 1) + ")\"><span id=" + (j + 1) + " class='page-link' href=\"\">" +
                                       (j + 1) + "</span></li>";
                }
                else
                {
                    var cl = "";
                    int t;
                    var pagePreview = 0;
                    var soTrangVeLui = 2;
                    if (iPageCount - iCurrentPage == 1)
                        soTrangVeLui = soTrangVeLui + 1;
                    else if (iPageCount - iCurrentPage == 0) soTrangVeLui = soTrangVeLui + 2;
                    for (t = iCurrentPage - soTrangVeLui; t <= iCurrentPage; t++)
                    {
                        if (t < 1) continue;
                        cl = t == iCurrentPage ? "active" : "";
                        strPage += t == iCurrentPage
                            ? "<li class='paginate_button page-item active' onclick=\"page(" + t + ")\" style=\"background-color: #CDCDCD;\"><span class='page-link " + cl +
                              " ' id=" + t + "  href=\"\">" + t + "</span></li>"
                            : "<li  class='paginate_button page-item'  onclick=\"page(" + t + ")\"><span class=' page-link " + cl + "' id=" + t + "  href=\"\">" + t +
                              "</span></li>";
                        pagePreview++;
                    }

                    t = 0;
                    cl = "";
                    if (iCurrentPage == 1)
                    {
                        for (t = iCurrentPage + 1; t < iCurrentPage + 5; t++)
                        {
                            if (t >= t + 5 || t > iPageCount) continue;
                            cl = t == iCurrentPage ? "a-active" : "";
                            strPage += t == iCurrentPage
                                ? "<li class='paginate_button page-item active' onclick=\"page(" + t + ")\" style=\"background-color: #CDCDCD;\"><span class='page-link " +
                                  cl + "' id=" + t + "  href=\"\">" + t + "</span></li>"
                                : "<li   class='paginate_button page-item '  onclick=\"page(" + t + ")\"><span class='page-link " + cl + "' id=" + t + "  href=\"\">" +
                                  t + "</span></li>";
                        }
                    }
                    else if (iCurrentPage > 1)
                    {
                        var incr = 5 - (pagePreview - 1);
                        for (t = iCurrentPage + 1; t < iCurrentPage + incr; t++)
                        {
                            if (t >= t + incr || t > iPageCount) continue;
                            cl = t == iCurrentPage ? "a-active" : "";
                            strPage += t == iCurrentPage
                                ? "<li onclick=\"page(" + t + ")\" style=\"background-color: #CDCDCD;\"><span class='page-link " +
                                  cl + "' id=" + t + "  href=\"\">" + t + "</span></li>"
                                : "<li   onclick=\"page(" + t + ")\"><span class='page-link " + cl + "' id=" + t + "  href=\"\">" +
                                  t + "</span></li>";
                        }
                    }
                }

                if (iCurrentPage < iPageCount)
                    strPage += "<li class='paginate_button page-item next' onclick=\"page('next')\"><span class='page-link' id=\"next\"  href=\"\">></span></li>";
                strPage += "</ul></div>";
                return strPage;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return string.Empty;
            }
        }

        public static string WritePagingPortal(decimal iPageCount, int iCurrentPage, int iTotalRecords, int iPageSize)
        {
            try
            {

                var strPage = "";

                strPage += "<div class='pagination text-center'><ul>";
                if (iPageCount <= 1) return strPage;
                if (iCurrentPage > 1)
                    strPage += "<li onclick=\"page('prev')\"><a href='#' rel='prev'>«</a></li>";
                if (iPageCount <= 5)
                {
                    for (var j = 0; j < iPageCount; j++)
                        if (iCurrentPage == j + 1)
                            strPage += "<li class='active'  onclick=\"page(" + (j + 1) +
                                       ")\"><a>" + (j + 1) +
                                       "</a></li>";
                        else
                            strPage += "<li onclick=\"page(" + (j + 1) + ")\"><a>" +
                                       (j + 1) + "</a></li>";
                }
                else
                {
                    var cl = "";
                    int t;
                    var pagePreview = 0;
                    var soTrangVeLui = 2;
                    if (iPageCount - iCurrentPage == 1)
                        soTrangVeLui = soTrangVeLui + 1;
                    else if (iPageCount - iCurrentPage == 0) soTrangVeLui = soTrangVeLui + 2;
                    for (t = iCurrentPage - soTrangVeLui; t <= iCurrentPage; t++)
                    {
                        if (t < 1) continue;
                        cl = t == iCurrentPage ? "active" : "";
                        strPage += t == iCurrentPage
                            ? "<li class='active' onclick=\"page(" + t + ")\"><a>" + t + "</a></li>"
                            : "<li onclick=\"page(" + t + ")\"><a>" + t + "</a></li>";
                        pagePreview++;
                    }

                    t = 0;
                    cl = "";
                    if (iCurrentPage == 1)
                    {
                        for (t = iCurrentPage + 1; t < iCurrentPage + 5; t++)
                        {
                            if (t >= t + 5 || t > iPageCount) continue;
                            cl = t == iCurrentPage ? "a-active" : "";
                            strPage += t == iCurrentPage
                                ? "<li class='active' onclick=\"page(" + t + ")\"><a>" + t + "</a></li>"
                                : "<li onclick=\"page(" + t + ")\"><a>" + t + "</a></li>";
                        }
                    }
                    else if (iCurrentPage > 1)
                    {
                        var incr = 5 - (pagePreview - 1);
                        for (t = iCurrentPage + 1; t < iCurrentPage + incr; t++)
                        {
                            if (t >= t + incr || t > iPageCount) continue;
                            cl = t == iCurrentPage ? "a-active" : "";
                            strPage += t == iCurrentPage
                                ? "<li class='active' onclick=\"page(" + t + ")\"><a>" + t + "</a></li>"
                                : "<li onclick=\"page(" + t + ")\"><a>" + t + "</a></li>";
                        }
                    }
                }

                if (iCurrentPage < iPageCount)
                    strPage += " <li onclick=\"page('next')\"><a>»</a></li>";
                strPage += "</ul></div>";
                return strPage;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return string.Empty;
            }
        }

        public static string WritePaging(int iPageCount, int iCurrentPage, int iTotalRecords, int iPageSize)
        {
            try
            {
                var strPage = "";
                strPage += "<div id='d_total_rec'>" + "Có tổng " + iTotalRecords + " bản ghi</div>";
                strPage += "<div id='d_number_of_page'>";
                if (iPageCount <= 1) return strPage;
                if (iCurrentPage > 1)
                    strPage += "<li onclick=\"page('prew')\"><span id=\"back\"  href=\"\"><</span></li>";
                if (iPageCount <= 5)
                {
                    for (var j = 0; j < iPageCount; j++)
                        if (iCurrentPage == j + 1)
                            strPage += "<li style=\"background-color: #CDCDCD;\" onclick=\"page(" + (j + 1) +
                                       ")\"><span class=\"a-active\" id=" + (j + 1) + "  href=\"\">" + (j + 1) +
                                       "</span></li>";
                        else
                            strPage += "<li onclick=\"page(" + (j + 1) + ")\"><span id=" + (j + 1) + "  href=\"\">" +
                                       (j + 1) + "</span></li>";
                }
                else
                {
                    var cl = "";
                    int t;
                    var pagePreview =
                        0; //nếu đang ở trang 2 thì vẽ đc có 1 trang trước nó nên sẽ vẽ thêm 3 trang phía sau 
                    //default là vẽ 2 trang trc 2 trang sau 
                    var soTrangVeLui = 2;
                    if (iPageCount - iCurrentPage == 1)
                        soTrangVeLui = soTrangVeLui + 1;
                    else if (iPageCount - iCurrentPage == 0) soTrangVeLui = soTrangVeLui + 2;
                    for (t = iCurrentPage - soTrangVeLui; t <= iCurrentPage; t++) //ve truoc 2 trang 
                    {
                        if (t < 1) continue;
                        cl = t == iCurrentPage ? "a-active" : "";
                        strPage += t == iCurrentPage
                            ? "<li onclick=\"page(" + t + ")\" style=\"background-color: #CDCDCD;\"><span class=" + cl +
                              " id=" + t + "  href=\"\">" + t + "</span></li>"
                            : "<li   onclick=\"page(" + t + ")\"><span class=" + cl + " id=" + t + "  href=\"\">" + t +
                              "</span></li>";
                        //strPage += "<li onclick=\"page(" + (t) + ")\"><span class=" + cl + " id=" + (t) + "  href=\"\">" + (t) + "</span></li>";
                        pagePreview++;
                    }

                    t = 0;
                    cl = "";
                    if (iCurrentPage == 1) //truong hop trang dau tien thi ve du 5 trang 
                    {
                        for (t = iCurrentPage + 1; t < iCurrentPage + 5; t++)
                        {
                            if (t >= t + 5 || t > iPageCount) continue;
                            cl = t == iCurrentPage ? "a-active" : "";
                            strPage += t == iCurrentPage
                                ? "<li onclick=\"page(" + t + ")\" style=\"background-color: #CDCDCD;\"><span class=" +
                                  cl + " id=" + t + "  href=\"\">" + t + "</span></li>"
                                : "<li   onclick=\"page(" + t + ")\"><span class=" + cl + " id=" + t + "  href=\"\">" +
                                  t + "</span></li>";
                            //strPage += "<li  onclick=\"page(" + (t) + ")\"><span class=" + cl + " id=" + (t) + "  href=\"\">" + (t) + "</span></li>";
                        }
                    }
                    else if (iCurrentPage > 1
                    ) //truogn hop ma la trang lon hon 1 thi se ve 4 trang ke tiep + 1 trang truoc 
                    {
                        var incr = 5 - (pagePreview - 1);
                        for (t = iCurrentPage + 1; t < iCurrentPage + incr; t++)
                        {
                            if (t >= t + incr || t > iPageCount) continue;
                            cl = t == iCurrentPage ? "a-active" : "";
                            strPage += t == iCurrentPage
                                ? "<li onclick=\"page(" + t + ")\" style=\"background-color: #CDCDCD;\"><span class=" +
                                  cl + " id=" + t + "  href=\"\">" + t + "</span></li>"
                                : "<li   onclick=\"page(" + t + ")\"><span class=" + cl + " id=" + t + "  href=\"\">" +
                                  t + "</span></li>";
                            //strPage += "<li onclick=\"page(" + (t) + ")\"><span class=" + cl + " id=" + (t) + "  href=\"\">" + (t) + "</span></li>";
                        }
                    }
                }

                if (iCurrentPage < iPageCount)
                    strPage += "<li onclick=\"page('next')\"><span id=\"next\"  href=\"\">></span></li>";
                strPage += "</div>";
                return strPage;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return string.Empty;
            }
        }

        ///// <summary>
        /// HungTD:24-09-2014: gen html gropdownlist voi selected item
        /// </summary>
        /// <param name="p_id_selected"> id lay len tu form</param>
        /// <param name="p_DicOject"> danh sach key va values lay tu db</param>
        /// <param name="p_control_id"> id control tren html</param>
        /// <returns>html code</returns>
        public static string GenDropDownList(string p_id_selected, Dictionary<string, string> p_DicOject,
            string p_attribute, string p_control_id, string p_defaul_item)
        {
            try
            {
                if (p_DicOject == null) return "";
                var _SrtBuilder = new StringBuilder();
                _SrtBuilder.Append(
                    "<select " + p_attribute + " id='" + p_control_id + "' name ='" + p_control_id + "'>");
                _SrtBuilder.Append(p_defaul_item);
                if (p_DicOject.Count == 0)
                {
                    _SrtBuilder.Append("</select>");
                    return _SrtBuilder.ToString();
                }

                foreach (var pair in p_DicOject)
                    if (pair.Key.ToUpper().Trim() == p_id_selected.ToUpper().Trim()
                    ) //neu co ton tai id                 
                    {
                        _SrtBuilder.Append("<option selected='selected' value='" + pair.Key + "'>");
                        _SrtBuilder.Append(pair.Value);
                        _SrtBuilder.Append("</option>");
                    }
                    else
                    {
                        _SrtBuilder.Append("<option  value='" + pair.Key + "'>");
                        _SrtBuilder.Append(pair.Value);
                        _SrtBuilder.Append("</option>");
                    }

                _SrtBuilder.Append("</select>");
                return _SrtBuilder.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return "";
            }
        }

        public static String ReplaceUnicodeUrl(string str)
        {
            try
            {
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = str.Normalize(NormalizationForm.FormD);
                temp = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                string partern = "~!@#$%^&*()_+|{}[]:'><?\"/,.;”“";
                for (int i = 0; i < partern.Length; i++)
                {
                    temp = temp.Replace(partern[i].ToString(), "");
                }
                while (temp.IndexOf("  ") > 0)
                {
                    temp = temp.Replace("  ", " ");
                }
                if (temp.Contains('–'))
                {
                    temp.Replace("–", "");
                }
                temp = temp.Trim().Replace(' ', '-');
                if (temp.Contains("–"))
                {
                    temp = temp.Remove(temp.IndexOf("–"), 1);
                }
                while (temp.IndexOf("--") > 0)
                {
                    temp = temp.Replace("--", "-");
                }
                if (temp.Length > 150)
                {
                    temp = temp.Substring(0, 150);
                }
                return temp;
            }
            catch
            {
                return "";
            }
        }
    }

}
