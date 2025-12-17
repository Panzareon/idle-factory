
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
  public struct LargeInteger
  {
    /// <summary>
    /// Not sure if this should use base 10 (for easier display), or base 2 (for easier calculations).
    /// </summary>
    public const int Base = 10;
    private const int MaxDigitsToShowWithoutScientificNotation = 6;
    public static readonly int MaxPrecisionExponent = 15;

    public static readonly double MaxPrecision;

    /// <summary>
    /// Stores powers of <see cref="Base"/>, with the index being the exponent, from 0 to <see cref="MaxExponentInLong"/>.
    /// </summary>
    public static readonly ulong[] ExponentValues;

    /// <summary>
    /// Initializes some values used for the calculations
    /// </summary>
		static LargeInteger()
    {
      ExponentValues = new ulong[MaxPrecisionExponent + 1];
      ulong value = 1;
      ExponentValues[0] = value;
      for (var i = 1; i <= MaxPrecisionExponent; i++)
      {
        value *= Base;
        ExponentValues[i] = value;
      }

      MaxPrecision = 1.0 / ExponentValues[MaxPrecisionExponent];
    }

    private LargeInteger(double baseValue, long exponent)
    {
      this.BaseValue = baseValue;
      this.Exponent = exponent;
    }

    public static LargeInteger Create(double baseValue, long exponent)
    {
      if (baseValue == 0)
      {
        exponent = 0;
      }
      else if (baseValue > 0)
      {
        while (baseValue >= Base)
        {
          baseValue /= Base;
          exponent++;
        }

        while (baseValue < 1)
        {
          baseValue *= Base;
          exponent--;
        }
      }
      else
      {
        while (baseValue <= -Base)
        {
          baseValue /= Base;
          exponent++;
        }

        while (baseValue > -1)
        {
          baseValue *= Base;
          exponent--;
        }
      }

      if (exponent < 0)
      {
        return new LargeInteger(0, 0);
      }

      if (exponent < MaxPrecisionExponent)
      {
        baseValue = Math.Round(baseValue, (int)exponent);
      }

      return new LargeInteger(baseValue, exponent);
    }

    /// <summary>
    /// Gets the base value of this integer, that is multiplied with a power of the <see cref="Base"/>.
    /// </summary>
    public double BaseValue { get; }

    /// <summary>
    /// Gets the exponent specifying the power of the <see cref="Base"/> that the <see cref="BaseValue"/>
    /// should be multiplied with, to get the actual value.
    /// </summary>
    public long Exponent { get; }

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
      return this.ToString(MaxPrecisionExponent);
    }

    public readonly string ToString(int maxPrecisionForScientificNotation)
    {
      if (this.Exponent > MaxDigitsToShowWithoutScientificNotation)
      {
        if (maxPrecisionForScientificNotation > this.Exponent)
        {
          maxPrecisionForScientificNotation = (int)this.Exponent;
        }

        return $"{this.BaseValue.ToString($"F{maxPrecisionForScientificNotation}")}E{this.Exponent}";
      }

      return (this.BaseValue * Math.Pow(Base, this.Exponent)).ToString("F0");
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

      var maxPrecision = MaxPrecision;
      if (this.Exponent < MaxPrecisionExponent)
      {
        maxPrecision = 1.0 / ExponentValues[this.Exponent];
      }

      if (largeInteger.BaseValue + maxPrecision >= this.BaseValue
        && largeInteger.BaseValue - maxPrecision <= this.BaseValue
        && largeInteger.Exponent == this.Exponent)
      {
        return true;
      }

      return false;
    }
    public static bool operator >(LargeInteger left, LargeInteger right)
    {
      if (left.Exponent < right.Exponent)
      {
        return false;
      }
      else if (left.Exponent > right.Exponent)
      {
        return true;
      }
      else
      {
        return left.BaseValue > right.BaseValue;
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

    public static implicit operator LargeInteger(long value)
    {
      return Create(value, 0);
    }

    public static implicit operator LargeInteger(int value)
    {
      return Create(value, 0);
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
        if (left.Exponent > right.Exponent + MaxPrecisionExponent)
        {
          // right value is too small to add to the left, so result is roughly the same as left
          return left;
        }

        right = right.SetExponent(left.Exponent);
        return Create(left.BaseValue + right.BaseValue, left.Exponent);
      }
    }

    public static LargeInteger operator -(LargeInteger left, LargeInteger right)
    {
      if (left.Exponent > right.Exponent)
      {
        if (left.Exponent > right.Exponent + MaxPrecisionExponent)
        {
          // right value is too small to remove from the left, so result is roughly the same as left
          return left;
        }

        right = right.SetExponent(left.Exponent);
        return Create(left.BaseValue - right.BaseValue, left.Exponent);
      }
      else
      {
        if (right > left)
        {
          Debug.Fail("LargeInteger does not support negative values");
          return 0;
        }

        left = left.SetExponent(right.Exponent);
        return Create(left.BaseValue - right.BaseValue, left.Exponent);
      }
    }

    public static LargeInteger operator *(LargeInteger left, LargeInteger right)
    {
      return Create(left.BaseValue * right.BaseValue, left.Exponent + right.Exponent);
    }

    public static LargeInteger operator *(LargeInteger left, long right)
    {
      return Create(left.BaseValue * right, left.Exponent);
    }

    public static LargeInteger operator *(LargeInteger left, int right)
    {
      return Create(left.BaseValue * right, left.Exponent);
    }

    public static LargeInteger operator *(LargeInteger left, double right)
    {
      return Create(left.BaseValue * right, left.Exponent);
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

    private readonly LargeInteger SetExponent(long targetExponent)
    {
      var multiplier = ExponentValues[targetExponent - this.Exponent];
      return new LargeInteger(this.BaseValue / multiplier, targetExponent);
    }
  }
}
