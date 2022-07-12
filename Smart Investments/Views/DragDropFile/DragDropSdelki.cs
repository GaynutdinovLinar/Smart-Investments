using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using Smart_Investments.Services;
using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Services.LocationDataBaseService;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Smart_Investments.Views.DragDropFile
{
    class DragDropSdelki : IDropTarget, INotifyPropertyChanged
    {
        public DragDropSdelki(TableOperations tableOperations)
        {
            _tableOperations = tableOperations;

            Message = "Отчет брокера Сбербанк Инвестор \"Сделки\"";
        }

        private readonly TableOperations _tableOperations;

        private string _pathFile;

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }


        public void DragOver(IDropInfo dropInfo)
        {

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

            var dataObject = dropInfo.Data as DataObject;


            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                var files = dataObject.GetFileDropList();

                if (files != null && files.Count == 1)
                {
                    var type = Path.GetExtension(files[0]);

                    if (type == ".xlsx")
                    {
                        _pathFile = files[0];
                        dropInfo.Effects = DragDropEffects.Move;
                    }
                }
            }
            else dropInfo.Effects = DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {

            if (_tableOperations.DropFile(_pathFile)) Message = "Файл успешно добавлен";
            else Message = "Данный файл не подходит";

        }

        #region Commands

        public ICommand Button_Click
        {
            get => new DelegateCommand((obj) =>
            {
                OpenFileDialog myDialog = new OpenFileDialog();
                myDialog.Filter = "xlsx - файлы(*.xlsx) | *.xlsx";

                myDialog.CheckPathExists = true;
                myDialog.CheckFileExists = true;

                if (myDialog.ShowDialog() == true)
                {
                    if (_tableOperations.DropFile(myDialog.FileName)) Message = "Файл успешно добавлен";
                    else Message = "Данный файл не подходит";
                }
            });
        }

        #endregion


        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
