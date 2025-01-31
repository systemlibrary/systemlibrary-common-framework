using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ObjectExtensionsTests : BaseTest
{
    [TestMethod]
    public void Json_CamelCase_Flag_Set_To_True_Converts_Correctly()
    {
        var employee = Employee.Create();

        var json = employee.Json(true);

        Assert.IsTrue(json.Contains("firstName"), "firstName");
        Assert.IsTrue(!json.Contains("FirstName"), "FirstName");
        Assert.IsFalse(json.Contains("lastName"), "lastName");
        Assert.IsTrue(json.Contains("isPaid"), "IsPaid");
    }

    [TestMethod]
    public void Json_No_Params_Converts_Correctly()
    {
        var employee = Employee.Create();

        var json = employee.Json();

        Assert.IsTrue(json.Contains("FirstName"), "FirstName");
        Assert.IsTrue(!json.Contains("firstName"), "firstName");
        Assert.IsTrue(!json.Contains("LastName"), "LastName");
        Assert.IsTrue(json.Contains("IsPaid"), "IsPaid");
    }

    [TestMethod]
    public void Json_Enum_Unset_Prints_First_Value()
    {
        var employee = Employee.Create();

        var json = employee.Json();

        Assert.IsTrue(json.Contains(BackgroundColor.Red.ToValue()));
    }

    [TestMethod]
    public void Json_With_Custom_Date_Time_Converter()
    {
        var employee = Employee.Create();

        var json = employee.Json(new DateTimeJsonConverter("yy-MM-dd"));

        var datetext = DateTime.Now.ToString("yy-MM-dd");

        Assert.IsTrue(json.Contains(datetext), "Date Married is not set or invalid: " + json);
    }

    [TestMethod]
    public void Json_Enum_Uses_Value()
    {
        var employee = Employee.Create();

        employee.favoriteColor = BackgroundColor.Orange;

        var json = employee.Json();

        Assert.IsTrue(json.Contains(BackgroundColor.Orange.ToValue()));
    }

    [TestMethod]
    public void Json_With_JsonSerializerOptions_CamelCased()
    {
        var employee = Employee.Create();

        var options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        };

        var json = employee.Json(options);

        Assert.IsTrue(json.Contains("firstName"), "firstName " + json);
        Assert.IsTrue(json.Contains("age"));
        Assert.IsTrue(json.Contains("isFired"));
        Assert.IsTrue(json.Contains("isHired"));
    }

    [TestMethod]
    public void Convert_User_To_String_Camel_Casing()
    {
        var employee = Employee.Create();

        var options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
            IncludeFields = true
        };

        var json = employee.Json(options);
        
        Assert.IsTrue(json.Contains("firstName"), "firstName " + json);
        Assert.IsTrue(json.Contains("age"));
        Assert.IsTrue(json.Contains("isFired"));
        Assert.IsTrue(json.Contains("isHired"));
    }

    [TestMethod]
    public void Json_Prints_Default_Casing_As_CSharp_Model()
    {
        var employee = Employee.Create();

        var json = employee.Json();

        Assert.IsTrue(json.Contains("FirstName"));
        Assert.IsTrue(json.Contains("favoriteColor"));
    }

    [TestMethod]
    public void Json_Members_Are_Printed_Based_On_Visibility()
    {
        var employee = Employee.Create();

        var json = employee.Json();
        
        Assert.IsTrue(json.Contains("FirstName"));
        Assert.IsFalse(json.Contains("LastName"));
        
        Assert.IsFalse(json.Contains("MiddleName"), "Json ignored skipped?");
        Assert.IsTrue(json.Contains("PhoneNumber"));
        Assert.IsTrue(json.Contains("IsHired"));
        Assert.IsTrue(json.Contains("IsFired"));
        Assert.IsTrue(json.Contains("Height"));
        Assert.IsFalse(json.Contains("Width"));
    }

    [TestMethod]
    public void Json_Enum_With_Underscore_Int_Prints_Enum_As_Is_Or_Enum_Value()
    {
        var employee = Employee.Create();

        employee.favoriteColor = BackgroundColor._997;

        var json = employee.Json();
        Assert.IsTrue(json.Contains("997"), "997: " + json);

        employee.favoriteColor = BackgroundColor._999;
        
        json = employee.Json();
        Assert.IsTrue(json.Contains("998"), "999: " + json);
    }

    [TestMethod]
    public void Json_Prints_Enum_Name_And_Reads_Back_Based_On_Value_To_Proper_Enum()
    {
        var employee = Employee.Create();

        employee.favoriteColor = BackgroundColor._997;

        var json = employee.Json();

        json = json.Replace("997", "990");

        var employee2 = json.Json<Employee>();

        Assert.IsTrue(employee2.favoriteColor.ToValue() == "990", "990");

        json = json.Replace("990", "998");

        var employee3 = json.Json<Employee>();

        Assert.IsTrue(employee3.favoriteColor == BackgroundColor._999, "Could not convert 998 to 999");
        Assert.IsTrue(employee3.favoriteColor.ToValue() == "998", "It is 998 still, its invalid Enum [or removed key], it shouldve matched _999");
    }

    [TestMethod]
    public void Json_With_Norwegian_Prints_Correctly()
    {
        var employee = Employee.Create("ÆØÅ æøå");

        var json = employee.Json();

        Assert.IsTrue(json.Contains("ÆØÅ æøå"));
    }

    [TestMethod]
    public void Json_Serialize_To_Employee()
    {
        var data = Assemblies.GetEmbeddedResource("_Assets/employee.json");

        var employee = data.Json<Employee>();

        Assert.IsTrue(employee != null);

        Assert.IsTrue(employee.BirthDate > DateTime.MinValue, "Birth 1");
        Assert.IsTrue(employee.BirthDate.Year > 1970, "Year 1");

        Assert.IsTrue(employee.MarriedDate == DateTime.MinValue, "Married 1");

        Assert.IsTrue(employee.Invoices.Count > 0, "No invoices");
    }
}
