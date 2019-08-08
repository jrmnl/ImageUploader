using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImageUploader.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace ImageUploader.IntegrationTests
{
    public class ApiTests
    {
        private readonly HttpClient _client;

        public ApiTests()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>()
                    {
                        { "FILE_PATH", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) }
                    })
                .Build();
            var hostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(config)
                .UseStartup<Startup>();
            var server = new TestServer(hostBuilder);
            _client = server.CreateClient();
        }

        [Fact]
        public async Task UploadsBase64Images()
        {
            //arrange
            var encodedImage = await File.ReadAllTextAsync("Image1.txt");

            //act
            var ids = await UploadEncodedImages(new[] { encodedImage, encodedImage, encodedImage });

            //assert
            Assert.True(ids.Length == 3);
        }

        [Fact]
        public async Task DownloadsImage()
        {
            //arrange
            var encodedImage = await File.ReadAllTextAsync("Image1.txt");
            var ids = await UploadEncodedImages(new[] { encodedImage });

            //act
            var stream = await GetImage(ids[0]);

            //assert
            using (stream)
            {
                var expectedWH = GetEncodedImageMeta(encodedImage);
                var resultWH = GetImageMeta(stream);
                Assert.True(expectedWH == resultWH);
            }
        }

        [Fact]
        public async Task CreatesThumbnailImage()
        {
            //arrange
            var encodedImage = await File.ReadAllTextAsync("Image1.txt");
            var ids = await UploadEncodedImages(new[] { encodedImage });

            //act
            var stream = await GetThumbnailImage(ids[0]);

            //assert
            using (stream)
            {
                var resultWH = GetImageMeta(stream);
                Assert.True(resultWH.Width <=100 && resultWH.Height <= 100);
            }
        }

        private async Task<string[]> UploadEncodedImages(string[] images)
        {
            var input = new ImagesController.ImageUploadRequest
            {
                Type = ImagesController.ImageUploadType.ByBase64,
                EncodedImages = images
            };
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/images/", content);

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        private async Task<Stream> GetThumbnailImage(string id)
        {
            var response = await _client.GetAsync($"/api/images/{id}/thumbnail");
            return await response.Content.ReadAsStreamAsync();
        }

        private async Task<Stream> GetImage(string id)
        {
            var response = await _client.GetAsync($"/api/images/{id}");
            return await response.Content.ReadAsStreamAsync();
        }

        private static (int Width, int Height) GetEncodedImageMeta(string encodedImage)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(encodedImage)))
            using (var image = Image.FromStream(stream))
            {
                return (image.Width, image.Height);
            }
        }

        private static (int Width, int Height) GetImageMeta(Stream stream)
        {
            using (var image = Image.FromStream(stream))
            {
                return (image.Width, image.Height);
            }
        }
    }
}
