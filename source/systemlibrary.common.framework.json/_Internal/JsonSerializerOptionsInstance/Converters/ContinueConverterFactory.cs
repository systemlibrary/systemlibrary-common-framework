﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class ContinueConverterFactory : JsonConverter<Enum>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return false;
    }

    public override Enum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

internal class ContinueConverterNext : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => false;
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException(); // This should never be called
}