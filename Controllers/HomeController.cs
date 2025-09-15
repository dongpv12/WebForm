using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Helpers;
using WebForm.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebForm.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NewsDA _newsDa = new NewsDA();
    private readonly ProductDA _productDa = new ProductDA();
    private readonly ProjectDa _projectDa = new ProjectDa();
    private readonly PageDA _pageDa = new PageDA();
    private readonly ColorWarehouseDA _colorWarehouseDa = new ColorWarehouseDA();

    private readonly IWebHostEnvironment _env;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task<IActionResult> IndexAsync()
    {
        List<News> list = DataMemory.c_lstNew.OrderByDescending(i => i.Special).ThenByDescending(i => i.Id).Take(4).ToList();
        List<Project> listProject = DataMemory.c_lstProject.FindAll(i => i.Special == "Y").ToList();

        var indexPortal = new IndexPortal()
        {
            LstNews = new ListNews()
            {
                Collection = list
            },
            LstProject = new ListProject()
            {
                Collection = listProject
            }
        };
        return View(indexPortal);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet, Route("chart")]
    public IActionResult Chart(string symbol, string p_username)
    {
        ViewBag.symbol = symbol;
        ViewBag.language = "vi";
        ViewBag.username = p_username;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    private ListNews SearchNewsForPortal(PortalSearchNews portalSearchNews, out News news)
    {
        news = new News();
        var total = 0;
        var ds = _newsDa.GetForPortalIndex(portalSearchNews, ref total);
        //var list = CBO.Fill2ListFromDataSet<News>(ds, typeof(News));
        List<News> list = CBO<News>.FillCollectionFromDataSet(ds);
        var totalPage = Math.Ceiling((decimal)total / ConfigInfo.RecordOnPageIndex);
        var paging = HtmlControllHelpers.WritePagingPortal(totalPage, portalSearchNews.CurrentPage, total,
            ConfigInfo.RecordOnPageIndex);
        var listNews = new ListNews
        {
            Start = portalSearchNews.Start,
            Collection = list,
            Paging = paging,
            TotalRecord = total,
            TotalPage = totalPage,
            CurrentPage = portalSearchNews.CurrentPage
        };

        ds = _newsDa.GetSpecialNews();

        news = (News)CBO.FillObjectFromDataSet(ds, typeof(News));
        return listNews;
    }

    [HttpPost]
    [Route("register-advisory")]
    public ActionResult RegisterAdvisory(RegisterAdvisory request)
    {
        var emailInfo = DataMemory.EmailOriginal;

        emailInfo.Content = string.Format(ConfigInfo.EmailTemplate, request.FullName, request.Phone, request.Email,
            request.AppointmentDate, request.Content);

        emailInfo.MailTo = ConfigInfo.EmailBusiness;
        string oMsg = "";
        var result = EmailHelper.SendMail(emailInfo, out oMsg);

        return Json(new
        {
            Status = result
        });
    }

    private Page GetById(int id)
    {
        try
        {
            //var ds = _pageDa.GetById(id);
            //var page = (Page)CBO.FillObjectFromDataSet(ds, typeof(Page));
            //return page;

            Page _page = DataMemory.c_lstPage.Find(m => m.Id == id);
            return _page;
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return new Page();
        }
    }


    [HttpGet]
    [Route("chi-tiet/{id}")]
    public ActionResult ChitietTinTuc(string id)
    {
        try
        {
            var ds = _newsDa.GetById(Convert.ToInt32(id));
            var news = (News)CBO.FillObjectFromDataSet(ds, typeof(News));
            ViewBag.New = news;
            return View();
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }

    [HttpGet]
    [Route("chi-tiet-bao-cao/{id}")]
    public ActionResult ChitietBaoCao(string id)
    {
        try
        {
            var ds = _newsDa.GetById(Convert.ToInt32(id));
            var news = (News)CBO.FillObjectFromDataSet(ds, typeof(News));
            ViewBag.New = news;
            return View();
        }
        catch (Exception e)
        {
            Logger.Log.Error("log chi tiet bao cao id =" + id + " " + e.ToString());
            return null;
        }
    }





    private decimal ExtractTypeFromRoute(string? path)
    {
        var segments = path?.Split('/');
        if (segments != null && segments.Length > 2 && decimal.TryParse(segments[2], out var result))
            return result;

        return -1;
    }



    [HttpGet]
    [Route("danh-sach-tin/{type}")]
    [Route("danh-sach-tin-tuc-chung-khoan")]                   // Alias 1
    [Route("xu-huong-thi-truong")]
    [Route("xu-huong-nganh")]
    [Route("co-phieu-dan-dat")]
    public ActionResult XHTT(decimal type = 1)
    {
        try
        {

            var path = HttpContext.Request.Path.Value?.ToLower();

            type = path switch
            {
                "/danh-sach-tin-tuc-chung-khoan" => 1,
                "/xu-huong-thi-truong" => 2,
                "/xu-huong-nganh" => 3,
                "/co-phieu-dan-dat" => 4,
                _ => ExtractTypeFromRoute(path) // Trả về decimal
            };



            List<News> list = DataMemory.c_lstNew.Where(i => Convert.ToDecimal(i.CategoryType) == type).OrderByDescending(i => i.Id).ToList();

            var HotNews = list.FirstOrDefault();

            if (type != 1 && type != 5)
            {
                list = list.Where(i => i.Id != HotNews?.Id).ToList();
            }

            ViewBag.HotNews = HotNews;

            var total = list.Count();
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);

            // danh sách tin

            int start = 1;
            int end = ConfigInfo.RecordOnPage;


            if (list?.Count > 0 && list.Count >= start)
            {
                int numberTake = Math.Min((list.Count - start + 1), (end - start + 1));
                list = list.Skip(start - 1).Take(numberTake).ToList();

            }
            Allcode_Info typeNews = DataMemory.GetAllcodeByName("NEWS", "CATEGORYTYPE").Where(X => Convert.ToDecimal(X.CdValue) == type).FirstOrDefault();

            ViewBag.Header = typeNews.CdContent;

            var paging = HtmlControllHelpers.WritePaging(totalPage, 1, total, ConfigInfo.RecordOnPage, "tin " + typeNews.CdContent);

            ViewBag.Paging = paging;
            ViewBag.List = list;
            ViewBag.CategoryType = type;
            return View();
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }






    [HttpPost]
    public ActionResult SearchXHTT([FromBody] SearchNewsRequest request)
    {
        try
        {
            List<News> list = DataMemory.c_lstNew.Where(i => Convert.ToDecimal(i.CategoryType) == Convert.ToDecimal(request.CategoryType)).OrderByDescending(i => i.Id).ToList();

            var HotNews = list.FirstOrDefault();

            if (request.CategoryType != "1" && request.CategoryType != "5")
            {
                list = list.Where(i => i.Id != HotNews?.Id).ToList();
            }

            ViewBag.HotNews = HotNews;

            var total = list.Count();
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);
            // danh sách tin

            int start = request.Start;
            int end = request.End;


            if (list?.Count > 0 && list.Count >= start)
            {
                int numberTake = Math.Min((list.Count - start + 1), (end - start + 1));
                list = list.Skip(start - 1).Take(numberTake).ToList();

            }
            Allcode_Info typeNews = DataMemory.GetAllcodeByName("NEWS", "CATEGORYTYPE").Where(X => Convert.ToDecimal(X.CdValue) == Convert.ToDecimal(request.CategoryType)).FirstOrDefault();
            ViewBag.Header = typeNews.CdContent;

            var paging = HtmlControllHelpers.WritePaging(totalPage, request.CurrentPage, total, ConfigInfo.RecordOnPage, "tin " + ViewBag.Header);

            ViewBag.Paging = paging;
            ViewBag.List = list;

            return PartialView("DataNewsPages");
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }


    [HttpGet]
    [Route("danh-sach-bao-cao-art/{type}")]
    [Route("danh-sach-bao-cao-vimo")]                   // Alias 1
    [Route("danh-sach-bao-cao-nganh")]
    [Route("danh-sach-bao-cao-doanh-nghiep")]
    [Route("danh-sach-bao-cao-ctck")]
    public ActionResult ReportArt(decimal type = 6)
    {
        try
        {


            var path = HttpContext.Request.Path.Value?.ToLower();

            type = path switch
            {
                "/danh-sach-bao-cao-vimo" => 8,
                "/danh-sach-bao-cao-nganh" => 6,
                "/danh-sach-bao-cao-doanh-nghiep" => 10,
                "/danh-sach-bao-cao-ctck" => 7,
                _ => ExtractTypeFromRoute(path) // Trả về decimal
            };



            List<News> list = DataMemory.c_lstNew.Where(i => Convert.ToDecimal(i.CategoryType) == type).OrderByDescending(i => i.Id).ToList();

            var total = list.Count();
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);

            // danh sách tin

            int start = 1;
            int end = ConfigInfo.RecordOnPage;


            if (list?.Count > 0 && list.Count >= start)
            {
                int numberTake = Math.Min((list.Count - start + 1), (end - start + 1));
                list = list.Skip(start - 1).Take(numberTake).ToList();

            }

            Allcode_Info typeNews = DataMemory.GetAllcodeByName("NEWS", "CATEGORYTYPE").Where(X => Convert.ToDecimal(X.CdValue) == type).FirstOrDefault();

            if (typeNews.CdValue == "10")
            {
                ViewBag.Header = "Báo cáo doanh nghiệp khuyến nghị";
            }
            else if (typeNews.CdValue == "6")
            {
                ViewBag.Header = "Báo cáo ngành";
            }
            else
            {
                ViewBag.Header = typeNews.CdContent;
            }


            var paging = HtmlControllHelpers.WritePaging(totalPage, 1, total, ConfigInfo.RecordOnPage, ViewBag.Header);
            ViewBag.Paging = paging;
            ViewBag.List = list;
            ViewBag.CategoryType = type;

            ViewBag.Nganh = "0";

            return View();
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }



    [HttpGet]

    [Route("danh-sach-bao-cao-nganh/{nganh}")]
    public ActionResult ReportArtNganh(string nganh = "0")
    {
        try
        {


            var path = HttpContext.Request.Path.Value?.ToLower();

            decimal type = 6;



            List<News> list = DataMemory.c_lstNew.Where(i => Convert.ToDecimal(i.CategoryType) == type).OrderByDescending(i => i.Id).ToList();

            if (nganh != null && nganh != "")
            {
                list = DataMemory.c_lstNew
                                             .Where(i => Convert.ToDecimal(i.CategoryType) == type && i.Industry == nganh)
                                             .OrderByDescending(i => i.Id)
                                             .ToList();
            }



            var total = list.Count();
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);

            // danh sách tin

            int start = 1;
            int end = ConfigInfo.RecordOnPage;


            if (list?.Count > 0 && list.Count >= start)
            {
                int numberTake = Math.Min((list.Count - start + 1), (end - start + 1));
                list = list.Skip(start - 1).Take(numberTake).ToList();

            }

            Allcode_Info typeNews = DataMemory.GetAllcodeByName("NEWS", "CATEGORYTYPE").Where(X => Convert.ToDecimal(X.CdValue) == type).FirstOrDefault();

            if (typeNews.CdValue == "10")
            {
                ViewBag.Header = "Báo cáo doanh nghiệp khuyến nghị";
            }
            else if (typeNews.CdValue == "6")
            {
                ViewBag.Header = "Báo cáo ngành";
            }
            else
            {
                ViewBag.Header = typeNews.CdContent;
            }


            var paging = HtmlControllHelpers.WritePaging(totalPage, 1, total, ConfigInfo.RecordOnPage, ViewBag.Header);
            ViewBag.Paging = paging;
            ViewBag.List = list;
            ViewBag.CategoryType = type;

            ViewBag.Nganh = nganh;

            return View("ReportArt");
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }














    [HttpPost]
    public ActionResult SearchReport_Art([FromBody] SearchNewsRequest request)
    {
        try
        {
            List<News> list = DataMemory.c_lstNew.Where(i => Convert.ToDecimal(i.CategoryType) == Convert.ToDecimal(request.CategoryType)).OrderByDescending(i => i.Id).ToList();

            var total = list.Count();
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);
            // danh sách tin

            int start = request.Start;
            int end = request.End;


            if (list?.Count > 0 && list.Count >= start)
            {
                int numberTake = Math.Min((list.Count - start + 1), (end - start + 1));
                list = list.Skip(start - 1).Take(numberTake).ToList();

            }


            Allcode_Info typeNews = DataMemory.GetAllcodeByName("NEWS", "CATEGORYTYPE").Where(X => Convert.ToDecimal(X.CdValue) == Convert.ToDecimal(request.CategoryType)).FirstOrDefault();
            ViewBag.Header = typeNews.CdContent;

            var paging = HtmlControllHelpers.WritePaging(totalPage, request.CurrentPage, total, ConfigInfo.RecordOnPage, ViewBag.Header);
            ViewBag.Paging = paging;
            ViewBag.List = list;
            return PartialView("DataReportPages");
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }

    [HttpGet]
    [Route("co-phieu-tu-van")]
    public ActionResult CoPhieuTuVan()
    {
        try
        {
            return View();
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return View();
        }
    }

    [HttpGet]
    public IActionResult GetStocks()
    {
        try
        {
            // danh sách cổ phiếu
            List<Symbol_Notify_Info> listSymbol = DataMemory.GetAllSymbol();
            decimal _price = 0;
            decimal _matchPrice = 0;
            foreach (var item in listSymbol)
            {
                StockMemInfo info = StockMem.GetBySymbol(item.Symbol);
                item.Price_Text = (item.Price / 1000).ToNumberStringN31();
                if (info != null && item.Status != "2")

                {

                    item.Current_Price = info.MatchPrice;

                }
                else
                {
                    item.Current_Price = item.T_PRICE_Target;
                }

                if (item.DoanhThu == 0)
                {
                    item.PE = 0;
                }
                else if (item != null)
                {
                    item.PE = item.Current_Price / item.DoanhThu;
                }

                if (item.T_PRICE_Target == 0)
                {
                    item.Price_Position = 0;
                }
                else if (item != null)
                {
                    item.Price_Position = item.Current_Price / item.T_PRICE_Target;
                }



                if (item.Price == 0)
                {
                    item.Upside = 0;
                }
                else if (item != null)
                {
                    item.Upside = (item.F_PRICE_Target - item.Price) * 100 / item.Price;
                }

                if (item.Price == 0)
                {
                    item.HieuQua = 0;
                }
                else if (item != null)
                {
                    item.HieuQua = Math.Ceiling(((item.Current_Price - item.Price) * 100 / item.Price) * 10) / 10;

                }
            }






            //listSymbol = listSymbol
            //    .Where(x => x.Status != "2" || ((DateTime.Now.Date - x.Date_Pause.Date).Days <= 7))
            //    .OrderBy(x => x.Status == "2" ? 1 : 0)
            //    .ThenByDescending(x =>
            //        !string.IsNullOrWhiteSpace(x.Open_Position_Date)
            //            ? DateTime.ParseExact(x.Open_Position_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            //            : DateTime.MinValue
            //    )
            //    .ToList();


            listSymbol = listSymbol
                .OrderBy(x => x.Status == "2" ? 1 : 0)
                .ThenByDescending(x =>
                    !string.IsNullOrWhiteSpace(x.Open_Position_Date)
                        ? DateTime.ParseExact(x.Open_Position_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        : DateTime.MinValue
                )
                .ToList();




            var stocks = listSymbol.Select(x => new
            {
                Id = x.Id,
                Symbol = x.Symbol,
                Name = x.Name,
                Open_Position_Date = x.Open_Position_Date,
                Price = x.Price,
                Price_Text = x.Price_Text,
                Current_Price = x.Current_Price,
                Heso_Text = x.Heso_Text,
                Status_Text = x.Status_Text,
                Status = x.Status,
                HieuQua = x.HieuQua.ToNumberStringN31() + "%",
                HieuQua_Text = x.HieuQua.ToNumberStringN31(),
                Current_Price_Text = (x.Current_Price / 1000).ToNumberStringN31(),
                PRICE_Exp_Text = (x.F_PRICE_Exp / 1000).ToNumberStringN31() + " - " + (x.T_PRICE_Exp / 1000).ToNumberStringN31(),
                PRICE_Taget_Text = (x.F_PRICE_Target / 1000).ToNumberStringN31(),
                T_Pause = (x.T_Pause / 1000).ToNumberStringN31(),
                LoiNhuan = x.LoiNhuan.ToNumberStringN31(),
                DoanhThu = x.DoanhThu.ToNumberStringN31(),
                PE = (Math.Ceiling((x.PE) * 10) / 10).ToNumberStringN31(),
                Upside = (Math.Ceiling((x.Upside) * 10) / 10).ToNumberStringN31(),
                Price_Position_Text = (Math.Ceiling((x.Price_Position) * 10) / 10).ToNumberStringN31(),
                IsSpecial = x.IsSpecial,
                Note = x.Note,


            }).ToArray();




            return Ok(stocks);
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }


    [HttpGet]
    [Route("tt-co-phieu/{matp}")]
    public async Task<ActionResult> GetStocksCode(string matp)
    {
        try
        {

            if (matp == null || matp == "")
            {
                return null;
            }
            // danh sách cổ phiếu
            Symbol_Notify_Info _Symbol = DataMemory.GetAllSymbol().Where(x => x.Symbol.ToUpper() == matp.ToUpper()).FirstOrDefault();
            if (_Symbol != null)
            {
                StockMemInfo info = StockMem.GetBySymbol(_Symbol.Symbol);
                _Symbol.Price_Text = _Symbol.Price.ToNumberStringN31();

                if (info != null && _Symbol != null)
                {
                    if (_Symbol.Status == "2")
                    {
                        _Symbol.Current_Price = info.MatchPrice;

                    }
                    else
                    {
                        _Symbol.Current_Price = info.MatchPrice;
                        _Symbol.Current_Price_Text = info.MatchPrice.ToNumberStringN31(); ;
                        if (_Symbol.Price == 0)
                        {
                            _Symbol.Heso = 100;
                        }
                        else
                        {

                            _Symbol.Heso = Math.Ceiling(((info.MatchPrice - _Symbol.Price) * 100 / _Symbol.Price) * 10) / 10;
                        }
                        _Symbol.Heso_Text = _Symbol.Heso.ToNumberStringN31();

                    }

                }
                else
                {
                    _Symbol.Heso_Text = "0";
                }



                var stock = new
                {
                    Id = _Symbol.Id,
                    Symbol = _Symbol.Symbol,
                    Heso_Text = _Symbol.Heso_Text,

                    CurrentPrice = (_Symbol.Current_Price / 1000).ToNumberStringN31(),


                };

                return Ok(stock);
            }
            return null;
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }


    [HttpGet]
    [Route("danh-muc-tu-van")]
    public ActionResult FinArtTuvan()
    {

        return View();
    }

    [HttpGet]
    [Route("chi-tiet-co-phieu/{matp}")]
    public async Task<ActionResult> ChiTietTP(string matp)
    {

        try
        {
            List<Symbol_Notify_Info> listSymbol = DataMemory.GetAllSymbol();
            Symbol_Notify_Info _info = listSymbol.Where(x => x.Symbol.ToUpper() == matp.ToUpper()).FirstOrDefault();


            if (_info != null)
            {
                StockAnalysis stockAnalysis = await Analysis_Symbol.AnalyzeStockDataAsync(matp);
                if (stockAnalysis != null)
                {
                    ViewBag.StockAnalysis = stockAnalysis;
                }

                ViewBag.MaTP = matp;
                ViewBag.Info = _info;
                return View();
            }
            else
            {
                return View();
            }

        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }

    }


    [HttpPost]
    public ActionResult SearchReport_Art_BySymbol([FromBody] SearchNewsRequest request)
    {
        try
        {
            List<News> list = DataMemory.c_lstNew.Where(i => Convert.ToDecimal(i.CategoryType) == Convert.ToDecimal(request.CategoryType) && i.Symbol.ToUpper() == request.Symbol.ToUpper()).OrderByDescending(i => i.Id).ToList();

            var total = list.Count();
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);
            // danh sách tin

            int start = request.Start;
            int end = request.End;


            if (list?.Count > 0 && list.Count >= start)
            {
                int numberTake = Math.Min((list.Count - start + 1), (end - start + 1));
                list = list.Skip(start - 1).Take(numberTake).ToList();

            }


            Allcode_Info typeNews = DataMemory.GetAllcodeByName("NEWS", "CATEGORYTYPE").Where(X => Convert.ToDecimal(X.CdValue) == Convert.ToDecimal(request.CategoryType)).FirstOrDefault();
            ViewBag.Header = typeNews.CdContent;

            var paging = HtmlControllHelpers.WritePaging(totalPage, request.CurrentPage, total, ConfigInfo.RecordOnPage, ViewBag.Header);
            ViewBag.Paging = paging;
            ViewBag.List = list;
            return PartialView("DataReportPages_BySymbol");
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }

    [HttpGet]
    [Route("ve-chung-toi")]
    public ActionResult AboutUsNew()
    {
        return View();
    }

    [HttpGet]
    [Route("lien-he")]
    public ActionResult LienHe()
    {
        return View();
    }


    [HttpGet]
    [Route("co-phieu-tang-truong")]
    public ActionResult CophieuTangTruong()
    {
        return View();
    }

    [HttpGet]
    [Route("co-phieu-da-chot")]
    public ActionResult CophieuDaChot()
    {
        List<Symbol_Notify_Info> _Symbol = DataMemory.GetAllSymbol().Where(x => x.Status == "2").ToList();

        try
        {


            var listSorted = _Symbol.OrderByDescending(x =>
            {
                if (string.IsNullOrWhiteSpace(x.Close_Position_Date))
                    return DateTime.MaxValue; // cho xuống cuối nếu null hoặc trống

                DateTime date;
                bool ok = DateTime.TryParseExact(
                x.Close_Position_Date,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date
            );

                return ok ? date : DateTime.MaxValue; // nếu sai định dạng thì cho xuống cuối
            }).ToList();


            ViewBag.ListSorted = listSorted;
        }
        catch (Exception ex)
        {
            ViewBag.ListSorted = _Symbol;
            Logger.Log.Error(ex.ToString());
        }
        return View();
    }


    [HttpGet]
    [Route("danh-sach-khuyen-nghi")]
    public ActionResult DanhSachKhuyenNghi()
    {
        try
        {

        }
        catch (Exception ex)
        {
            Logger.Log.Error(ex.ToString());
        }
        return View();
    }





    [HttpPost]
    public ActionResult SearchCoPhieu([FromBody] SearchSymbolRequest request)
    {
        try
        {

            var format = "dd/MM/yyyy";
            var culture = CultureInfo.InvariantCulture;

            List<Symbol_Notify_Info> _Symbol = DataMemory.GetAllSymbol().Where(x => x.Status == "2").ToList();

            if (request.Code != null && request.Code != "")
            {

                _Symbol = _Symbol
                            .Where(x => !string.IsNullOrEmpty(x.Symbol)
                                        && x.Symbol.ToUpper().Contains(request.Code.ToUpper()))
                            .ToList();
            }

            if (request.To_Date != null && request.To_Date != "")
            {
                DateTime toDate;
                bool ok = DateTime.TryParseExact(request.To_Date, format, culture, DateTimeStyles.None, out toDate);

                if (ok)
                {
                    _Symbol = _Symbol
                        .Where(x =>
                        {
                            DateTime closeDate;
                            return DateTime.TryParseExact(x.Close_Position_Date, format, culture, DateTimeStyles.None, out closeDate)
                                   && closeDate.Date <= toDate.Date;
                        })
                        .ToList();
                }
                else
                {
                    // nếu To_Date không hợp lệ => không lọc hoặc xử lý theo nhu cầu
                }
            }

            if (request.From_Date != null && request.From_Date != "")
            {
                DateTime fromDate;
                bool ok = DateTime.TryParseExact(request.From_Date, format, culture, DateTimeStyles.None, out fromDate);

                if (ok)
                {
                    _Symbol = _Symbol
                        .Where(x =>
                        {
                            DateTime closeDate;
                            return DateTime.TryParseExact(x.Close_Position_Date, format, culture, DateTimeStyles.None, out closeDate)
                                   && closeDate.Date >= fromDate.Date;
                        })
                        .ToList();
                }
                else
                {
                    // nếu To_Date không hợp lệ => không lọc hoặc xử lý theo nhu cầu
                }
            }


            var listSorted = _Symbol.OrderByDescending(x =>
            {
                if (string.IsNullOrWhiteSpace(x.Close_Position_Date))
                    return DateTime.MaxValue; // cho xuống cuối nếu null hoặc trống

                DateTime date;
                bool ok = DateTime.TryParseExact(
                x.Close_Position_Date,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date
            );

                return ok ? date : DateTime.MaxValue; // nếu sai định dạng thì cho xuống cuối
            }).ToList();


            ViewBag.ListSorted = listSorted;

            return PartialView("DataSymbolBuy");
        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }



    [HttpPost]
    public ActionResult ExportCoPhieu([FromBody] SearchSymbolRequest request)
    {
        try
        {

            var format = "dd/MM/yyyy";
            var culture = CultureInfo.InvariantCulture;

            List<Symbol_Notify_Info> _Symbol = DataMemory.GetAllSymbol().Where(x => x.Status == "2").ToList();

            if (request.Code != null && request.Code != "")
            {

                _Symbol = _Symbol
                            .Where(x => !string.IsNullOrEmpty(x.Symbol)
                                        && x.Symbol.ToUpper().Contains(request.Code.ToUpper()))
                            .ToList();
            }

            if (request.To_Date != null && request.To_Date != "")
            {
                DateTime toDate;
                bool ok = DateTime.TryParseExact(request.To_Date, format, culture, DateTimeStyles.None, out toDate);

                if (ok)
                {
                    _Symbol = _Symbol
                        .Where(x =>
                        {
                            DateTime closeDate;
                            return DateTime.TryParseExact(x.Close_Position_Date, format, culture, DateTimeStyles.None, out closeDate)
                                   && closeDate.Date <= toDate.Date;
                        })
                        .ToList();
                }
                else
                {
                    // nếu To_Date không hợp lệ => không lọc hoặc xử lý theo nhu cầu
                }
            }

            if (request.From_Date != null && request.From_Date != "")
            {
                DateTime fromDate;
                bool ok = DateTime.TryParseExact(request.From_Date, format, culture, DateTimeStyles.None, out fromDate);

                if (ok)
                {
                    _Symbol = _Symbol
                        .Where(x =>
                        {
                            DateTime closeDate;
                            return DateTime.TryParseExact(x.Close_Position_Date, format, culture, DateTimeStyles.None, out closeDate)
                                   && closeDate.Date >= fromDate.Date;
                        })
                        .ToList();
                }
                else
                {
                    // nếu To_Date không hợp lệ => không lọc hoặc xử lý theo nhu cầu
                }
            }

            List<Symbol_Notify_Info> _Symbol_Search = new List<Symbol_Notify_Info>();

            foreach (var item in _Symbol)
            {

                Symbol_Notify_Info _info = new Symbol_Notify_Info();
                _info.Symbol = item.Symbol;
                _info.Open_Position_Date = item.Open_Position_Date;
                _info.Close_Position_Date = item.Close_Position_Date;
                _info.Ghichu = item.Ghichu;

               
                _info.Price = item.Price / 1000;
                _info.Sell_Price = item.Sell_Price / 1000;


                if (item.Price == 0)
                {
                    _info.Heso = 0;
                }
                else
                {
                    _info.Heso = Math.Round((item.Sell_Price - item.Price) * 100 / item.Price, 2);
                }

                _Symbol_Search.Add(_info);

            }


            var listSorted = _Symbol_Search.OrderByDescending(x =>
            {
                if (string.IsNullOrWhiteSpace(x.Close_Position_Date))
                    return DateTime.MaxValue; // cho xuống cuối nếu null hoặc trống

                DateTime date;
                bool ok = DateTime.TryParseExact(
                x.Close_Position_Date,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date
            );

                return ok ? date : DateTime.MaxValue; // nếu sai định dạng thì cho xuống cuối
            }).ToList();




            DataSet _ds = new DataSet();

            DataTable _data = new DataTable();

            _data = CBO1<Symbol_Notify_Info>.ConvertToDataTable(listSorted);

            _data.TableName = "DT";

            // Thêm DataTable vào DataSet
            _ds.Tables.Add(_data);


            FlexCel.Report.FlexCelReport flcReport = new FlexCel.Report.FlexCelReport();

            string templateFilePath = Path.Combine(_env.ContentRootPath + "/wwwroot/Content/ExportTemplate/") + "DanhsachCophieu.xlsx";
            //string exportedFileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_ThongTinCongBo" + ".xlsx";
            string exportedFileName = $"DanhSachCoPhieu" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";

            string exportedFilePath = Path.Combine(_env.ContentRootPath + "/wwwroot/Content/Export/") + exportedFileName;
            int is_err = -1;
            string c_err = string.Empty;
            if (_ds != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
            {
                _ds.Tables[0].TableName = "DT";
                ExportDataHelpers.SetValueExportByDataTable(ref flcReport, _ds);
                is_err = ExportDataHelpers.ExportExcel(flcReport, templateFilePath, exportedFilePath, ref c_err);
                if (is_err == 0) // ko co loi
                {
                    return Json(new { success = 1, filePath = exportedFileName });
                }
            }

            c_err = "Không có dữ liệu để kết xuất!";
            return Json(new { success = -3, c_err = c_err });


        }
        catch (Exception e)
        {
            Logger.Log.Error(e.ToString());
            return null;
        }
    }

    [Route("download/{filename}"), HttpGet]
    public FileResult DownloadFileTemp(string filename)
    {
        IFileProvider provider = new PhysicalFileProvider(_env.ContentRootPath + "/wwwroot/Content/Export/");
        IFileInfo fileInfo = provider.GetFileInfo(filename);
        var readStream = fileInfo.CreateReadStream();
        var mimeType = "application/vnd.ms-excel";
        return File(readStream, mimeType, filename);
    }

}


