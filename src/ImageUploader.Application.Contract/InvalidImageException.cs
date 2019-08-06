using System;

namespace ImageUploader.Application.Contract
{

    [Serializable]
    public class InvalidImageException : Exception
    {
        public InvalidImageException() { }
    }
}
