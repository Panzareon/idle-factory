
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace IdleFactory.Data
{
  /// <summary>
  /// An data type that allows handling extremely large integer values, without calculating them too accurately (loosing some precisicion).
  /// </summary>
  /// <remarks>
  /// Basically stores the value as <see cref="BaseValue"/> * <see cref="Base"/> ^ <see cref="Exponent"/>.
  /// </remarks>
  public struct LargeInteger(ulong baseValue, long exponent)
  {
    /// <summary>
    /// Not sure if this should use base 10 (for easier display), or base 2 (for easier calculations).
    /// </summary>
    public const int Base = 10;

    /// <summary>
    /// Stores what the maximum exponent for the <see cref="Base"/> would be, that can still be stored in a <see cref="ulong"/>.
    /// </summary>
    public static readonly int MaxExponentInLong;

    /// <summary>
    /// Stores powers of <see cref="Base"/>, with the index being the exponent, from 0 to <see cref="MaxExponentInLong"/>.
    /// </summary>
    public static readonly ulong[] ExponentValues;

    /// <summary>
    /// Initializes some values used for the calculations
    /// </summary>
		static LargeInteger()
    {
      MaxExponentInLong = (int)MathF.Log(ulong.MaxValue, Base);
      ExponentValues = new ulong[MaxExponentInLong + 1];
      ulong value = 1;
      ExponentValues[0] = value;
      for (var i = 1; i <= MaxExponentInLong; i++)
      {
        value *= Base;
        ExponentValues[i] = value;
      }
    }

    /// <summary>
    /// Gets the base value of this integer, that is multiplied with a power of the <see cref="Base"/>.
    /// </summary>
    public ulong BaseValue { get; } = baseValue;

    /// <summary>
    /// Gets the exponent specifying the power of the <see cref="Base"/> that the <see cref="BaseValue"/>
    /// should be multiplied with, to get the actual value.
    /// </summary>
    public long Exponent { get; } = exponent;

    public readonly LargeInteger ToThePower(int exponent)
    {
      LargeInteger result = 1;
      for (var i = 0; i < exponent; i++)
      {
        result = result * this;
      }

      return result;
    }

    public override readonly string ToString()
    {
      if (this.Exponent > 0)
      {
        var targetExponent = MaxExponentInLong;
        for (var i = 0; i <= MaxExponentInLong; i++)
        {
          if (this.BaseValue < ExponentValues[i])
          {
            targetExponent = i - 1;
          }
        }

        var value = this.BaseValue.ToString();
        return $"{value.Substring(0, 1)}.{value.Substring(1)}E{this.Exponent + targetExponent}";
      }

      return this.BaseValue.ToString();
    }

    /// <summary>
    /// Checks if the other <paramref name="obj"/> is a <see cref="LargeInteger"/> with the same value
    /// (currently doesn't consider values with same value but different accuracy).
    /// </summary>
    /// <param name="obj">The other object to check against.</param>
    /// <returns>True, if both objects are equal.</returns>
		public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
      if (obj is not LargeInteger largeInteger)
      {
        return false;
      }

      if (largeInteger.BaseValue == this.BaseValue && largeInteger.Exponent == this.Exponent)
      {
        return true;
      }

      return false;
    }
    public static bool operator >(LargeInteger left, LargeInteger right)
    {
      if (left.Exponent < right.Exponent)
      {
        var exponentDifferent = right.Exponent - left.Exponent;
        if (exponentDifferent > MaxExponentInLong)
        {
          return false;
        }

        return left.BaseValue / ExponentValues[exponentDifferent] > right.BaseValue;
      }
      else
      {
        var exponentDifferent = left.Exponent - right.Exponent;
        if (exponentDifferent > MaxExponentInLong)
        {
          return true;
        }

        return left.BaseValue > right.BaseValue / ExponentValues[exponentDifferent];
      }
    }
    public static bool operator <(LargeInteger left, LargeInteger right)
    {
      return right > left;
    }
    public static bool operator <=(LargeInteger left, LargeInteger right)
    {
      return !(left > right);
    }
    public static bool operator >=(LargeInteger left, LargeInteger right)
    {
      return !(left < right);
    }

    public static implicit operator LargeInteger(ulong value)
    {
      return new LargeInteger(value, 0).EnsureBelowLimit();
    }

    public static implicit operator LargeInteger(uint value)
    {
      return new LargeInteger(value, 0).EnsureBelowLimit();
    }

    public static LargeInteger operator +(LargeInteger left, LargeInteger right)
    {
      if (left.Exponent < right.Exponent)
      {
        return Add(right, left);
      }

      return Add(left, right);

      static LargeInteger Add(LargeInteger left, LargeInteger right)
      {
        if (left.Exponent > right.Exponent + MaxExponentInLong)
        {
          // right value is too small to add to the left, so result is roughly the same as left
          return left;
        }

        right = right.SetExponent(left.Exponent);
        return new LargeInteger(left.BaseValue + right.BaseValue, left.Exponent).EnsureBelowLimit();
      }
    }

    public static LargeInteger operator -(LargeInteger left, LargeInteger right)
    {
      if (left.Exponent > right.Exponent)
      {
        if (left.Exponent > right.Exponent + MaxExponentInLong)
        {
          // right value is too small to remove from the left, so result is roughly the same as left
          return left;
        }

        right = right.SetExponent(left.Exponent);
        return new LargeInteger(left.BaseValue - right.BaseValue, left.Exponent).EnsureBelowLimit();
      }
      else
      {
        if (right > left)
        {
          Debug.Fail("LargeInteger does not support negative values");
          return 0;
        }

        left = left.SetExponent(right.Exponent);
        return new LargeInteger(left.BaseValue - right.BaseValue, left.Exponent).EnsureBelowLimit();
      }
    }

    public static LargeInteger operator *(LargeInteger left, LargeInteger right)
    {
      return new LargeInteger(left.BaseValue, left.Exponent + right.Exponent) * right.BaseValue;
    }

    public static LargeInteger operator *(LargeInteger left, ulong right)
    {
      if (right == 0)
      {
        return new LargeInteger(0, 0);
      }

      var maxTarget = MaxExponentInLong / 2;
      int maxExponent = 0;

      // Check if this can be multiplied without reducing accuracy of one side
      for (var i = 1; i <= maxTarget; i++)
      {
        var currentTarget = ExponentValues[i];
        if (currentTarget > right)
        {
          maxExponent = MaxExponentInLong - i;
          left = left.ReduceAccuracy(maxExponent);

          return new LargeInteger(left.BaseValue * right, left.Exponent);
        }

        if (currentTarget > left.BaseValue)
        {
          maxExponent = MaxExponentInLong - i;
          return ReduceAccuracyAndMultiply(left, right, maxExponent);
        }
      }

      // Both values (without the exponent) are too big, to allow multiplying them without loosing
      left = left.ReduceAccuracy(maxTarget);
      return ReduceAccuracyAndMultiply(left, right, maxTarget);

      static LargeInteger ReduceAccuracyAndMultiply(LargeInteger left, ulong right, int maxExponent)
      {
        var targetValue = ExponentValues[maxExponent];
        var exponent = left.Exponent;
        while (right > targetValue)
        {
          right /= Base;
          exponent++;
        }

        return new LargeInteger(left.BaseValue * right, exponent);
      }
    }

    /// <summary>
    /// Ensures that the value is small enough, that simple operations can be used without causing an overflow.
    /// </summary>
    /// <returns>A copy of this with reduced accuracy, if the <see cref="BaseValue"/> is too big.</returns>
    private readonly LargeInteger EnsureBelowLimit()
    {
      if (this.BaseValue > ExponentValues[MaxExponentInLong - 1])
      {
        return this.ReduceAccuracy();
      }

      return this;
    }

    private readonly LargeInteger ReduceAccuracy()
    {
      return new LargeInteger(this.BaseValue / Base, this.Exponent + 1);
    }

    private readonly LargeInteger ReduceAccuracy(int maxExponent)
    {
      var targetValue = ExponentValues[maxExponent];
      var current = this;
      while (current.BaseValue > targetValue)
      {
        current = current.ReduceAccuracy();
      }

      return current;
    }

    private readonly LargeInteger SetExponent(long targetExponent)
    {
      var multiplier = ExponentValues[targetExponent - this.Exponent];
      return new LargeInteger(this.BaseValue / multiplier, targetExponent);
    }

    public override readonly int GetHashCode()
    {
      return HashCode.Combine(this.BaseValue, this.Exponent);
    }

    public static bool operator ==(LargeInteger left, LargeInteger right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(LargeInteger left, LargeInteger right)
    {
      return !(left == right);
    }
  }
}
