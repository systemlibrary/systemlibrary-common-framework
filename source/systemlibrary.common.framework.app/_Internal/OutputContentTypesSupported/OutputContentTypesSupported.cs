using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Framework.App;

internal class OutputContentTypesSupported : StringOutputFormatter
{
    internal OutputContentTypesSupported()
    {
        SupportedMediaTypes.Add("*/*");
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return true;
    }
}
