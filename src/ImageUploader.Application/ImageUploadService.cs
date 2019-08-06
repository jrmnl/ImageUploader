using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageUploader.Application.Contract;

namespace ImageUploader.Application
{
    public class ImagesService : IImagesService
    {
        private readonly string _path;
        private readonly HttpClient _client;

        public ImagesService(string path, HttpClient client)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path)) throw new ArgumentException($"Directory '{path}' doesn't exist");

            _path = path;
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Stream Get(Guid id)
        {
            var name = CreateName(id);
            return OpenRead(name);
        }

        public Stream GetThumbnail(Guid id)
        {
            var name = CreateThumbnailName(id);
            return OpenRead(name);
        }

        public Guid Upload(string encodedImage)
        {
            var image = Convert.FromBase64String(encodedImage);

            using (var stream = new MemoryStream(image))
            {
                return Upload(stream);
            }
        }

        public Guid Upload(Stream stream)
        {
            var id = Guid.NewGuid();

            using (var image = Image.FromStream(stream))
            {
                var name = CreateName(id);
                Save(name, image);

                using (var thumbnail = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero))
                {
                    var thumbName = CreateThumbnailName(id);
                    Save(thumbName, thumbnail);
                }
            }

            return id;
        }

        public async Task<Guid> UploadFrom(string uri)
        {
            using (var streamImage = await _client.GetStreamAsync(uri))
            {
                return Upload(streamImage);
            }
        }

        private Stream OpenRead(string name) => File.OpenRead(Path.Combine(_path, name));

        private void Save(string name, Image image)
        {
            using (var fileStream = File.Create(Path.Combine(_path, name)))
            {
                image.Save(fileStream, ImageFormat.Jpeg);
            }
        }

        private static string CreateName(Guid id) => id.ToString("D");

        private static string CreateThumbnailName(Guid id) => $"{CreateName(id)}-thumb";
    }
}