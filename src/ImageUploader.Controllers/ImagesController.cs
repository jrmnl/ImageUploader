using System;
using System.Threading.Tasks;
using ImageUploader.Application.Contract;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageUploader _uploader;

        public ImagesController(IImageUploader uploader)
        {
            _uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]UploadRequest request)
        {
            try
            {
                switch (request.UploadType)
                {
                    case UploadType.ByBase64s:
                        var images = request.Base64Images ?? throw new ArgumentNullException(nameof(request.Base64Images));
                        await _uploader.UploadByBase64(images);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(List<IFormFile> files)
        //{
        //    long size = files.Sum(f => f.Length);
        //
        //    // full path to file in temp location
        //    var filePath = Path.GetTempFileName();
        //
        //    foreach (var formFile in files)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }
        //        }
        //    }
        //
        //    // process uploaded files
        //    // Don't rely on or trust the FileName property without validation.
        //
        //    return Ok(new { count = files.Count, size, filePath });
        //}
    }
    public enum UploadType
    {
        ByBase64s
    }

    public class UploadRequest
    {
        public UploadType UploadType { get; set; }
        public string[] Base64Images { get; set; }
        public string[] Urls { get; set; }
    }
}
