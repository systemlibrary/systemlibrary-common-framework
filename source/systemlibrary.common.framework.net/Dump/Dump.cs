using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Dump any object to a local file for easy debugging and logging purposes
/// <para>Dump.Write calls should only occur during development as it is slow and not thread safe</para>
/// <para>Has a write lock of 100ms, so thread-safe to some extent, one might see multiple dump files in a real async world</para>
/// </summary>
/// <remarks>
/// "Equivalent" to javascripts 'console.log'
/// </remarks>
public static class Dump
{
    static string LogFullPath;
    static string Folder;
    static bool Initialized;
    static List<int> Visited = new List<int>();

    /// <summary>
    /// Deletes the current log file if exists
    /// </summary>
    public static void Clear()
    {
        if (LogFullPath == null)
        {
            Initialize();
        }

        try
        {
            File.Delete(LogFullPath);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Dump any object to the dump file
    /// </summary>
    /// <remarks>
    /// "Equivalent" to javascripts 'console.log'
    /// </remarks>
    /// <example>
    /// <code class="language-xml hljs">
    /// class Car {
    ///     public string Name {get;set;}
    /// }
    /// var list = new List&lt;Car&gt;();
    /// 
    /// list.Add(new Car { Name = "Vehicle 1" });
    /// list.Add(new Car { Name = "Vehicle 2" });
    /// 
    /// Dump.Write(list);
    /// // Outputs:
    /// // List of Car (1)
    /// //  - Name = Vehicle 1
    /// //  - Name = Vehicle 2
    /// </code>
    /// </example>
    public static void Write(object o)
    {
        try
        {
            Initialize();

            Visited.Clear();

            var logString = new StringBuilder();

            Build(logString, o, 0, 3);

            if (logString.IsNot())
                logString.Append(o.GetType().Name + " skipped");

            logString.Insert(0, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "\t");

            WriteToFileWithDateTime(logString);
        }
        catch (Exception ex)
        {
            try
            {
                File.AppendAllText(Folder + "DumpWrite" + DateTime.Now.Millisecond + ".log", ex.ToString() + "\n");
            }
            catch
            {
                //Swallow infinite loop, in case of:
                //File already opened exception (multi threaded scenario)
                //Write access exception
                //Full disk exception
            }
        }
    }

    internal static object Lock = new object();

    static void Initialize()
    {
        if (Initialized) return;

        lock (Lock)
        {
            if (Initialized) return;

            if (!Directory.Exists(Folder))
            {
                try
                {
                    Directory.CreateDirectory(Folder);
                }
                catch
                {
                }
            }

            LogFullPath = FrameworkConfig.Current.Dump.GetFullLogPath();
            Folder = new FileInfo(LogFullPath).DirectoryName + "\\";

            Initialized = true;
        }
    }

    static string WriteBoolProperty(string n, bool b)
    {
        if (b)
            return ", " + n;
        return "";
    }

    static string WriteType(Type t)
    {
        return t.FullName + " "
            + WriteBoolProperty("IsClass", t.IsClass)
            + WriteBoolProperty("IsInterface", t.IsInterface)
            + WriteBoolProperty("IsEnum", t.IsEnum)
            + WriteBoolProperty("IsValueType", t.IsValueType)
            + WriteBoolProperty("IsAbstract", t.IsAbstract)
            + WriteBoolProperty("IsPrimitive", t.IsPrimitive)
            + WriteBoolProperty("IsArray", t.IsArray)
            + WriteBoolProperty("IsSerializable", t.IsSerializable)
            + WriteBoolProperty("IsAutoClass", t.IsAutoClass)
            + WriteBoolProperty("IsPointer", t.IsPointer)
            + WriteBoolProperty("IsGenericType", t.IsGenericType)
            + WriteBoolProperty("IsGenericParameter", t.IsGenericParameter);
    }

    static void WriteList(StringBuilder logString, Type type, int level, IEnumerable e)
    {
        var arguments = type.GetGenericArguments();
        var genericType = type;
        if (arguments != null && arguments.Length > 0)
            genericType = arguments[0];

        var collectionIncrementTabs = 0;

        if (e is IDictionary d)
            logString.Append(" dictionary count (" + d.Count + ")\n");

        else if (e is Array a)
            logString.Append(genericType.Name + " length (" + a.Length + ")\n");

        else if (e is IList l)
            logString.Append("IList<" + genericType.Name + "> (" + l.Count + ")\n");

        else if (e is ICollection ic)
            logString.Append(" collection count (" + ic.Count + ")\n");
        else if (e.GetType().Name[0] == '<' && e.GetType().Name.Contains("__"))
            logString.Append(" enumerable function count" + "\n");
        else
            logString.Append(" unknown count" + "\n");

        if (e is IDictionary || e is IList || e is Array || e is ICollection)
            collectionIncrementTabs = 2;

        var tabs = GetTabs(level + collectionIncrementTabs);

        logString.Append(tabs);

        if (e is Array)
        {
            if (type == typeof(int[]) ||
                type == typeof(byte[]) ||
                type == typeof(bool[]) ||
                type == typeof(Int64[]))
            {
                foreach (var item in e)
                {
                    logString.Append(item + " ");
                }
                return;
            }
            if (type == typeof(int[,]))
            {
                var aa = e as int[,];

                for (int i = 0; i < aa.GetLength(0); i++)
                {
                    for (int j = 0; j < aa.GetLength(1); j++)
                    {
                        logString.Append(aa[i, j] + " ");
                    }

                    if (i + 1 < aa.GetLength(0))
                        logString.Append("\n" + tabs);
                }
                return;
            }
            if (type == typeof(long[,]))
            {
                var aa = e as long[,];

                for (int i = 0; i < aa.GetLength(0); i++)
                {
                    for (int j = 0; j < aa.GetLength(1); j++)
                    {
                        logString.Append(aa[i, j] + " ");
                    }

                    if (i + 1 < aa.GetLength(0))
                        logString.Append("\n" + tabs);
                }
                return;
            }
        }

        foreach (var item in e)
        {
            Build(logString, item, level, 3);

            var t = item.GetType();
            if (t.IsNativeType() && t != SystemType.StringType)
                logString.Append(" ");
            else
                logString.Append("\n" + tabs);
        }
    }

    static void WriteClass(StringBuilder logString, object value, Type type, int level)
    {
        if (type == SystemType.ExceptionType ||
            type.Name == "NullReferenceException" ||
            type.Name == "RuntimeType" ||
            type.Name == "RuntimeMethodInfo" ||
            type.Name == "ModelBindingMessageProvider" ||
            type.Name == "")
            return;

        if (type.Name == "RuntimeAssembly" ||
            type.Name == "Constructor")
        {
            logString.Append(type.Name + (type.IsClassType() ? " (class, skipped)" : ""));
            return;
        }

        var arguments = type.GetGenericArguments();

        var genericType = (Type)null;

        if (arguments != null && arguments.Length > 0)
            genericType = arguments[0];

        var typeName = type.Name;

        if (genericType != null)
            typeName = typeName + "<" + genericType?.Name + ">";

        if (type.IsInterface)
            logString.Append(typeName + " (interface)");
        else
        {
            if (type.Name == "SafeWaitHandle")
            {
                logString.Append(typeName + " skipped");
                return;
            }
            if (type.Name.Contains("Task`"))
            {
                logString.Append(typeName + " ");
                var t = value as Task;
                logString.Append("Ex: " + t.Exception?.Message + ", Completed: " + t.IsCompleted + ", IsFaulted: " + t.IsFaulted + ", IsCompletedSuccessfully: " + t.IsCompletedSuccessfully + ", Cancelled: " + t.IsCanceled);
                return;
            }
            else if (typeName.Contains("Action`"))
                return;
            else
            {
                logString.Append(typeName + (type.IsClassType() ? " (class)" : ""));
            }
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);

        if (properties != null && properties.Length > 0)
        {
            logString.Append("\n");
            var propL = properties.Length;
            var propI = 0;
            foreach (var property in properties)
            {
                propI++;
                if (!property.CanRead)
                {
                    logString.Append(GetTabs(level) + "\t");
                    logString.Append(property.Name + ": cant read, continuing...\n");
                    continue;
                }
                if (property?.PropertyType == null) continue;
                if (property.PropertyType == SystemType.CharType) continue;
                if (property.PropertyType.Name == "RuntimeType") continue;

                logString.Append(GetTabs(level) + "\t");

                try
                {
                    var propertyValue = property.GetValue(value);

                    AppendVariable(logString, property.PropertyType, property.Name, propertyValue, level + 1);
                }
                catch
                {
                    logString.Append(property.Name + ": could not retrieve value, continuing...\n");
                }

                if (propI < propL)
                    logString.Append("\n");
            }
        }

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);

        if (fields != null && fields.Length > 0)
        {
            if (properties == null || properties.Length == 0)
                logString.Append("\n");

            var fieldsL = fields.Length;
            var fieldsI = 0;
            foreach (var field in fields)
            {
                fieldsI++;

                if (field?.FieldType == null) continue;

                if (field.IsPrivate) continue;

                logString.Append("\t");
                try
                {
                    var fieldValue = field.GetValue(value);

                    AppendVariable(logString, field.FieldType, field.Name, fieldValue, level);
                }
                catch
                {
                    logString.Append(field.Name + ": could not retrieve value, continuing...");
                }

                if (fieldsI < fieldsL)
                    logString.Append("\n");
            }
        }
    }

    static void AppendVariable(StringBuilder logString, Type variableType, string name, object variableValue, int level)
    {
        logString.Append(GetTabs(level) + name + ": ");

        if (variableType.IsClassType() && !IsListType(variableValue))
        {
            Build(logString, variableValue, level + 1, 3);
        }
        else
        {
            Build(logString, variableValue, level, 3);
        }
    }

    internal static void Build(StringBuilder logString, object value, int level, int maxDepth = 3)
    {
        if (level >= maxDepth)
            return;

        if (logString.Length > 30000) return;

        var v = GetVariableValue(value);

        if (v != null)
        {
            logString.Append(v);
        }
        else if (value is Type t)
        {
            logString.Append(WriteType(t));
        }
        else
        {
            var type = value.GetType();

            if (IsListType(value))
            {
                WriteList(logString, type, level, value as IEnumerable);
                return;
            }

            if (type.IsClassType())
            {
                if (type.BaseType != typeof(ValueType))
                {
                    var hash = value.GetHashCode();

                    if (hash > 1)
                    {
                        // Self-referenced objects are added to a 'already written queue' so it is ignored, every other time
                        if (Visited.Contains(hash))
                        {
                            logString.Append("Object already logged, continuing...\n");
                            Visited.Remove(hash);
                            return;
                        }
                        Visited.Add(hash);
                    }
                }

                WriteClass(logString, value, type, level);

                return;
            }

            Append(logString, type, value, level);
        }
    }

    static string GetVariableValue(object value)
    {
        if (value == null)
            return "(null)";

        else if (value is Exception e)
        {
            if (e is AggregateException agg)
            {
                return agg.Flatten().ToString();
            }
            return e.ToString();
        }
        else if (value is string str)
            if (str.Length > 50)
                return str + " (Length: " + str.Length + ")";
            else
                return str;

        else if (value is StringBuilder sb)
            if (sb.Length > 50)
                return sb + " (Length: " + sb.Length + ")";
            else
                return sb.ToString();

        else if (value is int i)
            return i.ToString();

        else if (value is DateTime dt)
            return dt.ToString();

        else if (value is DateTimeOffset dto)
            return dto.ToString();

        else if (value is TimeSpan ts)
            return ts.ToString();

        else if (value is bool b)
            return b.ToString();

        else if (value is double d)
            return d.ToString();

        else if (value is float f)
            return f.ToString();

        else if (value is char c)
            return c + "";

        else if (value is Enum en)
            return en.ToText() + " (enum value: " + en.ToValue() + ")\n";

        else if (value is long i64)
            return i64.ToString();

        else if (value is short i16)
            return i16.ToString();

        else if (value is decimal)
            return value.ToString();

        else if (value is bool?)
            return (value as bool?).Value + "";

        else if (value is int?)
            return (value as int?).Value + "";

        else if (value is double?)
            return (value as double?).Value + "";

        else if (value is short?)
            return (value as short?).Value + "";

        else if (value is long?)
            return (value as long?).Value + "";

        else if (value is decimal?)
            return (value as decimal?).Value + "";

        else if (value is Memory<string> memString)
            return "Memory<string> " + memString.Span.ToString();

        else if (value is Memory<bool> memBool)
            return "Memory<bool> " + memBool.Span.ToString();

        else if (value is Memory<int> memInt)
            return "Memory<int> " + memInt.Span.ToString();

        else if (value is Memory<DateTime> memDateTime)
            return "Memory<DateTime> " + memDateTime.Span.ToString();

        else if (value is ReadOnlyMemory<string> romString)
            return "ReadOnlyMemory<string> " + romString.Span.ToString();

        else if (value is ReadOnlyMemory<int> romInt)
        {
            var tmp = "ReadOnlyMemory<int> ";
            foreach (var number in romInt.Span)
            {
                tmp += number + ", ";
            }
            return tmp;
            //return romInt.Span.ToString();
        }

        else if (value is ReadOnlyMemory<DateTime> romDateTime)
            return romDateTime.Span.ToString();

        else if (value is CultureInfo cult)
            return "CultureInfo name: " + cult.Name + ", two-letter-iso: " + cult.TwoLetterISOLanguageName + ", three-letter-iso: " + cult.ThreeLetterISOLanguageName;

        return null;
    }

    static void Append(StringBuilder sb, Type type, object value, int level)
    {
        sb.Append(GetTabs(level) + type?.Name + ": " + value);
    }

    static string GetTabs(int level)
    {
        if (level == 0) return "";
        var tabs = "";

        for (int i = 1; i < level; i++)
            tabs += "\t";

        return tabs;
    }

    static bool IsListType(object o)
    {
        return o is IEnumerable && o is not string;
    }

    static void WriteToFileWithDateTime(StringBuilder logString)
    {
        logString.Append("\n");

        SafeWrite(logString.ToString());
    }

    static ReaderWriterLock readWriteLock = new ReaderWriterLock();

    static void SafeWrite(string message)
    {
        if (Assemblies.IsKestrelMainHost)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
            return;
        }

        try
        {
            try
            {
                readWriteLock.AcquireWriterLock(175);
            }
            catch
            {
            }

            File.AppendAllText(LogFullPath, message, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            try
            {
                File.AppendAllText(Folder + @"DumpWrite" + DateTime.Now.Millisecond + ".log", "Error writing to dump file: " + ex.Message + "\nDumped message was:" + message + "\n");
            }
            catch
            {
            }
        }
        finally
        {
            try
            {
                if (readWriteLock.IsWriterLockHeld)
                    readWriteLock.ReleaseWriterLock();
            }
            catch
            {
            }
        }
    }

}
