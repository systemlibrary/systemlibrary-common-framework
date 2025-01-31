using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

public record Person(string Name, int Age);

[TestClass]
public class LogDumpTests : BaseTest
{
    const string DumpFullPath = "C:\\Logs\\systemlibrary-common-framework-tests.log";
    
    static string ReadFile()
    {
        Thread.Sleep(125);
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

    [TestMethod]
    public void Log_Dump_Simple_Strings_And_Null()
    {
        Log.Dump(null);
        Log.Dump("");
        Log.Dump("Hello world");
        Log.Dump("Hello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello worldHello world Hello world Hello world Hello world Hello world");
        Log.Dump("Hello world Hello world Hello world Hello world Hello world");

        var content = ReadFile();

        Assert.IsTrue(content.Is());
    }

    [TestMethod]
    public void Write_Various_Objects_Without_Exceptions()
    {
        Action<int> myAction = (x) => Console.WriteLine(x);
        var myTuple = (42, "Hello", (3.14, DateTime.Now), (true, Guid.NewGuid()));
        Span<byte> byteSpan = new Span<byte>(new byte[10]);
        Span<int> intSpan = new Span<int>([1, 3, 5, 88]);
        ReadOnlyMemory<int> readOnlyMemory = new ReadOnlyMemory<int>([1, 3, 5, 99]);
        int[,] multiDimArray = new int[3, 3];
        var person = new Person("John", 30);
        var myValueTuple = ("Test", 42, 3.14);
        dynamic obj = new ExpandoObject();
        obj.Name = "John";
        obj.Age = 30;
        var myList = new List<Dictionary<string, (int, string)>>()
        {
            new Dictionary<string, (int, string)> { { "key1", (1, "one") } }
        };
        int? nullableInt = 42;
        DateTime? nullableDate = DateTime.Now;
        DateTimeOffset? nullableDto = DateTimeOffset.Now;
        ReadOnlyCollection<int> readOnlyList = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3 });
        ReadOnlyCollection<BackgroundColor> readOnlyListEnum = new ReadOnlyCollection<BackgroundColor>(new List<BackgroundColor> { BackgroundColor.Red, BackgroundColor.Blue, BackgroundColor.Orange, BackgroundColor.Red });
        Point point = new Point { X = 10, Y = 20 };
        Lazy<int> lazyInt = new Lazy<int>(() => 42);
        KeyValuePair<string, int> pair = new KeyValuePair<string, int>("One", 1);
        BitArray bitArray = new BitArray(new bool[] { true, false, true });
        ArraySegment<int> segment = new ArraySegment<int>(new int[] { 1, 2, 3, 4 }, 1, 2);
        StackFrame frame = new StackFrame(1);
        Task<int> task = Task.FromResult(42);
        var ex = new Exception("Test", new ArgumentException("Inner exception"));
        Memory<byte> memory = new Memory<byte>(new byte[10]);
        var taskCompletionSource = new TaskCompletionSource<string>();
        GCHandle handle = GCHandle.Alloc(new object());
        SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        CultureInfo culture = new CultureInfo("en-US");
        var immutableList = ImmutableList<int>.Empty.Add(1).Add(2);
        WeakReference<object> weakRef = new WeakReference<object>(obj);
        Type intType = typeof(int);
        Type genericType = typeof(List<List<string>>);

        var testObjects = new object[]
        {
            weakRef,
            myAction,
            myTuple,
            intSpan.ToArray(),
            byteSpan.ToArray(),
            readOnlyMemory,
            multiDimArray,
            person,
            myValueTuple,
            obj,
            myList,
            nullableInt,
            (DateTime?)null,
            nullableDate,
            nullableDto,
            readOnlyList,
            readOnlyListEnum,
            point,
            lazyInt,
            pair,
            bitArray,
            segment,
            frame,
            task,
            ex,
            memory,
            taskCompletionSource,
            handle,
            semaphore,
            culture,
            immutableList,
            intType,
            genericType
        };

        foreach (var o in testObjects)
        {
            Log.Dump(o);
        }

        Log.Dump(Employee.Create());
        Log.Dump(Employee.Create());
        Log.Dump(Employee.Create());

        var list = Employee.CreateList();
        Log.Dump(list);

        list.AddRange(list);
        Log.Dump(list);

        var content = ReadFile();

        Assert.IsTrue(content.Is());
    }

    [TestMethod]
    public void Write_Exceptions_Prints_Inner_Exceptions_Too()
    {
        var e1 = new Exception("Hello world1");
        var e2 = new Exception("Hello world2", e1);
        var e3 = new Exception("Hello world3", e2);

        Log.Dump(e3);

        Log.Clear();

        Log.Dump(e3);

        var content = ReadFile();

        Assert.IsTrue(content.Contains("Hello world3"), "Missing 3");
        Assert.IsTrue(content.Contains("Hello world2"), "Missing 2");
        Assert.IsTrue(content.Contains("Hello world1"), "Missing 1");
    }

    [TestMethod]
    public void Write_Employees_With_Invoice_List()
    {
        var employees = Employee.CreateList();

        Log.Clear();

        Log.Dump(employees);

        var content = ReadFile();

        Assert.IsTrue(content.Contains("John1"), "Missing John1");
        Assert.IsTrue(content.Contains("John2"), "Missing John2");

        Assert.IsTrue(content.Contains("BirthDate: 1999-01-"), "Missing datetime");
        Assert.IsTrue(content.Contains("Invoice 00"), "Missing Invoice N");
        Assert.IsTrue(content.Contains("123456789"), "Missing bankAccountNumber");
        Assert.IsTrue(content.Contains("123456789"), "Missing bankAccountNumber");
        Assert.IsTrue(content.Contains("123-456"), "Missing phone number");
    }

    [TestMethod]
    public void Write_String_Array()
    {
        var arr = new string[] { "Hello", "World" };

        Log.Clear();

        Log.Dump(arr);

        var content = ReadFile();

        Assert.IsTrue(content.Contains("Hello"), "Missing Hello");
        Assert.IsTrue(content.Contains("World"), "Missing World");
    }

    [TestMethod]
    public void Write_Int_Array()
    {
        var arr = new int[] { 1234, 5678 };

        Log.Clear();

        Log.Dump(arr);

        var content = ReadFile();

        Assert.IsTrue(content.Contains("1234"), "Missing 1234");
        Assert.IsTrue(content.Contains("5678"), "Missing 5678");
    }

    [TestMethod]
    public void Write_Various_Types()
    {
        var sb = new StringBuilder("Hello World1");

        Log.Clear();
        Log.Dump(false);
        Log.Dump(1);
        Log.Dump(222222);
        Log.Dump(true);
        Log.Dump("Hello World2");
        Log.Dump("Hello World3\n with multiple lines\n\n\n \t\tlast line");
        Log.Dump("Hello World4");
        Log.Dump(sb);
        Log.Dump(DateTime.Now);
        Log.Dump(BackgroundColor.Blue);

        var content = ReadFile();

        Assert.IsTrue(content.Contains("False"), "Missing False");
        Assert.IsTrue(content.Contains("True"), "Missing True");
        Assert.IsTrue(content.Contains("222222"), "Missing 222222");
        Assert.IsTrue(content.Contains("Hello World1"), "Missing Hello World1");
        Assert.IsTrue(content.Contains("Hello World2"), "Missing Hello World2");
        Assert.IsTrue(content.Contains("Hello World3"), "Missing Hello World3");
        Assert.IsTrue(content.Contains("Hello World4"), "Missing Hello World4");
        Assert.IsTrue(content.Contains("last line"), "Missing last line");
        Assert.IsTrue(content.Contains("BLUE"), "Missing BLUE");
    }

    [TestMethod]
    public void Write_Types()
    {
        Log.Clear();

        Log.Dump(typeof(string));
        Log.Dump(typeof(StringBuilder));
        Log.Dump(typeof(List<int>));
        Log.Dump(typeof(List<>));
        Log.Dump(typeof(int));
        Log.Dump(typeof(Log));
        Log.Dump(typeof(Config<>));
        Log.Dump(typeof(Async));

        var content = ReadFile();

        Assert.IsTrue(content.Contains("String"));
        Assert.IsTrue(content.Contains("Log"));
        Assert.IsTrue(content.Contains("SystemLibrary.Common.Framework.Async"));
        Assert.IsTrue(content.Contains("IsClass"));
        Assert.IsTrue(content.Contains("IsValueType"));
    }

    [TestMethod]
    public void Write_Clear_Write_Again_Does_Not_Throw()
    {
        Log.Clear();
        Log.Clear();
        Log.Clear();
        Log.Dump("Hello world");
        Log.Dump("Hello world");
        Log.Clear();
        Log.Clear();
        Log.Clear();
        Log.Dump("Hello world");
        Log.Clear();
        Log.Clear();
    }

    [TestMethod]
    public void Write_Employee()
    {
        var employee = Employee.Create("Johnny");

        Log.Clear();

        Log.Dump(employee);

        var content = ReadFile();

        Assert.IsTrue(content.Contains("Johnny"), "Missing Johnny");

        Assert.IsTrue(content.Contains("Skipped as max depth reached"), "Max depth not reached");
    }

    [TestMethod]
    public void Write_Type_Skips_RuntimeType()
    {
        var employees = new List<Employee>();

        var type = employees.GetType();

        Log.Clear();

        Log.Dump(type);

        var content = ReadFile();

        Assert.IsTrue(content?.Length <= 255, "Length is above threshold " + content?.Length);
    }

    [TestMethod]
    public void Log_Dump_Async()
    {
        void LocalOverheadWriting(string m)
        {
            Log.Dump(m);
        }

        Thread.Sleep(333);

        Log.Clear();
        
        var tasks = new List<Task>();

        for (int i = 0; i < 40; i++)
        {
            int tmp = i;
            tasks.Add(Task.Run(() =>
            {
                LocalOverheadWriting("Iteration number " + tmp);
            }));
        }

        var task = Task.WhenAll(tasks.ToArray());

        task.ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        var content = ReadFile();

        for (int i = 0; i < 40; i++)
        {
            Assert.IsTrue(content.Contains("Iteration number " + i), "Missing " + i);
        }
    }
}
