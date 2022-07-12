using Smart_Investments.ViewsModels;
using System.Windows;

namespace Smart_Investments
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            VMLocator.Init();

            base.OnStartup(e);
        }
    }
}
