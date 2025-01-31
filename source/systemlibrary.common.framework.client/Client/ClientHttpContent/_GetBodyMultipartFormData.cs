using System.IO;
using System.Net.Http;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class ClientHttpContent
    {
        static HttpContent GetBodyMultipartFormData(object data)
        {
            if (data is byte[] bytes)
            {
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(new MemoryStream(bytes)), "file" }
                };

                return content;
            }

            throw new System.Exception("To send an image/document/file, the 'data' must be a byte[] of the file you're sending. Pass byte[] the Post() or Put() method...");
        }
    }
}
