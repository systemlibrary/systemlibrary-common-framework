using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    public void Get_Multiple_Async_Cached_Clients_Success()
    {
        var bin = new HttpBin();

        var dict = new ConcurrentDictionary<string, string>();

        var tasks = new List<Task>();
        var tasks2 = new List<Task>();

        void Add(ClientResponse<string> response, int i)
        {
            dict.TryAdd(i + "", response.Data);
        }

        tasks.Add(Task.Run(() => Add(bin.Get(), 1)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 2)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 3)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 4)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 5)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 6)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 7)));
        tasks.Add(Task.Run(() => Add(bin.Get(), 8)));

        var task = Task.WhenAll(tasks.ToArray());

        task.ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(dict.Count == 8, "Dictionary does not contain 8 responses: " + dict.Count);

        System.Threading.Thread.Sleep(250);

        tasks2.Add(Task.Run(() => Add(bin.Get(), 9)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 10)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 11)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 12)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 13)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 14)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 15)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 16)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 17)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 18)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 19)));
        tasks2.Add(Task.Run(() => Add(bin.Get(), 20)));

        var task2 = Task.WhenAll(tasks2.ToArray());

        task2.ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Assert.IsTrue(dict.Count == 20, "Dictionary does not contain 20 responses: " + dict.Count);
    }
}
