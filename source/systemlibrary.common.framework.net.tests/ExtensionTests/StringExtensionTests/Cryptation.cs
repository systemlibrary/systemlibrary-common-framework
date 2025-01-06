using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

partial class StringExtensionsTests : BaseTest
{
    [TestMethod]
    public void Cryptation_With_Custom_Key_Ring_File_Even_AppName_Is_Set_Success()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCommonServices()
            .AddDataProtection()
            .SetApplicationName("CustomAppNameUsedAsKey")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100));

        var emptyProvider = serviceCollection.BuildServiceProvider();

        var fileName = "key-13F7D4C1-E781-4824-8270-0BE22A226220.xml";
        var fileContent = "key encryption";
        var dir = AppInstance.ContentRootPath;

        if (File.Exists(dir + "\\" + fileName))
        {
            File.Delete(dir + "\\" + fileName);
            Thread.Sleep(10);
        }
        File.AppendAllText(dir + "\\" + fileName, fileContent);

        CryptationKey._Key = null;

        ServiceProviderInstance.Instance = emptyProvider;

        var data = "Hello world";

        var enc = data.Encrypt();
        var dec = enc.Decrypt();

        if (File.Exists(dir + "\\" + fileName))
            File.Delete(dir + "\\" + fileName);

        var prev = "1hGSKl2xIGoboY7NmkctiEZCS52o6+C2MeGTBe5YAYQ=";

        Assert.IsTrue(prev.Length == enc.Length && enc.EndsWith("="), "Enc with default 32 char key changed: " + enc);
        Assert.IsTrue(dec == data, "Decrypt has changed: " + dec);
    }

    [TestMethod]
    public void Cryptation_With_DataProtection_AppName_Success()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCommonServices()
            .AddDataProtection()
            .SetApplicationName("CustomAppNameUsedAsKey")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100));

        var emptyProvider = serviceCollection.BuildServiceProvider();

        CryptationKey._Key = null;

        ServiceProviderInstance.Instance = emptyProvider;

        var data = "Hello world";

        var enc = data.Encrypt();

        var prev = "1hGSKl2xIGoboY7NmkctiEZCS52o6+C2MeGTBe5YAYQ=";

        Assert.IsTrue(prev.Length == enc.Length && enc.EndsWith("="), "Enc with default 32 char key changed: " + enc);

        var dec = enc.Decrypt();

        Assert.IsTrue(dec == data, "Decrypt has changed: " + dec);
    }

    [TestMethod]
    public void Cryptation_Built_In_Key_Iv_Success()
    {
        var data = "Hello world";

        var enc = data.Encrypt();

        var prev = "1hGSKl2xIGoboY7NmkctiEZCS52o6+C2MeGTBe5YAYQ=";

        Assert.IsTrue(prev.Length == enc.Length && enc.EndsWith("="), "Enc with default 32 char key changed: " + enc);

        var dec = enc.Decrypt();

        Assert.IsTrue(dec == data, "Decrypt has changed: " + dec);
    }

    [TestMethod]
    public void Cryptation_With_Diff_Key_IV_Multiple_Invocations_Success()
    {
        var data = Assemblies.GetEmbeddedResource("_Files/employee.json") + "#¤%&,._<>?!|"; ;
        var key = "aaaaa.CCCDDeeeFF-GGGG.HHHiiiii";
        var iv = "11223344556677";

        for (var i = 10; i < 100; i++)
        {
            var tempKey = key + i;
            var tempIv = iv + i;
            var encrypted = data.Encrypt(tempKey.GetBytes(), tempIv.GetBytes());

            var decrypted = encrypted.Decrypt(tempKey.GetBytes(), tempIv.GetBytes());

            Assert.IsTrue(data == decrypted, "Index failed " + i);
        }

        key = "aaaaa.CCCDDeee";
        iv = "11223344556677";

        for (var i = 10; i < 100; i++)
        {
            var tempKey = key + i;
            var tempIv = iv + i;
            var encrypted = data.Encrypt(tempKey.GetBytes(), tempIv.GetBytes());

            var decrypted = encrypted.Decrypt(tempKey.GetBytes(), tempIv.GetBytes());

            Assert.IsTrue(data == decrypted, "Index failed " + i);
        }
    }

    [TestMethod]
    public void Cryptation_Built_In_Key_IV_With_Null_Empty_Short_And_SpecialCharacters_Success()
    {
        string data = null;
        string result = data;

        Assert.IsTrue(result == data.Encrypt().Decrypt(), "Data null failed");

        data = "";
        result = data;
        Assert.IsTrue(result == data.Encrypt().Decrypt(), "Blank: " + data.Encrypt());

        data = "abcdef";
        result = data;
        Assert.IsTrue(result == data.Encrypt().Decrypt(), "abcdef: " + data.Encrypt());

        data = "@£$$€{[]}abcdefghijklmno \n\n\n\t\tpqr stuvwxyzæø åABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ^^*'?=)(/&%¤#\"!|`1234567890 <>;:,.-_ /*-+";
        result = data;
        Assert.IsTrue(result == data.Encrypt().Decrypt(), "long: " + data.Encrypt());
    }

    [TestMethod]
    public void Cryptation_Custom_Key_IV_Added_Returns_Success()
    {
        string data = "Hello";
        string result = data;
        var key = "1234567890123456";

        Assert.IsTrue(result == data.Encrypt(key).Decrypt(key), "Data null failed");

        var enc1 = data.Encrypt(key, addIV: true);
        var enc2 = data.Encrypt(key, addIV: true);
        var enc3 = data.Encrypt(key, addIV: true);

        Assert.IsFalse(enc1 == enc2 && enc2 == enc3, "Is deterministic with null as IV, it shouldve used random IV");

        var dec1 = enc1.Decrypt(key, addedIV: true);
        var dec2 = enc2.Decrypt(key, addedIV: true);
        var dec3 = enc3.Decrypt(key, addedIV: true);

        Assert.IsTrue(dec1 == dec2 && dec2 == dec3 && dec3 == data, "Error: " + dec1);

        var iv = "1234567890123456";

        // ENCRYPT WITH KEY,IV, but ADD IV to output, just need Key to decrypt properly
        var enc4 = data.Encrypt(key, iv, addIV: true);
        var enc5 = data.Encrypt(key, iv, addIV: true);
        var enc6 = data.Encrypt(key, iv, addIV: true);

        Assert.IsTrue(enc4 == enc5 && enc5 == enc6, "Is NOT deterministic with null as IV, it shouldve used random IV");

        var dec4 = enc4.Decrypt(key, addedIV: true);
        var dec5 = enc5.Decrypt(key, addedIV: true);
        var dec6 = enc6.Decrypt(key, addedIV: true);

        Assert.IsTrue(dec4 == dec5 && dec5 == dec6 && dec6 == data, "Error: " + dec1);

        // ENCRYPT WITH KEY,IV, not adding IV to output
        var enc7 = data.Encrypt(key, iv);
        var enc8 = data.Encrypt(key, iv);
        var enc9 = data.Encrypt(key, iv);

        Assert.IsTrue(enc7 == enc8 && enc8 == enc9, "Is NOT deterministic with null as IV, it shouldve used random IV");

        var dec7 = enc7.Decrypt(key, iv);
        var dec8 = enc8.Decrypt(key, iv);
        var dec9 = enc9.Decrypt(key, iv);

        Assert.IsTrue(dec7 == dec8 && dec8 == dec9 && dec9 == data, "Error: " + dec1);
    }

    [TestMethod]
    public void Cryptation_Hardcoded_Key_As_Bytes_IV_Added_Returns_Success()
    {
        string data = null;
        string result = data;

        var key = "ABCDEF1234567890".GetBytes();

        Assert.IsTrue(result == data.Encrypt(key).Decrypt(key), "Data null failed");

        data = "";
        result = data;
        Assert.IsTrue(result == data.Encrypt(key).Decrypt(key), "Blank: " + data.Encrypt(key));

        data = "abcdef";
        result = data;
        Assert.IsTrue(result == data.Encrypt(key).Decrypt(key), "abcdef: " + data.Encrypt(key) + " VS " + data.Encrypt(key).Decrypt(key));

        data = "@£$$€{[]}abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ^^*'?=)(/&%¤#\"!|`1234567890 <>;:,.-_ /*-+";
        result = data;
        Assert.IsTrue(result == data.Encrypt(key).Decrypt(key), "long: " + data.Encrypt(key));
    }

    [TestMethod]
    public void Cryptation_Key_And_IV_Is_16_Length_Returns_Success()
    {
        string data = null;
        string result = data;

        var salt16 = "ABCDEF1234567890";

        var key = "1234567890123456";

        Assert.IsTrue(result == data.Encrypt(key, salt16).Decrypt(key, salt16), "Data null failed");

        data = "";
        result = data;
        Assert.IsTrue(result == data.Encrypt(key, salt16).Decrypt(key, salt16), "Blank: " + data.Encrypt(salt16));

        data = "abcdef";
        result = data;
        Assert.IsTrue(result == data.Encrypt(key, salt16).Decrypt(key, salt16), "abcdef: " + data.Encrypt(salt16));

        data = "@£$$€{[]}abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ^^*'?=)(/&%¤#\"!|`1234567890 <>;:,.-_ /*-+";
        result = data;
        Assert.IsTrue(result == data.Encrypt(key, salt16).Decrypt(key, salt16), "long: " + data.Encrypt(salt16));
    }


    [TestMethod]
    public void Cryptation_Built_In_Key_IV_Multiple_Invocations_Success()
    {
        for (var i = 0; i < 2000; i++)
        {
            var value = "abcdef";

            var encrypted = value.Encrypt();

            var result = encrypted.Decrypt();

            Assert.IsTrue(encrypted.Is() && encrypted != value);
            Assert.IsTrue(result == value);
        }
    }

    static int Decrypt_In_Async_Startup_Success_Counter = 0;
    static void Decrypt_In_Async_Startup_Success_Counter_Increment()
    {
        Decrypt_In_Async_Startup_Success_Counter++;
    }

    [TestMethod]
    public void Cryptation_Use_Built_In_Key_ABC_And_IV_In_Async_Invocations_Success()
    {
        var serviceCollection = new ServiceCollection();

        var emptyProvider = serviceCollection.AddCommonServices().BuildServiceProvider();

        CryptationKey._Key = null;

        ServiceProviderInstance.Instance = emptyProvider;

        var data = "1hGSKl2xIGoboY7NmkctiEZCS52o6+C2MeGTBe5YAYQ=";
        var result = data.Decrypt();

        var r = new Random(DateTime.Now.Millisecond);

        Async.FireAndForget(() => Call(0));
        Async.FireAndForget(() => Call(0));
        Async.FireAndForget(() => Call(0));
        Async.FireAndForget(() => Call(0));
        Async.FireAndForget(() => Call(0));

        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));
        Async.FireAndForget(() => Call(r.Next(1, 125)));

        System.Threading.Thread.Sleep(333);

        Assert.IsTrue(Decrypt_In_Async_Startup_Success_Counter == 0, "Exception counter was: " + Decrypt_In_Async_Startup_Success_Counter);
    }

    static void Call(int sleep)
    {
        Thread.Sleep(sleep);
        try
        {
            var data = "1hGSKl2xIGoboY7NmkctiEZCS52o6+C2MeGTBe5YAYQ=";
            var result = data.Decrypt();

            if (result != "Hello world")
                Decrypt_In_Async_Startup_Success_Counter_Increment();
        }
        catch (Exception ex)
        {
            Dump.Write(ex.Message + " Encrypted data was: " + "Hello world".Encrypt());

            Decrypt_In_Async_Startup_Success_Counter_Increment();
        }
    }
}