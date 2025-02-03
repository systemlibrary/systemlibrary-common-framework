using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class BlacklistConverterFactory : JsonConverterFactory
{
    static HashSet<Type> Blacklisted = new() {
        typeof(Exception),
        typeof(RuntimeWrappedException),
        typeof(EventHandler),
        typeof(IntPtr),
        typeof(UIntPtr),
        typeof(Delegate),
        typeof(Thread),
        typeof(Stream),
        typeof(CancellationToken),
        typeof(Mutex),
        typeof(Semaphore),
        typeof(Process),
        typeof(GCHandle),
        typeof(WeakReference),
        typeof(RuntimeTypeHandle),
        typeof(Task),
        typeof(Task<>)
    };

    public override bool CanConvert(Type type) => Blacklisted.Contains(type);

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        => new BlacklistConverter(type);

    class BlacklistConverter : JsonConverter<object>
    {
        Type _type;
        public BlacklistConverter(Type type) => _type = type;

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value != null)
                Log.Debug($"{_type.Name} is missing a [JsonIgnore], skipped by framework.");

            writer.WriteNullValue();
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => null;
    }
}
