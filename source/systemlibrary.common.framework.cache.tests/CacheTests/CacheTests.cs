using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public class CacheTests
{
    const string CacheKey = "helloworld";

    [TestMethod]
    public void Get_From_Cache_NotExisting_Success()
    {
        var item = Cache.Get<string>(CacheKey + "1");
        Assert.IsTrue(item == null, "Item is not null");
    }

    [TestMethod]
    public void Add_To_Cache_Success()
    {
        var cached = Cache.Get(() =>
        {
            return "Text";
        },
        cacheKey: CacheKey + "2");

        Assert.IsTrue(cached == "Text", "Cache did not return the GetItem() value");

        cached = Cache.Get<string>(CacheKey + "2");

        Assert.IsTrue(cached == "Text", "Cached was null");
    }

    [TestMethod]
    public void Add_To_Cache_Auto_CacheKey_Passing_Object_As_Field0_Adds_FieldAndPropValues_To_Key()
    {
        var input = new CacheKeyParams();

        input.Name = "TestPerson";
        input.Age = 87878;
        input.Year = "2000-12-24".ToDateTime();

        CacheKeyParams.Phone = "9004400044";

        var result = Cache.Get(() =>
        {
            return input.Name + input.Age + input.Year + input.Flag;
        });

        var cacheKey = Cache.PrevCacheKey;

        Assert.IsTrue(cacheKey.Contains("TestPerson".GetCompressedKey()), "Name");
        Assert.IsTrue(cacheKey.Contains("9004400044".GetCompressedKey()), "Phone");
        Assert.IsTrue(cacheKey.Contains("Street 1000".GetCompressedKey()), "Address");
        Assert.IsTrue(cacheKey.Contains("001224000000"), "Year");
        Assert.IsTrue(cacheKey.Contains("87878"), "Age");
        Assert.IsTrue(cacheKey.Contains("0a9536ad3cf4a0d"), "Guid");

        var cached = Cache.Get<string>(cacheKey);

        Assert.IsTrue(cached.Is(), "Not in cache " + cacheKey);
        Assert.IsTrue(cached.Contains("TestPerson") && cached.Contains("87878"), "Invalid text");

        cached = Cache.Get<string>(cacheKey);
        cached = Cache.Get<string>(cacheKey);
        cached = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cached.Contains("TestPerson") && cached.Contains("87878"), "Invalid text");
    }

    [TestMethod]
    public void Add_To_Cache_Auto_CacheKey_Passing_Function_As_GetItem_Success()
    {
        var getItems = () => GetText("Hello", 101, true);

        var cached = Cache.Get(GetItemsFromFunction);
        Assert.IsTrue(cached.Is());

        cached = Cache.Get(GetItemsFromFunction);
        Assert.IsTrue(cached.Is(), "No cached item");
        Assert.IsTrue(cached.Contains("55") && cached.Contains("World"), "Err 1: " + cached);

        var cacheKey = "SLF%SystemLibrary.Common.Framework.App.TestsGetItemsFromFunctionCacheTestsString";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is(), "Not found");
        Assert.IsTrue(cached.Contains("55") && cached.Contains("World"), "Err 2: " + cached);
    }

    static string GetItemsFromFunction()
    {
        string a = "World";
        int b = 55;
        bool c = true;

        return GetText(a, b, c);
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Function_Without_Outside_Vars_Success()
    {
        var getItems = () => GetText("Hello", 123, true);

        var cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Is());

        cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Is());
        Assert.IsTrue(cached.Contains("123"));

        var cacheKey = "SLF%SystemLibrary.Common.Framework.App.Tests<Auto_Create_CacheKey_By_Passing_Function_Without_Outside_Vars_Success>b__6_0<>cString";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is());
        Assert.IsTrue(cachedItem.Contains("123"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success()
    {
        var cached = Cache.Get(() => GetText("Hello", 111, true));
        Assert.IsTrue(cached.Is(), "It is not  hello");

        var cacheKey = "SLF%SystemLibrary.Common.Framework.App.Tests<Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success>b__7_0<>cString";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is(), "Does not exist 1");
        Assert.IsTrue(cachedItem.Contains("111"));

        var cached2 = Cache.Get(() => GetText("Hello", 222, true));
        var cacheKey2 = "SLF%SystemLibrary.Common.Framework.App.Tests<Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success>b__7_1<>cString";
        var cachedItem2 = Cache.Get<string>(cacheKey2);
        Assert.IsTrue(cachedItem2.Is(), "Does not exist 2");
        Assert.IsTrue(cachedItem2.Contains("222"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Inline_Lambda_With_Outside_Vars_Vars_Are_Part_Of_CacheKey_Success()
    {
        string a = "Hello";
        int b = 333;
        bool c = true;
        var cached = Cache.Get(() =>
        {
            return GetText(a, b, c);
        });
        Assert.IsTrue(cached.Is());

        var cacheKey = Cache.PrevCacheKey;
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem?.Contains(b.ToString()) == true, "B not in value " + cachedItem);
        Assert.IsTrue(cachedItem?.Contains(c.ToString()) == true, "C not in value " + cachedItem);
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Function_With_Outside_Vars_Success()
    {
        var a = "Hello";
        var b = 555;
        var c = true;

        var getItems = () => GetText(a, b, c);

        var cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Contains("555"));

        cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Contains("555"));

        var cacheKey = Cache.PrevCacheKey;
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Contains("555"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Dictionary_Success()
    {
        var a = new Dictionary<string, string>();

        a.Add("Hello", "World");

        var getItems = () => GetText(a["Hello"].ToString());

        var cached = Cache.Get(getItems);

        var cacheKey = Cache.PrevCacheKey;
        var item = Cache.Get<string>(cacheKey);

        Assert.IsTrue(item != null, "Wrong cachekey");
        Assert.IsTrue(item.Contains("World99"));
    }

    [TestMethod]
    public void Upsert_Lock_Tests()
    {
        var a = 0;
        var b = 0;
        if (Cache.Lock((CacheDuration)1))
        {
            a++;
        }
        if (Cache.Lock((CacheDuration)1))
        {
            a++;
        }
        if (Cache.Lock())
        {
            b++;
            a++;
        }
        if (Cache.Lock())
        {
            a++;
        }
        if (Cache.Lock())
        {
            a++;
            b++;
        }

        if (Cache.Lock((CacheDuration)1))
        {
            a++;
        }

        // Duration param is part of cache key hence A is 2
        Assert.IsTrue(a == 2 && b == 1, "A: " + a + ", B: " + b);

        System.Threading.Thread.Sleep(1100);
        if (Cache.Lock((CacheDuration)1))
        {
            a++;
        }

        Assert.IsTrue(a == 3 && b == 1, "A: " + a + ", B: " + b);

        if (Cache.Lock((CacheDuration)1))
        {
            a++;
        }
        Assert.IsTrue(a == 3 && b == 1, "A: " + a + ", B: " + b);
    }

    static string GetText(string a, int b = 99, bool c = false)
    {
        return a + b + c + "";
    }

    public class AutoCacheKeyLongParams
    {
        public StringBuilder StringBuilder { get; set; }
        public string LongString;
    }

    public class CacheKeyParams
    {
        public CacheKeyParams()
        {
            Flag = true;
            LastName = "Mr Smith";
            MiddleName = "None";
            TS = TimeSpan.FromSeconds(7);
        }

        static CacheKeyParams()
        {
            Address = "Street 1000";
        }

        public string Name { get; set; }
        public bool Flag { get; }
        public int Age;
        public DateTime Year { get; set; }
        string LastName;
        string MiddleName { get; set; }

        public static string Address { get; }
        public static string Phone;
        public Guid Guid = Guid.Parse("e0a9536ad3cf4a0d91af438ffb3b8f7a");
        public TimeSpan TS { get; set; }
        public bool Flag2 = true;
    }

    [TestMethod]
    public void TryGet_From_Cache_Throws_Returns_Default()
    {
        var item = Cache.TryGet<string>(CacheKey + "try-get-1", () => throw new Exception("OK"));

        Assert.IsTrue(item == null, "Item is not null");
    }

    [TestMethod]
    public void TryGet_From_Cache_DoesNotThrow_Returns_Default()
    {
        var item = Cache.TryGet<string>(CacheKey + "try-get-2", () => "Hello world");

        Assert.IsTrue(item == "Hello world", "Item is not null");
    }

    [TestMethod]
    public void TryGet_MultipleTimes_From_Cache_DoesNotThrow_Returns_Default()
    {
        string item;
        for (int i = 0; i < 100; i++)
        {
            item = Cache.TryGet<string>(CacheKey + "try-get-3", () => "Hello world");

            Assert.IsTrue(item == "Hello world", "Item is not null");
        }

        item = Cache.Get<string>(CacheKey + "try-get-3", () => "Hello world");
        Assert.IsTrue(item == "Hello world");

        item = Cache.Get<string>(CacheKey + "try-get-3");
        Assert.IsTrue(item == "Hello world");
    }

    [TestMethod]
    public void Try_Add_Multiple_And_Clear_Cache_Afterwards()
    {
        for (int i = 0; i < 500; i++)
        {
            var item = "hello" + i;

            Cache.Set(item, item);
        }

        for (int i = 0; i < 500; i++)
        {
            var item = "hello" + i;

            var cached = Cache.Get<string>(item);

            Assert.IsTrue(item == cached, "Wrong " + item + " VS " + cached);
        }

        Thread.Sleep(2600);

        for (int i = 0; i < 500; i++)
        {
            var item = "hello" + i;

            var cached = Cache.Get<string>(item);

            Assert.IsTrue(item == cached, "Wrong after sleep: " + item + " VS " + cached);
        }

        Cache.Clear();

        for (int i = 0; i < 500; i++)
        {
            var item = "hello" + i;

            var cached = Cache.Get<string>(item);

            Assert.IsFalse(item == cached);
        }
    }

    [TestMethod]
    public void Get_From_Fallback_Success()
    {
        var k = CacheKey + "Fallback1";

        var item = Cache.Get<string>(k);
        Assert.IsTrue(item == null);

        Cache.Set(k, "Cache", (CacheDuration)1);

        Thread.Sleep(75);
        item = Cache.Get<string>(k);
        Assert.IsTrue(item == "Cache", item);

        Thread.Sleep(1000);
        item = Cache.Get<string>(k);
        Assert.IsTrue(item == null, "Not expired " + item);

        item = Cache.Get<string>(k, () => "Cache2", duration: (CacheDuration)1);
        Assert.IsTrue(item == "Cache2", "Cache2? " + (item == null) + " or blank? " + (item == ""));

        Thread.Sleep(100);
        item = Cache.Get<string>(k, () => "Do not return me", duration: (CacheDuration)1);
        Assert.IsTrue(item == "Cache2", "Cache2, second:" + item);

        Thread.Sleep(100);
        item = Cache.Get<string>(k, () => "Do not return me", duration: (CacheDuration)1);
        Assert.IsTrue(item == "Cache2", "Cache2, third:" + item);

        Thread.Sleep(975);
        item = Cache.Get<string>(k, () => "Cache3", duration: (CacheDuration)2);
        Assert.IsTrue(item == "Cache3", "Cache3: " + item);

        Thread.Sleep(333);
        item = Cache.Get<string>(k, () => "Do not return me");
        Assert.IsTrue(item == "Cache3", "Cache3 wrong: " + item);

        Thread.Sleep(1500);
        item = Cache.Get<string>(k, () => "Do not return me");
        Assert.IsTrue(item == "Cache3", "Cache3 wrong: " + item);

        Thread.Sleep(50);
        item = Cache.Get<string>(k, () => throw new Exception("Throw first, will hit fallback cache"));
        Assert.IsTrue(item == "Cache3", "Fallback not hit: " + item);

        Thread.Sleep(250);
        item = Cache.Get<string>(k, () => throw new Exception("Throw again after expiration, will still hit fallback cache"));
        Assert.IsTrue(item == "Cache3", "Fallback not hit: " + item);

        try
        {
            Thread.Sleep(3100);

            item = Cache.Get<string>(k, () => throw new Exception("Throw as fallback has expired too"));

            Assert.IsTrue(false, "Should throw exception as both caches have expired");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Throw as fallback has expired too"), ex.Message);
        }

        item = Cache.Get<string>(k, () => "Cache4", (CacheDuration)1);
        Assert.IsTrue(item == "Cache4", "Cache4 wrong: " + item);
        Thread.Sleep(100);
        item = Cache.Get<string>(k, () => "Do not return me", (CacheDuration)1);
        Assert.IsTrue(item == "Cache4", "Cache4 wrong: " + item);
        Thread.Sleep(100);
        item = Cache.Get<string>(k, () => "Do not return me", (CacheDuration)1);
        Assert.IsTrue(item == "Cache4", "Cache4 wrong: " + item);

        Thread.Sleep(1000);
        item = Cache.Get<string>(k, () => null, (CacheDuration)(1));
        Assert.IsTrue(item == "Cache4", "Cache4 did not hit fallback on null data: " + item);

        Thread.Sleep(3100);
        item = Cache.Get<string>(k, () => null, (CacheDuration)(250));
        Assert.IsTrue(item == null, "Cache4 did not return null, data was in cache? " + item);
    }

    [TestMethod]
    public void XAdd_To_Cache_Auto_CacheKey_Long_Urls_Success()
    {
        var url = "https://www.systemlibrary.com/folder1/folder2/folder3/folder4/folder5/folder6/folder7/folder8/folder9/folder10/folder11/folder12/?name=firstName&surname=lastName&phone=1234567890";

        var getItems = () => GetText("Hello" + url, 101, true);

        var cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Is());

        url += "&email=abc@abc.com";

        var cached2 = Cache.Get(getItems);
        Assert.IsTrue(cached2.Is());

        Assert.IsTrue(cached != cached2, "Error: auto gen key failed, the same value was returned when URLs should be added as a whole");
    }


    [TestMethod]
    public void ZTry_Set_Same_Cache_Key_Index_Is_Equal()
    {
        for (int i = 0; i < 500; i++)
        {
            var item = "hello1";

            Cache.Set(item, item);
        }

        for (int i = 0; i < 500; i++)
        {
            var item = "hello1";

            var cached = Cache.Get<string>(item);

            Assert.IsTrue(cached != null, "Wrong " + i);
        }
    }

    [TestMethod]
    public void Auto_Create_Cache_Key_Long_String_Long_StringBuilder_Success()
    {
        var data = new AutoCacheKeyLongParams();

        // 1152 chars
        data.LongString = "zzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAA";
        data.StringBuilder = new StringBuilder(data.LongString + " STRINGBUILDER");
        
        var data2 = new AutoCacheKeyLongParams();

        // 1152 chars
        data2.LongString = "zzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAAzzzzZZZZaaaaAAAA";
        data2.StringBuilder = new StringBuilder(data.LongString + " STRINGBUILDER2");

        var result = Cache.Get(() =>
        {
            return data.StringBuilder + " FROM CACHE";
        });
        var cacheKey = "SLF%SystemLibrary.Common.Framework.App.Tests<Auto_Create_Cache_Key_Long_String_Long_StringBuilder_Success>b__0<>c__DisplayClass22_0String165362a1166169850Z1152165197a1167169850Z1152";
        var cached = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cached.Is(), "Not in cache1 " + cacheKey);
        Assert.IsTrue(cached.Contains(" STRINGBUILDER FROM CACHE"), "Returned not the initial cached version: " + cached);
  
        var result2 = Cache.Get(() =>
        {
            return data2.StringBuilder + " FROM CACHE";
        });

        var cacheKey2 = "SLF%SystemLibrary.Common.Framework.App.Tests<Auto_Create_Cache_Key_Long_String_Long_StringBuilder_Success>b__1<>c__DisplayClass22_0String165362a1166169850Z1152165197a1167169850Z1152";
        var cached2 = Cache.Get<string>(cacheKey2);
        Assert.IsTrue(cached2.Is(), "Not in cache2 " + cacheKey);
        Assert.IsTrue(cached2.Contains(" STRINGBUILDER2 FROM CACHE"), cached2);
    }

    static Func<string> CallOutlinedFunction(AutoCacheKeyLongParams data2)
    {
        return () =>
        {
            return data2.StringBuilder + " FROM CACHE";
        };
    }
}
