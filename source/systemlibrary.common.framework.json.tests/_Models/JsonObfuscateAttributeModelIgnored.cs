using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

public class JsonObfuscateAttributeModelIgnored
{
    public int Id { get; set; }
    public int Id2;

    [JsonObfuscate]
    public int ID3 { get; set; }

    [JsonObfuscate]
    public int id4;

    [JsonObfuscate]
    public string id7 { get; set; }

    [JsonObfuscate]
    public string id8;

    [JsonObfuscate]
    public long id9 { get; set; }
}
