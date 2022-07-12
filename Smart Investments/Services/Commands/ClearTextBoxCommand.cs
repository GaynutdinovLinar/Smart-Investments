using Smart_Investments.Services.Commands.Base;

namespace Smart_Investments.Services.Commands
{
    class ClearTextBoxCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)parameter;

            tb.Text = default;
        }
    }
}
