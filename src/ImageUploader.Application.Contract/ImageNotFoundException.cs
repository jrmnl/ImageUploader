using System;

namespace ImageUploader.Application.Contract
{

    [Serializable]
    public class ImageNotFoundException : Exception
    {
        public ImageNotFoundException() { }
    }
}
