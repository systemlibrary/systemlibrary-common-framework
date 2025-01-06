using SystemLibrary.Common.Framework.Attributes;

partial class Log
{
    public enum LogLevel
    {
        Trace = 0,

        Information = 1,

        Debug,

        Warning,

        [EnumValue("Critical")]
        Error,

        None = 999
    }
}