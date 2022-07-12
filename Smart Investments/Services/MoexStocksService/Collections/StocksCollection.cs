using Smart_Investments.Services.DataMarket;
using Smart_Investments.Services.LocationDataBaseService;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.MoexStocksService;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Smart_Investments.Services
{
    internal class StocksCollection
    {
        public StocksCollection(BoardsCollection boardsCollection, ExceptionService exceptionService, LocationDataBase locationDataBase)
        {
            _boardsCollection = boardsCollection;
            _exceptionService = exceptionService;
            _locationDataBase = locationDataBase;

            Stocks = new ObservableCollection<UserStock>();
        }

        private readonly BoardsCollection _boardsCollection;
        private readonly ExceptionService _exceptionService;
        private readonly LocationDataBase _locationDataBase;

        private  CancellationTokenSource _tokenSource;
        private  CancellationToken _token;

        public event Action<ObservableCollection<UserStock>> OnStocksChange;

        public event Action StocksStartLoad;
        public event Action StocksLoaded;

        public ObservableCollection<UserStock> Stocks { get; private set; }

        public bool StocksComplete { get => Stocks != null && Stocks.Count > 0; }

        public bool IsStarted { get; private set; } = false;

        public async void Start(int timeUppdate)
        {
            if (!IsStarted)
            {
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;


                if (Stocks != null && Stocks.Count > 0) Stocks.Clear();

                IsStarted = true;

                while (IsStarted)
                {
                    await Complete(_token);

                    try
                    {
                        await Task.Delay(timeUppdate, _token);
                    }
                    catch
                    {
                        return;
                    }
                    
                }
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                _tokenSource.Cancel();
                IsStarted = false;
            }
        }

        private Task Complete(CancellationToken token)
        {
            return Task.Run(async() =>
            {
                StocksStartLoad?.Invoke();

                if (!_boardsCollection.BoardsCompleted) await _boardsCollection.BoardsCollectionComplete();

                ObservableCollection<UserStock> userStocks = new ObservableCollection<UserStock>();

                var selUser = _locationDataBase.SelectedUser;

                if (selUser != null)
                {
                    selUser.CurrentStocksCost = 0;  // Обнуление всех показателей
                    selUser.ChangeCostToday = 0;
                }

                

                try
                {
                    foreach (var b in _boardsCollection.Boards)
                    {
                        if (b != null) await b.GetStocksAsync((stocks) =>
                        {
                            if (token.IsCancellationRequested) return;

                            var findedStock = selUser?.UserStocks.FirstOrDefault(stock => stock.Stock.Secid == stocks.Secid);
                            
                            if(findedStock != null)
                            {
                                selUser.CompleteUserStock(findedStock, stocks);

                                userStocks.Add(findedStock);
                            }
                            else
                            {
                                UserStock uStock = new UserStock();
                                uStock.Stock = stocks;

                                userStocks.Add(uStock);
                            }
                        });
                    }
                    selUser?.InfoUpdate();
                }  
                catch (Exception e)
                {
                    _exceptionService.NewException(e, TypeException.Ethernet);
                    return;
                }

                if (userStocks.Count > 0) Stocks = userStocks;

                OnStocksChange?.Invoke(userStocks);

                StocksLoaded?.Invoke();
            }, token);
        }


    }
}
