using System;
using System.IO;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public class IServiceCollectionExtensionsTests
{
    [TestMethod]
    public void Use_Existing_Key_Ring_File_With_Auto_Data_Protection_Policy_True_Gives_Just_Debug_Message()
    {
        IServiceCollection service = new ServiceCollection();

        service.AddDataProtection()
            .DisableAutomaticKeyGeneration()
            .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\"))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
            .SetApplicationName("TestApp");

        var options = new FrameworkServicesOptions();

        options.UseAutomaticDataProtectionPolicy = true;

        service.UseAutomaticDataProtectionPolicy(options);

        service.UseFrameworkServiceProvider();

        var data = "Hello world";
        var enc = data.Encrypt();
        var dec = enc.Decrypt();
        Assert.IsTrue(dec == data, "Wrong decryption");
    }

    [TestMethod]
    public void Use_Existing_Key_Ring_File_With_Auto_Data_Protection_Policy_False_No_Debug_Message()
    {
        IServiceCollection service = new ServiceCollection();

        service.AddDataProtection()
            .DisableAutomaticKeyGeneration()
            .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\"))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000))
            .SetApplicationName("TestApp");

        var options = new FrameworkServicesOptions();

        options.UseAutomaticDataProtectionPolicy = false;

        service.UseAutomaticDataProtectionPolicy(options);

        service.UseFrameworkServiceProvider();

        var data = "Hello world";

        var enc = data.Encrypt();

        var dec = enc.Decrypt();

        Assert.IsTrue(dec == data, "Wrong decryption");
    }

    [TestMethod]
    public void Use_Auto_Generated_App_Name_As_Key()
    {
        IServiceCollection service = new ServiceCollection();

        service.AddDataProtection()
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 1000));

        var options = new FrameworkServicesOptions();

        options.UseAutomaticDataProtectionPolicy = true;

        service.UseAutomaticDataProtectionPolicy(options);

        service.UseFrameworkServiceProvider();

        var data = "Hello world";
        var enc = data.Encrypt();
        var dec = enc.Decrypt();

        Assert.IsTrue(dec == data, "Wrong decryption");
    }
}