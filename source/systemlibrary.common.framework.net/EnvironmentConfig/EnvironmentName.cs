using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Enum list of common built-in environment names allowed
/// <para>Config tranformations and EnvironmentName will work with other env-names, but the IsLocal, IsTest, and IsProd will not</para>
/// </summary>
public enum EnvironmentName
{
    [EnumValue("")]
    None,
    AT,
    Dev,
    Development,
    Integration,
    Local,
    PreProd,
    PreProduction,
    Prod,
    Production,
    QA,
    Sandbox,
    Stage,
    Staging,
    Test,
    UAT,
    UnitTest
}
