namespace SystemLibrary.Common.Framework;

internal class XmlConfig : Config<XmlConfig>
{
    public string FirstName { get; set; }
    public string lastname { get; set; }
    public bool FlagCamelCase { get; set; }
    public bool FlagPascalCase { get; set; }
    public int Phone { get; set; }
}
