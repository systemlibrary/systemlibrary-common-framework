partial class Log
{
    public static void Clear()
    {
        if (FullFilePath.IsNot()) return;

        FileInfo logFile = new(FullFilePath);

        if (!logFile.Exists) return;

        try
        {
            File.WriteAllText(FullFilePath, string.Empty);
        }
        catch
        {
            Thread.Sleep(14);
            try
            {
                File.WriteAllText(FullFilePath, string.Empty);
            }
            catch
            {
                Thread.Sleep(24);
                try
                {
                    File.WriteAllText(FullFilePath, string.Empty);
                }
                catch
                {
                }
            }
        }
    }
}