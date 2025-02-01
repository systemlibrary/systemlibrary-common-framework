using System.Xml.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class AssembliesTests : BaseTest
{
    [TestMethod]
    public void FindAllTypesInheriting_Returns_Inheriting_Or_Implements_Not_Interface_Itself_Success()
    {
        var ilogwriters = Assemblies.FindAllTypesInheriting<ILogWriter>();

        Assert.IsTrue(ilogwriters.Count() == 1, "ILogWriters wrong count: " + ilogwriters.Count());

        var type = ilogwriters.FirstOrDefault();

        Assert.IsTrue(type.Name == "LogWriter", type.Name);
    }

    [TestMethod]
    public void FindAllTypesInheritingWithAttribute_Success()
    {
        var sysLibAttributesAreIgnoredAsAsmIsBlacklisted = Assemblies.FindAllTypesInheritingWithAttribute<Attribute, AttributeUsageAttribute>();

        Assert.IsTrue(sysLibAttributesAreIgnoredAsAsmIsBlacklisted.Count() == 0, "Wrong count " + sysLibAttributesAreIgnoredAsAsmIsBlacklisted.Count());

        var employees = Assemblies.FindAllTypesInheriting<BaseEmployee>();

        Assert.IsTrue(employees.Count() == 1, "Wrong employees count " + employees.Count());

        var employeeClass = Assemblies.FindAllTypesInheritingWithAttribute<BaseEmployee, XmlSerializerAssemblyAttribute>();

        Assert.IsTrue(employeeClass.Count() == 1, "Wrong employeeClass count " + employeeClass.Count());
    }

    [TestMethod]
    public void GetEmbeddedResource_Non_Existing_Throws()
    {
        try
        {
            Assemblies.GetEmbeddedResource("root-file-do-not-exist");

            Assert.IsTrue(false, "Non existing embedded resource should throw"); ;
        }
        catch
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void GetEmbeddedResource_Returns_String()
    {
        var text = Assemblies.GetEmbeddedResource("/_Assets/embeddedResource.json");

        Assert.IsTrue(text.Contains("johndoe"));
    }

    [TestMethod]
    public void GetEmbeddedResource_File_Name_Only_Matches_First_Entry_String()
    {
        var text = Assemblies.GetEmbeddedResource("embeddedResource.json");

        Assert.IsTrue(text.Contains("johndoe"));
    }

    [TestMethod]
    public void GetEmbeddedResourceAsBytes_Returns_Bytes_Of_Image()
    {
        var bytes = Assemblies.GetEmbeddedResourceAsBytes("/image.png");

        Assert.IsTrue(bytes.Length > 1);

        var datetime = DateTime.Now.ToString("mm.ss.fffff");
        
        var path = @"C:\Temp\" + datetime + ".png";

        using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
        {
            using (var memory = new MemoryStream(bytes))
            {
                memory.WriteTo(fileStream);
            }
        }

        Thread.Sleep(5);

        Assert.IsTrue(File.Exists(path));

        if (File.Exists(path))
            File.Delete(path);
    }
}
