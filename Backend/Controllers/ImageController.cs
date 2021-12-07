using HeinjoFood.Api.Data;
using HeinjoFood.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeinjoFood.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;

        public ImageController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        [HttpGet]
        [Route("{fileName}", Name = "GetImageByName")]
        public async Task<IActionResult> Get(string fileName)
        {
            var stream = await _fileStorage.GetAsync(fileName);
            if (stream == null)
                return NotFound();
            var contentType = _fileStorage.GetContentType(fileName) ?? "application/unknown";
            return File(stream, contentType);
        }

        [HttpPost]
        [Route("", Name = "UploadImage")]
        public async Task<IActionResult> Post([FromForm] FileUploadModel model)
        {
            try
            {
                if (model.File?.Length > 0)
                {
                    using var memStream = new MemoryStream();
                    await model.File.CopyToAsync(memStream);
                    await memStream.FlushAsync();
                    return Created(await _fileStorage.Add(model.File.FileName, memStream), null);
                }
                else
                {
                    return BadRequest("No files");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}