using NUnit.Framework;
using Quartzified.QuickKeep;
using System;

public class QuickTests
{
    [Test]
    public void TestBool()
    {
        QuickKeep.SetBool("BoolTest", true, "UnitTests");

        Assert.AreEqual(true, QuickKeep.GetBool("BoolTest", "UnitTests"));
    }

    [Test]
    public void TestByte()
    {
        QuickKeep.SetByte("ByteTest", 205, "UnitTests");

        Assert.AreEqual(205, QuickKeep.GetByte("ByteTest", "UnitTests"));
    }

    [Test]
    public void TestShort()
    {
        QuickKeep.SetShort("ShortTest", 32222, "UnitTests");

        Assert.AreEqual(32222, QuickKeep.GetShort("ShortTest", "UnitTests"));
    }

    [Test]
    public void TestInteger()
    {
        QuickKeep.SetInt("IntTest", 422712, "UnitTests");

        Assert.AreEqual(422712, QuickKeep.GetInt("IntTest", "UnitTests"));
    }

    [Test]
    public void TestLong()
    {
        QuickKeep.SetLong("LongTest", 643839284843, "UnitTests");

        Assert.AreEqual(643839284843, QuickKeep.GetLong("LongTest", "UnitTests"));
    }

    [Test]
    public void TestString()
    {
        QuickKeep.SetString("StringTest_1", "Quartzi is very Epic!", "UnitTests");

        Assert.AreEqual("Quartzi is very Epic!", QuickKeep.GetString("StringTest_1", "UnitTests"));
        Assert.AreNotEqual("Quartzi is very Epic", QuickKeep.GetString("StringTest_1", "UnitTests"));
        Assert.AreNotEqual("quartzi is very Epic!", QuickKeep.GetString("StringTest_1", "UnitTests"));
    }

    [Test]
    public void TestFloat()
    {
        QuickKeep.SetFloat("FloatTest", 643.0145f, "UnitTests");

        Assert.AreEqual(643.0145f, QuickKeep.GetFloat("FloatTest", "UnitTests"));
    }

    [Test]
    public void TestDouble()
    {
        QuickKeep.SetDouble("DoubleTest_1", 236495.128, "UnitTests");
        QuickKeep.SetDouble("DoubleTest_2", 62e46, "UnitTests");


        Assert.AreEqual(236495.128, QuickKeep.GetDouble("DoubleTest_1", "UnitTests"));
        Assert.AreEqual(62e46, QuickKeep.GetDouble("DoubleTest_2", "UnitTests"));
    }

    [Test]
    public void TestDateTime()
    {
        DateTime dt = DateTime.UtcNow;
        DateTime dtEpoch = DateTime.UnixEpoch;

        QuickKeep.SetDateTime("DateTimeTest_1", dt, "UnitTests");
        QuickKeep.SetDateTime("DateTimeTest_2", dtEpoch, "UnitTests");

        Assert.AreEqual(dt, QuickKeep.GetDateTime("DateTimeTest_1", "UnitTests"));
        Assert.AreEqual(dtEpoch, QuickKeep.GetDateTime("DateTimeTest_2", "UnitTests"));
    }

    [Test]
    public void TestAllData()
    {
        string[] data = new string[] { "Alpha", "Beta", "Charlie", "Delta", "Enemy", "Friendly" };

        QuickKeep.SetAllData(data, "AllDataTests");

        string[] readData = QuickKeep.GetAllData("AllDataTests");

        Assert.AreEqual("Alpha", readData[0]);
        Assert.AreEqual("Beta", readData[1]);
        Assert.AreEqual("Charlie", readData[2]);
        Assert.AreNotEqual("Delta", readData[4]);

        Assert.AreEqual("Enemy", QuickKeep.GetAllData("AllDataTests")[4]);
        Assert.NotNull(QuickKeep.GetAllData("AllDataTests")[5]);
    }
}
