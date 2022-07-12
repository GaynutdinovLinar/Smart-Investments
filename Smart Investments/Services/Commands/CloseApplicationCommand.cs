using Smart_Investments.Services.Commands.Base;

namespace Smart_Investments.Services.Commands
{
    class CloseApplicationCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter) => App.Current.Shutdown();
    }
}
