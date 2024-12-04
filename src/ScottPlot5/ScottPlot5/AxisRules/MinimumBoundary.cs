namespace ScottPlot.AxisRules;

public class MinimumBoundary(IXAxis xAxis, IYAxis yAxis, AxisLimits limits) : IAxisRule
{
    readonly IXAxis XAxis = xAxis;
    readonly IYAxis YAxis = yAxis;
    public AxisLimits Limits { get; set; } = limits;

    public void Apply(RenderPack rp, bool beforeLayout)
    {
        double xSpan = Math.Max(XAxis.Range.Span, Limits.HorizontalSpan);
        double ySpan = Math.Max(YAxis.Range.Span, Limits.VerticalSpan);

        if (XAxis.Range.Max < Limits.Right)
        {
            XAxis.Range.Set(Limits.Right - xSpan, Limits.Right);
        }

        if (XAxis.Range.Min > Limits.Left)
        {
            XAxis.Range.Set(Limits.Left, Limits.Left + xSpan);
        }

        if (YAxis.Range.Max < Limits.Top)
        {
            YAxis.Range.Set(Limits.Top - ySpan, Limits.Top);
        }

        if (YAxis.Range.Min > Limits.Bottom)
        {
            YAxis.Range.Set(Limits.Bottom, Limits.Bottom + ySpan);
        }
    }
}
