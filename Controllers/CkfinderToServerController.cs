using Microsoft.AspNetCore.Mvc;
using WebForm.Models;

namespace WebForm.Controllers
{
    [Route("ckfinder-ftp")]
    public class CkfinderToServerController : Controller
    {


        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode, string type)
        {
            if (upload != null && upload.Length > 0)
            {
                // Đường dẫn thư mục wwwroot/ImgUpload
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgUpload");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }


                var originalFileName = Path.GetFileNameWithoutExtension(upload.FileName);
                var extension = Path.GetExtension(upload.FileName);
                var timestamp = DateTime.Now.ToString("ddMMyyHHmmss"); // ngày + giờ chính xác hơn

                var fileName = $"{originalFileName}_{timestamp}{extension}";

                // Tạo tên file an toàn
                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file vào wwwroot/ImgUpload
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                // Tạo URL truy cập ảnh
                var fileUrl = Url.Content("~/ImgUpload/" + fileName);

                // Gọi hàm CKEditor để chèn ảnh
                var script = $@"<script>
                                window.opener.CKEDITOR.tools.callFunction({CKEditorFuncNum}, '{fileUrl}');
                                window.close();
                            </script>";


                //string output = string.Format("<html><body><script>window.parent.CKEDITOR.tools.callFunction({0}, \"{1}\", \"{2}\");</script></body></html>", CKEditorFuncNum, fileUrl, "");
                //return Content(output);

                return Content("<script>alert('Tải file lên thành công'); window.close();</script>", "text/html");
            }

            return Content("<script>alert('Chưa chọn file hoặc file không hợp lệ'); window.close();</script>", "text/html");
        }





        [HttpGet("browse")]
        public IActionResult Browse(string CKEditorFuncNum, string CKEditor, string langCode, string type)
        {
            try
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgUpload");
                var baseUrl = $"{Request.Scheme}://{Request.Host}/ImgUpload";

                List<Filett> lstFiles = new List<Filett>();

                if (Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath);
                    foreach (var file in files)
                    {
                        Filett _Filett = new Filett();
                        var fileName = Path.GetFileName(file);
                        var fileInfo = new FileInfo(file);
                        _Filett.FileSize = fileInfo.Length / 1024;
                        _Filett.FileName = fileInfo.Name;
                        _Filett.CreatedTime = fileInfo.CreationTime;
                        _Filett.ModifiedTime = fileInfo.LastWriteTime;
                        _Filett.FileUrl = $"{baseUrl}/{fileName}";
                        lstFiles.Add(_Filett);
                    }
                }
                ViewBag.CKEditorFuncNum = CKEditorFuncNum;
                return View(lstFiles); // hoặc xử lý gì đó
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [HttpGet("browseImage")]
        public IActionResult BrowseImage()
        {
            try
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgUpload");
                var baseUrl = $"{Request.Scheme}://{Request.Host}/ImgUpload";

                List<Filett> lstFiles = new List<Filett>();

                if (Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath);
                    foreach (var file in files)
                    {
                        Filett _Filett = new Filett();
                        var fileName = Path.GetFileName(file);
                        var fileInfo = new FileInfo(file);
                        _Filett.FileSize = fileInfo.Length / 1024;
                        _Filett.FileName = fileInfo.Name;
                        _Filett.CreatedTime = fileInfo.CreationTime;
                        _Filett.ModifiedTime = fileInfo.LastWriteTime;
                        _Filett.FileUrl = $"{baseUrl}/{fileName}";
                        lstFiles.Add(_Filett);
                    }
                }
                return View(lstFiles); // hoặc xử lý gì đó
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [HttpPost("uploadImage")]
        public async Task<IActionResult> uploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                // Đường dẫn thư mục wwwroot/ImgUpload
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgUpload");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var originalFileName = Path.GetFileNameWithoutExtension(upload.FileName);
                var extension = Path.GetExtension(upload.FileName);
                var timestamp = DateTime.Now.ToString("ddMMyyHHmmss"); // ngày + giờ chính xác hơn

                var fileName = $"{originalFileName}_{timestamp}{extension}";

                // Tạo tên file an toàn
                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file vào wwwroot/ImgUpload
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                // Tạo URL truy cập ảnh
                var fileUrl = Url.Content("~/ImgUpload/" + fileName);

                return Content("<script>alert('Tải file lên thành công'); window.close();</script>", "text/html");
            }

            return Content("<script>alert('Chưa chọn file hoặc file không hợp lệ'); window.close();</script>", "text/html");
        }

    }
}
