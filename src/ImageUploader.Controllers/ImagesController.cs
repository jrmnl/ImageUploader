using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageUploader.Application.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _service;

        public ImagesController(IImagesService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var image = _service.Get(id);
            return Ok(image);
        }

        [HttpGet("{id}/thumbnail")]
        public IActionResult GetThumbnail(Guid id)
        {
            var image = _service.GetThumbnail(id);
            return Ok(image);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UploadRequest request)
        {
            if (request is null) return BadRequest();

            var ids = new Guid[0];
            switch (request.UploadType)
            {
                case UploadType.Base64:
                    var images = request.Base64Images;
                    if (images is null) return BadRequest();
                    ids = UploadByEncodedImages(images);
                    break;

                case UploadType.Url:
                    var urls = request.Urls;
                    if (urls is null) return BadRequest();
                    ids = await UploadByUrls(urls);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return Ok(ids);
        }

        private async Task<Guid[]> UploadByUrls(IEnumerable<string> urls)
        {
            var ids = new List<Guid>();
            foreach (var url in urls)
            {
                var id = await _service.UploadFrom(url);
                ids.Add(id);
            }
            return ids.ToArray();
        }

        private Guid[] UploadByEncodedImages(IEnumerable<string> encodedImages)
        {
            var ids = new List<Guid>();
            foreach (var base64 in encodedImages)
            {
                var id = _service.Upload(base64);
                ids.Add(id);
            }
            return ids.ToArray();
        }

        [HttpPost("content")]
        public IActionResult Post([FromForm]IEnumerable<IFormFile> files)
        {
            if (files is null) return BadRequest();

            var ids = new List<Guid>();
            foreach (var formFile in files)
            {
                using (var stream = formFile.OpenReadStream())
                {
                    var id = _service.Upload(stream);
                    ids.Add(id);
                }
            }

            return Ok(ids);
        }
    }

    public enum UploadType
    {
        Base64,
        Url
    }

    public class UploadRequest
    {
        public UploadType UploadType { get; set; }
        public string[] Base64Images { get; set; }
        public string[] Urls { get; set; }
    }
}
