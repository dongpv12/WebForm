using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Models;

namespace WebForm.Areas.Admin.Controllers
{
    public class PageController : Controller
    {
        private PageDA _pageDa = new PageDA();
        public ActionResult AboutUs()
        {
            var model = GetById(1);
            return View("~/Areas/Admin/Views/Page/AboutUs.cshtml", model);
        }

        public ActionResult AgencyPolicy()
        {
            var model = GetById(2);
            return View("~/Areas/Admin/Views/Page/AgencyPolicy.cshtml", model);
        }


        public ActionResult Recruitment()
        {
            var model = GetById(4);
            return View("~/Areas/Admin/Views/Page/Recruitment.cshtml", model);
        }
        public ActionResult ContactUs()
        {
            var model = GetById(3);
            return View("~/Areas/Admin/Views/Page/ContactUs.cshtml", model);
        }

        private Page GetById(int id)
        {
            try
            {
                var ds = _pageDa.GetById(id);
                var page = (Page)CBO.FillObjectFromDataSet(ds, typeof(Page));
                return page;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.ToString());
                return new Page();
            }
        }

        public ActionResult Edit(Page request)
        {
            try
            {
                var result = _pageDa.Edit(request);
                DataMemory.LoadPage();
                return Json(new
                {
                    Status = result > 0
                });
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.ToString());
            }
            return Json(new
            {
                Status = false
            });
        }
    }
}