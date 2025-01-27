partial class Log
{
    public static void Dump(object obj)
    {
        var message = Build(obj);

        AppendSkippedObject(message, obj);

        PrefixDateTime(message);

        message.Append("\n");

        AddMessageToQueue(message.ToString());
        //SafeWrite(message.ToString());
    }
}