using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.MoexStocksService;
using Smart_Investments.ViewsModels;
using System;
using System.Collections.ObjectModel;

namespace Smart_Investments.Models
{
    public class MarketPageModel
    {
        public MarketPageModel()
        {
            SortTypeChange(SortType.Default);
        }

        public ObservableCollection<string> BoardsType { get; set; }

        public double MaxLotCost;

        public double MinLotCost;

        private event Func<Stock, bool> SortFunc;

        public void SortTypeChange(SortType st)
        {
            SortFunc -= DefaultSort;
            SortFunc -= FallingSort;
            SortFunc -= GrowingSort;

            switch (st)
            {
                case SortType.Default:
                    SortFunc += DefaultSort;
                    break;
                case SortType.Falling:
                    SortFunc += FallingSort;
                    break;
                case SortType.Growing:
                    SortFunc += GrowingSort;
                    break;
            }
        }

        #region SortMethod

        private bool GrowingSort(Stock ms)
        {
            return ms.ChangeCost < 0;
        }

        private bool FallingSort(Stock ms)
        {
            return ms.ChangeCost > 0;
        }

        private bool DefaultSort(Stock ms)
        {
            return true;
        }

        public bool FilterStock(Stock stock)
        {
            if (SortFunc is null) return true;
            else
            {
                bool b = true;
                foreach (Func<Stock, bool> del in SortFunc.GetInvocationList())
                {
                    if (!del.Invoke(stock))
                    {
                        b = false;
                        break;
                    }
                }

                return b;
            }
        }

        public bool FilterBoardType(Stock stock)
        {
            if (BoardsType != null)
            {
                return BoardsType.Contains(stock.BoardStock.BoardId);
            }
            else return true;
        }

        public void FilterBoardAdd(ObservableCollection<BoardType> boardsType)
        {
            if (boardsType != null && boardsType.Count > 0)
            {
                ObservableCollection<string> boards = new ObservableCollection<string>();

                foreach (var b in boardsType)
                {
                    if (b.IsActive) boards.Add(b.BoardId);
                }

                BoardsType = boards;

                SortFunc += FilterBoardType;
            }
            else SortFunc -= FilterBoardType;
        }

        public bool FilterLotCostStock(Stock stock)
        {
            if (stock.LotCost != null)
            {
                var cost = (double)stock.LotCost;
                return cost > MinLotCost && cost < MaxLotCost;
            }
            else return true;
        }

        public void FilterLotAdd(int? minCostLotString, int? maxCostLotString)
        {
            if (minCostLotString != null && maxCostLotString != null)
            {
                MinLotCost = (int)minCostLotString;
                MaxLotCost = (int)maxCostLotString;

                SortFunc += FilterLotCostStock;
            }
            else SortFunc -= FilterLotCostStock;
        }

        #endregion

        public void MathStockComplete(ObservableCollection<UserStock> Stocks, ObservableCollection<UserStock> stocks)
        {
            if (Stocks != null && stocks != null)
            {
                if (Stocks.Count == 0)
                {
                    for (int i = 0; i < stocks.Count; i++) Stocks.Add(stocks[i]);
                }
                else
                {
                    if (Stocks.Count > stocks.Count)
                    {
                        while (Stocks.Count != stocks.Count)
                        {
                            Stocks.RemoveAt(Stocks.Count - 1);
                        }
                    }

                    for (int i = 0; i < Stocks.Count; i++)
                    {
                        Stocks[i] = stocks[i];
                    }

                    if (Stocks.Count < stocks.Count)
                    {
                        while (Stocks.Count != stocks.Count)
                        {
                            Stocks.Add(stocks[Stocks.Count - 1]);
                        }
                    }
                }
            }
        }

    }


    public enum SortType
    {
        Default,
        Falling,
        Growing
    }
}
