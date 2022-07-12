using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using System.Collections.ObjectModel;

namespace Smart_Investments.Models
{
    public class BagPageModel
    {
        public void MathStockComplete(ObservableCollection<UserStock> Stocks, ObservableCollection<UserStock> stocks)
        {
            if (Stocks != null && stocks != null)
            {
                Stocks.Clear();

                for (int i = 0; i < stocks.Count; i++)
                {
                    if (stocks[i].StocksCount > 0) Stocks.Add(stocks[i]);
                }
            }
        }
    }
}
