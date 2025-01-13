using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

public class JsonEncryptAttributeModel
{
    public string FirstName { get; set; }
    public int Age { get; set; }
    [JsonEncrypt]
    public string Password { get; set; }
    [JsonEncrypt]
    public string Password2 { get; set; }
}
