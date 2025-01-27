partial class Log
{
    public static void Clear()
    {
        try
        {
            File.Delete(FullFilePath);
        }
        catch
        {
        }
    }
}