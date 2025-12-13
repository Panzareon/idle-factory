using IdleFactory.Data;

namespace IdleFactory.Tests.Data
{
  internal class LargeIntegerTests
  {
    [Test]
    public void AddLargeIntegerTests()
    {
      var value1 = LargeInteger.Create(1234, 5);
      var value2 = LargeInteger.Create(1234, 4);
      var expectedResult = LargeInteger.Create(13574, 4);

      Assert.That(value1 + value2, Is.EqualTo(expectedResult));
      Assert.That(value2 + value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void AddAboveMaxLong()
    {
      LargeInteger value1 = long.MaxValue;
      LargeInteger value2 = long.MaxValue;
      var expectedResult = LargeInteger.Create(1844674407370955, 4);

      Assert.That(value1 + value2, Is.EqualTo(expectedResult));
      Assert.That(value2 + value1, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void SubtractLargeIntegerTests()
    {
      var value1 = LargeInteger.Create(1234, 5);
      var value2 = LargeInteger.Create(1357, 5);
      var expectedResult = LargeInteger.Create(123, 5);

      Assert.That(value2 - value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void SubtractAboveMaxLong()
    {
      LargeInteger value1 = long.MaxValue;
      var value2 = LargeInteger.Create(368_934_881_474_191_032, 2);
      var expectedResult = LargeInteger.Create(276_701_161_105_643_274, 2);

      Assert.That(value2 - value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultiplyTest()
    {
      var value1 = LargeInteger.Create(100_000_000_000_000_000, 15);
      var value2 = LargeInteger.Create(100_000_000_000_000_000, 10);

      var expectedResult = LargeInteger.Create(1_000_000_000_000_000_000, 41);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult));
      Assert.That(value2 * value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultiplyRandomValuesTest()
    {
      var value1 = LargeInteger.Create(258_942_578_434_345_463, 15);
      var value2 = LargeInteger.Create(234_456_789_675_534_244, 15);

      var expectedResult = LargeInteger.Create(6071084565002191, 49);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult));
      Assert.That(value2 * value1, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultipleWithDoubleTest()
    {
      var value1 = LargeInteger.Create(100, 10);
      var value2 = 100.0;

      var expectedResult = LargeInteger.Create(100_000_000_000_000, 0);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult), $"{value1} * {value2} should be {expectedResult}, but was {value1 * value2}");
    }

    [Test]
    public void MultipleWithSmallDoubleTest()
    {
      var value1 = LargeInteger.Create(100, 10);
      var value2 = 0.0001;

      var expectedResult = LargeInteger.Create(100_000_000, 0);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult), $"{value1} * {value2} should be {expectedResult}, but was {value1 * value2}");
    }


    [Test]
    public void MultiplyLargeValuesTest()
    {
      var value1 = LargeInteger.Create(258_942_578_434_345_463, 15);
      var value2 = 234_456_789_675_534_244.0;

      var expectedResult = LargeInteger.Create(6071084565002190, 34);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult), $"{value1} * {value2} should be {expectedResult}, but was {value1*value2}");
    }


    [Test]
    public void MultiplyLargeValueWithSmallValueTest()
    {
      var value1 = LargeInteger.Create(258_942_578_434_345_463, 15);
      var value2 = 0.000_000_234_456_789_675_534_244;

      var expectedResult = LargeInteger.Create(6071084565002188, 10);
      Assert.That(value1 * value2, Is.EqualTo(expectedResult), $"{value1} * {value2} should be {expectedResult}, but was {value1*value2}");
    }

    [Test]
    public void IsLargerTest()
    {
      var value1 = LargeInteger.Create(1000, 1);
      var value2 = LargeInteger.Create(1000, 0);

      Assert.That(value1 > value2, Is.True);
      Assert.That(value1 < value2, Is.False);
      Assert.That(value1 >= value2, Is.True);
      Assert.That(value1 <= value2, Is.False);
    }

    [Test]
    public void IsLargerWithDifferenceInBaseValueTest()
    {
      var value1 = LargeInteger.Create(1000, 2);
      var value2 = LargeInteger.Create(100100, 0);

      Assert.That(value1 > value2, Is.False);
      Assert.That(value1 < value2, Is.True);
      Assert.That(value1 >= value2, Is.False);
      Assert.That(value1 <= value2, Is.True);
    }

    [Test]
    public void IsLargerWithEqualValuesTest()
    {
      var value1 = LargeInteger.Create(1000, 2);
      var value2 = LargeInteger.Create(100000, 0);

      Assert.That(value1 > value2, Is.False);
      Assert.That(value1 < value2, Is.False);
      Assert.That(value1 >= value2, Is.True);
      Assert.That(value1 <= value2, Is.True);
    }
  }
}
