using Microsoft.AspNetCore.Mvc;
using System;

using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Helpers;
using WebForm.Models;

namespace WebForm.Areas.Admin.Controllers
{
    [Route("color")]
    public class ColorWarehouseController : Controller
    {
        private readonly ColorWarehouseDA _colorWarehouseDa = new ColorWarehouseDA();

        [Route("danh-sach-kho-mau.htm")]
        [HttpGet]
        public ActionResult List()
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            var request = new SearchColorWarehouseRequest()
            {
                CurrentPage = 1,
                Start = 1,
                End = ConfigInfo.RecordOnPage
            };
             
            return View("~/Areas/Admin/Views/ColorWarehouse/List.cshtml", SearchColorWarehouse(request));
        }
        [Route("tao-moi.htm")]
        [HttpGet]
        public ActionResult Create()
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            return View("~/Areas/Admin/Views/ColorWarehouse/Create.cshtml");
        }

        [Route("create-color")]
        [HttpPost]
        public ActionResult Create(ColorWarehouse request)
        {
            var result = _colorWarehouseDa.Create(request);
            if (result > 0)
                return Json(new
                {
                    Status = true,
                    Message = "Tạo kho màu thành công"
                });
            return Json(new
            {
                Status = false,
                Message = "Tạo kho màu thất bại"
            });
        }


        [Route("edit-color-{id}.htm")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = this.HttpContext.CurrentUser();
            if (user == null)
            {
                return Redirect("/admin/dang-nhap");
            }
            var project =
                (ColorWarehouse) CBO.FillObjectFromDataSet(_colorWarehouseDa.GetById(id), typeof(ColorWarehouse));

            return View("~/Areas/Admin/Views/ColorWarehouse/Edit.cshtml", project);
        }

        [HttpPost]
        public ActionResult Edit(ColorWarehouse request)
        {
            var result = _colorWarehouseDa.Edit(request);
            if (result > 0)
                return Json(new
                {
                    Status = true,
                    Message = "Sửa kho màu thành công"
                });
            return Json(new
            {
                Status = false,
                Message = "Sửa kho màu thất bại"
            });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = _colorWarehouseDa.Delete(id);
            if (result > 0)
                return Json(new
                {
                    Status = true,
                    Message = "Xóa thành công"
                });
            return Json(new
            {
                Status = false,
                Message = "Xóa thất bại"
            });
        }

        [HttpPost]
        public ActionResult Search(SearchColorWarehouseRequest request)
        {
            return PartialView("~/Areas/Admin/Views/ColorWarehouse/_listColor.cshtml", SearchColorWarehouse(request));
        }

        private ListColorWarehouse SearchColorWarehouse(SearchColorWarehouseRequest  request)
        {
            var total = 0;
            var ds = _colorWarehouseDa.Search(request, ref total);
            //var lstNew = CBO.Fill2ListFromDataSet<Project>(ds, typeof(Project));
            var lstNew = CBO<ColorWarehouse>.FillCollectionFromDataSet(ds);
            var totalPage = Math.Ceiling((decimal) total / ConfigInfo.RecordOnPage);
            var paging = HtmlControllHelpers.WritePaging(totalPage, request.CurrentPage, total, ConfigInfo.RecordOnPage,
                "bản ghi");
            var listNews = new ListColorWarehouse()
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