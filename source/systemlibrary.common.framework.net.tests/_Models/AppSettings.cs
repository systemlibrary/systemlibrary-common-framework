namespace SystemLibrary.Common.Framework.Configs;

public class AppSettings : Config<AppSettings>
{
    public string PascalCase { get; set; }
    public string lowercase { get; set; }
    public string UPPERCASE { get; set; }
    public string camelCase { get; set; }
    public bool boolCase { get; set; }
    public int intCase { get; set; }
    public string unsetCase { get; }
    public string unsetCase2;
    string unsetCase3;
    public BackgroundColor BackgroundColor1 { get;set;}
    public BackgroundColor BackgroundColor2 { get; set; }
    public BackgroundColor BackgroundColor3;
    public BackgroundColor BackgroundColor4 { get; set; }

    public string USERNAME { get; set; }
    public string Username { get; set; }
    public string username { get; set; }
    public string UserName { get; set; }
    public string userName { get; set; }

    public ChildAppSettings Child { get; set; }

    public AppSettings()
    {
        Child = new ChildAppSettings();
    }
}

public class ChildAppSettings
{
    public ChildAppSettings()
    {
        GrandChild = new GrandChildAppSettings();
    }
    public string Color { get; set; }

    public GrandChildAppSettings GrandChild { get; set; }
}

public class GrandChildAppSettings
{
    public string Color { get; set; }
    public GreatGrandChildAppSettings GreatGrandChild { get; set; }

    public GrandChildAppSettings()
    {
        GreatGrandChild = new GreatGrandChildAppSettings();
    }
}
public class GreatGrandChildAppSettings
{
    public string Color { get; set; }
    public BabyAppSettings Baby { get; set; }
    public GreatGrandChildAppSettings()
    {
        Baby = new BabyAppSettings();
    }
}

public class BabyAppSettings
{
    public string Color { get; set; }
    public int number { get; set; }
}