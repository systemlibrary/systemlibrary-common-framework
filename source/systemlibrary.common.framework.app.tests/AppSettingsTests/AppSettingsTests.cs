using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.App;

[TestClass]
public class AppSettingsTests
{
    [TestMethod]
    public void Logging_Level_Read_While_Unset_Variables_Are_Default()
    {
        var config = AppSettings.Current;

        var defaultLogLevel = config.Logging.LogLevel.Default;

        Assert.IsTrue(defaultLogLevel == "None", "Value is " + defaultLogLevel);

        Assert.IsTrue(config.DataProtection != null);
        Assert.IsTrue(config.SystemLibraryCommonFramework != null);
        Assert.IsTrue(config.SystemLibraryCommonFramework.Json != null);
        Assert.IsTrue(config.SystemLibraryCommonFramework.Log != null);
        Assert.IsTrue(config.SystemLibraryCommonFramework.Cache != null);
        Assert.IsTrue(config.SystemLibraryCommonFramework.Client != null);
        Assert.IsTrue(config.SystemLibraryCommonFramework.Client.Timeout == 40001);
    }

    [TestMethod]
    public void Non_Existing_Log_Level_In_Enum_Throws()
    {
        try
        {
            var config = AppSettings.Current.SystemLibraryCommonFramework.Json;
            Assert.IsTrue(false, "Should throw");
        }
        catch
        {
            Assert.IsTrue(true);
        }
    }


    //[TestMethod]
    //public void Read_LogLevel_Success_With_Invalid_Does_Not_Throw()
    //{
    //    // NOTE: Set Level to "Critical" which is not yet part of the Enum LogLevel, which throws without this custom converter
    //    var enumType = typeof(Enum);
    //    var enumTypeConverter = TypeDescriptor.GetConverter(enumType);
    //    if (enumTypeConverter == null || !(enumTypeConverter is ExtendedEnumConverter))
    //    {
    //        TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(ExtendedEnumConverter)));
    //    }

    //    Read_LogLevel_Success();
    //}

    //[TestMethod]
    //public void Read_LogLevel_Success()
    //{
    //    var enumType = typeof(Enum);
    //    var enumTypeConverter = TypeDescriptor.GetConverter(enumType);
    //    if (enumTypeConverter == null || !(enumTypeConverter is ExtendedEnumConverter))
    //    {
    //        TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(ExtendedEnumConverter)));
    //    }

    //    var log = GetAppSettingsConfiguration("Log");

    //    var logProps = log.GetType().GetProperties();
    //    string logValue = null;
    //    foreach (var prop in logProps)
    //    {
    //        if (prop.Name == "Level")
    //            logValue = prop.GetValue(log)?.ToString();
    //    }

    //    var logging = GetAppSettingsConfiguration("LogLevel", "Logging");

    //    var loggingProps = logging.GetType().GetProperties();
    //    var loggingValue = "";
    //    foreach (var prop in loggingProps)
    //    {
    //        if (prop.Name == "Default")
    //            loggingValue = prop.GetValue(logging)?.ToString();
    //    }

    //    Assert.IsTrue(logValue == null || logValue == "Information" || logValue == "Error", "Level is not Information nor Error " + logValue);
    //    Assert.IsTrue(loggingValue == "Debug" || loggingValue == "None", "logging value is not Debug or None " + loggingValue);

    //    if (logValue == null)
    //    {
    //        var def = loggingValue;
    //        if (def.Is())
    //        {
    //            logValue = def;
    //        }
    //        else
    //        {
    //            logValue = "Information";
    //        }
    //    }
    //    var minValue = logValue.ToEnum<LogLevel>();

    //    if (logValue == "Error")
    //    {
    //        Assert.IsTrue(minValue == LogLevel.Error, "Expected error: Value is " + logValue);
    //    }
    //    else
    //    {
    //        Assert.IsTrue(minValue == LogLevel.Information, "Expected inf: Value is " + logValue);
    //    }
    //}

    //[TestMethod]
    //public void Read_Client_Configurations()
    //{
    //    var clientConfigurations = GetAppSettingsConfiguration("Client");

    //    Assert.IsTrue(clientConfigurations != null, "Client is null");

    //    var clientProperties = clientConfigurations.GetType().GetProperties();

    //    Assert.IsTrue(clientProperties.Count() >= 6, "Too few props in clientProps");
    //    var count = 0;
    //    foreach (var property in clientProperties)
    //    {
    //        var value = property.GetValue(clientConfigurations)?.ToString();
    //        if (property.Name.ToLower() == "timeout")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "8686", "timeout is not 8686: " + value);
    //        }

    //        if (property.Name.ToLower() == "clientcacheduration")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "1200", "clientcacheduration is not 1200: " + value);
    //        }

    //        if (property.Name.ToLower() == "retrytimeout")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "5500", "retrytimeout is not 5500: " + value);
    //        }

    //        if (property.Name.ToLower() == "ignoresslerrors")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "True", "ignoreSslErrors is: " + value);
    //        }

    //        if (property.Name.ToLower() == "userequestbreakerpolicy")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "True", "userequestbreakerpolicy is: " + value);
    //        }

    //        if (property.Name.ToLower() == "throwonunsuccessful")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "True", "throwonunsuccessful is: " + value);
    //        }

    //        if (property.Name.ToLower() == "useretrypolicy")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "True", "useretrypolicy is: " + value);
    //        }
    //    }
    //    Assert.IsTrue(count == 7, "Too few properties found for clientConfig: " + count);

    //    var cacheConfig = GetAppSettingsConfiguration("cache");

    //    Assert.IsTrue(cacheConfig != null, "cacheConfig is null");

    //    var cacheProperties = cacheConfig.GetType().GetProperties();
    //    count = 0;
    //    foreach (var property in cacheProperties)
    //    {
    //        var value = property.GetValue(cacheConfig)?.ToString();
    //        if (property.Name.ToLower() == "duration")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "5", "Duration is not 5: " + value);
    //        }
    //        if (property.Name.ToLower() == "fallbackduration")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "3", "fallbackduration is not 3: " + value);
    //        }
    //    }
    //    Assert.IsTrue(count == 2, "Too few properties found for cacheConfig: " + count);

    //    var logging = GetAppSettingsConfiguration("LogLevel", "Logging");

    //    Assert.IsTrue(logging != null, "LogLevel is null");

    //    var logProperties = logging.GetType().GetProperties();
    //    count = 0;
    //    foreach (var property in logProperties)
    //    {
    //        var value = property.GetValue(logging)?.ToString();

    //        if (property.Name.ToLower() == "default")
    //        {
    //            count++;
    //            Assert.IsTrue(value == "Debug" || value == "None", "Default is not 'Debug', it is " + value);
    //        }
    //    }
    //    Assert.IsTrue(count == 1, "Too few properties found for logConfig: " + count);
    //}

    //static object GetAppSettingsConfiguration(string appSettingPropertyName, string nextPropertyName = null)
    //{
    //    var config = GetAppSettingsConfig();

    //    var configProperty = GetAppSettingsConfigPropertyInfo(nextPropertyName);

    //    var configuration = configProperty.GetValue(config);
    //    var jsonProperty = configuration.GetType().GetProperties()
    //        .Where(x => x.Name.ToLower() == appSettingPropertyName.ToLower())
    //        .FirstOrDefault();

    //    return jsonProperty.GetValue(configuration);
    //}

    //static PropertyInfo GetAppSettingsConfigPropertyInfo(string prop)
    //{
    //    object config = GetAppSettingsConfig();

    //    if (prop != null)
    //        return config.GetType()
    //       .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty)
    //       .Where(x => x.Name == prop)
    //       .FirstOrDefault();

    //    return config.GetType()
    //       .GetProperties()
    //       .Where(x => x.Name == "SystemLibraryCommonWeb")
    //       .FirstOrDefault();
    //}

    //static object GetAppSettingsConfig()
    //{
    //    var type = Type.GetType("SystemLibrary.Common.Web.AppSettings, SystemLibrary.Common.Web");

    //    var config = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)
    //        .Where(x => x.Name == "Current")
    //        .FirstOrDefault()
    //        .GetValue(null);

    //    return config;
    //}

    //static void ValidateProperties(PropertyInfo[] properties, string errorMessage, string propertyName)
    //{
    //    if (properties == null || properties.Length == 0)
    //        throw new Exception(errorMessage);

    //    var names = properties.Select(x => x.Name.ToLower()).ToArray();

    //    if (!names.Has(propertyName.ToLower()))
    //        throw new Exception(errorMessage + " Does not contain property: " + propertyName + ". Contains: " + string.Join(" ", names));
    //}
}
