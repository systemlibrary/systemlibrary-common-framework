using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App.Tests;

[TestClass]
public partial class AppSettingsTests
{
    [TestMethod]
    public void Read_LogLevel_Success_With_Invalid_Does_Not_Throw()
    {
        // NOTE: Set Level to "Critical" which is not yet part of the Enum LogLevel, which throws without this custom converter
        Read_LogLevel_Success();
    }

    [TestMethod]
    public void Read_LogLevel_Success()
    {
        var enumType = typeof(Enum);

        var converters = TypeDescriptor.GetConverter(enumType);

        if (!(converters is GlobalEnumConverter))
        {
            TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(GlobalEnumConverter)));
        }

        var log = GetSettings(false, "systemLibraryCommonFramework", "log");

        var logProps = log.GetType().GetProperties();
        string logValue = null;
        foreach (var prop in logProps)
        {
            if (prop.Name == "Level")
                logValue = prop.GetValue(log)?.ToString();
        }

        var logging = GetSettings(false, "Logging", "LogLevel");

        var loggingProps = logging.GetType().GetProperties();
        var loggingValue = "";
        foreach (var prop in loggingProps)
        {
            if (prop.Name == "Default")
                loggingValue = prop.GetValue(logging)?.ToString();
        }

        Assert.IsTrue(logValue == null || logValue == "Information" || logValue == "Error", "Level is not Information nor Error " + logValue);
        Assert.IsTrue(loggingValue == "Debug" || loggingValue == "None", "logging value is not Debug or None " + loggingValue);

        if (logValue == null)
        {
            var def = loggingValue;
            if (def.Is())
            {
                logValue = def;
            }
            else
            {
                logValue = "Information";
            }
        }
        var minValue = logValue.ToEnum<LogLevel>();

        if (logValue == "Error")
        {
            Assert.IsTrue(minValue == LogLevel.Error, "Expected error: Value is " + logValue);
        }
        else
        {
            Assert.IsTrue(minValue == LogLevel.Information, "Expected inf: Value is " + logValue);
        }
    }

    [TestMethod]
    public void Read_Client_Configurations()
    {
        var clientConfigurations = GetSettings(true, "systemLibraryCommonFramework", "client");

        Assert.IsTrue(clientConfigurations != null, "Client is null");

        var clientProperties = clientConfigurations.GetType().GetProperties();

        Assert.IsTrue(clientProperties.Count() >= 6, "Too few props in clientProps");
        var count = 0;
        foreach (var property in clientProperties)
        {
            var value = property.GetValue(clientConfigurations)?.ToString();
            if (property.Name.ToLower() == "timeout")
            {
                count++;
                Assert.IsTrue(value == "8686", "timeout is not 8686: " + value);
            }

            if (property.Name.ToLower() == "clientcacheduration")
            {
                count++;
                Assert.IsTrue(value == "1200", "clientcacheduration is not 1200: " + value);
            }

            if (property.Name.ToLower() == "retrytimeout")
            {
                count++;
                Assert.IsTrue(value == "5500", "retrytimeout is not 5500: " + value);
            }

            if (property.Name.ToLower() == "ignoresslerrors")
            {
                count++;
                Assert.IsTrue(value == "True", "ignoreSslErrors is: " + value);
            }

            if (property.Name.ToLower() == "userequestbreakerpolicy")
            {
                count++;
                Assert.IsTrue(value == "True", "userequestbreakerpolicy is: " + value);
            }

            if (property.Name.ToLower() == "throwonunsuccessful")
            {
                count++;
                Assert.IsTrue(value == "True", "throwonunsuccessful is: " + value);
            }

            if (property.Name.ToLower() == "useretrypolicy")
            {
                count++;
                Assert.IsTrue(value == "True", "useretrypolicy is: " + value);
            }
        }
        Assert.IsTrue(count == 7, "Too few properties found for clientConfig: " + count);

        var cacheConfig = GetSettings(true, "systemLibraryCommonFramework", "cache");

        Assert.IsTrue(cacheConfig != null, "cacheConfig is null");

        var cacheProperties = cacheConfig.GetType().GetProperties();
        count = 0;
        foreach (var property in cacheProperties)
        {
            var value = property.GetValue(cacheConfig)?.ToString();
            if (property.Name.ToLower() == "duration")
            {
                count++;
                Assert.IsTrue(value == "5", "Duration is not 5: " + value);
            }
            if (property.Name.ToLower() == "fallbackduration")
            {
                count++;
                Assert.IsTrue(value == "3", "fallbackduration is not 3: " + value);
            }
        }
        Assert.IsTrue(count == 2, "Too few properties found for cacheConfig: " + count);


        var logging = GetSettings(false, "Logging", "LogLevel");

        Assert.IsTrue(logging != null, "LogLevel is null");

        var logProperties = logging.GetType().GetProperties();
        count = 0;
        foreach (var property in logProperties)
        {
            var value = property.GetValue(logging)?.ToString();

            if (property.Name.ToLower() == "default")
            {
                count++;
                Assert.IsTrue(value == "Debug" || value == "None", "Default is not 'Debug', it is " + value);
            }
        }
        Assert.IsTrue(count == 1, "Too few properties found for logConfig: " + count);
    }

    static object GetAppSetting(bool isInWeb, string appSettingPropertyName)
    {
        var appSettings = GetAppSettingsConfig(isInWeb);

        var setting = GetPropertyFromSetting(appSettings, appSettingPropertyName);

        return setting.GetValue(appSettings);
    }

    static object GetSettings(bool isInWeb, string appSettingPropertyName, string nestedPropertyName)
    {
        var appSetting = GetAppSetting(isInWeb, appSettingPropertyName);

        if(nestedPropertyName != null)
        {
            var setting = GetPropertyFromSetting(appSetting, nestedPropertyName);

            appSetting = setting.GetValue(appSetting);
        }

        return appSetting;
    }

    static PropertyInfo GetPropertyFromSetting(object setting, string propertyName)
    {
        var properties = setting.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty);

        return 
            properties
            .Where(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();
    }

    static object GetAppSettingsConfig(bool isInWeb)
    {
        var typeName = "SystemLibrary.Common.Framework.App.AppSettings, SystemLibrary.Common.Framework.App";
        if(!isInWeb)
            typeName = "SystemLibrary.Common.Framework.AppSettings, SystemLibrary.Common.Framework.Net";

        var type = Type.GetType(typeName);

        if (type == null) throw new Exception(typeName + " do not exist, typo?");

        var config = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "Current")
            .FirstOrDefault()
            .GetValue(null);

        if (config == null)
            throw new Exception("appSettings class has been moved or renamed, could not be found in Type: " + typeName);

        return config;
    }
}
