using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework.App;

public class FrameworkAppTests
{
    [TestMethod]
    public void Start_Does_Not_Throw()
    {
        FrameworkApp.Start();
    }
}
