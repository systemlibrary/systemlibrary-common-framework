using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class LogWriterTests : BaseTest
{
    const string DumpFullPath = @"C:\Logs\systemlibrary-common-framework-tests.log";

    static string ReadFile()
    {
        Thread.Sleep(50);
        try
        {
            return File.ReadAllText(DumpFullPath);
        }
        catch
        {
            Thread.Sleep(25);
            return File.ReadAllText(DumpFullPath);
        }
    }

    static void CleanDumpFile()
    {
        if (File.Exists(DumpFullPath))
        {
            Thread.Sleep(5);
            File.Delete(DumpFullPath);
            Thread.Sleep(5);
        }
    }

    [TestMethod]
    public void Write_Multiple_Objects_Successfully()
    {
        CleanDumpFile();

        Log.Write("Error param 1", "Error param 2", "Error param 3", true, false, 12345);

        var content = ReadFile();

        Assert.IsTrue(content.Contains(12345.ToString()), "missing int");
        Assert.IsTrue(content.Contains(true.ToString()), "missing true");
        Assert.IsTrue(content.Contains(false.ToString()), "missing false");
        Assert.IsTrue(content.Contains("Error param 3"), "missing param 3");
    }

    [TestMethod]
    public void Dump_Does_Not_Output_Log_Level()
    {
        CleanDumpFile();

        Log.Dump("Err");
     
        var content = ReadFile();

        Assert.IsTrue(content.Contains("Err"), "Does not contain 'Err': " + content);
        Assert.IsTrue(!content.Contains("Error"));
        Assert.IsTrue(!content.Contains("Debug"));
    }

    [TestMethod]
    public void Dump_Does_Discard_Messages_On_Threshold_Reached()
    {
        CleanDumpFile();

        int i = 500000;
        while (i > 0)
        {
            i--;
            Log.Dump("Err");
        }

        var content = ReadFile();

        Assert.IsTrue(content.Contains("log is overflown"));

        Assert.IsTrue(content.Length < 75000, "Content is too large (usually should be around 30K), discarding messages not working? " + content.Length);
    }

    //[TestMethod]
    //public void ILogWriter_Uses_Implementation_When_Writing_Objects_Success()
    //{
    //    var list = new List<string>
    //    {
    //        "hello",
    //        "world",
    //        "!"
    //    };
    //    FrameworkApp.Start();

    //    CleanDumpFile();

    //    Log.Error(list);

    //    var content = ReadFile();

    //    Assert.IsTrue(content.Contains("world"), "world");

    //    Assert.IsTrue(!content.Contains("ILogWriter is not registered"), "ILogWriter is not registered");

    //    var employee = Employee.Create();

    //    Log.Error(employee);

    //    content = ReadFile();

    //    Assert.IsTrue(content.Contains("John"), "Missing John");
    //    Assert.IsTrue(content.Contains(".doe@example.com"), "Missing email");
    //    Assert.IsTrue(content.Contains("Invoice 002"), "Missing second invoice");
    //}

    [TestMethod]
    public void Write_Exception_As_Warning()
    {
        CleanDumpFile();

        Log.Warning(new Exception("Hello world"));

        var content = ReadFile();

        Assert.IsTrue(content.Contains("Hello world"), "Missing hello world");
    }

    [TestMethod]
    public void Write_Log_Levels_Success()
    {
        CleanDumpFile();

        Log.Error("11111");
        Log.Warning("22222");
        Log.Debug("33333");
        Log.Information("44444");
        Log.Trace("66666");
        Log.Critical("9999");
        Log.Dump("55555");

        var content = ReadFile();
        Assert.IsTrue(content.Contains("CRITICAL: "), "Critical level missing as prefix");
        Assert.IsTrue(content.Contains("ERROR: "), "Error: level missing as prefix");
        Assert.IsTrue(content.Contains("DEBUG: "), "Debug: level missing as prefix");
        Assert.IsTrue(content.Contains("WARNING: "), "Warn: level missing as prefix");
        Assert.IsTrue(content.Contains("INFORMATION: "), "Information is missing as prefix");
        Assert.IsTrue(!content.Contains("TRACE: "), "Trace is outputted yet it should be set to Warning (warn debug err and crit are logged)");
    }

    [TestMethod]
    public void Write_Json_To_Log_Without_Conversion_As_Message()
    {
        CleanDumpFile();

        Log.Error("[{ \"name\": \"hello\" }]");
        Log.Error("{ \"name\": \"hello2\" }");
        Log.Error("[{ \"name\": \"hello3\" }]");
        Log.Error("{ \"name\": \"hello4\" }");
        Log.Error("hello5");
        Log.Error("hello6");

        var content = ReadFile();

        Assert.IsTrue(content.Contains("hello3"));
        Assert.IsTrue(content.Contains("hello5"));
        Assert.IsTrue(content.Contains("hello6"));
    }
}
