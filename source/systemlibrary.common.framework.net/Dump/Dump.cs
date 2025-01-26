//using System.Collections;
//using System.Diagnostics;
//using System.Globalization;
//using System.Reflection;
//using System.Text;

//using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
//using Microsoft.Win32.SafeHandles;

//using SystemLibrary.Common.Framework;
//using SystemLibrary.Common.Framework.Extensions;

//public static class Dump
//{
//    static HashSet<string> BlacklistMemberNames = new();
//    static HashSet<string> BlacklistClassTypes = new();

//    static string LogFullPath;
//    static string Folder;

//    static Dump()
//    {
//        BlacklistClassTypes.Add(typeof(Exception).Name);
//        BlacklistClassTypes.Add(typeof(NullReferenceException).Name);
//        BlacklistClassTypes.Add(typeof(RuntimeTypeHandle).Name);
//        BlacklistClassTypes.Add(typeof(ModelBindingMessageProvider).Name);
//        BlacklistClassTypes.Add(typeof(SafeWaitHandle).Name);
//        BlacklistClassTypes.Add(typeof(char).Name);
//        BlacklistClassTypes.Add("RuntimeType");
//        BlacklistClassTypes.Add("RuntimeMethodInfo");
//        BlacklistClassTypes.Add("RuntimeAssembly");
//        BlacklistClassTypes.Add("Constructor");

//        try
//        {
//            LogFullPath = FrameworkConfig.Current.Dump.GetFullLogPath();
//            Folder = new FileInfo(LogFullPath).DirectoryName + "\\";
//        }
//        catch
//        {
//            Folder = "C:\\logs\\";
//            LogFullPath = Folder + "dump.log";
//        }

//        if (!Directory.Exists(Folder))
//            Directory.CreateDirectory(Folder);
//    }

//    public static void Write(object obj)
//    {
//        var message = Build(obj);

//        AppendSkippedObject(message, obj);

//        PrefixDateTime(message);

//        message.Append("\n");

//        SafeWrite(message.ToString());
//    }

//    public static StringBuilder Build(object obj)
//    {
//        var message = new StringBuilder(128);

//        var visited = new List<int>();

//        Append(message, obj, 0, 5, visited);

//        visited.Clear();

//        visited = null;

//        return message;
//    }

//    public static void Clear()
//    {
//        try
//        {
//            File.Delete(LogFullPath);
//        }
//        catch
//        {
//        }
//    }

//    static void Append(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
//    {
//        if (AppendVariable(message, obj, level)) return;

//        if (AppendType(message, obj, level)) return;

//        if (level > maxLevel)
//        {
//            Add(message, "Max depth reached, skipping deeper, continue...", level);
//            return;
//        }

//        if (AppendEnumerable(message, obj, level, maxLevel, visited)) return;

//        if (AppendClass(message, obj, level, maxLevel, visited)) return;

//        Add(message, obj.ToString(), level);
//    }

//    static bool AppendVariable(StringBuilder message, object obj, int level)
//    {
//        var value = GetVariableValue(obj);

//        if (value != null)
//        {
//            Add(message, value, level);

//            return true;
//        }
//        return false;
//    }

//    static bool AppendType(StringBuilder message, object obj, int level)
//    {
//        string PrintBool(string n, bool b)
//        {
//            if (b)
//                return n + ", ";
//            return "";
//        }

//        if (obj is Type t)
//        {
//            var name = t.FullName;
//            var index = name.IndexOf(',');
//            if (index > 0)
//            {
//                var tmp = name.Substring(0, index);
//                var countBrackets = 0;
//                foreach (var a in name)
//                    if (a == '[')
//                        countBrackets++;

//                foreach (var a in tmp)
//                    if (a == ']')
//                        countBrackets--;

//                while (countBrackets > 0)
//                {
//                    tmp += "]";
//                    countBrackets--;
//                }
//                name = tmp;
//            }

//            name = name.Replace("System.Collections.Generic.", "")
//                .Replace("System.Collections.", "")
//                .Replace("System.", "");

//            var value = name + ": "
//                + (PrintBool("IsClass", t.IsClass)
//                + PrintBool("IsValueType", t.IsValueType)
//                + PrintBool("IsInterface", t.IsInterface)
//                + PrintBool("IsEnum", t.IsEnum)
//                + PrintBool("IsGenericType", t.IsGenericType)
//                + PrintBool("IsPrimitive", t.IsPrimitive)
//                + PrintBool("IsArray", t.IsArray)
//                + PrintBool("IsAbstract", t.IsAbstract)
//                + PrintBool("IsAutoClass", t.IsAutoClass)
//                + PrintBool("IsPointer", t.IsPointer)
//                + PrintBool("IsGenericParameter", t.IsGenericParameter)).TrimEnd(", ");

//            Add(message, value, level);

//            return true;
//        }
//        return false;
//    }

//    static bool AppendEnumerable(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
//    {
//        var isList = obj is IEnumerable && obj is not string;

//        if (!isList) return false;

//        if (obj is IEnumerable enumerable)
//        {
//            var originalType = obj.GetType();
//            var type = (Type)null;
//            var type2 = (Type)null;
//            var args = (Type[])null;

//            if (originalType.IsArray)
//            {
//                type = originalType.GetElementType();
//            }
//            else
//            {
//                args = originalType.GetGenericArguments();
//                if (args?.Length > 0) type = args[0];
//                if (args?.Length > 1) type2 = args[1];
//            }
//            if (type == null) type = originalType;

//            int i = 0;

//            if (enumerable is IDictionary d) i = d.Count;

//            else if (enumerable is Array a) i = a.Length;

//            else if (enumerable is IList l) i = l.Count;

//            else if (enumerable is ICollection c) i = c.Count;

//            else foreach (var item in enumerable) i++;

//            if (args == null || args.Length == 0)
//                Add(message, type.Name + " (" + i + ")", level);
//            else if (args.Length == 1)
//                Add(message, "<" + type.Name + "> (" + i + ")", level);
//            else
//                Add(message, "<" + type.Name + "," + type2.Name + "> (" + i + ")", level);

//            var printAsOneLine = IsListTypePrintableAsOneLine(type);

//            var printAsMatrix = !printAsOneLine && type == typeof(int[,]) || type == typeof(long[,]) || type == typeof(string[,]);

//            if (printAsOneLine)
//            {
//                var tmp = new StringBuilder(": ");

//                if (type.IsEnum)
//                {
//                    foreach (var item in enumerable)
//                        tmp.Append(PrintEnum(item) + ", ");
//                }
//                else
//                {
//                    foreach (var item in enumerable)
//                        tmp.Append(item + ", ");
//                }

//                Add(message, tmp.ToString().TrimEnd(", "), level);
//            }
//            else if (printAsMatrix)
//            {
//                var arr = enumerable as Array;

//                message.Append("\n" + new string('\t', level + 1));

//                for (int j = 0; j < arr.GetLength(0); j++)
//                {
//                    for (int k = 0; k < arr.GetLength(1); k++)
//                    {
//                        message.Append(arr.GetValue(j, k) + " ");
//                    }

//                    if (j + 1 < arr.GetLength(0))
//                        message.Append("\n" + new string('\t', level + 1));
//                }
//            }
//            else
//            {
//                Add(message, "\n", level);
//                int curr = 1;
//                foreach (var item in enumerable)
//                {
//                    Append(message, item, level + 1, maxLevel, visited);
//                    if (curr < i)
//                    {
//                        curr++;
//                        Add(message, "\n", level + 1);
//                    }
//                }
//            }
//        }

//        return true;
//    }

//    static bool AppendClass(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
//    {
//        var type = obj.GetType();

//        if (!type.IsClassType()) return false;

//        var hash = obj.GetHashCode();

//        if (IsVisited(type, hash, visited))
//        {
//            Add(message, obj.GetType().Name + " class " + obj.GetHashCode() + " already printed, continue...", level);
//            return true;
//        }

//        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
//            .Where(p => p.CanRead && !BlacklistMemberNames.Contains(p.Name) && !BlacklistClassTypes.Contains(p.PropertyType.Name))
//            .ToList();

//        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
//            .Where(f => !BlacklistMemberNames.Contains(f.Name) && !BlacklistClassTypes.Contains(f.FieldType?.Name))
//            .ToList();

//        var args = type.GetGenericArguments();
//        var genericType = (Type)null;

//        if (args?.Length > 0)
//            genericType = args[0];

//        var typeName = type.Name;

//        if (genericType != null)
//            typeName = typeName + "<" + genericType?.Name + ">";

//        Add(message, typeName + " (hash: " + hash + ")", level);

//        Add(message, "\n", 0);

//        level += 1;

//        if (!fields.Any() && !properties.Any())
//        {
//            Add(message, "{ no fields or props? } " + fields?.Count + " " + properties?.Count, level);
//            return true;
//        }

//        if (properties.Any())
//        {
//            for (int i = 0; i < properties.Count; i++)
//            {
//                var property = properties[i];

//                if (property?.PropertyType == null) continue;

//                if (!property.CanRead) continue;

//                Add(message, property.Name + ": ", level);

//                try
//                {
//                    var value = property.GetValue(obj);
//                    var sb = new StringBuilder(128);
//                    Append(sb, value, level, maxLevel, visited);

//                    int index = level;
//                    if (index > 0)
//                        sb.Remove(0, index);

//                    Add(message, sb.ToString(), 0);
//                }
//                catch
//                {
//                    Add(message, "(error reading, continue...)", level);
//                }
//                if (i < properties.Count - 1)
//                    Add(message, "\n", 0);


//            }
//        }

//        if (fields.Any())
//        {
//            if (properties.Any())
//                Add(message, "\n", 0);

//            for (int i = 0; i < fields.Count; i++)
//            {
//                var field = fields[i];

//                if (field?.FieldType == null) continue;

//                if (field.IsPrivate) continue;

//                Add(message, field.Name + ": ", level);

//                try
//                {
//                    var value = field.GetValue(obj);

//                    var sb = new StringBuilder(128);
//                    Append(sb, value, level, maxLevel, visited);

//                    int index = level;
//                    if (index > 0)
//                        sb.Remove(0, index);

//                    Add(message, sb.ToString(), 0);
//                }
//                catch
//                {
//                    Add(message, "(error reading, continue...)", level);
//                }
//                if (i < fields.Count - 1)
//                    Add(message, "\n", 0);
//            }
//        }
//        return true;
//    }

//    static void AppendSkippedObject(StringBuilder message, object obj)
//    {
//        if (message.Length == 0)
//            message.Append(obj.GetType().Name + " skipped");
//    }

//    static void Add(StringBuilder message, string value, int level)
//    {
//        message.Append(new string('\t', level));

//        message.Append(value);
//    }

//    static bool IsVisited(Type type, int hash, List<int> visit)
//    {
//        if (type.BaseType == typeof(ValueType)) return false;


//        if (hash == 0) return false;

//        if (visit.Contains(hash)) return true;

//        visit.Add(hash);

//        return false;
//    }

//    static void PrefixDateTime(StringBuilder message)
//    {
//        message.Insert(0, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "\t");
//    }

//    static bool IsListTypePrintableAsOneLine(Type listTypeArg)
//    {
//        return listTypeArg.IsEnum ||
//               listTypeArg == typeof(short) ||
//               listTypeArg == typeof(int) ||
//               listTypeArg == typeof(long) ||
//               listTypeArg == typeof(byte) ||
//               listTypeArg == typeof(decimal) ||
//               listTypeArg == typeof(double) ||
//               listTypeArg == typeof(float) ||
//               listTypeArg == typeof(bool);
//    }

//    static string GetVariableValue(object obj)
//    {
//        if (obj == null) return "(null)";

//        if (obj is Exception e)
//        {
//            if (e is AggregateException agg)
//            {
//                return agg.Flatten().ToString().Replace("\n", "\n\t");
//            }
//            return e.ToString().Replace("\n", "\n\t");
//        }

//        if (obj is string str)
//        {
//            if (str == "") return "(empty)";

//            var strLength = str.Length;
//            if (strLength > 8192)
//            {
//                var lastWords = new StringBuilder();
//                int spaceCount = 0;

//                for (int i = strLength - 1; i >= 0 && spaceCount < 3; i--)
//                {
//                    if (str[i] == ' ') spaceCount++;

//                    lastWords.Insert(0, str[i]);

//                    if (lastWords.Length > 192) break;
//                }

//                var l = 8000 + 4 + lastWords.Length;

//                return $"{str.Substring(0, 8000)}...{lastWords} (length: {l}/{strLength})";
//            }
//            return strLength > 32 ? $"{str} (length: {strLength})" : str;
//        }

//        if (obj is StringBuilder sb) return sb.ToString();

//        if (obj is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss.fffzzz");

//        if (obj is DateTimeOffset dto) return dto.ToString("yyyy-MM-dd HH:mm:ss.fffzzz");

//        if (obj is ValueType &&
//           !(obj is IEnumerable) &&
//           !(obj is ReadOnlyMemory<int>) &&
//           !(obj is ReadOnlyMemory<string>))
//        {
//            return obj.ToString();
//        }

//        if (obj is Enum enu) return PrintEnum(enu);

//        if (obj is CultureInfo cult)
//            return "CultureInfo: " + cult.Name + ", two-letter-iso: " + cult.TwoLetterISOLanguageName + ", three-letter-iso: " + cult.ThreeLetterISOLanguageName;

//        if (obj is bool?)
//            return (obj as bool?).Value + "";

//        if (obj is int?)
//            return (obj as int?).Value + "";

//        if (obj is double?)
//            return (obj as double?).Value + "";

//        if (obj is short?)
//            return (obj as short?).Value + "";

//        if (obj is long?)
//            return (obj as long?).Value + "";

//        if (obj is decimal?)
//            return (obj as decimal?).Value + "";

//        if (obj is Memory<string> memString)
//            return "Memory<string> " + memString.Span.ToString();

//        if (obj is Memory<bool> memBool)
//            return "Memory<bool> " + memBool.Span.ToString();

//        if (obj is Memory<int> memInt)
//            return "Memory<int> " + memInt.Span.ToString();

//        if (obj is Memory<DateTime> memDateTime)
//            return "Memory<DateTime> " + memDateTime.Span.ToString();

//        if (obj is ReadOnlyMemory<string> romString)
//            return "ReadOnlyMemory<string> " + romString.Span.ToString();

//        if (obj is ReadOnlyMemory<int> romInt)
//        {
//            var tmp = "ReadOnlyMemory<int> ";
//            foreach (var number in romInt.Span)
//            {
//                tmp += number + ", ";
//            }
//            return tmp;
//        }

//        if (obj is ReadOnlyMemory<DateTime> romDateTime) return romDateTime.Span.ToString();

//        if (obj is StackFrame stackFrame)
//            return "StackFrame " + (stackFrame.GetFileName() + " " + stackFrame.GetMethod()?.Name + " in type " + stackFrame.GetMethod()?.DeclaringType?.Name).Trim();

//        var type = obj.GetType();

//        if (type.Name.Contains("Task`"))
//        {
//            var task = obj as Task;
//            return type.Name + " Message: " + task.Exception?.Message + ", IsCompleted: " + task.IsCompleted + ", IsFaulted: " + task.IsFaulted + ", IsCompletedSuccessfully: " + task.IsCompletedSuccessfully + ", Cancelled: " + task.IsCanceled;
//        }

//        if (type.Name.Contains("Action`")) return "";


//        return null;
//    }

//    static string PrintEnum(object item)
//    {
//        return (item as Enum).ToText() + " (" + (item as Enum).ToValue() + ")";
//    }

//    static ReaderWriterLock WriteLock = new ReaderWriterLock();

//    static void SafeWrite(string message)
//    {
//        if (Assemblies.IsKestrelMainHost)
//        {
//            var previousColor = Console.ForegroundColor;
//            Console.ForegroundColor = ConsoleColor.Gray;
//            Console.WriteLine(message);
//            Console.ForegroundColor = previousColor;
//            return;
//        }

//        try
//        {
//            try
//            {
//                WriteLock.AcquireWriterLock(175);
//            }
//            catch
//            {
//            }

//            File.AppendAllText(LogFullPath, message, Encoding.UTF8);
//        }
//        catch (Exception ex)
//        {
//            try
//            {
//                File.AppendAllText(Folder + @"dump." + DateTime.Now.Millisecond + ".log", "Error writing to dump file: " + ex.Message + "\nMessage was: " + message + "\n");
//            }
//            catch
//            {
//            }
//        }
//        finally
//        {
//            try
//            {
//                if (WriteLock.IsWriterLockHeld)
//                    WriteLock.ReleaseWriterLock();
//            }
//            catch
//            {
//            }
//        }
//    }
//}