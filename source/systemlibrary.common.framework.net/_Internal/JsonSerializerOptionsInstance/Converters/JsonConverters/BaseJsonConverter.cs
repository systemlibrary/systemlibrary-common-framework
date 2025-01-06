using System.Text.Json;
using System.Text.Json.Serialization;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal abstract class BaseJsonConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return !typeToConvert.IsArray && !typeToConvert.IsEnum && !typeToConvert.IsGenericType;
    }

    protected object GetValue(ref Utf8JsonReader reader, Type typeToConvert)
    {
        if (reader.TokenType == JsonTokenType.Null) return typeToConvert.Default();

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (typeToConvert == SystemType.Int16Type && reader.TryGetInt16(out short s))
                return s;

            if (typeToConvert == SystemType.IntType && reader.TryGetInt32(out int i))
                return i;

            if (typeToConvert == SystemType.Int64Type && reader.TryGetInt64(out long l))
                return l;

            if (typeToConvert == SystemType.UIntType && reader.TryGetUInt32(out uint ui))
                return ui;

            if (typeToConvert == SystemType.UInt64Type && reader.TryGetUInt64(out ulong ul))
                return ul;

            if (reader.TryGetDouble(out double d))
                return d;

            return reader.GetString();
        }

        if (reader.TokenType == JsonTokenType.True)
        {
            if (typeToConvert == SystemType.StringType) return true.ToString();
            if (typeToConvert == SystemType.IntType) return 1;
            if (typeToConvert == SystemType.Int64Type) return 1;

            return true;
        }

        if (reader.TokenType == JsonTokenType.False) return typeToConvert.Default();

        try
        {
            return reader.GetBytesFromBase64();
        }
        catch
        {
            return reader.GetString();
        }
    }

    protected object GetDeValued(string devalued, Type typeToConvert)
    {
        if (devalued.IsNot()) return typeToConvert.Default();

        if (typeToConvert == SystemType.StringType)
            return devalued;

        if (typeToConvert == SystemType.Int16Type)
            return short.Parse(devalued);
        if (typeToConvert == SystemType.IntType)
            return int.Parse(devalued);
        if (typeToConvert == SystemType.Int64Type)
            return long.Parse(devalued);
        if (typeToConvert == SystemType.BoolType)
            return bool.Parse(devalued);

        if (typeToConvert == SystemType.DoubleType)
            return double.Parse(devalued);

        if (typeToConvert == SystemType.DateTimeType)
            return devalued.ToDateTime();

        if (typeToConvert == SystemType.DateTimeOffsetType)
            return devalued.ToDateTimeOffset();

        throw new Exception("Cannot convert " + devalued + " to " + typeToConvert.Name);
    }

}
