using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Framework.App;

internal class DefaultSupportedMediaTypes : StringOutputFormatter
{
    static HashSet<string> BlockedExtensions = new()
    {
        ".exe",
        ".dll",
        ".iso",
        ".msi",
        ".ps1",
        ".cmd",
        ".sh",
        ".bash",
        ".vbs",
        ".dmg",
        ".config",
        ".env",
        ".ini",
        ".key",
        ".pem"
    };

    internal DefaultSupportedMediaTypes()
    {
        SupportedMediaTypes.Add("*/*");
    }
    
    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        var requestPath = context?.HttpContext?.Request?.Path.ToString();

        if (requestPath != null && requestPath.Length > 4)
        {
            if (BlockedExtensions.Any(ext => requestPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }
        
        return true;
    }
}
