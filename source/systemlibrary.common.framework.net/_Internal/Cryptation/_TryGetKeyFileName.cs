namespace SystemLibrary.Common.Framework;

partial class CryptationKey
{
    internal static string TryGetKeyFileName()
    {
        var keyFile = FindFrameworkKeyFile();

        if (keyFile.IsNot())
        {
            keyFile = FindFrameworkKeyFileInContentRootParent();

            if (keyFile.Is())
            {
                Debug.Log("[Encryption] found file a parent folder " + keyFile.MaxLength(6));
            }
            else
            {
                return null;
            }
        }
        else
        {
            Debug.Log("[Encryption] found file in " + CryptationKeyDirectory.Path);
        }

        keyFile = keyFile.Substring(0, keyFile.Length - 4) + ".xml";

        return Path.GetFileName(keyFile);
    }

    static string FindFrameworkKeyFileInContentRootParent()
    {
        var path = EnvironmentConfig.ContentRootPath;

        if (path.IsNot()) return null;

        var parent = new DirectoryInfo(path).Parent;

        int num = 9;

        var keyFile = (string)null;

        while (keyFile == null && num > 0)
        {
            if (parent == null)
            {
                return null;
            }

            num--;

            keyFile = FindKeyFileInPath(parent.FullName);

            if (keyFile.IsNot())
            {
                parent = parent.Parent;
            }
        }

        return keyFile;
    }

    static string FindFrameworkKeyFile()
    {
        var path = CryptationKeyDirectory.Path;

        if (path.IsNot()) return null;

        if (path.IsFile())
        {
            throw new Exception("[Encryption] FrameworkKeyDirectory you've set contains an extension: " + path + ". Please specify a directory path, for instance: C:/temp or /temp and put the frameworkKey.enc file within the folder. Remember the filename contains the key, so we do not want that in code.");
        }

        if (path.StartsWith("./"))
        {
            path = Path.Combine(AppInstance.ContentRootPath, path.Substring(2));

            Debug.Log("[Encryption] Key directory path resolved to: " + path);
        }

        return FindKeyFileInPath(path);
    }

    static string FindKeyFileInPath(string path)
    {
        try
        {
            var fileNames = Directory.GetFiles(path, "*.key", SearchOption.TopDirectoryOnly);

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

                if (validated != null)
                    return validated.Replace("enc-", "", StringComparison.OrdinalIgnoreCase);
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

        if (!fullFileName.Contains("enc-", StringComparison.OrdinalIgnoreCase)) return null;

        return fullFileName;
    }
}
