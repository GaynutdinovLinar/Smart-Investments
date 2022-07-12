using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Services.LocationDataBaseService;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Smart_Investments.Views.DragDropFile
{
    class DragDropDepositsAndDebits : IDropTarget, INotifyPropertyChanged
    {
        public DragDropDepositsAndDebits(TableDepositsAndDebits tableDepositsAndDebits)
        {
            _tableDepositsAndDebits = tableDepositsAndDebits;

            Message = "Отчет брокера Сбербанк Инвестор \"Зачисления и списания\"";
        }

        private readonly TableDepositsAndDebits _tableDepositsAndDebits;

        private string _pathFile;

        public string PathFile
        {
            get
            {
                return _pathFile;
            }
            set
            {
                _pathFile = value;
            }
        }

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
                        PathFile = files[0];
                        dropInfo.Effects = DragDropEffects.Move;
                    }
                }
            }
            else dropInfo.Effects = DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (_tableDepositsAndDebits.DropFile(_pathFile)) Message = "Файл успешно добавлен";
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
                    if (_tableDepositsAndDebits.DropFile(myDialog.FileName)) Message = "Файл успешно добавлен";
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
