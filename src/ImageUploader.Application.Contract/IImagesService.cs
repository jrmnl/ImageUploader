using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageUploader.Application.Contract
{
    public interface IImagesService
    {
        Stream Get(Guid id);
        Stream GetThumbnail(Guid id);

        Guid Upload(Stream stream);
        Guid Upload(string encodedImage);
        Task<Guid> UploadFrom(string uri);
    }
}