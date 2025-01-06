namespace SystemLibrary.Common.Framework;

partial class Assemblies
{
    static byte[] ReadEmbeddedResourceAsBytes(string relativeName, System.Reflection.Assembly asm)
    {
        var embeddedResourceFullName = GetManifestResourceName(relativeName, asm);

        if (embeddedResourceFullName == null)
            throw new Exception("Embedded resource was not found " + relativeName + " in assembly " + asm.GetName()?.Name + ". Make sure it is compiled with 'embedded resource' as 'Build Action'");

        using (Stream stream = asm.GetManifestResourceStream(embeddedResourceFullName))
        {
            if (stream == null) return null;

            if (stream.Length > int.MaxValue)
                throw new Exception("The embedded resource is too large, max bytes supported is: " + int.MaxValue);

            var bytes = new byte[stream.Length];

            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }
    }

    static string ReadEmbeddedResourceAsString(string relativeEmbeddedResourceName, System.Reflection.Assembly asm)
    {
        var embeddedResourceFullName = GetManifestResourceName(relativeEmbeddedResourceName, asm);

        if (embeddedResourceFullName == null)
            throw new Exception("Embedded resource was not found " + relativeEmbeddedResourceName + " in assembly " + asm.GetName()?.Name + ". Make sure it is compiled with 'embedded resource' as 'Build Action'");

        using (Stream stream = asm.GetManifestResourceStream(embeddedResourceFullName))
        {
            if (stream == null) return null;

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}