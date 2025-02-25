using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Framework.App;

internal class OutputContentTypesSupported : StringOutputFormatter
{
    static string[] BlockedExtensions =
    [
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
    ];

    static int BlockedExtensionsLength = BlockedExtensions.Length;

    internal OutputContentTypesSupported()
    {
        SupportedMediaTypes.Add("*/*");
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        var requestPath = context?.HttpContext?.Request?.Path.ToString();

        if (requestPath == null) return true;

        var l = requestPath.Length;

        if (l <= 5) return true;

        if (requestPath[l - 1] == '/') return true;

        var extensionPosition = requestPath.LastIndexOf('.', l - 1, 7); 

        if (extensionPosition == -1) return true;

        // 11 hellowor.ld
        // 8
        // 11 - 8 == 3 
        var extensionLength = l - extensionPosition;

        if (extensionLength < 3) return true;

        for (int i = 0; i < BlockedExtensionsLength; i++)
        {
            if (requestPath.EndsWith(BlockedExtensions[i], StringComparison.OrdinalIgnoreCase)) return false;
        }

        return true;
    }
}
