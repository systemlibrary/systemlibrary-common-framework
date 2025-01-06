using System.Globalization;

partial class StringExtensions
{

    static CultureInfo[] Cultures = new[]
      {
        new CultureInfo("no-NO"),
        new CultureInfo("es-ES"),
        new CultureInfo("en-US"),
        new CultureInfo("en-GB"),
        new CultureInfo("en-CA"),
        new CultureInfo("ru-RU"),
        new CultureInfo("fr-FR"),
        new CultureInfo("sv-SE"),
        new CultureInfo("da-DK"),
        new CultureInfo("de-DE"),
        new CultureInfo("pl-PL")
    };

    static string[] _AllCultureFormats;
    static string[] AllCultureFormats
    {
        get
        {
            if (_AllCultureFormats == null)
            {
                var formats = new List<string>();
                foreach (var culture in Cultures)
                {
                    formats.AddRange(culture.DateTimeFormat.GetAllDateTimePatterns());
                }

                _AllCultureFormats = formats.ToArray();
            }
            return _AllCultureFormats;
        }
    }

    static string[] DateTimeFormatsCulture = new[]
    {
        "MMMM dd, yyyy",
        "MMM dd, yyyy",
        "dddd, dd MMMM yyyy HH:mm:ss",
        "dddd, dd MMM yyyy HH:mm:ss",
        "dd. MMMM yyyy",
        "dd. MMM yyyy - HH:mm",
        "dd MMM yyyy"
    };

    static string[] AllDateTimeFormats = new[] {
        "dd-MM-yyyy",                           // Norwegian datetime formats
        "dd-MM-yyyy HH:mm",
        "dd-MM-yyyy HH:mm:ss",
        "dd-MM-yyyy HH:mm:ss.fff",
        "dd-MM-yyyy HH:mm:ss.fffffff",

        "dd.MM.yyyy",                           // Norwegian datetime formats
        "dd.MM.yyyy HH:mm",
        "dd.MM.yyyy HH:mm:ss",
        "dd.MM.yyyy HH:mm:ss.fff",
        "dd.MM.yyyy HH:mm:ss.fffffff",

        "MM/dd/yyyy HH:mm:ss.fffffff",          // English datetime formats
        "MM/dd/yyyy HH:mm:ss.fff",

        "ddd, dd MMM yyyy HH:mm:ss CET",        // RFC 1123

        "yyyyMMddTHHmmssK",                     // Basic format without separators

        "yyyyMMddTHHmmss.fff",                  // ISO 8601 Basic without dashes or colons
        "yyyyMMddTHHmmss.fffffff",              // ISO 8601 Basic without dashes or colons

        "yyyyMMdd HHmmss",                      // Compact format with space separator
        "yyyyMMdd HHmmss.fff",                  // Compact format with space separator
        "yyyyMMdd HHmmss.fffffff",              // Compact format with space separator
        "yyyy-MM-dd"
    };

    static string[] ShortDateTimeFormats = AllDateTimeFormats.Where(x => x.Length <= 12).ToArray();
    static string[] ShortDateTimeFormatsEndsInZ = ShortDateTimeFormats.Where(x => x[x.Length - 1] == 'Z' || x[x.Length - 1] == 'z').ToArray();

    static string[] LongDateTimeFormats = AllDateTimeFormats.Where(x => x.Length > 12).ToArray();
    static string[] LongDateTimeFormatsWithPlus = LongDateTimeFormats.Where(x => x.EndsWith("K") || x.EndsWith("Z") || x.EndsWith("Z")).ToArray();
    static string[] LongDateTimeFormatsEndsInZ = LongDateTimeFormatsWithPlus.Concat(LongDateTimeFormats.Where(x => x[x.Length - 1] == 'Z' || x[x.Length - 1] == 'z')).ToArray();

    static string[] MonthlyNameDateTimeFormats = AllDateTimeFormats.Where(x => x.Contains("MMMM ")).ToArray();

    static bool TryParseWithFormats(string date, string[] formats, out DateTime result)
    {
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(date, format, null, DateTimeStyles.None, out result))
                return true;
        }

        result = DateTime.MinValue;

        return false;
    }
    static bool TryParseWithFormats(string date, string[] formats, out DateTimeOffset result)
    {
        foreach (var format in formats)
        {
            if (DateTimeOffset.TryParseExact(date, format, null, DateTimeStyles.None, out result))
                return true;
        }

        result = DateTimeOffset.MinValue;

        return false;
    }
}
