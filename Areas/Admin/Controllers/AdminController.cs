using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data; 
using WebForm.Common;
using WebForm.DataAccess;
using WebForm.Models;
using System.Threading.Tasks;

namespace WebForm.Areas.Admin.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly UsersDA _usersDa = new UsersDA();

        [Route("dang-nhap")]
        [HttpGet]
        public ActionResult Login()
        {
            var user = this.HttpContext.CurrentUser();
            if (user != null)
            {
                return Redirect("/News/List");
            }
            try
            {
                return View("~/Areas/Admin/Views/Admin/Login.cshtml");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.ToString());
                throw;
            }
        }

        [HttpPost]
        [Route("dang-nhap")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "Dữ liệu đăng nhập không hợp lệ"
                    });
                }

                var user = new User
                {
                    UserName = request.username,
                    Password = request.password
                };

                var ds = new DataSet();
                
                if (!_usersDa.CheckLogin(user, ref ds))
                    return Json(new
                    {
                        Status = false,
                        Message = "Sai tên đăng nhập hoặc mật khẩu"
                    });

                user = (User) CBO.FillObjectFromDataSet(ds, typeof(User));
                //DataMemory.CurrentUser = user;

                this.HttpContext.Session.SetObjectAsJson("user", user);

                return Json(new
                {
                    Status = true,
                    Message = "Đăng nhập thành công"
                });
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.ToString());
                throw;
            }
        }

        [Route("dang-xuat")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                User currentUser = this.HttpContext.CurrentUser();
                await HttpContext.SignOutAsync(ConstData.authType);

                // remove cookies
                Response.Cookies.Delete("user");
                HttpContext.Session.Clear();

                this.HttpContext.Session.Remove_Session_ByKey("user");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.ToString());
                throw;
            }
            return Redirect("/admin/dang-nhap");
        }
    }
}