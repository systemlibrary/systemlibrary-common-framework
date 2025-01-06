using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Extensions;
using SystemLibrary.Common.Framework.Net.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class ListStringExtensionsTests : BaseTest
{
    [TestMethod]
    public void ToEnumList_With_List_Converts_Successfully()
    {
        List<string> data = null;
        var result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 0, "Count is not 0 from null list");

        data = new List<string>();
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 0, "Count is not 0 from empty list");

        data = new List<string> { "Red", "Blue", "Green" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 3, "Wrong count " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red);
        Assert.IsTrue(result[1] == BackgroundColor.Blue);
        Assert.IsTrue(result[2] == BackgroundColor.Green);

        data = new List<string> { "RED", "BLUE", "GREEN" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 3, "Wrong count 3: " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red);
        Assert.IsTrue(result[1] == BackgroundColor.Blue);
        Assert.IsTrue(result[2] == BackgroundColor.Green);

        data = new List<string> { null, null, null, "Red", "Yellow" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 5, "Wrong count 5: " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red);
        Assert.IsTrue(result[1] == BackgroundColor.Red);
        Assert.IsTrue(result[2] == BackgroundColor.Red);
        Assert.IsTrue(result[3] == BackgroundColor.Red);
        Assert.IsTrue(result[4] == BackgroundColor.yellow);

        data = new List<string> { "", "Unknown", "BrownNotExisting", "Orange" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 4, "Wrong count 4: " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red, "First is not car2");
        Assert.IsTrue(result[1] == BackgroundColor.Red, "Second is not car1");
        Assert.IsTrue(result[2] == BackgroundColor.Red);
        Assert.IsTrue(result[3] == BackgroundColor.Orange);
    }

    [TestMethod]
    public void ToEnumList_With_IList_Converts_Successfully()
    {
        IList<string> data = null;
        var result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 0, "Count is not 0 from null list");

        data = new List<string> { "Red", "Blue", "Green" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 3, "Wrong count " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red);
        Assert.IsTrue(result[1] == BackgroundColor.Blue);
        Assert.IsTrue(result[2] == BackgroundColor.Green);

        data = new List<string> { "RED", "BLUE", "GREEN" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 3, "Wrong count 3: " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red);
        Assert.IsTrue(result[1] == BackgroundColor.Blue);
        Assert.IsTrue(result[2] == BackgroundColor.Green);

        data = new List<string> { null, null, null, "Red", "Yellow" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 5, "Wrong count 5: " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red);
        Assert.IsTrue(result[1] == BackgroundColor.Red);
        Assert.IsTrue(result[2] == BackgroundColor.Red);
        Assert.IsTrue(result[3] == BackgroundColor.Red);
        Assert.IsTrue(result[4] == BackgroundColor.yellow);

        data = new List<string> { "", "Unknown", "BrownNotExisting", "Orange" };
        result = data.ToEnumList<BackgroundColor>();
        Assert.IsTrue(result.Count == 4, "Wrong count 4: " + result.Count);
        Assert.IsTrue(result[0] == BackgroundColor.Red, "First is not car2");
        Assert.IsTrue(result[1] == BackgroundColor.Red, "Second is not car1");
        Assert.IsTrue(result[2] == BackgroundColor.Red);
        Assert.IsTrue(result[3] == BackgroundColor.Orange);
    }
}
