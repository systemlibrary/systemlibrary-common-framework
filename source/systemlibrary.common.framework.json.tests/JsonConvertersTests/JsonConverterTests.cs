using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class JsonConverterTests : BaseTest
{
    [TestMethod]
    public void DateTimeJsonConverter_Returns_Different_Date_Strings()
    {
        var employee = Employee.Create();

        var json = employee.Json(new DateJsonConverter());

        var json2 = employee.Json(new DateTimeJsonConverter());

        var json3 = employee.Json(new DateTimeOffsetJsonConverter("yyyy-MM-dd"));

        IsOk(!json.Contains(":00:00 "));

        IsOk(json2.Contains(":00:00 "));

        IsOk(json.Contains("0001"));

        IsOk(!json3.Contains(":00:00 "));
    }
}
