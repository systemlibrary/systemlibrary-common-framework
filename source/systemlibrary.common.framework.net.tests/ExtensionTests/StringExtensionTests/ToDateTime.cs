using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework;

partial class StringExtensionsTests
{
    [TestMethod]
    public void To_DateTime()
    {
        foreach (var date in dateStrings)
        {
            var dateTime = date.ToDateTime();

            ValidateConvertedStringToDateTime(dateTime, date);
        }
    }

    [TestMethod]
    public void To_DateTimeOffset()
    {
        foreach (var date in dateStrings)
        {
            var dateTime = date.ToDateTimeOffset();

            ValidateConvertedStringToDateTime(dateTime, date);
        }
    }

    static void ValidateConvertedStringToDateTime(DateTimeOffset dt, string date)
    {
        var expected = GetExpectedDateTime(date);

        Assert.IsTrue(dt == expected || dt.DateTime == expected, "\nInput " + date + " \nResult: " + dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + " \nExpected: " + expected.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + " " + expected.Kind + " " + dt.LocalDateTime);
    }

    static void ValidateConvertedStringToDateTime(DateTime dt, string date)
    {
        var expected = GetExpectedDateTime(date);

        Assert.IsTrue(dt == expected, "\nInput " + date + " \nResult: " + dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + " \nExpected: " + expected.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
    }

    static DateTime GetExpectedDateTime(string date)
    {
        var y = 2001;
        var M = 1;
        var d = 1;
        var h = 0;
        var m = 0;
        var s = 0;
        var ms = 0;
        var t = 0;

        if (date.Contains("12") || date.Contains("ec") || date.Contains("es"))
        {
            M = 12;
        }

        if (date.Contains("24"))
        {
            d = 24;
        }

        if (date.Contains("23") || date.Contains("PM"))
        {
            h = 23;
        }

        if (date.Contains("22"))
        {
            m = 22;
        }

        if (date.Contains("59"))
        {
            s = 59;
        }

        if (date.Contains("777"))
        {
            ms = 777;
        }

        if (date.Contains("8888888"))
        {
            ms = 888;
            t = 8888;
        }

        if (date.Contains("Z") || date.Contains("GMT") || date.Contains("CET"))
        {
            d = 25;
            h = 0;
        }

        if (date.Contains("+04"))
        {
            h = 20;
        }

        if (date.Contains("1009"))
        {
            y = 2001;
            M = 12;
            d = 24;
            h = 23;
            m = 22;
            s = 59;
            if (date.Contains("777"))
                ms = 777;
            else
                ms = 0;
        }

        var expected = new DateTime(y, M, d, h, m, s, ms, DateTimeKind.Unspecified);

        if (t > 0)
        {
            expected = expected.AddTicks(t);
        }
        return expected;
    }


    // A date example is always 24th December 2001, at 23:22:59.777 OR 23:22.59.8888888 
    // note: millisecond, the whole time itself, and day and month, are optional
    static string[] dateStrings = new string[] {
        "24-12-2001 23:22:59",
        "24-12-2001 23:22",
        "24-12-2001 23:22:59.777",
        "24-12-2001",

        "24.12.2001 23:22:59",
        "24.12.2001 23:22",
        "24.12.2001 23:22:59.777",
        "24.12.2001",

        "12/24/2001 23:22:59",
        "12/24/2001 23:22",
        "12/24/2001 23:22:59.777",
        "12/24/2001",

        "2001-12-24T23:22:59.8888888+04:00",
        "2001-12-24T23:22:59.777+04:00",
        "2001-12-24T23:22:59+04:00",
        "2001-12-24T23:22+04:00",

        "2001/12/24T23:22:59+04:00",
        "2001/12/24T23:22+04:00",
        "2001-12-24T23:22:59.8888888Z",
        "2001/12/24T23:22:59.8888888Z",
        "2001-12-24T23:22:59.777Z",
        "2001/12/24T23:22:59.777Z",
        "2001-12-24T23:22:59",
        "2001-12-24T23:22:59Z",
        "2001/12/24T23:22:59Z",
        "2001-12-24T23:22Z",
        "2001/12/24T23:22Z",

        "2001-12-24",
        "2001/12/24",
        "2001.12.24",

        "Mon, 24 Dec 2001 23:22:59 GMT",
        "Mon, 24 Dec 2001 23:22:59 +04:00",
        "Mon, 24 Dec 2001 23:22:59 +04:00",
        "Tue, 25 Dec 2001 00:22:59 CET",

        "2001-12-24T23:22:59Z",
        "2001-12-24T23:22:59.777Z",
        "2001-12-24T23:22:59.8888888Z",
        "2001-12-24T23:22Z",

        "12/24/01",
        "20011224T232259",
        "20011224T232259Z",
        "20011224T232259+0400",

        "2001-12-24T23:22:59+04:00",
        "12/24/2001 11:22:59 PM",
        "Mandag, 24 Desember 2001 23:22:59",
        "Monday, 24 December 2001 23:22:59",
        "1009236179",

        "24. December 2001",
        "24. Desember 2001",
        "24. Dec 2001 - 23:22",
        "24. Des 2001 - 23:22",
        "24 Des 2001",
        "24 Des 2001",
        "Desember 24, 2001",
        "2001-12-24T23:22:59",

        "24-12-2001 23:22:59.8888888",      // "MM-dd-yyyy HH:mm:ss.fffffff",
        
        
        "12/24/2001 23:22:59.8888888",      // "MM/dd/yyyy HH:mm:ss.fffffff"
        "12/24/2001 23:22:59.777",          // "MM/dd/yyyy HH:mm:ss.fff"
        "12/24/2001 23:22:59",              // "MM/dd/yyyy HH:mm:ss"
        "12/24/2001 23:22",                 // "MM/dd/yyyy HH:mm"
        "12/24/2001",                       // "MM/dd/yyyy"
        
        "2001-12-24T23:22:59.8888888",      // "yyyy-MM-ddTHH:mm:ss.fffffff"
        "2001-12-24T23:22:59.8888888Z",     // "yyyy-MM-ddTHH:mm:ss.fffffffK"
        "2001-12-24T23:22:59.777Z",         // "yyyy-MM-ddTHH:mm:ss.fffK"
        "2001-12-24T23:22:59.777",          // "yyyy-MM-ddTHH:mm:ss.fff"
        "2001-12-24T23:22:59Z",             // "yyyy-MM-ddTHH:mm:ssK"
        "2001-12-24T23:22:59",              // "yyyy-MM-ddTHH:mm:ss"
        "2001-12-24T23:22Z",                // "yyyy-MM-ddTHH:mmK"
        "2001-12-24T23:22",                 // "yyyy-MM-ddTHH:mm"
        
        "2001-12-24",                       // "yyyy-MM-dd"
        "2001/12/24",                       // "yyyy/MM/dd"
        "2001.12.24",                       // "yyyy.MM.dd"
        
        "2001-12-24T23:22:59.777Z",         // "yyyy-MM-ddTHH:mm:ss.fffK"
        "2001-12-24T23:22:59.8888888Z",     // "yyyy-MM-ddTHH:mm:ss.fffffffK"
        
        "Mon, 24 Dec 2001 23:22:59 +01:00", // "ddd, dd MMM yyyy HH':'mm':'ss zzz"
        "Mon, 24 Dec 2001 23:22:59 GMT",    // "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"
        "Mon, 24 Dec 2001 23:22:59 GMT",    // "ddd, dd MMM yyyy HH:mm:ss GMT"
        "Tue, 25 Dec 2001 00:22:59 CET",    // "ddd, dd MMM yyyy HH':'mm':'ss 'CET'"
            
        "2001",
        "2001-12",
        "2001-12-24",
        "2001/12",
        "12/24/2001",

        "20011224T232259",
        "20011224T232259.777",
        "20011224T232259.8888888",

        "2001-12-24T23:22:59+04:00",
        "2001-12-24T23:22:59.777+04:00",
        "2001-12-24T23:22:59.8888888+04:00",

        "20011224 232259",
        "20011224 232259.777",
        "20011224 232259.8888888",

        "24 December 2001 23:22:59",
        "24 December 2001 23:22:59.777",
        "24 December 2001 23:22:59.8888888",

        "1009236179777",
    };
}

