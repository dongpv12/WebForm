using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Helpers;
using WebForm.Models;

namespace WebForm.Areas.Admin.Controllers
{
    public class SymbolController : Controller
    {
        private readonly SymbolDA _Da = new SymbolDA();
        [HttpGet]
        public ActionResult List()
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            var request = new SearchSymbolRequest
            {
                CurrentPage = 1,
                Start = 1,
                End = ConfigInfo.RecordOnPage,
                OrderBy = "CreateDate",
                OrderByType = "DESC"
            };

            return View("~/Areas/Admin/Views/Symbol/List.cshtml", SearchNews(request));
        }


        private ListSymbol SearchNews([FromBody] SearchSymbolRequest request)
        {
            var total = 0;
            var ds = _Da.Search(request, ref total);
            var lst = CBO<Symbol_Notify_Info>.FillCollectionFromDataSet(ds);
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);
            var paging = HtmlControllHelpers.WritePaging(totalPage, request.CurrentPage, total, ConfigInfo.RecordOnPage,
                "Cổ phiếu");
            var list = new ListSymbol
            {
                Start = request.Start,
                Collection = lst,
                Paging = paging,
                TotalRecord = total,
                TotalPage = totalPage,
                CurrentPage = request.CurrentPage
            };
            return list;
        }
        [HttpPost]
        public ActionResult Search([FromBody] SearchSymbolRequest request)
        {
            return PartialView("~/Areas/Admin/Views/Symbol/_listNews.cshtml", SearchNews(request));
        }

        [HttpGet]
        public ActionResult Create()
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            return View("~/Areas/Admin/Views/Symbol/Create.cshtml");
        }

        [HttpPost]
        //[Route("Do-Create")]
        public ActionResult DoCreate([FromBody] Symbol_Notify_Info model)
        {

            // 
            List<Symbol_Notify_Info> list = DataMemory.GetAllSymbol();
            if (list.Count(x=>x.Symbol == model.Symbol) > 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Tạo cổ phiếu thất bại, Đã tồn tại mã cổ phếu trong hệ thống"
                });
            }

            var ketqua = _Da.Create(model);
            if (ketqua > 0)
            {
                DataMemory.LoadSymbol();
                return Json(new
                {
                    Status = true,
                    Message = "Tạo cổ phiếu thành công"
                });
            }
            else
            {
                return Json(new
                {
                    Status = false,
                    Message = "Tạo cổ phiếu thất bại"
                });
            }
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            var news = DataMemory.GetAllSymbol().Where(x=>x.Id == id).FirstOrDefault();

            return View("~/Areas/Admin/Views/Symbol/Edit.cshtml", news);
        }

        [HttpPost]
        public ActionResult Edit([FromBody] Symbol_Notify_Info request)
        {

            List<Symbol_Notify_Info> list = DataMemory.GetAllSymbol();
            if (list.Count(x => x.Symbol == request.Symbol && x.Id != request.Id) > 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Sửa cổ phiếu thất bại, Đã tồn tại mã cổ phếu trong hệ thống"
                });
            }

            var result = _Da.Edit(request);
            if (result >= 0)
            {
                DataMemory.LoadSymbol();
                return Json(new
                {
                    Status = true,
                    Message = "Sửa cổ phiếu thành công"
                });
            }
            else
            {
                return Json(new
                {
                    Status = false,
                    Message = "Sửa cổ phiếu thất bại"
                });
            }
        }

        [HttpPost]
        public ActionResult Delete([FromBody] News request)
        {
            var result = _Da.Delete(request.Id);
            if (result > 0)
            {
                DataMemory.LoadSymbol();
                return Json(new
                {
                    Status = true,
                    Message = "Xóa thành công"
                });
            }
            else
            {
                return Json(new
                {
                    Status = false,
                    Message = "Xóa thất bại"
                });
            }
        }


    }
}
