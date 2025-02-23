using System.Diagnostics;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// A Clock to measure an actions execution time based on Stopwatch
/// </summary>
public static class Clock
{
    /// <summary>
    /// Measure an actions execution time in milliseconds used
    /// <para>Specify a message to also Log.Debug() the output</para>
    /// </summary>
    /// <example>
    /// <code>
    /// Clock.Measure(() => "hello", "Print hello");
    /// </code>
    /// </example>
    /// <returns>Milliseconds used</returns>
    public static long Measure(Action method, string message = null)
    {
        var sw = new Stopwatch();

        sw.Start();
        sw.Stop();
        sw.Reset();
        sw.Start();

        method();

        sw.Stop();

        if (message != null)
            Log.Debug(message + " executed in total time: " + sw.ElapsedTicks + " ticks, " + sw.ElapsedMilliseconds + "ms");

        var ms = sw.ElapsedMilliseconds;

        sw.Reset();

        return ms;
    }
}
