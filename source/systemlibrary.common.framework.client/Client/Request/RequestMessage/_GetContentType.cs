using System.Collections;
using System.Dynamic;
using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

partial class Client
{
    partial class Request
    {
        static ContentType GetContentType(RequestOptions options)
        {
            if (options.Data == null) return ContentType.None;

            if (options.ContentType == ContentType.Auto)
                return DetectContentType(options);

            return options.ContentType;
        }

        static ContentType DetectContentType(RequestOptions options)
        {
            var data = options.Data;

            if (data is string txt)
                return GetStringContentType(txt);

            if (data is byte[] bytes)
                return GetByteArrayContentType(bytes);

            var type = data.GetType();

            if (data is List<KeyValuePair<string, string>> ||
                data is ExpandoObject ||
                data is IDictionary)
                return ContentType.xwwwformUrlEncoded;

            if (
                (
                    type.IsGenericType &&
                    type.Name.Contains("Anonymous")
                ) ||
                (
                    type.IsClassType() &&
                    !type.IsGenericType
                ))
                return ContentType.json;

            throw new InvalidOperationException("Unable to auto-detect content type. Please set it manually.");
        }

        static ContentType GetStringContentType(string data)
        {
            if (data.IsJson()) return ContentType.json;

            if (data.IsXml())
            {
                if (data.StartsWith("<svg") || data.StartsWith(" <svg"))
                    return ContentType.svg;

                return ContentType.xml;
            }

            if (data.Contains("content-disposition: form-data"))
                return ContentType.multipartFormData;

            if (data.Length > 2 &&
                !data.StartsWith("/") &&
                !data.StartsWith("#") &&
                !data.StartsWith(" ") &&
                !data.StartsWith("?") &&
                !data.Contains("\n") &&
                !data.Contains("\t") &&
                data.Contains("&") &&
                data.Contains("="))
                return ContentType.xwwwformUrlEncoded;

            if (data.StartsWith("<!DOCTYPE html") || data.StartsWith("<html"))
                return ContentType.html;

            if (data.Contains("function") && data.Contains(";") && data.ContainsAny("var", "const", "let", "=>"))
                return ContentType.javascript;

            return ContentType.text;
        }

        static ContentType GetByteArrayContentType(byte[] data)
        {
            if (data == null || data.Length < 4) return ContentType.octetStream;

            if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF) return ContentType.jpeg;

            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47) return ContentType.png;

            if (data.Length >= 5 && data[0] == 0x25 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46 && data[4] == 0x2D) return ContentType.pdf;

            if (data[0] == 0x50 && data[1] == 0x4B)
            {
                if (data.Length > 256)
                {
                    var docx = Encoding.UTF8.GetString(data, 0, 256);

                    if (docx.Contains("[Content_Types].xml") && docx.Contains("word/"))
                        return ContentType.docx;
                }

                return ContentType.zip;
            }

            var txt = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 48)).ToLower();

            if (txt.StartsWithAny("<svg", " <svg", "\r\n<svg", "\n<svg")) return ContentType.svg;
            if (txt.StartsWithAny("<", " <", "\r\n<", "\n<")) return ContentType.xml;

            if (txt.Contains("content-disposition: form-data")) return ContentType.multipartFormData;

            if (txt.IsJson())
            {
                if (txt.ContainsAny("\"query\"", "\"mutation\"", "\"operationName\"")) return ContentType.graphql;

                return ContentType.json;
            }

            return ContentType.octetStream;
        }
    }
}
