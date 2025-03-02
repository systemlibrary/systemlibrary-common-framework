using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public class LogWriterTests
{
    const string DumpFullPath = @"C:\Logs\systemlibrary-common-web-unit-tests.txt";

    [TestMethod]
    public void Write_Multiple_Objects()
    {
        System.Threading.Thread.Sleep(5);

        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Assert.IsFalse(System.IO.File.Exists(DumpFullPath));

        Log.Write("Error param 1", "Error param 2", "Error third param!", true, false, 12345);
        var content = System.IO.File.ReadAllText(DumpFullPath);
        Assert.IsTrue(content.Contains(12345.ToString()), "missing int");
        Assert.IsTrue(content.Contains(true.ToString()), "missing true");
        Assert.IsTrue(content.Contains(false.ToString()), "missing false");
        Assert.IsTrue(content.Contains("third param!"), "missing false");
    }

    [TestMethod]
    public void Write_Same_CorrId_InARow()
    {
        System.Threading.Thread.Sleep(20);

        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Assert.IsFalse(System.IO.File.Exists(DumpFullPath));

        Log.Write("Err");
    }


    [TestMethod]
    public void Write_Various_Log_Levels_Without_Registering_ILogWriter_Success()
    {
        System.Threading.Thread.Sleep(100);

        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Assert.IsFalse(System.IO.File.Exists(DumpFullPath));

        Log.Write("Err");
        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Err"), "Does not contain 'Err': " + content);
        Assert.IsTrue(!content.Contains("Error"));
        Assert.IsTrue(!content.Contains("Warn"));
        Assert.IsTrue(!content.Contains("Debug"));
    }

    [TestMethod]
    public void Write_StringList_And_Class_Success()
    {
        System.Threading.Thread.Sleep(200);
        App.Start<ILogWriter, LogWriter>();

        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        var list = new List<string>
        {
            "hello",
            "world",
            "!"
        };

        Log.Error(list);

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("world"), content + " no world");

        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        var car = new Car();

        car.Name = "Ferrari";
        car.Names = new string[] { "Hello1", "Hello2" };
        car.LastNames = new List<string> { "LastName1", "LastName2", "LastName3" };
        car.Ages = new int[] { 4, 5, 6, 7, 100 };
        car.Age = 1000;
        car.Vehicle = new Car() { Name = "Volvo" };

        car.Born = DateTime.Now;
        car.Death = DateTimeOffset.Now;
        car.IsEnabled = true;

        Log.Error(car);

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Ferrari"), "Ferrari");

        var logwriter = Services.Get<ILogWriter>();

        Assert.IsTrue(logwriter != null, "ILogWriter is null from Services()");
    }

    [TestMethod]
    public void Write_Exception_As_Warning()
    {
        System.Threading.Thread.Sleep(400);
        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Log.Error("Custom stacktrace");

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Custom stack"), "Content: " + content);
    }

    [TestMethod]
    public void Write_Various_Log_Levels_Success()
    {
        System.Threading.Thread.Sleep(600);
        Dump.Clear();

        Log.Error("12345");

        var content = System.IO.File.ReadAllText(DumpFullPath);

        try
        {
            Assert.IsTrue(content.Contains("Error: "), "Prefix 'level' which is of type 'Error' was not added as plaintext format, try again with json format...");
        }
        catch
        {
            Assert.IsTrue(content.Contains("\"Error\","), "Prefix 'level' which is of type 'Error' was not added as json either");
        }

        Assert.IsTrue(content.Contains("12345"), "Text 12345 was not logged");

        Dump.Clear();

        Log.Warning("67890");

        content = System.IO.File.ReadAllText(DumpFullPath);

        try
        {
            Assert.IsTrue(content.Contains("Warning: "), "Prefix 'level' which is of type 'Warning' was not added as plaintext format, try again with json format...");
        }
        catch
        {
            Assert.IsTrue(content.Contains("\"Warning\","), "Prefix 'level' which is of type 'Warning' was not added as json either");
        }

        Assert.IsTrue(content.Contains("67890"), "Text 67890 was not logged");

        Dump.Clear();

        Log.Debug("Debugging 101");

        content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Debugging 101"), "Debugging 101");

        Dump.Clear();

        Log.Error("1234");
        Log.Error("456");
        Log.Error("789");

        Log.Trace("abcdef");

        content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("123"));
        Assert.IsTrue(content.Contains("456"));
        Assert.IsTrue(content.Contains("789"));

        Assert.IsTrue(content.Contains("abcdef") == false, "abcdef is logged to file, Trace level is enabled? Or bug?");

        //NOTE: appSettings.json has "Debug" assigned as log level, manually change it to Info to test this
        //Assert.IsTrue(content.Contains("Info: "), "Info: prefix text was not added to message");
        //Assert.IsTrue(content.Contains("abcdef"), "Text abcdef was not logged");

        //Dump.Clear();
    }

    [TestMethod]
    public void Write_Json_To_Log_Without_Conversion_As_Message()
    {
        System.Threading.Thread.Sleep(800);
        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Log.Error("[{ \"name\": \"ferrari\" }]");
        Log.Error("{ \"name\": \"ferrari\" }");
        Log.Error("[{ \"name\": \"ferrari2\" }]");
        Log.Error("{ \"name\": \"ferrari3\" }");
        Log.Error(2);
        Log.Error(2);
        Log.Error(9999);

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("ferrari2"));
        Assert.IsTrue(content.Contains("ferrari3"));
        Assert.IsTrue(content.Contains("9999"));
    }
}
