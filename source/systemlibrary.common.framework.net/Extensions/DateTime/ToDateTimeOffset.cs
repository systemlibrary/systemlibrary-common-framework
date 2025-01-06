using System.Globalization;

partial class StringExtensions
{
    /// <summary>
    /// Converts input date to a DateTimeOffset by trying different formats till successfully converted or throwing exception
    /// </summary>
    /// <example>
    /// <code>
    /// var date = "2000-12-24";
    /// var dateTime = date.ToDateTimeOffset();
    /// </code>
    /// </example>
    /// <returns>Returns DateTimeOffset.MinValue if input is too short</returns>
    public static DateTimeOffset ToDateTimeOffset(this string date, string format = null)
    {
        if (date == null)
            return DateTimeOffset.MinValue;

        var l = date.Length;

        if (l < 4)
            return DateTimeOffset.MinValue;

        if (l == 4)
            return new DateTimeOffset(new DateTime(Convert.ToInt32(date), 1, 1));


        if (DateTimeOffset.TryParse(date, out DateTimeOffset res))
            return res;

        if (format.Is())
        {
            if (DateTimeOffset.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTimeOffset res2))
                return res2;

            if (DateTimeOffset.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.AssumeUniversal, out res2))
                return res2;

            if (DateTimeOffset.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out res2))
                return res2;
        }

        if (DateTimeOffset.TryParse(date, null, DateTimeStyles.RoundtripKind, out res) ||
            DateTimeOffset.TryParse(date, null, DateTimeStyles.AssumeUniversal, out res))
        {
            return res;
        }

        var monthName = char.IsAsciiLetter(date[4]) || char.IsAsciiLetter(date[0]);

        if (monthName)
        {
            if (TryParseWithFormats(date, MonthlyNameDateTimeFormats, out res))
                return res;
        }

        var z = date[l - 1] == 'Z' || date[l - 1] == 'z';

        if (l <= 12)
        {
            if (z)
            {
                if (TryParseWithFormats(date, ShortDateTimeFormatsEndsInZ, out res))
                    return res;
            }
            else
            {
                if (TryParseWithFormats(date, ShortDateTimeFormats, out res))
                    return res;
            }
        }
        else
        {
            if (z)
            {
                if (TryParseWithFormats(date, LongDateTimeFormatsEndsInZ, out res))
                    return res;
            }
            else
            {
                var plus = date[l - 6] == '+';

                if (plus)
                {
                    if (TryParseWithFormats(date, LongDateTimeFormatsWithPlus, out res))
                        return res;
                }
                else
                {
                    if (TryParseWithFormats(date, LongDateTimeFormats, out res))
                        return res;
                }
            }
        }

        if (long.TryParse(date, out long unixTimestamp))
        {
            if (unixTimestamp > 99999999999)
                return new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimestamp));
            else
                return new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp));
        }

        foreach (var culture in Cultures)
        {
            foreach (var cultureFormat in DateTimeFormatsCulture)
            {
                if (DateTimeOffset.TryParseExact(date, cultureFormat, culture, DateTimeStyles.RoundtripKind, out res))
                    return res;
            }
        }

        if (TryParseWithFormats(date, AllCultureFormats, out res))
            return res;

        throw new Exception("Input was not recognized as a valid DateTime. No matching format provided for: " + date + ". You sent in: " + format);
    }


}
