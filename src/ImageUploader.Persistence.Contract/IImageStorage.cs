using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageUploader.Persistence.Contract
{
    public interface IImageStorage
    {
        Task Save(string name, Stream image);
    }
}
