using Smart_Investments.Models;
using Smart_Investments.Services;
using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Services.DataMarket;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.PageService;
using Smart_Investments.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Smart_Investments.ViewsModels
{
    class MarketPageViewModel : INotifyPropertyChanged
    {

        public MarketPageViewModel(MarketPageModel marketPageModel, StocksCollection stocksCollection, MarketPagesStack marketPagesStack,
            BoardsCollection boardsCollection, ExceptionService exceptionService, ThemeSwitchService themeSwitchService)
        {
            _marketPageModel = marketPageModel;
            _stocksCollection = stocksCollection;
            _marketPagesStack = marketPagesStack;
            _boardsCollection = boardsCollection;
            _exceptionService = exceptionService;
            _themeSwitchService = themeSwitchService;

            BoardsType = StocksTypeComplete(_boardsCollection.BoardsStr);

            _stocks = new ObservableCollection<UserStock>();

            StocksView = CollectionViewSource.GetDefaultView(_stocks);
            StocksView.Filter += StocksFilter;

            _stocksCollection.OnStocksChange += StocksComplete;

            _stocksCollection.StocksStartLoad += () => { Condition = "Данные обновляются"; };
            _stocksCollection.StocksLoaded += () => { if (_stocksCollection.StocksComplete) Condition = "Готово"; };

            _exceptionService.GetException += (e, te) => { if (te == TypeException.Ethernet) Condition = e.Message; };

            _themeSwitchService.Update += () => StocksView.Refresh();
        }

        #region Value

        private readonly MarketPageModel _marketPageModel;
        private readonly StocksCollection _stocksCollection;
        private readonly MarketPagesStack _marketPagesStack;
        private readonly BoardsCollection _boardsCollection;
        private readonly ExceptionService _exceptionService;
        private readonly ThemeSwitchService _themeSwitchService;

        private ObservableCollection<UserStock> _stocks;


        private ICollectionView _stocksView;

        private string _searchStock = string.Empty;

        private int? _minCostLot = default;
        private int? _maxCostLot = default;

        private string _condition = "Готово";


        #endregion

        #region Properties

        public int? MinCostLot{ get; set; }

        public int? MaxCostLot {  get; set; }

        public ObservableCollection<BoardType> BoardsType { get; private set; }

        public string SearchStock
        {
            get
            {
                return _searchStock;
            }
            set
            {
                _searchStock = value;
                OnPropertyChanged();
                StocksView.Refresh();
            }
        }

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

        public UserStock SelectedStock { get; private set; }

        public string Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                _condition = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Methods

        private ObservableCollection<BoardType> StocksTypeComplete(Dictionary<string,string> typeColl)
        {
            var boardsType = new ObservableCollection<BoardType>();

            foreach (var type in typeColl)
            {
                boardsType.Add(new BoardType(type.Key, type.Value));
            }

            return boardsType;
        }

        private bool StocksFilter(object obj)
        {
            if (obj is UserStock mstock)
            {
                return (mstock.Stock.Secid.Contains(SearchStock, StringComparison.CurrentCultureIgnoreCase) ||
                    mstock.Stock.Secname.Contains(SearchStock, StringComparison.CurrentCultureIgnoreCase) ||
                    mstock.Stock.Shortname.Contains(SearchStock, StringComparison.CurrentCultureIgnoreCase)) && _marketPageModel.FilterStock(mstock.Stock);
            }
            return false;
        }

        private void StocksComplete(ObservableCollection<UserStock> stocks)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _marketPageModel.MathStockComplete(_stocks, stocks);
                StocksView.Refresh();
            });
        }

        #endregion

        #region Commands

        public ICommand SortDefault_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _marketPageModel.SortTypeChange(SortType.Default);
                StocksView.SortDescriptions.Clear();
                StocksView.Refresh();
            });
        }

        public ICommand SortFalling_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _marketPageModel.SortTypeChange(SortType.Falling);
                StocksView.SortDescriptions.Clear();
                StocksView.SortDescriptions.Add(new SortDescription("Stock.ProcentChangeCostDay", ListSortDirection.Descending));
                StocksView.Refresh();
            });
        }

        public ICommand SortGrowing_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _marketPageModel.SortTypeChange(SortType.Growing);
                StocksView.SortDescriptions.Clear();
                StocksView.SortDescriptions.Add(new SortDescription("Stock.ProcentChangeCostDay", ListSortDirection.Ascending));
                StocksView.Refresh();
            });
        }

        public ICommand Stock_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (obj is UserStock ustock)
                {
                    StockPage stockPage = new StockPage();
                    _marketPagesStack.AddPage(stockPage);

                    var dc = (StockPageViewModel)stockPage.DataContext;
                    dc.BackPageClick = () => _marketPagesStack.RemovePage();
                    dc.Initialize(ustock);
                }
            });
        }

        public ICommand FilterAccept_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _marketPageModel.FilterBoardAdd(BoardsType);

                _marketPageModel.FilterLotAdd(MinCostLot, MaxCostLot);

                StocksView.Refresh();
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

    public class BoardType
    {
        public BoardType(string boardId, string boardName, bool iActive = true)
        {
            BoardId = boardId;
            BoardName = boardName;
            IsActive = iActive;
        }

        public string BoardId { get; set; }

        public string BoardName { get; set; }

        public bool IsActive { get; set; }
    }
}
