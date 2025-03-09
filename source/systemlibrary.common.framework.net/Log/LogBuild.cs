using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Extensions;

partial class Log
{
    public static StringBuilder Build(object obj)
    {
        var message = new StringBuilder(128);

        var visited = new List<int>();

        Append(message, obj, 0, 5, visited);

        visited.Clear();

        visited = null;

        return message;
    }

    static void Append(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
    {
        if (AppendVariable(message, obj, level)) return;

        if (AppendType(message, obj, level)) return;

        if (level > maxLevel)
        {
            Add(message, "Skipped as max depth reached, continuing...", level);
            return;
        }

        if (AppendEnumerable(message, obj, level, maxLevel, visited)) return;

        if (AppendClass(message, obj, level, maxLevel, visited)) return;

        if (AppendKeyValuePair(message, obj, level, maxLevel, visited)) return;

        Add(message, obj.ToString(), level);
    }

    static bool AppendVariable(StringBuilder message, object obj, int level)
    {
        var value = GetVariableValue(obj);

        if (value != null)
        {
            Add(message, value, level);

            return true;
        }
        return false;
    }

    static bool AppendType(StringBuilder message, object obj, int level)
    {
        string PrintBool(string n, bool b)
        {
            if (b)
                return n + ", ";
            return "";
        }

        if (obj is Type t)
        {
            var name = t.FullName;
            var index = name.IndexOf(',');
            if (index > 0)
            {
                var tmp = name.Substring(0, index);
                var countBrackets = 0;
                foreach (var a in name)
                    if (a == '[')
                        countBrackets++;

                foreach (var a in tmp)
                    if (a == ']')
                        countBrackets--;

                while (countBrackets > 0)
                {
                    tmp += "]";
                    countBrackets--;
                }
                name = tmp;
            }

            name = name.Replace("System.Collections.Generic.", "")
                .Replace("System.Collections.", "")
                .Replace("System.", "");

            var value = "Type " + name + ": "
                + (PrintBool("IsClass", t.IsClass)
                + PrintBool("IsValueType", t.IsValueType)
                + PrintBool("IsInterface", t.IsInterface)
                + PrintBool("IsEnum", t.IsEnum)
                + PrintBool("IsGenericType", t.IsGenericType)
                + PrintBool("IsPrimitive", t.IsPrimitive)
                + PrintBool("IsArray", t.IsArray)
                + PrintBool("IsAbstract", t.IsAbstract)
                + PrintBool("IsAutoClass", t.IsAutoClass)
                + PrintBool("IsPointer", t.IsPointer)
                + PrintBool("IsGenericParameter", t.IsGenericParameter)).TrimEnd(", ");

            Add(message, value, level);

            return true;
        }
        return false;
    }

    static bool AppendKeyValuePair(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
    {
        var objType = obj.GetType();
        if (objType.IsGenericType &&
           objType.GetGenericTypeDefinition() == SystemType.KeyValueType)
        {
            var value = objType.GetProperty("Value").GetValue(obj);

            if (value is IEnumerable enumerable)
            {
                // AppendEnumerable(message, enumerable, level, maxLevel, visited);

                var key = objType.GetProperty("Key").GetValue(obj);
                var list = ((Array)value).Cast<object>();
                var joined = string.Join(", ", list);
                if (list.Count() > 1)
                {
                    Add(message, "[" + key + ", [" + joined + "]]", level);
                }
                else
                {
                    Add(message, "[" + key + ", " + joined + "]", level);
                }
            }
            else
            {
                Add(message, obj.ToString(), level);
            }
            return true;
        }

        return false;
    }

    static bool AppendEnumerable(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
    {
        var isList = obj is IEnumerable && obj is not string;

        if (!isList) return false;

        if (obj is IEnumerable enumerable)
        {
            var originalType = obj.GetType();

            var type = (Type)null;
            var type2 = (Type)null;
            var args = (Type[])null;

            if (originalType.IsArray)
            {
                type = originalType.GetElementType();
            }
            else
            {
                args = originalType.GetGenericArguments();
                if (args?.Length > 0) type = args[0];
                if (args?.Length > 1) type2 = args[1];
            }
            if (type == null) type = originalType;

            int count = GetEnumerableCount(enumerable);

            AppendEnumerableTypeName(message, enumerable, level, type, type2, args, count);

            if (count == 0) return true;

            var printAsOneLine = IsListTypePrintableAsOneLine(type);

            var printAsMatrix = !printAsOneLine && type == typeof(int[,]) || type == typeof(long[,]) || type == typeof(string[,]);

            if (printAsOneLine)
            {
                var tmp = new StringBuilder(": ");

                if (type.IsEnum)
                {
                    foreach (var item in enumerable)
                        tmp.Append(PrintEnum(item) + ", ");
                }
                else
                {
                    foreach (var item in enumerable)
                        tmp.Append(item + ", ");
                }

                Add(message, tmp.ToString().TrimEnd(", "), level);
            }
            else if (printAsMatrix)
            {
                var arr = enumerable as Array;

                message.Append("\n" + new string('\t', level + 1));

                for (int j = 0; j < arr.GetLength(0); j++)
                {
                    for (int k = 0; k < arr.GetLength(1); k++)
                    {
                        var val = arr.GetValue(j, k);
                        message.Append(val + " ");
                    }

                    if (j + 1 < arr.GetLength(0))
                        message.Append("\n" + new string('\t', level + 1));
                }
            }
            else
            {
                Add(message, "\n", level);
                int curr = 1;

                foreach (var item in enumerable)
                {
                    if (item == null) continue;

                    var tmp = new StringBuilder();

                    Append(tmp, item, level + 1, maxLevel, visited);

                    if (tmp.Length == 0)
                        Add(tmp, item.ToString(), level + 1);

                    if (curr < count)
                    {
                        curr++;
                        Add(message, tmp.ToString(), 0);
                        Add(message, "\n", level + 1);
                    }
                    else
                    {
                        Add(message, tmp.ToString(), 0);
                    }
                }
            }
        }

        return true;
    }

    static void AppendEnumerableTypeName(StringBuilder message, IEnumerable enumerable, int level, Type type, Type type2, Type[] args, int count)
    {
        if (args == null || args.Length == 0)
            Add(message, PrintTypeName(type) + "[] (" + count + ")", level);

        else
        {
            if (enumerable is IDictionary)
            {
                Add(message, "IDictionary<" + PrintTypeName(type) + ", " + PrintTypeName(type2) + "> (" + count + ")", level);
            }
            else if (enumerable is Array)
            {
                Add(message, PrintTypeName(type) + "[] (" + count + ")", level);
            }
            else if (enumerable is IList)
            {
                Add(message, "List<" + PrintTypeName(type) + "> (" + count + ")", level);
            }
            else if (enumerable is ICollection)
            {
                if (args.Length == 1)
                {
                    Add(message, "ICollection<" + PrintTypeName(type) + "> (" + count + ")", level);
                }
                else
                {
                    Add(message, "ICollection<" + PrintTypeName(type) + ", " + PrintTypeName(type2) + "> (" + count + ")", level);
                }
            }
            else
                Add(message, PrintTypeName(type) + " (" + count + ")", level);
        }
    }

    static string PrintTypeName(Type type)
    {
        if (type == null) return "";
        if (type == SystemType.IntType) return "int";
        if (type == SystemType.StringType) return "string";
        if (type == SystemType.CharType) return "char";
        if (type == SystemType.Int64Type) return "long";
        if (type == SystemType.Int16Type) return "short";
        if (type == SystemType.BoolType) return "bool";
        if (type == SystemType.DoubleType) return "double";
        return type.Name;
    }

    static bool AppendClass(StringBuilder message, object obj, int level, int maxLevel, List<int> visited)
    {
        var type = obj.GetType();

        if (!type.IsClassType()) return false;

        if (BlacklistClassTypes.Contains(type.Name)) return true;

        var reference = obj.GetHashCode();

        if (IsVisited(type, reference, visited))
        {
            Add(message, obj.GetType().Name + " ref: " + (obj.GetHashCode()) + " already printed, continue...", level);
            return true;
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
            .Where(p => p.CanRead && !BlacklistMemberNames.Contains(p.Name) && !BlacklistClassTypes.Contains(p.PropertyType.Name))
            .ToList();

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => !BlacklistMemberNames.Contains(f.Name) && !BlacklistClassTypes.Contains(f.FieldType.Name))
            .ToList();

        var args = type.GetGenericArguments();
        var genericType = (Type)null;

        if (args?.Length > 0)
            genericType = args[0];

        var typeName = type.Name;

        if (genericType != null)
            typeName = typeName + "<" + genericType?.Name + ">";

        Add(message, typeName + " (ref: " + reference + ")", level);

        Add(message, "\n", 0);

        level += 1;

        if (!fields.Any() && !properties.Any())
        {
            Add(message, "{ fieldCount: " + fields?.Count + ", propertyCount: " + properties?.Count + " } ", level);
            return true;
        }

        if (properties.Any())
        {
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];

                if (property?.PropertyType == null) continue;

                if (!property.CanRead) continue;

                Add(message, property.Name + ": ", level);

                try
                {
                    var value = property.GetValue(obj);

                    var sb = new StringBuilder(128);

                    Append(sb, value, level, maxLevel, visited);
                    int index = level;
                    if (index > 0 && sb.Length > index)
                        sb.Remove(0, index);

                    Add(message, sb.ToString(), 0);
                }
                catch
                {
                    Add(message, "(error reading prop, continue...)", level);
                }
                if (i < properties.Count - 1)
                    Add(message, "\n", 0);
            }
        }

        if (fields.Any())
        {
            if (properties.Any())
                Add(message, "\n", 0);

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                if (field?.FieldType == null) continue;

                if (field.IsPrivate) continue;

                Add(message, field.Name + ": ", level);

                try
                {
                    var value = field.GetValue(obj);

                    var sb = new StringBuilder(128);
                    Append(sb, value, level, maxLevel, visited);

                    int index = level;
                    if (index > 0)
                        sb.Remove(0, index);

                    Add(message, sb.ToString(), 0);
                }
                catch
                {
                    Add(message, "(error reading field, continue...)", level);
                }
                if (i < fields.Count - 1)
                    Add(message, "\n", 0);
            }
        }
        return true;
    }

    static void AppendSkippedObject(StringBuilder message, object obj)
    {
        if (message.Length == 0)
            message.Append(obj.GetType().Name + " skipped");
    }

    static void Add(StringBuilder message, string value, int level)
    {
        message.Append(new string('\t', level));

        message.Append(value);
    }

    static bool IsVisited(Type type, int hash, List<int> visit)
    {
        if (type.BaseType == typeof(ValueType)) return false;

        if (hash == 0) return false;

        if (visit.Contains(hash)) return true;

        visit.Add(hash);

        return false;
    }

    static void PrefixDateTime(StringBuilder message)
    {
        message.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t");
    }

    static bool IsListTypePrintableAsOneLine(Type listTypeArg)
    {
        return listTypeArg.IsEnum ||
               listTypeArg == typeof(short) ||
               listTypeArg == typeof(int) ||
               listTypeArg == typeof(long) ||
               listTypeArg == typeof(byte) ||
               listTypeArg == typeof(decimal) ||
               listTypeArg == typeof(double) ||
               listTypeArg == typeof(float) ||
               listTypeArg == typeof(bool);
    }

    static int GetEnumerableCount(IEnumerable enumerable)
    {
        int c = 0;

        if (enumerable is IDictionary d) c = d.Count;

        else if (enumerable is Array a) c = a.Length;

        else if (enumerable is IList l) c = l.Count;

        else if (enumerable is ICollection ic) c = ic.Count;

        else foreach (var item in enumerable) c++;

        return c;
    }

    static string GetVariableValue(object obj)
    {
        if (obj == null) return "(null)";

        if (obj is Exception e)
        {
            if (e is AggregateException agg)
            {
                return agg.Flatten().ToString().Replace("\n", "\n\t");
            }
            return e.ToString().Replace("\n", "\n\t");
        }

        if (obj is string str)
        {
            if (str == "") return "(empty)";

            var strLength = str.Length;
            if (strLength > 16384)
            {
                var lastWords = new StringBuilder();
                int spaceCount = 0;

                for (int i = strLength - 1; i >= 0 && spaceCount < 3; i--)
                {
                    if (str[i] == ' ') spaceCount++;

                    lastWords.Insert(0, str[i]);

                    if (lastWords.Length > 192) break;
                }

                var l = 16192 + 4 + lastWords.Length;

                return $"{str.Substring(0, 16192)}...{lastWords} (length: {l}/{strLength})";
            }
            return strLength > 32 ? $"{str} (length: {strLength})" : str;
        }

        if (obj is StringBuilder sb) return sb.ToString();

        if (obj is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss.fffzzz");

        if (obj is DateTimeOffset dto) return dto.ToString("yyyy-MM-dd HH:mm:ss.fffzzz");

        if (obj is Enum enu) return PrintEnum(enu);

        if (obj is ValueType &&
           !(obj is IEnumerable) &&
           !(obj is ReadOnlyMemory<int>) &&
           !(obj is ReadOnlyMemory<string>))
        {
            var objType = obj.GetType();
            if (objType.IsGenericType &&
               objType.GetGenericTypeDefinition() == SystemType.KeyValueType)
            {
                return null;
            }

            return obj.ToString();
        }

        if (obj is CultureInfo cult)
            return "CultureInfo: " + cult.Name + ", two-letter-iso: " + cult.TwoLetterISOLanguageName + ", three-letter-iso: " + cult.ThreeLetterISOLanguageName;

        if (obj is Uri uri)
        {
            return $"{uri.OriginalString} | Scheme: {uri.Scheme} | Host: {uri.Host} | Path: {uri.AbsolutePath} | Query: {(uri.Query.Is() ? "(empty)" : uri.Query)} | IsAbsolute: {uri.IsAbsoluteUri} | IsFile: {uri.IsFile} | Authority: {uri.Authority}";
        }

        if (obj is Version v)
        {
            return v.ToString();
        }
        if (obj is HttpMethod m)
        {
            return m.ToString();
        }

        if (obj is bool?)
            return (obj as bool?).Value + "";

        if (obj is int?)
            return (obj as int?).Value + "";

        if (obj is double?)
            return (obj as double?).Value + "";

        if (obj is short?)
            return (obj as short?).Value + "";

        if (obj is long?)
            return (obj as long?).Value + "";

        if (obj is decimal?)
            return (obj as decimal?).Value + "";

        if (obj is Memory<string> memString)
            return "Memory<string> " + memString.Span.ToString();

        if (obj is Memory<bool> memBool)
            return "Memory<bool> " + memBool.Span.ToString();

        if (obj is Memory<int> memInt)
            return "Memory<int> " + memInt.Span.ToString();

        if (obj is Memory<DateTime> memDateTime)
            return "Memory<DateTime> " + memDateTime.Span.ToString();

        if (obj is ReadOnlyMemory<string> romString)
            return "ReadOnlyMemory<string> " + romString.Span.ToString();

        if (obj is ReadOnlyMemory<int> romInt)
        {
            var tmp = "ReadOnlyMemory<int> ";
            foreach (var number in romInt.Span)
            {
                tmp += number + ", ";
            }
            return tmp;
        }

        if (obj is ReadOnlyMemory<DateTime> romDateTime) return romDateTime.Span.ToString();

        if (obj is StackFrame stackFrame)
            return "StackFrame " + (stackFrame.GetFileName() + " " + stackFrame.GetMethod()?.Name + " in type " + stackFrame.GetMethod()?.DeclaringType?.Name).Trim();

        var type = obj.GetType();

        if (type.Name.Contains("Task`"))
        {
            var task = obj as Task;
            return type.Name + " Message: " + task.Exception?.Message + ", IsCompleted: " + task.IsCompleted + ", IsFaulted: " + task.IsFaulted + ", IsCompletedSuccessfully: " + task.IsCompletedSuccessfully + ", Cancelled: " + task.IsCanceled;
        }

        if (type.Name.Contains("Action`")) return "";

        if (type.Name == "DBNull") return "null";


        return null;
    }

    static string PrintEnum(object item)
    {
        var value = (item as Enum).ToValue();

        if (value.Is())
            return (item as Enum).ToText() + " (" + value + ")";

        return (item as Enum).ToText();
    }
}