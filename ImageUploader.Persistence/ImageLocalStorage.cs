using System;
using System.IO;
using System.Threading.Tasks;
using ImageUploader.Persistence.Contract;

namespace ImageUploader.Persistence
{
    public class ImageLocalStorage : IImageStorage
    {
        private string _path;

        public ImageLocalStorage(string path)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path)) throw new ArgumentException($"Directory '{path}' doesn't exist");

            _path = path;
        }

        public async Task Save(string name, Stream image)
        {
            using (var fileStream = File.Create(Path.Combine(_path, name)))
            {
                await image.CopyToAsync(fileStream);
            }
        }
    }
}
