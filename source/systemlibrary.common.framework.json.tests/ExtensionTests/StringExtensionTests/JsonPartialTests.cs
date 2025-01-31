using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework;


partial class StringExtensionsTests 
{
    static string GetEmployeeData() => Assemblies.GetEmbeddedResource("_Assets/employee.json");
    static string GetResponseData() => Assemblies.GetEmbeddedResource("_Assets/response.json");

    [TestMethod]
    public void JsonPartial_With_SearchPath_As_Property_Name_Returns_Response()
    {
        var response = GetResponseData().JsonPartial<Response>("headers");

        Assert.IsTrue(response != null, "Headers property not found as 'response'");
        Assert.IsTrue(response.Host.Contains("bin"), "Missing Host: " + response.Host);
        Assert.IsTrue(response.AcceptLanguage.Contains("en"), "Missing AcceptLanguage: " + response.AcceptLanguage);
    }

    [TestMethod]
    public void JsonPartial_With_SearchPath_As_Property_Name_UPPER_CASED_Returns_Response()
    {
        var response = GetResponseData().JsonPartial<Response>("HEADERS");

        Assert.IsTrue(response != null, "Headers property not found as 'response'");
        Assert.IsTrue(response.Host.Contains("bin"), "Missing Host: " + response.Host);
        Assert.IsTrue(response.AcceptLanguage.Contains("en"), "Missing AcceptLanguage: " + response.AcceptLanguage);
    }

    [TestMethod]
    public void JsonPartial_With_SearchPath_Returns_Response()
    {
        var data = GetEmployeeData();

        var invoices = data.JsonPartial<List<Invoice>>("/invoices/");

        Assert.IsTrue(invoices != null, "Invoices not found");
        Assert.IsTrue(invoices.Count == 4, "Invoice count wrong " + invoices.Count);
        Assert.IsTrue(invoices[0].Title == "Invoice 0");
        Assert.IsTrue(invoices[1].Title == "Invoice 1");
        Assert.IsTrue(invoices[2].Title == "Invoice 2");
        Assert.IsTrue(invoices[3].Title == "Invoice 3");

        Assert.IsTrue(invoices[0].BankAccountNumber == 1234567890);
        Assert.IsTrue(invoices[1].BankAccountNumber == 1234567891);
        Assert.IsTrue(invoices[2].BankAccountNumber == 1234567892);
        Assert.IsTrue(invoices[3].BankAccountNumber == 1234567893);
    }

    [TestMethod]
    public void JsonPartial_With_Rooted_SearchPath_Returns_Response()
    {
        var data = GetEmployeeData();

        var invoices = data.JsonPartial<List<Invoice>>("~/invoices/");

        Assert.IsTrue(invoices != null, "Invoices not found");
        Assert.IsTrue(invoices.Count == 4, "Invoice count wrong " + invoices.Count);
        Assert.IsTrue(invoices[0].Title == "Invoice 0");
        Assert.IsTrue(invoices[1].Title == "Invoice 1");
        Assert.IsTrue(invoices[2].Title == "Invoice 2");
        Assert.IsTrue(invoices[3].Title == "Invoice 3");

        Assert.IsTrue(invoices[0].BankAccountNumber == 1234567890);
        Assert.IsTrue(invoices[1].BankAccountNumber == 1234567891);
        Assert.IsTrue(invoices[2].BankAccountNumber == 1234567892);
        Assert.IsTrue(invoices[3].BankAccountNumber == 1234567893);
    }

    [TestMethod]
    public void JsonPartial_With_Inferred_SearchPath_Returns_Response()
    {
        var data = GetEmployeeData();

        var invoices = data.JsonPartial<List<Invoice>>();

        Assert.IsTrue(invoices != null, "Invoices not found");
        Assert.IsTrue(invoices.Count == 4, "Invoice count wrong " + invoices.Count);
        Assert.IsTrue(invoices[0].Title == "Invoice 0");
        Assert.IsTrue(invoices[1].Title == "Invoice 1");
        Assert.IsTrue(invoices[2].Title == "Invoice 2");
        Assert.IsTrue(invoices[3].Title == "Invoice 3");

        Assert.IsTrue(invoices[0].BankAccountNumber == 1234567890);
        Assert.IsTrue(invoices[1].BankAccountNumber == 1234567891);
        Assert.IsTrue(invoices[2].BankAccountNumber == 1234567892);
        Assert.IsTrue(invoices[3].BankAccountNumber == 1234567893);
    }

    [TestMethod]
    public void JsonPartial_Multiple_Times_Returns_Responses()
    {
        var data = GetEmployeeData();

        var invoices = data.JsonPartial<List<Invoice>>();
        var invoices2 = data.JsonPartial<List<Invoice>>();
        var invoices3 = data.JsonPartial<List<Invoice>>();
        var invoices4 = data.JsonPartial<List<Invoice>>();

        var equalCount = invoices.Count == invoices2.Count && invoices2.Count == invoices3.Count && invoices3.Count == invoices4.Count;

        Assert.IsTrue(equalCount, "Invoices differs in count");

        Assert.IsTrue(invoices != null, "Invoices not found");
        Assert.IsTrue(invoices.Count == 4, "Invoice count wrong " + invoices.Count);
        Assert.IsTrue(invoices[0].Title == "Invoice 0");
        Assert.IsTrue(invoices2[1].Title == "Invoice 1");
        Assert.IsTrue(invoices3[2].Title == "Invoice 2");
        Assert.IsTrue(invoices4[3].Title == "Invoice 3");

        Assert.IsTrue(invoices[0].BankAccountNumber == 1234567890);
        Assert.IsTrue(invoices2[1].BankAccountNumber == 1234567891);
        Assert.IsTrue(invoices3[2].BankAccountNumber == 1234567892);
        Assert.IsTrue(invoices4[3].BankAccountNumber == 1234567893);
    }

    [TestMethod]
    public void JsonPartial_Reads_Single_String_Returns_String()
    {
        var data = GetEmployeeData();

        var value = data.JsonPartial<string>("firstName");

        Assert.IsTrue(value == "John");
    }

    [TestMethod]
    public void JsonPartial_Reads_Single_Int_Returns_Int()
    {
        var data = GetEmployeeData();

        var value = data.JsonPartial<int>("age");

        Assert.IsTrue(value == 39);
    }

    [TestMethod]
    public void JsonPartial_Reads_Single_Bool_Returns_Bool()
    {
        var data = GetEmployeeData();

        var value = data.JsonPartial<bool>("isHired");

        Assert.IsTrue(value);
    }
}
