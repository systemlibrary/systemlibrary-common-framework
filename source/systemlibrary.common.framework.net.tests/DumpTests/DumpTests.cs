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

using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

public record Person(string Name, int Age);

[TestClass]
public class DumpTests : BaseTest
{

    const string DumpPath = "C:\\Logs\\systemlibrary-common-framework-tests.log";
    [TestMethod]
    public void Tests()
    {
        Action<int> myAction = (x) => Console.WriteLine(x);
        var myTuple = (42, "Hello", (3.14, DateTime.Now), (true, Guid.NewGuid()));
        Span<byte> byteSpan = new Span<byte>(new byte[10]);
        Span<int> intSpan = new Span<int>(new int[10]);
        ReadOnlyMemory<int> readOnlyMemory = new ReadOnlyMemory<int>(new int[10]);
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
        ReadOnlyCollection<int> readOnlyList = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3 });
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
            readOnlyList,
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
            immutableList
        };

        foreach (var o in testObjects)
        {
       //     Dump.Write(o);
        }

        Dump.Write(Employee.CreateList());
    }

    [TestMethod]
    public void Write_Exceptions_Prints_Inner_Exceptions_Too()
    {
        var e1 = new Exception("Hello world1");
        var e2 = new Exception("Hello world2", e1);
        var e3 = new Exception("Hello world3", e2);

        Dump.Clear();
        Dump.Write(e3);

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content.Contains("Hello world3"), "Missing 3");
        Assert.IsTrue(content.Contains("Hello world2"), "Missing 2");
        Assert.IsTrue(content.Contains("Hello world1"), "Missing 1");
    }

    [TestMethod]
    public void Write_Employees_With_Invoice_List()
    {
        var employees = Employee.CreateList();

        Dump.Clear();

        Dump.Write(employees);

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content.Contains("John1"), "Missing John1");
        Assert.IsTrue(content.Contains("John2"), "Missing John2");

        Assert.IsTrue(content.Contains("12:00:00"), "Missing datetime");
        Assert.IsTrue(content.Contains("Invoice 00"), "Missing Invoice N");
        Assert.IsTrue(content.Contains("123456789"), "Missing bankAccountNumber");
        Assert.IsTrue(content.Contains("123456789"), "Missing bankAccountNumber");
        Assert.IsTrue(content.Contains("123-456"), "Missing phone number");
    }

    [TestMethod]
    public void Write_String_Array()
    {
        var arr = new string[] { "Hello", "World" };

        Dump.Clear();

        Dump.Write(arr);

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content.Contains("Hello"), "Missing Hello");
        Assert.IsTrue(content.Contains("World"), "Missing World");
    }

    [TestMethod]
    public void Write_Int_Array()
    {
        var arr = new int[] { 1234, 5678 };

        Dump.Clear();

        Dump.Write(arr);

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content.Contains("1234"), "Missing 1234");
        Assert.IsTrue(content.Contains("5678"), "Missing 5678");
    }

    [TestMethod]
    public void Write_Various_Types()
    {
        var sb = new StringBuilder("Hello World1");

        Dump.Clear();
        Dump.Write(false);
        Dump.Write(1);
        Dump.Write(222222);
        Dump.Write(true);
        Dump.Write("Hello World2");
        Dump.Write("Hello World3\n with multiple lines\n\n\n \t\tlast line");
        Dump.Write("Hello World4");
        Dump.Write(sb);
        Dump.Write(DateTime.Now);
        Dump.Write(BackgroundColor.Blue);

        var content = File.ReadAllText(DumpPath);

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
        Dump.Clear();

        Dump.Write(typeof(string));
        Dump.Write(typeof(StringBuilder));
        Dump.Write(typeof(List<int>));
        Dump.Write(typeof(List<>));
        Dump.Write(typeof(int));
        Dump.Write(typeof(Dump));
        Dump.Write(typeof(Config<>));
        Dump.Write(typeof(Async));

        var content = File.ReadAllText(DumpPath);
        Assert.IsTrue(content.Contains("System.String"));
        Assert.IsTrue(content.Contains("Dump"));
        Assert.IsTrue(content.Contains("SystemLibrary.Common.Framework.Async"));
        Assert.IsTrue(content.Contains("IsClass"));
        Assert.IsTrue(content.Contains("IsValueType"));
    }

    [TestMethod]
    public void Write_Clear_Write_Again_Does_Not_Throw()
    {
        Dump.Clear();
        Dump.Clear();
        Dump.Clear();
        Dump.Write("Hello world");
        Dump.Write("Hello world");
        Dump.Clear();
        Dump.Clear();
        Dump.Clear();
        Dump.Write("Hello world");
        Dump.Clear();
        Dump.Clear();
    }

    [TestMethod]
    public void Write_Employee()
    {
        var employee = Employee.Create("Johnny");

        Dump.Clear();
        Dump.Write(employee);

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content.Contains("Johnny"), "Missing Johnny");
    }

    [TestMethod]
    public void Write_Type_Skips_RuntimeType()
    {
        var employees = new List<Employee>();

        var type = employees.GetType();

        Dump.Clear();

        Dump.Write(type);

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content?.Length <= 255, "Length is above threshold " + content?.Length);
    }

    [TestMethod]
    public void Dump_Write_Async()
    {
        Thread.Sleep(250);

        void LocalOverheadWriting(string m)
        {
            Dump.Write(m);
        }

        Dump.Clear();

        Thread.Sleep(10);

        var tasks = new List<Task>();
        for (int i = 0; i < 1000; i++)
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

        var content = File.ReadAllText(DumpPath);

        Assert.IsTrue(content.Length > 10000);

        for (int i = 0; i < 1000; i++)
        {
            Assert.IsTrue(content.Contains("Iteration number " + i), "Missing " + i);
        }
    }
}
