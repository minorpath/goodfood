using System;
using System.IO;
using System.Threading.Tasks;
using Backend.Data;
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

        private readonly IFileStorage _fileStorage;

        public ImageUploadController(IWebHostEnvironment environment, IFileStorage fileStorage)
        {
            _environment = environment;
            _fileStorage = fileStorage;
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
                    using (var memStream = new System.IO.MemoryStream())
                    {
                        await objFile.file.CopyToAsync(memStream);
                        await memStream.FlushAsync();
                        return await _fileStorage.Add(objFile.file.FileName, memStream);
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