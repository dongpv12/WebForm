using Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Helpers;
using WebForm.Models;

namespace WebForm.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NewsDA _newsDa = new NewsDA();
    private readonly ProductDA _productDa = new ProductDA();
    private readonly ProjectDa _projectDa = new ProjectDa();
    private readonly PageDA _pageDa = new PageDA();
    private readonly ColorWarehouseDA _colorWarehouseDa = new ColorWarehouseDA();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
            Logger.Log.Error(e.ToString());
            return null;
        }
    }



    [HttpGet]
    [Route("danh-sach-tin/{type}")]
    public ActionResult XHTT(decimal type = 1)
    {
        try
        {
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
    public ActionResult ReportArt(decimal type = 6)
    {
        try
        {
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

            ViewBag.Header = typeNews.CdContent;

            var paging = HtmlControllHelpers.WritePaging(totalPage, 1, total, ConfigInfo.RecordOnPage, ViewBag.Header);
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
            foreach (var item in listSymbol)
            {
                StockMemInfo info = StockMem.GetBySymbol(item.Symbol);
                item.Price_Text = item.Price.ToNumberStringN31();
                if (info != null && item.Sell_Price == 0)
                {

                    item.Current_Price = info.MatchPrice;
                    item.Current_Price_Text = info.MatchPrice.ToNumberStringN31();
                   
                    if (item.Price == 0)
                    {
                        item.Heso = 100;
                    }
                    else
                    {
                        item.Heso = ((info.MatchPrice - item.Price) * 100) / item.Price;
                    }


                    item.Heso_Text = item.Heso.ToNumberStringN31();



                }
                else
                {
                    // giu nguyen gia trị trong DB

                    item.Heso_Text = item.Heso.ToNumberStringN31();
                    item.Current_Price_Text = item.Current_Price.ToNumberStringN31();
                }
                if (item.DoanhThu == 0)
                {
                    item.PE = 0;
                }
                else
                {
                    item.PE = item.Current_Price / item.DoanhThu;
                }
                   
            }

            listSymbol = listSymbol.Where(x => x.Status != "2" || (x.Status == "2" && ((DateTime.Now.Date - x.Date_Pause.Date).Days) <= 7)).OrderByDescending(x => x.Heso).ToList();

            var stocks = listSymbol.Select(x => new
            {
                Id = x.Id,
                Symbol = x.Symbol,
                Name = x.Name,
                Price = x.Price,
                Price_Text = x.Price_Text,
                Current_Price = x.Current_Price,
                Heso_Text = x.Heso_Text,
                Status_Text = x.Status_Text,
                Status = x.Status,
                Current_Price_Text = x.Current_Price_Text,
                PRICE_Exp_Text = x.F_PRICE_Exp.ToNumberStringN31() + " - " + x.T_PRICE_Exp.ToNumberStringN31(),
                PRICE_Taget_Text = x.F_PRICE_Target.ToNumberStringN31() + " - " + x.T_PRICE_Target.ToNumberStringN31(),
                T_Pause = x.T_Pause.ToNumberStringN31(),
                LoiNhuan = x.LoiNhuan.ToNumberStringN31(),
                DoanhThu = x.DoanhThu.ToNumberStringN31(),
                PE = x.PE.ToNumberStringN31(),
                IsSpecial = x.IsSpecial

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
    public IActionResult GetStocksCode()
    {
        try
        {
            // danh sách cổ phiếu
            List<Symbol_Notify_Info> listSymbol = DataMemory.GetAllSymbol();
            foreach (var item in listSymbol)
            {
                StockMemInfo info = StockMem.GetBySymbol(item.Symbol);
                item.Price_Text = item.Price.ToNumberStringN31();
                if (info != null)
                {

                    item.Current_Price = info.MatchPrice;
                    item.Current_Price_Text = info.MatchPrice.ToNumberStringN31(); ;
                    if (item.Price == 0)
                    {
                        item.Heso = 100;
                    }
                    else
                    {
                        item.Heso = (info.MatchPrice - item.Price) / item.Price;
                    }


                    item.Heso_Text = item.Heso.ToNumberStringN31();



                }
                else
                {
                    item.Heso_Text = "0";
                }
            }

            listSymbol = listSymbol.Where(x => x.Status != "2" || (x.Status == "2" && ((DateTime.Now.Date - x.Date_Pause.Date).Days) < 7)).OrderByDescending(x => x.Heso).ToList();

            var stocks = listSymbol.Select(x => new
            {
                Id = x.Id,
                Symbol = x.Symbol,
                Name = x.Name,
                Price = x.Price,
                Price_Text = x.Price_Text,
                Current_Price = x.Current_Price,
                Heso_Text = x.Heso_Text,
                Status_Text = x.Status_Text,
                Status = x.Status
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

            StockAnalysis stockAnalysis = await Analysis_Symbol.AnalyzeStockDataAsync(matp);
            if (stockAnalysis != null)
            {
                ViewBag.StockAnalysis = stockAnalysis;
            }

            ViewBag.MaTP = matp;
            ViewBag.Info = _info;
            return View();
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

}


