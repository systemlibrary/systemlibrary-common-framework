using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Framework.Tests;

namespace SystemLibrary.Common.Framework;

[TestClass]
public class RandomnessTests : BaseTest
{
    [TestMethod]
    public void Double_Returns_Random_Double_Within_Range_And_Next_10_Still_Differs_From_Initial()
    {
        // NOTE: test might crash as there is a 1 in a billion chance that test2 is equal to test1
        var test1 = Randomness.Double(1.01, 3.99);

        Assert.IsTrue(test1 > 1 && test1 < 4);

        for (int i = 0; i < 10; i++)
        {
            var test2 = Randomness.Double(1.01, 3.99);
            Assert.IsTrue(test1 != test2);
        }
    }

    [TestMethod]
    public void Int_Returns_Random_Integer_Based_On_Input_Rules()
    {
        var test1 = Randomness.Int();
        var test2 = Randomness.Int();
        var test3 = Randomness.Int(2);
        var test4 = Randomness.Int(2);
        var test5 = Randomness.Int(2);
        var test6 = Randomness.Int(50, 50);

        Assert.IsTrue(test1 - test2 > 1 || test1 - test2 < -1, "Both numbers were equal " + test1 + ", unlucky?");

        Assert.IsTrue(test3 < 3);
        Assert.IsTrue(test4 < 3);
        Assert.IsTrue(test5 < 3);
        Assert.IsTrue(test6 == 50);
    }

    [TestMethod]
    public void String_Returns_Random_String_Based_On_Input_Rules()
    {
        var test1 = Randomness.String();
        var test2 = Randomness.String();
        var test3 = Randomness.String(2);

        Assert.IsTrue(test1.Length == 6);
        Assert.IsTrue(test1 != test2);
        Assert.IsTrue(test3.Length == 2);
    }

    [TestMethod]
    public void Bytes_Returns_Random_Bytes_Based_On_Input_Rules()
    {
        var test1 = Randomness.Bytes();
        var test2 = Randomness.Bytes();
        var test3 = Randomness.Bytes(2);

        Assert.IsTrue(test1.Length == 16);
        Assert.IsTrue(test2.Length == 16);
        Assert.IsTrue(test3.Length == 2);

        Assert.IsTrue(test1[0] > 0 || test2[0] > 0 || test3[0] > 0);
    }
}