using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.Tests;

public class FrameworkAppTests
{
    [TestMethod]
    public void Start_Does_Not_Throw()
    {
        FrameworkApp.Start();
    }

}
