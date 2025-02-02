using System.Xml.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class AssembliesTests : BaseTest
{
    [TestMethod]
    public void FindAllTypesInheriting_ILogWriter_Returns_One_LogWriter()
    {
        var ilogwriters = Assemblies.FindAllTypesInheriting<ILogWriter>();
        Assert.IsTrue(ilogwriters.Count() == 1, "ILogWriter wrong count: " + ilogwriters.Count());

        var type = ilogwriters.FirstOrDefault();
        Assert.IsTrue(type.Name == "LogWriter", "Wrong LogWriter type found: " + type.Name);
    }

    [TestMethod]
    public void FindAllTypesInheritingWithAttribute_Attribute_And_AttributeUsageAttribute_Returns_0_As_SystemLibrary_Types_Are_Ignored()
    {
        var sysLibAttributesAreIgnoredAsAsmIsBlacklisted = Assemblies.FindAllTypesInheritingWithAttribute<Attribute, AttributeUsageAttribute>();

        Assert.IsTrue(sysLibAttributesAreIgnoredAsAsmIsBlacklisted.Count() == 0, "Wrong count " + sysLibAttributesAreIgnoredAsAsmIsBlacklisted.Count());
    }

    [TestMethod]
    public void FindAllTypesInheriting_BaseEmploye_Returns_One()
    {
        var typesInheritingBaseEmployee = Assemblies.FindAllTypesInheriting<BaseEmployee>();

        Assert.IsTrue(typesInheritingBaseEmployee.Count() == 1, "BaseEmployee wrong count: " + typesInheritingBaseEmployee.Count());
    }

    [TestMethod]
    public void FindAllTypesInheritingWithAttribute_BaseEmployee_With_XmlSerializerAssemblyAttribute_Returns_One_In_Test_Projects()
    {
        var typesInheritingAndHavinXmlAttribute = Assemblies.FindAllTypesInheritingWithAttribute<BaseEmployee, XmlSerializerAssemblyAttribute>();

        Assert.IsTrue(typesInheritingAndHavinXmlAttribute.Count() == 1, "Wrong employeeClass count " + typesInheritingAndHavinXmlAttribute.Count());
    }

    [TestMethod]
    public void GetEmbeddedResource_Of_File_Not_Existing_Does_Throw()
    {
        try
        {
            Assemblies.GetEmbeddedResource("file-do-not-exist");

            Assert.IsTrue(false, "Reading embedded resource 'file-do-not-exist' did not throw, it should throw");
        }
        catch
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void GetEmbeddedResource_Reads_ValidFile_ReturnsItsData()
    {
        var text = Assemblies.GetEmbeddedResource("/_Assets/embeddedResource.json");

        Assert.IsTrue(text.Contains("johndoe"));
    }

    [TestMethod]
    public void GetEmbeddedResource_ValidFile_MatchesByFileName_ReturnsItsData()
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
