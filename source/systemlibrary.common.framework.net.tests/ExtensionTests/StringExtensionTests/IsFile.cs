using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework;

partial class StringExtensionsTests
{
    [TestMethod]
    public void IsFile_Tests()
    {
        var data = (string)null;
        var expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "a";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "A";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "a/a/a/a/a";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:/Hello/World/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.systemlibrary.com/hello/world/image?";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.png";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.png?quality=90";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.png?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/image.png?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/long/path/image.png";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/long/path/image.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "www.syslib.com/long/path/image.config";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "http://www.syslib.com/long/path/image.config?quality=190&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "image.config?quality=190&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.config?quality=190&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C://hello/world/image.config";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "C:\\hello\\world\\image.config?quality=190&hello=world&.....";
        expected = true;

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "C:\\hello\\world\\image.config?q=.&a=b&c=.&362623632%2020%20quality=190&hello=world&.....";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "/image.config?q=.&a=b&c=.&362623632%2020%20quality=190&hello=world&.....";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "image.config?q=.&a=b&c=.&362623632%2020%20quality=190&hello=world&.....";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "image.config?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "../image.config?";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "../image.config";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
        data = "../image.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "../../../image.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "../../../image.a";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "../../../image.";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no:8000/text////img.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no:8000/test////blog//anotherdepth/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/test////blog//anotherdepth/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/img";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/img.jpg";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/images/nofilepath";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/assets/nofilepath";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "https://localhost.no/assets/nofilepath/helloworld?q=1&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "assets/nofilepath/helloworld?q=1&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "/folder/ASSETS/nofilepath/helloworld?q=1&hello=world";
        expected = true;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "/folder/IMG/nofilepath/helloworld?q=1&hello=world";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "/folder/text.txt/";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);

        data = "/folder/text.txt/?q=h&a=b";
        expected = false;
        Assert.IsTrue(data.IsFile() == expected, "Wrong: " + data);
    }
}
