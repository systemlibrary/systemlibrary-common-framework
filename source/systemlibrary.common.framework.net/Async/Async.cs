using System.Collections.Concurrent;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Run methods asynchronously without blocking.
/// </summary>
public static class Async
{
    /// <summary>
    /// Execute methods asynchronously, appending each result to a list, and halt execution until all functions passed as parameters have completed.
    /// <para>A hardcoded 30-second limit for all methods to complete; if exceeded, some results may be omitted, an error is logged, but the data collected so far is returned.</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class Car {
    ///     public string Name { get; set; }
    /// }
    /// 
    /// class CarApi {
    ///     //Simple dummy method that pretends to return a list of cars based on the name from some API
    ///     List&lt;Car&gt; GetByName(string name) {
    ///         //Client exists in nuget package: SystemLibrary.Common.Framework.App
    ///         return Client.Get&lt;List&lt;Car&gt;&gt;("https://systemlibrary.com/cars/q=?" + name);   
    ///     }
    /// }
    /// 
    /// var carApi = new CarApi();
    /// var cars = Async.Tasks&lt;Car&gt;(
    ///     () => carApi.GetByName("blue"),
    ///     () => carApi.GetByName("red"),
    ///     () => carApi.GetByName("orange")
    /// ); 
    /// 
    /// // Variable 'cars' is filled after all three api requests has completed.
    /// // Assume we got 1 blue, 0 red and 1 orange
    /// // 'cars' now contain a total of 2 objects of type 'Car'
    /// </code>
    /// </example>
    public static List<T> Tasks<T>(params Func<T>[] functions)
    {
        return Tasks(30000, functions);
    }

    /// <summary>
    /// Execute methods in an async manner, appending each single result to a list, and it halts execution till all functions passed as params has completed 
    /// <para>Pass in a specific timeout limit in milliseconds, before the results will be returned 'as is'</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class Car {
    ///     public string Name { get; set; }
    /// }
    /// 
    /// class CarApi {
    ///     //Simple dummy method that pretends to return a list of cars based on the name from some API
    ///     List&lt;Car&gt; GetByName(string name) {
    ///         //Client exists in nuget package: SystemLibrary.Common.Framework.App
    ///         return Client.Get&lt;List&lt;Car&gt;&gt;("https://systemlibrary.com/cars/q=?" + name);   
    ///     }
    /// }
    /// 
    /// var carApi = new CarApi();
    /// var cars = Async.Tasks&lt;Car&gt;(7500,
    ///     () => carApi.GetByName("blue"),
    ///     () => carApi.GetByName("red"),
    ///     () => carApi.GetByName("orange")
    /// ); 
    /// 
    /// // Variable 'cars' is filled after all three api requests has completed.
    /// // Assume we got 1 blue, 0 red and 1 orange
    /// // 'cars' now contain a total of 2 objects of type 'Car'
    /// </code>
    /// </example>
    public static List<T> Tasks<T>(int timeoutMilliseconds, params Func<T>[] functions)
    {
        var results = new ConcurrentBag<T>();

        using var taskCancellation = new CancellationTokenSource();
        using var timeoutCancellation = new CancellationTokenSource();

        var tasks = functions
            .Where(f => f != null)
            .Select(f => Task.Run(() =>
            {
                try
                {
                    var result = f();
                    if (result != null)
                        results.Add(result);
                }
                catch
                {
                    // Swallow
                }
            }, taskCancellation.Token));

        var timeoutTask = Task.Delay(timeoutMilliseconds, timeoutCancellation.Token);

        var task = Task.WhenAll(tasks);

        task.ConfigureAwait(false);

        if (Task.WhenAny(task, timeoutTask).Result == timeoutTask)
        {
            Log.Error("Async.Run timed out after 30s without tasks finishing in time " + results.Count + "/" + tasks.Count());
            taskCancellation.Cancel();
        }
        else
        {
            timeoutCancellation.Cancel();
        }

        return results.ToList();
    }

    /// <summary>
    /// Execute methods in paralell manner where each result is appended to a List
    /// <para>Halts execution till all functions passed have ran till completion</para>
    /// </summary>
    /// <remarks>
    /// Parallel swallows exceptions if the methods passed does throw
    /// <para>Parallel requires the methods pass to always return a data</para>
    /// <para>Null values returned from the methods will be filtered away</para>
    /// <para>Parallel usage is against local file system, file searches and CPU bound calculations</para>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="functions"></param>
    /// <returns></returns>
    public static List<T> Parallel<T>(params Func<T>[] functions)
    {
        var results = new ConcurrentBag<T>();

        ParallelOptions options = new()
        {
            MaxDegreeOfParallelism = 4
        };

        System.Threading.Tasks.Parallel.ForEach(functions.Where(f => f != null), options, (f) =>
        {
            try
            {
                var result = f();

                if (result != null)
                    results.Add(result);
            }
            catch
            {
            }
        });

        return results.ToList();
    }

    /// <summary>
    /// Execute methods in a async manner, appending the range of results per function to the same list, and it halts execution till all functions passed as params has completed 
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class Car {
    ///     public string Name { get; set; }
    /// }
    /// 
    /// class CarApi {
    ///     //Simple dummy method that pretends to return a list of cars based on the name from some API
    ///     List&lt;Car&gt; GetByName(string name) {
    ///         //Client exists in nuget package: SystemLibrary.Common.Framework.App
    ///         return Client.Get&lt;List&lt;Car&gt;&gt;("https://systemlibrary.com/cars/q=?" + name);   
    ///     }
    /// }
    /// 
    /// var carApi = new CarApi();
    /// var cars = Async.Run&lt;Car&gt;(
    ///     () => carApi.GetByName("blue"),
    ///     () => carApi.GetByName("red"),
    ///     () => carApi.GetByName("orange")
    /// ); 
    /// 
    /// // Variable 'cars' is filled after all three api requests has completed.
    /// // Assume we got 2 blue, 3 red and 4 orange
    /// // 'cars' now contain a total of 9 objects of type 'Car'
    /// </code>
    /// </example>
    public static List<T> Run<T>(params Func<IEnumerable<T>>[] functions)
    {
        var results = new ConcurrentBag<T>();

        var tasks = functions
            .Where(f => f != null)
            .Select(f => Task.Run(() =>
            {
                try
                {
                    var result = f();
                    if (result != null)
                        results.Add((T)result);
                }
                catch
                {
                    // Swallow
                }
            }));

        Task.WhenAll(tasks).GetAwaiter().GetResult();

        return results.ToList();
    }

    /// <summary>
    /// Run all actions seperately in a non-blocking way
    /// <para>Each action passed is ran in a try catch without notifying callee</para>
    /// See the overloaded method if you want to ignore exceptions
    /// </summary>
    /// <remarks>
    /// All functions passed to this is ran in an unordered and non-blocking way
    /// <para>All functions passed will run till completion, erroring or till main thread is shut down</para>
    /// </remarks>
    /// <param name="onError">Callback invoked if an exception occured</param>
    /// <param name="actions">Array of methods to invoke in a non-blocking way</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// Async.FireAndForget((ex) => Log.Error(ex), () => System.IO.File.AppenAllText("C:\temp\text.log", "hello world"));
    /// </code>
    /// </example>
    public static void FireAndForget(Action<Exception> onError, params Action[] actions)
    {
        if (actions.IsNot()) return;

        foreach (var action in actions)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (onError != null)
                        onError(ex);
                }
            }
            );
        }
    }

    /// <summary>
    /// Run all actions seperately in a non-blocking way
    /// <para>Each action passed is ran in a try catch without notifying callee</para>
    /// See the overloaded method to add a callback for logging exceptions
    /// </summary>
    /// <remarks>
    /// All functions passed to this is ran in an unordered and non-blocking way
    /// <para>All functions passed will run till completion, erroring or till main thread is shut down</para>
    /// </remarks>
    /// <param name="actions">Array of methods to invoke in a non-blocking way</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// Async.FireAndForget(() => System.IO.File.AppenAllText("C:\temp\text.log", "hello world"));
    /// </code>
    /// </example>
    public static void FireAndForget(params Action[] actions)
    {
        FireAndForget(null, actions);
    }
}
