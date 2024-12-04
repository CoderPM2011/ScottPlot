namespace ScottPlot;

// TODO: strangle this class and replace with CoordinateRangeStruct

/// <summary>
/// Represents a range of values between two coordinates on a single axis
/// </summary>
public sealed class CoordinateRangeMutable(double min, double max) : IEquatable<CoordinateRangeMutable> // TODO: rename to MutableCoordinateRange or something
{
    public double Min { get; private set; } = min;
    public double Max { get; private set; } = max;

    public double TrueMin => Math.Min(Min, Max);
    public double TrueMax => Math.Max(Min, Max);
    public bool IsInverted => Min > Max;

    /// <summary>
    /// Distance from <see cref="Min"/> to <see cref="Max"/> (may be negative)
    /// </summary>
    public double Span => Max - Min;

    /// <summary>
    /// Value located in the center of the range, between <see cref="Min"/> and <see cref="Max"/> (may be negative)
    /// </summary>
    public double Center => (Min + Max) / 2;

    /// <summary>
    /// Distance from <see cref="Min"/> to <see cref="Max"/> (always positive)
    /// </summary>
    public double Length => Math.Abs(Span);

    // TODO: obsolete this
    public bool HasBeenSet
        => Min != double.PositiveInfinity && Max != double.NegativeInfinity;

    public CoordinateRange ToCoordinateRange => new(Min, Max);

    public override string ToString()
    {
        return IsInverted
            ? $"CoordinateRangeMutable [{TrueMin}, {TrueMax}] (inverted)"
            : $"CoordinateRangeMutable [{TrueMin}, {TrueMax}]";
    }

    // TODO: deprecate
    /// <summary>
    /// Reset this range to inverted infinite values to indicate the range has not yet been set
    /// </summary>
    public void Reset()
    {
        Set(double.PositiveInfinity, double.NegativeInfinity);
        if (HasBeenSet)
            throw new InvalidOperationException();
    }

    public void Set(double min, double max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Returns true if the given position is within the range (inclusive)
    /// </summary>
    public bool Contains(double value)
    {
        return TrueMin <= value && value <= TrueMax;
    }

    /// <summary>
    /// This magic value is used to indicate the range has not been set.
    /// It is equal to an inverted infinite range [∞, -∞]
    /// </summary>
    public static CoordinateRangeMutable NotSet => new(double.PositiveInfinity, double.NegativeInfinity);

    public bool Equals(CoordinateRangeMutable? other)
    {
        if (other is null)
            return false;

        return Equals(Min, other.Min) && Equals(Max, other.Max);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is CoordinateRangeMutable other)
            return Equals(other);

        return false;
    }

    public static bool operator ==(CoordinateRangeMutable a, CoordinateRangeMutable b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(CoordinateRangeMutable a, CoordinateRangeMutable b)
    {
        return !a.Equals(b);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Min.GetHashCode();
        hash = hash * 23 + Max.GetHashCode();
        return hash;
    }
}

public static class CoordinateRangeMutableHelper
{
    // TODO: deprecate
    /// <summary>
    /// Expand the range if needed to include the given point
    /// </summary>
    public static void Expand(this CoordinateRangeMutable range, double value)
    {
        if (double.IsNaN(value))
            return;

        double min = range.Min;
        if (double.IsNaN(range.Min) || value < range.Min)
            min = value;

        double max = range.Max;
        if (double.IsNaN(range.Max) || value > range.Max)
            max = value;

        range.Set(min, max);
    }

    // TODO: deprecate
    /// <summary>
    /// Expand this range if needed to ensure the given range is included
    /// </summary>
    public static void Expand(this CoordinateRangeMutable range, CoordinateRangeMutable newRange)
    {
        range.Expand(newRange.Min);
        range.Expand(newRange.Max);
    }

    /// <summary>
    /// Expand this range if needed to ensure the given range is included
    /// </summary>
    public static void Expand(this CoordinateRangeMutable range, CoordinateRange newRange)
    {
        range.Expand(newRange.Min);
        range.Expand(newRange.Max);
    }

    public static void Set(this CoordinateRangeMutable range, CoordinateRange newRange)
    {
        if (newRange.IsInverted)
        {
            range.Set(newRange.Max, newRange.Min);
        }
        else
        {
            range.Set(newRange.Min, newRange.Max);
        }
    }

    public static void Set(this CoordinateRangeMutable range, CoordinateRangeMutable newRange)
    {
        range.Set(newRange.Min, newRange.Max);
    }

    public static void Set(this CoordinateRangeMutable range, IAxis otherAxis)
    {
        range.Set(otherAxis.Min, otherAxis.Max);
    }

    public static void Pan(this CoordinateRangeMutable range, double delta)
    {
        range.Set(range.Min + delta, range.Max + delta);
    }

    public static void PanMouse(this CoordinateRangeMutable range, float mouseDeltaPx, float dataSizePx)
    {
        double pxPerUnitx = dataSizePx / range.Span;
        double delta = mouseDeltaPx / pxPerUnitx;
        range.Pan(delta);
    }

    public static void ZoomFrac(this CoordinateRangeMutable range, double frac)
    {
        range.ZoomFrac(frac, range.Center);
    }

    public static void ZoomOut(this CoordinateRangeMutable range, double multiple)
    {
        double newSpan = range.Span * multiple;
        double halfSpan = newSpan / 2;
        range.Set(range.Center - halfSpan, range.Center + halfSpan);
    }

    public static void ZoomMouseDelta(this CoordinateRangeMutable range, float deltaPx, float dataSizePx)
    {
        double deltaFracX = deltaPx / (Math.Abs(deltaPx) + dataSizePx);
        double fracX = Math.Pow(10, deltaFracX);
        range.ZoomFrac(fracX);
    }

    public static void ZoomFrac(this CoordinateRangeMutable range, double frac, double zoomTo)
    {
        double spanLeftX = zoomTo - range.Min;
        double spanRightX = range.Max - zoomTo;
        range.Set(zoomTo - spanLeftX / frac, zoomTo + spanRightX / frac);
    }
}
