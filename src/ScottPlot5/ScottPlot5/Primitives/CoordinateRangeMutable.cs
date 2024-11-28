namespace ScottPlot;

// TODO: strangle this class and replace with CoordinateRangeStruct

/// <summary>
/// Represents a range of values between two coordinates on a single axis
/// </summary>
public sealed class CoordinateRangeMutable(double min, double max) : IEquatable<CoordinateRangeMutable> // TODO: rename to MutableCoordinateRange or something
{
    public double Min { get; set; } = min;
    public double Max { get; set; } = max;

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

    /// <summary>
    /// Returns true if the given position is within the range (inclusive)
    /// </summary>
    public bool Contains(double value)
    {
        return TrueMin <= value && value <= TrueMax;
    }

    // TODO: deprecate
    /// <summary>
    /// Expand the range if needed to include the given point
    /// </summary>
    public void Expand(double value)
    {
        if (double.IsNaN(value))
            return;

        if (double.IsNaN(Min) || value < Min)
            Min = value;

        if (double.IsNaN(Max) || value > Max)
            Max = value;
    }

    // TODO: deprecate
    /// <summary>
    /// Expand this range if needed to ensure the given range is included
    /// </summary>
    public void Expand(CoordinateRangeMutable range)
    {
        Expand(range.Min);
        Expand(range.Max);
    }

    /// <summary>
    /// Expand this range if needed to ensure the given range is included
    /// </summary>
    public void Expand(CoordinateRange range)
    {
        Expand(range.Min);
        Expand(range.Max);
    }

    /// <summary>
    /// This magic value is used to indicate the range has not been set.
    /// It is equal to an inverted infinite range [∞, -∞]
    /// </summary>
    public static CoordinateRangeMutable NotSet => new(double.PositiveInfinity, double.NegativeInfinity);

    // TODO: deprecate
    /// <summary>
    /// Reset this range to inverted infinite values to indicate the range has not yet been set
    /// </summary>
    public void Reset()
    {
        Min = double.PositiveInfinity;
        Max = double.NegativeInfinity;
        if (HasBeenSet)
            throw new InvalidOperationException();
    }

    public void Set(double min, double max)
    {
        Min = min;
        Max = max;
    }

    public void Set(CoordinateRange range)
    {
        if (range.IsInverted)
        {
            Max = range.Min;
            Min = range.Max;
        }
        else
        {
            Min = range.Min;
            Max = range.Max;
        }
    }

    public void Set(CoordinateRangeMutable range)
    {
        Min = range.Min;
        Max = range.Max;
    }

    public void Set(IAxis otherAxis)
    {
        Min = otherAxis.Min;
        Max = otherAxis.Max;
    }

    public void Pan(double delta)
    {
        Min += delta;
        Max += delta;
    }

    public void PanMouse(float mouseDeltaPx, float dataSizePx)
    {
        double pxPerUnitx = dataSizePx / Span;
        double delta = mouseDeltaPx / pxPerUnitx;
        Pan(delta);
    }

    public void ZoomFrac(double frac)
    {
        ZoomFrac(frac, Center);
    }

    public void ZoomOut(double multiple)
    {
        double newSpan = Span * multiple;
        double halfSpan = newSpan / 2;
        Set(Center - halfSpan, Center + halfSpan);
    }

    public void ZoomMouseDelta(float deltaPx, float dataSizePx)
    {
        double deltaFracX = deltaPx / (Math.Abs(deltaPx) + dataSizePx);
        double fracX = Math.Pow(10, deltaFracX);
        ZoomFrac(fracX);
    }

    public void ZoomFrac(double frac, double zoomTo)
    {
        double spanLeftX = zoomTo - Min;
        double spanRightX = Max - zoomTo;
        Min = zoomTo - spanLeftX / frac;
        Max = zoomTo + spanRightX / frac;
    }

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
