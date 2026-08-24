using Syncfusion.Maui.Toolkit.Charts;

namespace MauiApp_SproutInAGarden.Pages.Controls
{
    public class LegendExt : ChartLegend
    {
        protected override double GetMaximumSizeCoefficient()
        {
            return 0.5;
        }
    }
}
