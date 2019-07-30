using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageUploader.Application.Contract
{
    public interface IImageUploader
    {
        Task UploadByBase64(IEnumerable<string> base64images);
    }
}