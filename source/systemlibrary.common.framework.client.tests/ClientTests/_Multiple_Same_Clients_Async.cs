using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

partial class ClientTests
{
    [TestMethod]
    [DataRow(8)]
    public void Short_Running_Multiple_Same_Clients_Cached_Async_Success(int repeat)
    {
        Run_Multi_Requests_Async(repeat, new HttpBin(), 0, 0);
    }

    [TestMethod]
    [DataRow(33)]
    public void Short_Running_Multiple_Diff_Clients_Cached_Async_Success(int repeat)
    {
        // This creates multiple HttpClients (against same domain, but they are new wherever timeout and ssl differs)
        Run_Multi_Requests_Async(repeat, new HttpBin(true), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(false), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(true, 6565), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 6565, 2);

        Run_Multi_Requests_Async(repeat, new HttpBin(false), 10000, 2);
    }

    [TestMethod]
    [DataRow(3)]
    public void Long_Running_Multiple_No_Clients_Cached_Async_Success(int repeat)
    {
        Run_Multi_Requests_Async(repeat, new HttpBin(true), 0, 0);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 500, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 7000, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 1250, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 11900, 3);

        Run_Multi_Requests_Async(repeat, new HttpBin(true), 33000, 3);
    }


    [TestMethod]
    public void Get_Anonymous_ContentType_Auto_Sends_Json_Returns_Ok()
    {
        var url = "https://httpbin.org/anything/payload";

        var payload = new
        {
            firstName = "Hello 1",
            lastName = "World 2"
        };

        var client = new Client(throwOnUnsuccessful: false);

        var response = client.Get<string>(url, payload);

        Assert.IsTrue(response?.Data?.Contains("\"lastName\": \"World 2\"") == true, "Error: response is wrong " + response?.Data);
    }

    [TestMethod]
    public void Post_Anonymous_ContentType_Auto_Sends_Json_Returns_Ok()
    {
        var url = "https://httpbin.org/anything/payload";

        var payload = new
        {
            firstName = "Hello 1",
            lastName = "World 2"
        };

        var client = new Client(throwOnUnsuccessful: false);

        var response = client.Post<string>(url, payload);

        Assert.IsTrue(response?.Data?.Contains("\"lastName\": \"World 2\"") == true, "Error: response is wrong " + response?.Data);
    }

    [TestMethod]
    public void Get_Poco_ContentType_Auto_Sends_Json_Returns_Ok()
    {
        var url = "https://httpbin.org/anything/payload";

        var payload = new Form();
        
        payload.file = "hello.txt";
        payload.hello = "world";

        var client = new Client(throwOnUnsuccessful: false);

        var response = client.Get<string>(url, payload);

        Assert.IsTrue(response?.Data?.Contains("\"data\": \"{\\\"file\\\":\\\"hello.txt\\\",\\\"hello\\\":\\\"world\\\"}\"") == true, "Error: response is wrong " + response?.Data);
    }

    [TestMethod]
    public void Post_Poco_ContentType_Auto_Sends_Json_Returns_Ok()
    {
        var url = "https://httpbin.org/anything/payload";

        var payload = new Form();

        payload.file = "hello.txt";
        payload.hello = "world";

        var client = new Client(throwOnUnsuccessful: false);

        var response = client.Get<string>(url, payload);

        Assert.IsTrue(response?.Data?.Contains("\"data\": \"{\\\"file\\\":\\\"hello.txt\\\",\\\"hello\\\":\\\"world\\\"}\"") == true, "Error: response is wrong " + response?.Data);
    }

    [TestMethod]
    public void Not_Found_Internet_Address()
    {
        var url = "https://notexisting.something.no/whatever/Public/RequestHandlerApi.ashx?queryFirst=1";

        var client = new Client(throwOnUnsuccessful: false);

        var response = client.Post<string>(url, new { world = "hello" }, ContentType.json);

        Assert.IsTrue(response != null, "response is null");
        Assert.IsTrue(response.Data == null, "Data is not null: " + response.Data);
        Assert.IsTrue((int)response.StatusCode > 499, "Status: " + response.StatusCode);
        Assert.IsTrue(response.Message.Is(), "message: " + response.Message);
    }


    [TestMethod]
    public void Not_Found_Internet_Address_Throws()
    {
        var url = "https://notexisting.something.no/whatever/Public/RequestHandlerApi.ashx?queryFirst=1";

        var client = new Client(throwOnUnsuccessful: true);
        try
        {
            var response = client.Post<string>(url, new { world = "hello" }, ContentType.json);

            Assert.IsTrue(false, "Post towards url should throw");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("No such host"), "Wrong exception message: " + ex.Message);
        }
    }

    [TestMethod]
    public void Actively_Refused_Connection_Throws()
    {
        return;
        // NOTE: must find a url that gives "No connection could be made because the target machine actively refused it"
        var url = "https://do.not.exist.com/productsapi/product/items?itemid=1";

        var client = new Client();
        try
        {
            var response = client.Post<string>(url, new { world = "hello" }, ContentType.json);

            Assert.IsTrue(false, "Post towards url should throw");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("No connection could"), "Wrong exception message: " + ex.Message);
        }
    }

    [TestMethod]
    public void Actively_Refused_Connection_Do_Not_Throw()
    {
        return;
        // NOTE: must find a url that gives "No connection could be made because the target machine actively refused it"
        var url = "https://do.not.exist.com/productsapi/product/items?itemid=1";

        var client = new Client(throwOnUnsuccessful: false);
        var response = client.Post<string>(url, new { world = "hello" }, ContentType.json);

        Assert.IsTrue(response.Data == null, "Post should return null data: " + response.Data);
        Assert.IsTrue(response.Response == null, "Post should return null response");
        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadGateway, "Post should return BadGateway");
    }


    //[TestMethod]
    //[DataRow(33)]
    //public void Very_Long_Running_Multiple_Retry_Clients_Cached_Async_Success(int repeat)
    //{
    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 0, 0);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 4500, 3);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 12500, 3);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 119000, 3);

    //    Run_Multi_Requests_Async(repeat, new HttpBin(true), 330000, 3);
    //}

    static object locking = new object();
    static void Run_Multi_Requests_Async(int repeat, HttpBin bin, int longSleep, int sleepIncremental)
    {
        var tasks = new List<Task>();
        var responses = new List<string>();

        var body = "Hello world";

        if (longSleep > 0)
            Thread.Sleep(longSleep);

        for (int i = 0; i < repeat; i++)
        {
            var sleep = i * sleepIncremental;
            tasks.Add(Task.Run(() =>
            {
                if (sleep > 0)
                    Thread.Sleep(sleep);

                var data = bin.PostAsync(body).Result;

                lock (locking)
                {
                    responses.Add("HttpStatusCode " + (int)data?.StatusCode + ", " + data?.Data);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        var temp = responses.ToArray().Where(x => !x.Contains("200")).ToArray();
        var err = string.Join(" ", temp);

        Assert.IsTrue(responses.Count == repeat, "After " + longSleep + " with incremenatal sleep of " + sleepIncremental + " one or more requests were not complete/added, expected " + repeat + " results, but got only " + responses.Count + ": " + err);

        foreach (var result in responses)
        {
            Assert.IsTrue(result.Contains("HttpStatusCode 200"), "One or more inital requests did not return status code 200");
            Assert.IsTrue(result.Contains(body), "Response did not include body message: " + body);
        }
    }
}
