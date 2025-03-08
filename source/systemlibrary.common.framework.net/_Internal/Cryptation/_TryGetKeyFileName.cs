namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetKeyFileName()
    {
        var keyFile = FindFrameworkKeyFile();

        if (keyFile.Is()) Debug.Log("Found framework key file in " + FrameworkKeyDirectory);

        return Path.GetFileName(keyFile);
    }

    static string FindFrameworkKeyFile()
    {
        if (FrameworkKeyDirectory.IsNot()) return null;

        if (FrameworkKeyDirectory.Contains(".xml"))
        {
            throw new Exception("FrameworkKeyDirectory that you've set contains an extension: " + FrameworkKeyDirectory + ". Please specify a directory path, for instance: C:/temp or /temp");
        }

        try
        {
            var fileNames = Directory.GetFiles(FrameworkKeyDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            if (fileNames == null || fileNames.Length == 0) return null;

            // Preserve and use the oldest key file always, so one can easily Decrypt old values to get the text, then delete old key file to encrypt again with latest if needed
            if (fileNames.Length > 1)
            {
                fileNames = fileNames
                    .OrderBy(file => file.Length)
                    .ThenBy(file =>
                    {
                        var creationTime = File.GetCreationTime(file);
                        return creationTime == DateTime.MinValue
                            ? File.GetLastWriteTime(file)
                            : creationTime;
                    })
                    .ToArray();
            }

            foreach (var fullFileName in fileNames)
            {
                var validated = ValidateFileName(fullFileName);

                if (validated != null) return validated.Replace("framework-key-", "");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return null;
    }

    static string ValidateFileName(string fullFileName)
    {
        if (fullFileName.Length <= 20) return null;

        if (!fullFileName.Contains("framework-key-")) return null;

        return fullFileName;
    }
}
