partial class Log
{
    /// <summary>
    /// Clear the content of the dump file
    /// </summary>
    /// <remarks>
    /// It writes just a single string.Empty to the file, discarding all text within it
    /// If it cannot write, it blocks the current thread for a few milliseconds and retries cleaning the file
    /// </remarks>
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
            Thread.Sleep(15);
            try
            {
                File.WriteAllText(FullFilePath, string.Empty);
            }
            catch
            {
                Thread.Sleep(30);
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