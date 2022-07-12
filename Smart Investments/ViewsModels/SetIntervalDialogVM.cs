using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Smart_Investments.ViewsModels
{
    public class SetIntervalDialogVM : INotifyPropertyChanged
    {


        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
