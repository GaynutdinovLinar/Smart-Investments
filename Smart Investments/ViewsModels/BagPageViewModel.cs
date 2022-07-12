using Smart_Investments.Models;
using Smart_Investments.Services;
using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Services.LocationDataBaseService;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.PageService;
using Smart_Investments.Views.Pages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Smart_Investments.ViewsModels
{
    class BagPageViewModel : INotifyPropertyChanged
    {
       public BagPageViewModel(StocksCollection stocksCollection, BagPageModel bagPageModel, BagPagesStack bagPagesStack, LocationDataBase locationDataBase,
           ThemeSwitchService themeSwitchService)
        {
            _stocksCollection = stocksCollection;
            _bagPageModel = bagPageModel;
            _bagPagesStack = bagPagesStack;

            _stocks = new ObservableCollection<UserStock>();

            StocksView = CollectionViewSource.GetDefaultView(_stocks);
            StocksView.Filter += StocksFilter;

            _stocksCollection.OnStocksChange += StocksComplete;

            themeSwitchService.Update += () => StocksView.Refresh();

            _isSub = false;
            _period = "Today";
            _selectedUser = new User("");

            locationDataBase.OnSelectedUserChange += (selUser) =>
            {
                if (_isSub)
                {
                    SelectedUser.OnUpdateInfo -= UpdateInfo;
                    _isSub = false;
                }

                SelectedUser = selUser;

                if (!_isSub)
                {
                    SelectedUser.OnUpdateInfo += UpdateInfo;
                    _isSub = true;
                }
            };
        }

        #region Events


        #endregion

        #region Values 

        private readonly StocksCollection _stocksCollection;
        private readonly BagPageModel _bagPageModel;
        private readonly BagPagesStack _bagPagesStack;

        private ObservableCollection<UserStock> _stocks;

        private User _selectedUser;

        private bool _isSub;
        private string _period;

        private ICollectionView _stocksView;

        #endregion

        #region Properties

        public ICollectionView StocksView
        {
            get
            {
                return _stocksView;
            }
            set
            {
                _stocksView = value;
            }
        }

        public User SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        public string Period
        {
            get
            {
                return _period;
            }
            set
            {
                _period = value;
                StocksView.Refresh();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public void UpdateInfo()
        {
            OnPropertyChanged(nameof(SelectedUser));
        }

        private bool StocksFilter(object obj)
        {
            if (obj is UserStock ustock)
            {
                return ustock.StocksCount > 0;
            }
            return true;
        }


        private void StocksComplete(ObservableCollection<UserStock> stocks)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _bagPageModel.MathStockComplete(_stocks, stocks);
                StocksView.Refresh();
            });
        }

        #endregion

        #region Commands

        public ICommand DragDropPageOpen_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _bagPagesStack.AddPage(new DragDropExcelPage());
            });
        }

        public ICommand RadioButtonToday_Click
        {
            get => new DelegateCommand((obj) =>
            {
                Period = "Today";
            });
        }

        public ICommand RadioButtonAll_Click
        {
            get => new DelegateCommand((obj) =>
            {

                Period = "All";
            });
        }

        public ICommand Stock_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (obj is UserStock ustock)
                {
                    StockPage stockPage = new StockPage();
                    _bagPagesStack.AddPage(stockPage);

                    var dc = (StockPageViewModel)stockPage.DataContext;
                    dc.BackPageClick = () => _bagPagesStack.RemovePage(); ;
                    dc.Initialize(ustock);
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
