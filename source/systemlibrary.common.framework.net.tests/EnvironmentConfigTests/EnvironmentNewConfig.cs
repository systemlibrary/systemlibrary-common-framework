namespace SystemLibrary.Common.Framework.Configs;

public class EnvironmentConfig : EnvironmentConfig<EnvironmentConfig, EnvironmentName>
{
    public string NewPropertyValue { get; set; }
    public string NewString { get; set; }
    public bool NewFlag { get; set; }
    public int NewInt { get; set; }
    public DateTime NewDateTime { get; set; }
    public ChildAppSettings Child { get; set; }

    public static bool IsProd => Current.EnvironmentName == EnvironmentName.Prod || Current.EnvironmentName == EnvironmentName.Production;

    public static bool IsTest => Current.EnvironmentName == EnvironmentName.Test ||
        Current.EnvironmentName == EnvironmentName.Stage ||
        Current.EnvironmentName == EnvironmentName.Staging ||
        Current.EnvironmentName == EnvironmentName.QA ||
        Current.EnvironmentName == EnvironmentName.UAT ||
        Current.EnvironmentName == EnvironmentName.Sandbox ||
        Current.EnvironmentName == EnvironmentName.AT;

    public static bool IsLocal => !IsTest && !IsProd;
}
