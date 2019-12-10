using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        public static IWebHostEnvironment _environment { get; set; }
        public ImageUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public class FileUploadModel
        {
            public IFormFile file { get; set; }
        }

        [HttpPost]
        public async Task<string> Post([FromForm]FileUploadModel objFile)
        {
            try
            {
                if (objFile.file.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");

                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.file.FileName))
                    {
                        objFile.file.CopyTo(fileStream);
                        fileStream.Flush();
                        return "\\Upload\\" + objFile.file.FileName;
                    }
                } 
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}