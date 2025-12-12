using IdleFactory.Data;

namespace IdleFactory.Tests.Data
{
  internal class LargeIntegerTests
  {
    [Test]
    public void AddLargeIntegerTests()
    {
      var value1 = new LargeInteger(1234, 5);
      var value2 = new LargeInteger(1234, 4);
      var expectedResult = new LargeInteger(1357, 5);

      Assert.That(value1 + value2, Is.EqualTo(expectedResult));
      Assert.That(value2 + value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void AddAboveMaxLong()
    {
      LargeInteger value1 = ulong.MaxValue;
      LargeInteger value2 = ulong.MaxValue;
      var expectedResult = new LargeInteger(368_934_881_474_191_032, 2);

      Assert.That(value1 + value2, Is.EqualTo(expectedResult));
      Assert.That(value2 + value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultiplyTest()
    {
      var value1 = new LargeInteger(100_000_000_000_000_000, 15);
      var value2 = new LargeInteger(100_000_000_000_000_000, 10);

      var expectedResult = new LargeInteger(1_000_000_000_000_000_000, 41);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult));
      Assert.That(value2 * value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultiplyRandomValuesTest()
    {
      var value1 = new LargeInteger(258_942_578_434_345_463, 15);
      var value2 = new LargeInteger(234_456_789_675_534_244, 15);

      var expectedResult = new LargeInteger(60_710_845_373_262_042, 48);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult));
      Assert.That(value2 * value1, Is.EqualTo(expectedResult));
    }
  }
}
