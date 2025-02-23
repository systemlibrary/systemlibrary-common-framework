using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

public enum BackgroundColor
{
    Red = 0,

    [EnumText("GREEN")]
    [EnumValue("GREEN!")]
    Green = 1,

    [EnumText("BLUE")]
    [EnumValue(100)]
    Blue = 2,

    [EnumValue("orange")]
    [EnumText("Orange")]
    Orange = 3,

    yellow = 40,

    _997 = 5,

    [EnumValue(998)]
    _999 = 1000,
}