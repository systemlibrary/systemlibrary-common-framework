using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class AssembliesTests : BaseTest
{
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
        var text = Assemblies.GetEmbeddedResource("/_Assets/employee.json");

        Assert.IsTrue(text.Contains("johndoe"));
    }

    [TestMethod]
    public void GetEmbeddedResource_File_Name_Only_Matches_First_Entry_String()
    {
        var text = Assemblies.GetEmbeddedResource("employee.json");

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
