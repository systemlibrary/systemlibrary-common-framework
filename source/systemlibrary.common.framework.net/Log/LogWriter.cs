﻿using System.Collections.Concurrent;
using System.Text;

using SystemLibrary.Common.Framework;

partial class Log
{
    internal static ConcurrentQueue<string> Queue = new();

    static Timer Interval;
    static object TimerLock = new();
    static bool QueueTimerStarted = false;
    static int QueueDiscardThreshold = 50;
    static int IntervalTimeMs = 90;

    static void AddMessageToQueue(string message)
    {
        if (Assemblies.IsKestrelMainHost)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
            return;
        }
        else
        {
            Queue.Enqueue(message);
            StartTimer();
        }
    }

    static void StartTimer()
    {
        if (QueueTimerStarted) return;

        lock (TimerLock)
        {
            if (QueueTimerStarted) return;

            if (Interval == null)
                Interval = new Timer(SafeWriteQueue, null, IntervalTimeMs, Timeout.Infinite);
            else
                Interval.Change(IntervalTimeMs, Timeout.Infinite);

            QueueTimerStarted = true;
        }
    }

    static void SafeWriteQueue(object state)
    {
        StringBuilder batch = new();

        try
        {
            int messageCount = 0;

            while (Queue.TryDequeue(out var logMessage))
            {
                batch.Append(logMessage);
                messageCount++;
                if (messageCount >= QueueDiscardThreshold)
                    break;
            }

            if (batch.Length > 0)
                File.AppendAllText(FullFilePath, batch.ToString(), Encoding.UTF8);

            if(messageCount >= QueueDiscardThreshold)
            {
                Queue.Clear();
                try
                {
                    string overflowMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Critical: log is overflown, log queue discarded due to threshold of {QueueDiscardThreshold} messages reached within {IntervalTimeMs}ms.";

                    File.AppendAllText(FullFilePath, overflowMessage, Encoding.UTF8);
                }
                catch
                {
                }
            }
        }
        catch
        {
            Thread.Sleep(IntervalTimeMs);

            try
            {
                if (batch.Length > 0)
                    File.AppendAllText(FullFilePath, batch.ToString(), Encoding.UTF8);
            }
            catch
            {
            }
        }
        finally
        {
            // Passing in "new object" internally to avoid anything in finally during app exit
            if (state == null)
            {
                if (!Queue.IsEmpty)
                {
                    Interval.Change(IntervalTimeMs, Timeout.Infinite);
                }
                else
                {
                    QueueTimerStarted = false;
                }
            }
        }
    }

    static void WriteToRandomGeneratedFile(Exception ex, string batch)
    {
        //var index = FullFilePath.IndexOf('.');

        //var tmp = FullFilePath.Substring(0, index) + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + ".log";

        //try
        //{
        //    File.AppendAllText(tmp, "Error writing to dump file: " + ex.Message + "\nBatch:\n" + batch);
        //}
        //catch
        //{
        //}
    }
}