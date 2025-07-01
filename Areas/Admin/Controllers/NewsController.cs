using Microsoft.AspNetCore.Mvc;
using System;

using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Helpers;
using WebForm.Models;

namespace WebForm.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsDA _newsDa = new NewsDA();

        [HttpGet]
        public ActionResult List()
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            var request = new SearchNewsRequest
            {
                CurrentPage = 1,
                Start = 1,
                End = ConfigInfo.RecordOnPage,
                OrderBy = "CreateDate",
                OrderByType = "DESC"
            };

            return View("~/Areas/Admin/Views/News/List.cshtml", SearchNews(request));
        }

        [HttpGet] 
        public ActionResult Create()
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            return View("~/Areas/Admin/Views/News/Create.cshtml");
        }

        [HttpPost]
        //[Route("Do-Create")]
        public ActionResult DoCreate([FromBody] NewsRequest model)
        {
            //var ketqua = _newsDa.Create(model);
            var ketqua = 1;
            if (ketqua > 0)
            {
                DataMemory.LoadNews();
                return Json(new
                {
                    Status = true,
                    Message = "Tạo tin mới thành công"
                });
            }
            else
            {
                return Json(new
                {
                    Status = false,
                    Message = "Tạo tin mới thất bại"
                });
            }
        }

        [HttpPost]
        public ActionResult Search([FromBody] SearchNewsRequest request)
        {
            return PartialView("~/Areas/Admin/Views/News/_listNews.cshtml", SearchNews(request));
        }

        [HttpGet]
        public ActionResult EditNews(int id)
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            var news = (News)CBO.FillObjectFromDataSet(_newsDa.GetById(id), typeof(News));

            return View("~/Areas/Admin/Views/News/Edit.cshtml", news);
        }

        [HttpPost]
        public ActionResult Edit([FromBody] News request)
        {
            //var result = _newsDa.Edit(request);
            var result = 1;
            if (result > 0)
            {
                DataMemory.LoadNews();
                return Json(new
                {
                    Status = true,
                    Message = "Sửa bài viết thành công"
                });
            }
            else
            {
                return Json(new
                {
                    Status = false,
                    Message = "Sửa bài viết thất bại"
                });
            }
        }

        [HttpPost]
        public ActionResult Delete([FromBody] int id)
        {
            var result = _newsDa.Delete(id);
            if (result > 0)
            {
                DataMemory.LoadNews();
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

        private ListNews SearchNews([FromBody] SearchNewsRequest request)
        {
            var total = 0;
            var ds = _newsDa.Search(request, ref total);
            var lstNew = CBO<News>.FillCollectionFromDataSet(ds);
            var totalPage = Math.Ceiling(total / (decimal)ConfigInfo.RecordOnPage);
            var paging = HtmlControllHelpers.WritePaging(totalPage, request.CurrentPage, total, ConfigInfo.RecordOnPage,
                "Tin tức");
            var listNews = new ListNews
            {
                Start = request.Start,
                Collection = lstNew,
                Paging = paging,
                TotalRecord = total,
                TotalPage = totalPage,
                CurrentPage = request.CurrentPage
            };
            return listNews;
        }
    }
}