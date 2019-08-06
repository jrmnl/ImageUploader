using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Post([FromBody]ImageUploadRequest request)
        {
            if (request is null) return BadRequest("Body is missing");

            switch (request.Type)
            {
                case ImageUploadType.ByUrls:
                    {
                        if (request.Urls is null)
                            return BadRequest("Urls are missing");

                        return await UploadByUrls(request.Urls);
                    }

                case ImageUploadType.ByBase64:
                    {
                        if (request.EncodedImages is null)
                            return BadRequest("EncodedImages are missing");

                        return UploadByEncodedImages(request.EncodedImages);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        [HttpPost("content")]
        public IActionResult Post([FromForm]IReadOnlyList<IFormFile> images)
        {
            if (images is null) return BadRequest();
            if (!images.All(file => file.ContentType == "image/jpeg"))
                return BadRequest("Only 'image/jpeg' supported");

            var ids = new List<Guid>();
            for (int i = 0; i < images.Count; i++)
            {
                using (var stream = images[i].OpenReadStream())
                {
                    try
                    {
                        var id = _service.Upload(stream);
                        ids.Add(id);
                    }
                    catch (InvalidImageException)
                    {
                        return InvalidImageType(i);
                    }
                }
            }

            return Ok(ids);
        }

        private async Task<IActionResult> UploadByUrls(IReadOnlyList<string> urls)
        {
            var ids = new List<Guid>();
            for (int i = 0; i < urls.Count; i++)
            {
                var url = urls[i];
                try
                {
                    var id = await _service.UploadFrom(url);
                    ids.Add(id);
                }
                catch (InvalidImageException)
                {
                    return InvalidImageType(i, url);
                }
                catch (ResourceNotFoundException)
                {
                    return UnprocessableEntity($"Item #{i} '{url}': resource not found");
                }
            }
            return Ok(ids);
        }

        private IActionResult UploadByEncodedImages(IReadOnlyList<string> encodedImages)
        {
            var ids = new List<Guid>();
            for (int i = 0; i < encodedImages.Count; i++)
            {
                try
                {
                    var id = _service.Upload(encodedImages[i]);
                    ids.Add(id);
                }
                catch (InvalidImageException)
                {
                    return InvalidImageType(i);
                }
            }
            return Ok(ids);
        }

        private IActionResult InvalidImageType(int itemIndex)
        {
            return UnprocessableEntity($"Item #{itemIndex}: invalid image type");
        }

        private IActionResult InvalidImageType(int itemIndex, string url)
        {
            return UnprocessableEntity($"Item #{itemIndex} - '{url}': invalid image type");
        }

        public enum ImageUploadType
        {
            ByUrls,
            ByBase64
        }

        public class ImageUploadRequest
        {
            public ImageUploadType Type { get; set; }
            public string[] Urls { get; set; }
            public string[] EncodedImages { get; set; }
        }
    }
}
