using ScottPlot;

namespace Sandbox.WinForms;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        Coordinates[] points =
        {
            new (0, 0.25),
            new (0.3, 0.75),
            new (0.7, 0.5),
            new (1, 1),
        };

        var poly = formsPlot1.Plot.Add.Scatter(points);
        poly.FillY = true;
        poly.FillYColor = Colors.Green;
    }
}
