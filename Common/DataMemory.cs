using System;
using System.Configuration;
using System.IO;
using System.Web;
using WebForm.Models;
using System.Collections.Generic;
using WebForm.DataAccess;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace WebForm.Common
{
    public class DataMemory
    {
        public static List<News> c_lstNew = new List<News>();
        public static List<Project> c_lstProject = new List<Project>();
        public static List<Page> c_lstPage = new List<Page>();
        public static List<Allcode_Info> c_lstAllcode = new List<Allcode_Info>();

        public static List<Symbol_Notify_Info> c_lstSymbol = new List<Symbol_Notify_Info>();

        public static List<Symbol_Notify_Info> c_lstSymbolData = new List<Symbol_Notify_Info>();

        public static void LoadNews()
        {
            try
            {
                NewsDA _newsDa = new NewsDA();

                var portalSearchNews = new PortalSearchNewsIndex
                {
                    Start = 1,
                    End = 0,
                    Id = 0
                };
                var pTotal = 0;
                var ds = _newsDa.GetForPortalDetail(portalSearchNews, ref pTotal);

                c_lstNew = CBO<News>.FillCollectionFromDataSet(ds);
                c_lstNew = c_lstNew.OrderByDescending(m => m.Id).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
        public static void GetAllcode()
        {
            try
            {
                UsersDA _da = new UsersDA();

               
                var ds = _da.GetAllcode();

                c_lstAllcode = CBO<Allcode_Info>.FillCollectionFromDataSet(ds);
                c_lstAllcode = c_lstAllcode.OrderBy(m => m.Lstodr).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }

        public static List<Allcode_Info> GetAllcodeByName(string cdType, string cdName)
        {
            List<Allcode_Info> lst = new List<Allcode_Info>();
            try
            {
                if (c_lstAllcode != null)
                {
                    lst = c_lstAllcode.Where(x => x.CdType == cdType && x.CdName == cdName).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return lst;
        }


        public static void LoadSymbol()
        {
            try
            {
                List<Symbol_Notify_Info> _lst = new List<Symbol_Notify_Info>();

                SymbolDA _da = new SymbolDA();
                var data = _da.SymbolGetAll();
                c_lstSymbolData = CBO<Symbol_Notify_Info>.FillCollectionFromDataSet(data);

                //var pTotal = 0;
                //var ds = _newsDa.GetForPortalDetail(portalSearchNews, ref pTotal);

                //c_lstNew = CBO<News>.FillCollectionFromDataSet(ds);
                //c_lstNew = c_lstNew.OrderByDescending(m => m.Id).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }



        public static List<Symbol_Notify_Info> GetAllSymbol()
        {
            try
            {
                return c_lstSymbolData;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new List<Symbol_Notify_Info>();
            }
        }



        public static void ReadDataSymbol()
        {
            try
            {
                string path = "DataSymbol/data.txt";
                c_lstSymbol = new List<Symbol_Notify_Info>();

                var symbolMap = new Dictionary<string, Symbol_Notify_Info>();

                if (!File.Exists(path))
                {
                    c_lstSymbol = new List<Symbol_Notify_Info>();
                }
                else
                {
                    foreach (var line in File.ReadLines(path))
                    {
                        try
                        {
                            // Bước 1: Deserialize dòng đầu tiên
                            var wrapper = JsonSerializer.Deserialize<Notify_WebSocket_Info>(line);

                            // Bước 2: Deserialize tiếp trường 'data' (vì nó là chuỗi JSON lồng)
                            if (!string.IsNullOrEmpty(wrapper?.data))
                            {
                                var dataModel = JsonSerializer.Deserialize<Symbol_Notify_Info>(wrapper.data);
                                if (dataModel != null && !string.IsNullOrEmpty(dataModel.Symbol))
                                {
                                    symbolMap[dataModel.Symbol] = dataModel;
                                }
                                    
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Lỗi khi đọc dòng: {ex.Message}");
                        }
                    }
                    c_lstSymbol = symbolMap.Values.ToList();
                }
                    

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }


        public static void LoadProject()
        {
            try
            {
                var request = new SearchProjectRequest
                {
                    Start = 1,
                    End = 0
                };
                var pTotal = 0;
                ProjectDa _ProjectDa = new ProjectDa();
                DataSet ds = _ProjectDa.Search(request, ref pTotal);
                c_lstProject = CBO<Project>.FillCollectionFromDataSet(ds);
                c_lstProject = c_lstProject.OrderByDescending(m => m.Id).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }

        public static void LoadPage()
        {
            try
            {
                var request = new SearchProjectRequest
                {
                    Start = 1,
                    End = 0
                };
                var pTotal = 0;
                PageDA _da = new PageDA();
                DataSet ds = _da.Search(request, ref pTotal);
                c_lstPage = CBO<Page>.FillCollectionFromDataSet(ds);
                c_lstPage = c_lstPage.OrderByDescending(m => m.Id).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }


        //public static PhongThuy PhongThuy { get; set; }
        //public static User CurrentUser
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["UserInfo"] == null)
        //            return null;
        //        else
        //            return (User)HttpContext.Current.Session["UserInfo"];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["UserInfo"] = value;
        //    } 
        //}

        //public static User CurrentUser(this HttpContext context)
        //{
        //    User user = new User();
        //    try
        //    {
        //        user = context.Session.GetObjectFromJson<User>("user");

        //        if (user != null)
        //        {
        //            if (MemoryData.c_dic_Function_ByUser.ContainsKey(user.User_Id))
        //            {
        //                user.List_User_Functions = MemoryData.c_dic_Function_ByUser[user.User_Id];
        //            }
        //            else
        //            {
        //                user.List_User_Functions = new List<Cb_Function_Info>();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        WebForm.Common.Logger.Log.Error(ex.ToString());
        //        user = new User();
        //    }
        //    return user;
        //}

        public static EmailInfo EmailOriginal { get; set; }

    }

    public class ConfigInfo
    {
        public static string Jwt_Issuer { get; set; } = "";
        public static string Jwt_Key { get; set; } = "";
        public static string wwwRootPath { get; set; } = "";
        public static string ContentRootPath { get; set; } = "";

        public static string BaseUrl { get; set; } = "";
        public static string BaseDir { get; set; } = "";
        public static string ConnectString { get; set; } = "";
        public static int RecordOnPage = 10;
        public static int RecordOnPageIndex = 5;
        public static string ProductTemplate { get; set; } = "";
        public static string EmailTemplate { get; set; } = "";
        public static string ContactPhone { get; set; } = "";
        public static string EmailBusiness { get; set; } = "";
        public static string ChartProtocal { get; set; } = "";
        public static string WebSocketData { get; set; } = "";
        
        public static void GetConfig(IConfiguration configuration)
        {
            try
            {
                ChartProtocal = configuration["ChartProtocal"]?.ToString() ?? "";
                WebSocketData = configuration["WebSocketData"]?.ToString() ?? "";

                BaseDir = configuration["BaseDir"]?.ToString() ?? "";
                BaseUrl = configuration["BaseUrl"]?.ToString() ?? "";
                RecordOnPage = Convert.ToInt32(configuration["RecordOnPage"]?.ToString() ?? "10");
                RecordOnPageIndex = Convert.ToInt32(configuration["RecordOnPageIndex"]?.ToString() ?? "10");
                ConnectString = configuration["ConnectString"]?.ToString() ?? "";
                EmailBusiness = configuration["EmailBusiness"]?.ToString() ?? "";
                var fileTemplate = System.IO.Path.Combine(ConfigInfo.ContentRootPath, "Template", "Product.html");
                ProductTemplate = File.ReadAllText(fileTemplate);
                fileTemplate = System.IO.Path.Combine(ConfigInfo.ContentRootPath, "Template", "DangKyTuVan.html");
                EmailTemplate = File.ReadAllText(fileTemplate);

                string _ssl = configuration["SSL"]?.ToString() ?? "N";
                DataMemory.EmailOriginal = new EmailInfo()
                {
                    Host = configuration["EMailHost"]?.ToString() ?? "",
                    Port = Convert.ToInt32(configuration["EmailPost"]?.ToString() ?? "21"),
                    Name = configuration["EMailFrom"]?.ToString() ?? "",
                    EmailCC = configuration["EMailCC"]?.ToString() ?? "",
                    PassWord = configuration["EMailPass"]?.ToString() ?? "",
                    DisplayName = configuration["DisplayName"]?.ToString() ?? "",
                    IsSsl = _ssl == "Y" ? true : false,
                };

                ContactPhone = configuration["ContactPhone"]?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
        }
    }

    public class Logger
    {
        public static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}