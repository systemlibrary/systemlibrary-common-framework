using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class LogWriterTests : BaseTest
{
    const string DumpFullPath = @"C:\Logs\systemlibrary-common-framework-tests.log";

    static string ReadFile()
    {
        Thread.Sleep(100);
        try
        {
            return File.ReadAllText(DumpFullPath);
        }
        catch
        {
            Thread.Sleep(33);
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
        var content = File.ReadAllText(DumpFullPath);
        Assert.IsTrue(content.Contains(12345.ToString()), "missing int");
        Assert.IsTrue(content.Contains(true.ToString()), "missing true");
        Assert.IsTrue(content.Contains(false.ToString()), "missing false");
        Assert.IsTrue(content.Contains("Error param 3"), "missing param 3");
    }

    [TestMethod]
    public void Write_Does_Not_Output_LogLevel_Success()
    {
        CleanDumpFile();

        int i = 1500000;
        while (i > 0)
        {
            i--;
            Log.Dump("Err");
        }

        var content = ReadFile();

        Assert.IsTrue(content.Contains("Err"), "Does not contain 'Err': " + content);
        Assert.IsTrue(!content.Contains("Error"));
        Assert.IsTrue(!content.Contains("Debug"));
    }

    [TestMethod]
    public void Register_ILogWriter_Uses_Implementation_Whe_Writing_Objects_Success()
    {
        //    CleanDumpFile();

        var list = new List<string>
        {
            "hello",
            "world",
            "!"
        };
        FrameworkApp.Start();

        Log.Error(list);

        //var content = File.ReadAllText(DumpFullPath);

        //Assert.IsTrue(content.Contains("world"), "world");

        //Assert.IsTrue(content.Contains("in LogWriter"), "LogWriter is not used");


        //var employee = Employee.Create();

        //Log.Error(employee);

        //content = File.ReadAllText(DumpFullPath);

        //Assert.IsTrue(content.Contains("John"), "Missing John");
        //Assert.IsTrue(content.Contains(".doe@example.com"), "Missing email");
        //Assert.IsTrue(content.Contains("Invoice 002"), "Missing second invoice");
    }

    [TestMethod]
    public void Write_Exception_As_Warning()
    {
        CleanDumpFile();

        Log.Warning(new Exception("Hello world"));

        var content = File.ReadAllText(DumpFullPath);

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
        Log.Dump("55555");

        var content = File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Error: "), "Error: level missing as prefix");
        Assert.IsTrue(content.Contains("Warning: "), "Warn: level missing as prefix");
        Assert.IsTrue(content.Contains("Debug: "), "Debug: level missing as prefix");
        Assert.IsTrue(content.Contains("Information: "), "Information: level missing as prefix");
        Assert.IsTrue(!content.Contains("Trace: "), "Trace is outputted yet not trace level is set");
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

        var content = File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("hello3"));
        Assert.IsTrue(content.Contains("hello5"));
        Assert.IsTrue(content.Contains("hello6"));
    }
}
