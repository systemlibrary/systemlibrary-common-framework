using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace SystemLibrary.Common.Framework;

internal class BlacklistedJsonPropertyTypes
{
    public RuntimeTypeHandle RuntimeTypeHandle { get; set; }
    public Type Type { get; set; }
    public IDictionary<int, string> IDictionaryTypeString { get; set; }
    public ModelBindingMessageProvider ModelBindingMessageProvider { get; set; }
    public RuntimeWrappedException RuntimeWrappedException { get; set; }
    public Exception Exception { get; set; }
}
