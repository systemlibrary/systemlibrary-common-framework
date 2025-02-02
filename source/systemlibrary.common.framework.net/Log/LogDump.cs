partial class Log
{
    /// <summary>
    /// Bypasses the ILogWriter and dumps the message to file destination of your own choice
    /// <para>Configure output location in the framework settings under "log/fullfilepath" section</para>
    /// </summary>
    /// <remarks>
    /// This uses a timer of 20ms before starting to dump the whole queue of messages within the 20ms threshold
    /// <para>After writing to disc if queue is not empty a 20ms timer is yet again started before the next write</para>
    /// <para>Writes the first 60 messages within 20ms, all other messages are discarded</para>
    /// </remarks>
    /// <param name="obj">Any object</param>
    public static void Dump(object obj)
    {
        var message = Build(obj);

        AppendSkippedObject(message, obj);

        PrefixDateTime(message);

        message.Append("\n");

        AddMessageToQueue(message.ToString());
    }
}