using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageUploader.Application.Contract;
using ImageUploader.Persistence.Contract;

namespace ImageUploader.Application
{
    public class ImageUploadService : IImageUploader
    {
        private readonly IImageStorage _storage;

        public ImageUploadService(IImageStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task UploadByBase64(IEnumerable<string> base64images)
        {
            var images = base64images.Select(base64 => base64.ToImageFromBase64());

            foreach (var image in images)
            {
                var randomName = Guid.NewGuid().ToString("G");
                await Save(randomName, image);
                var thumbnail = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                await Save($"{randomName}_thumb", thumbnail);
            }
        }

        private async Task Save(string name, Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                stream.Seek(0, SeekOrigin.Begin);
                await _storage.Save(name, stream);
            }
        }
    }
}