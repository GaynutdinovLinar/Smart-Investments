using System.Windows;

namespace Smart_Investments.Views
{
    /// <summary>
    /// Логика взаимодействия для SetIntervalDialogWindiw.xaml
    /// </summary>
    public partial class SetIntervalDialogWindiw : Window
    {
        public SetIntervalDialogWindiw()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
