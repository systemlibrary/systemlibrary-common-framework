namespace SystemLibrary.Common.Framework;

/// <summary>
/// EnvironmentConfig with an option to pass your own Configuration class, along with an enum that defines all environment names.
/// </summary>
public abstract class EnvironmentConfig<T, TEnvironmentNameEnum> : Config<T>
    where T : class
    where TEnvironmentNameEnum : struct, IComparable, IFormattable, IConvertible
{
    TEnvironmentNameEnum? _EnvironmentName;

    /// <summary>
    /// Returns the Environment Name as an Enum
    /// </summary>
    public TEnvironmentNameEnum EnvironmentName
    {
        get
        {
            if (_EnvironmentName == null) return default;

            return _EnvironmentName.Value;
        }
    }

    string _Name;
    /// <summary>
    /// Returns the environment name based on the ASPNETCORE_ENVIRONMENT variable passed during the startup of your application.
    /// <para>This variable name is used for configuration transformations for all C# config classes that inherit from Config&lt;&gt;.</para>
    /// </summary>
    /// <remarks>
    /// <para>Changing the environment name requires a shell restart (e.g., iisreset).</para>
    /// Transformation for the EnvironmentConfig class is run based on the ASPNETCORE_ENVIRONMENT passed. Additionally, a environmentConfig.someEnvName.json file may include a Name that differs from the environment name, in which case this Name in the transformed file will be returned instead.
    /// </remarks>
    /// <example>
    /// Test Explorer
    /// <code class="language-xml hljs">
    /// if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///     then: sets 'temp environment' as value
    ///     
    ///     if: 'temp environment' is set, but no transformation is found
    ///         then: sets 'temp environment' as value from 'Configuration Mode' in Visual Studio
    ///
    /// else:
    ///     then: sets 'temp environment' as value from 'Configuration Mode' in Visual Studio
    /// 
    /// if: environmentConfig.json exists
    ///     if transformation file exists for 'temp environment' 
    ///         then: run transformation for environmentConfig.json
    ///     
    ///     if: environmentConfig.json contains 'name' property
    ///         return: 'value'
    /// 
    /// if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///      return: 'value'
    ///     
    /// if: ASPNETCORE_ENVIRONMENT exists in 'Environment Variables on Windows'
    ///     return: 'value'
    /// 
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// Console Application
    /// <code class="language-xml hljs">
    /// if: environmentConfig.json do not exists:
    ///     if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///         return: 'value'
    ///     
    ///     if: ASPNETCORE_ENVIRONMENT exists in 'Environment Variables on Windows'
    ///         return: 'value'
    ///     
    /// else if: 
    ///     if: environmentConfig has transformation equal to 'configuration' pass in as argument
    ///         then: run transformation
    ///         
    ///     if: environmentConfig has property 'name'
    ///         return: 'value'
    /// 
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// DOTNET TEST 'csproj' --configuration 'release|debug|etc..' command
    /// <code class="language-csharp hljs">
    /// if: environmentConfig.json do not exists:
    ///     if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///         return: 'value'
    ///     
    /// if: ASPNETCORE_ENVIRONMENT exists in 'Environment Variables on Windows'
    ///     return: 'value'
    ///     
    /// else if: 
    ///     if: environmentConfig has transformation equal to 'configuration' pass in as argument
    ///         then: run transformation
    ///         
    ///     if: environmentConfig has property 'name'
    ///         return: 'value'
    /// 
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// IISExpress
    /// <code class="language-xml hljs">
    /// if: launchSettings.json exists
    ///     if: command "IISExpress" exists and contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: 'ASPNETCORE_ENVIRONMENT' exists
    ///             if: environmentConfig.json exists
    ///                 if: transformation file exists for 'value'
    ///                 then: run transformation
    ///                     
    ///                 if: environmentConfig contains 'name' property  
    ///                 return: 'value'
    ///                 
    ///             return: 'value'
    ///                 
    /// if: 'ASPNETCORE_ENVIRONMENT' exists in web.config
    ///     if: environmentConfig.json exists
    ///         if: transformation file exists for 'value'
    ///         then: run transformation
    ///         
    ///         if: environmentConfig contains 'name' property
    ///             return: 'value'
    ///             
    ///     return: 'value'
    ///         
    /// if: 'ASPNETCORE_ENVIRONMENT' exists as a 'environment variable' in Windows
    ///     if: environmentConfig.json exists
    ///         if: transformation file exists for 'value'
    ///         then: run transformation
    ///         
    ///         if: environmentConfig contains 'name' property
    ///             return: 'value'
    ///             
    ///     return: 'value'
    ///     
    /// if: launchSettings.json exists
    ///     if: "iisSettings" contains "iisExpress" and contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: environmentConfig.json exists
    ///             if: transformation file exist for 'value'
    ///             then: run transformation
    ///                 
    ///             if: environmentConfig.json contains 'name' property
    ///                 return: 'value'
    ///                         
    ///         return: 'value'
    ///         
    ///     if: "iisSettings" contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: environmentConfig.json exists
    ///             if: transformation file exists for 'value'
    ///             then: run transformation
    ///                     
    ///             if: environmentConfig.json contains 'name' property
    ///             return: 'value'
    ///                 
    ///         return: 'value'
    ///          
    ///     if: command "IIS" exists and contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: environmentConfig.json exists
    ///             if: transformation file exists for 'value'
    ///             then: run transformation
    ///                 
    ///             if: environmentConfig.json contains 'name' property
    ///                 return: 'value'
    ///                 
    /// if: environmentConfig.json exists
    ///     if: environmentConfig.json contains 'name' property
    ///         return: 'value'
    ///                         
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// IIS
    /// <code class="language-xml hljs">
    /// if: 'ASPNETCORE_ENVIRONMENT' exists in web.config
    ///     if: environmentConfig.json exists
    ///         if: transformation file exists 'value' 
    ///         then: run transformation 
    ///         
    ///         if: environmentConfig.json contains 'name' property
    ///             return: 'value'
    ///         
    ///     return: 'value'
    ///     
    /// if: launchSettings.json exists
    ///     if: "iisSettings" contains "iisExpress"
    ///         if: "iisExpress" contains 'environmentVariables'
    ///             if: 'ASPNETCORE_ENVIRONMENT' exists
    ///                 if: environmentConfig.json exists
    ///                     if: transformation file exist for 'value'
    ///                     then: run transformation
    ///                 
    ///                     if: environmentConfig.json contains 'name' property
    ///                         return: 'value'
    ///                     
    ///     if: "iisSettings" contains 'environmentVariables'
    ///         if: 'ASPNETCORE_ENVIRONMENT' exists
    ///             if: environmentConfig.json exists
    ///                 if: transformation file exist for 'value'
    ///                 then: run transformation
    ///                 
    ///                 if: environmentConfig.json contains 'name' property
    ///                     return: 'value'
    ///                     
    ///     if: "profiles" exists
    ///         if: command "IIS" exists and contains 'environmentVariables'
    ///             if: 'ASPNETCORE_ENVIRONMENT' exists
    ///                 if: environmentConfig.json exists
    ///                     if: trnasformation file exists for 'value'
    ///                     then: transformation is ran
    ///                 
    ///                     if: environmentConfig.json contains 'name' property
    ///                         return: 'value'
    ///                         
    ///                 return: 'value'
    ///                 
    /// return: "" as 'name', never null
    /// </code>
    /// </example>
    public string Name
    {
        get
        {
            if (_Name != null && _Name != "") return _Name;

            _Name = AppInstance.AspNetCoreEnvironment;
            //TODO: Consider throwing new Exception("Environment 'Name' is not set in either 'ASPNETCORE_ENVIRONMENT', or in environmentConfig.json file, or environmentConfig.json file is located in wrong folder");
            //TODO: Consider this way it works, returns empty name, so it never does any transformations
            //TODO: Consider supporting 'environment' in 'appSettings' for the package: systemLibraryCommonFramework { environment { name: '...' } }
            return _Name;
        }
        set
        {
            _Name = value;
            _EnvironmentName = _Name.ToEnum<TEnvironmentNameEnum>();
        }
    }
}

/// <summary>
/// Standard class for environmental configurations read from environmentConfig.json if exists
/// <para>If you've added more properties to environmentConfig.json than just the 'Name' you'd have to inherit 'EnvironmentConfig&lt;YourClass&gt;' and use that instead</para>
/// </summary>
/// <remarks>
/// See the documentation for 'Name' property on class 'EnvironmentConfig&lt;&gt;' for more details regarding transformations
/// </remarks>
public class EnvironmentConfig : EnvironmentConfig<EnvironmentConfig, EnvironmentName>
{
    /// <summary>
    /// Returns true if both IsTest and IsProd are false.
    /// </summary>
    public static readonly bool IsLocal = !IsProd && !IsTest;

    /// <summary>
    /// Returns true if the environment is set to 'prod' or 'production', otherwise false.
    /// </summary>
    public static readonly bool IsProd = Current.EnvironmentName == EnvironmentName.Prod || Current.EnvironmentName == EnvironmentName.Production;

    /// <summary>
    /// Returns true if both IsLocal and IsProd are false
    /// <para>Note: returns true for Test, PreProduction, Sandbox, Stage, QA and more...</para>
    /// </summary>
    public static readonly bool IsTest = Current.EnvironmentName == EnvironmentName.AT ||
        Current.EnvironmentName == EnvironmentName.Integration ||
        Current.EnvironmentName == EnvironmentName.PreProd ||
        Current.EnvironmentName == EnvironmentName.PreProduction ||
        Current.EnvironmentName == EnvironmentName.QA ||
        Current.EnvironmentName == EnvironmentName.Sandbox ||
        Current.EnvironmentName == EnvironmentName.Stage ||
        Current.EnvironmentName == EnvironmentName.Staging ||
        Current.EnvironmentName == EnvironmentName.Test ||
        Current.EnvironmentName == EnvironmentName.UAT ||
        Current.EnvironmentName == EnvironmentName.UnitTest;

    /// <summary>
    /// Returns the application's root folder full path.
    /// <para>Does not end with a slash.</para>
    /// <para>If the folder is 'bin' or inside it, the method will traverse up and return the parent folder of the 'bin' folder. Exceptions is if assembly ends in Tests for test projects, then 'up-traversal out of bin' is skipped</para>
    /// </summary>
    public static readonly string ContentRootPath = AppInstance.ContentRootPath;
}