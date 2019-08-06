using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageUploader.Application.Contract
{
    public interface IImagesService
    {
        /// <exception cref="ImageNotFoundException"></returns>
        Stream Get(Guid id);

        /// <exception cref="ImageNotFoundException"></returns>
        Stream GetThumbnail(Guid id);

        /// <exception cref="InvalidImageException"></returns>
        Guid Upload(Stream stream);

        /// <exception cref="InvalidImageException"></returns>
        Guid Upload(string encodedImage);

        /// <exception cref="ResourceNotFoundException"></returns>
        Task<Guid> UploadFrom(string uri);
    }
}