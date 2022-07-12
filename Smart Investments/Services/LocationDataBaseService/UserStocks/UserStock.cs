using Smart_Investments.Services.MoexStocksService;
using System;
using System.Collections.ObjectModel;

namespace Smart_Investments.Services.LocationDataBaseService.UserStocks
{
    public class UserStock
    {
        public UserStock()
        {
            Stock = new Stock();

            Operations = new ObservableCollection<Operation>();
        }

        public Stock Stock { get; set; }

        public int StocksCount { get; set; } = 0;

        public decimal Commision { get; set; } = 0; //Комиссия с операций

        public decimal ChangeCostAll { get => CurrentCost - FirstCost - Commision; } //Изменение стоимости всех штук акций за всё время

        public decimal ChangeCostToday { get; set; } = 0; //Изменение стоимости всех штук акций за день

        public decimal CurrentCost { get; set; } = 0; //Текущая стоимость всех штук акций

        public decimal FirstCost { get; set; } = 0; //Первоначальная стоимость акции

        public ObservableCollection<Operation> Operations { get; private set; }

        public void DataComplete(string operationType, int stocksCount, double stockCost, double tradingCommission, double bankCommission)
        {

            if (StocksCount - stocksCount == 0 || StocksCount + stocksCount == 0) //Проверка на продажу последней акции
            {
                if (operationType == "Покупка") //Вычисление количества и стоимости акции
                {
                    StocksCount += stocksCount;

                    FirstCost += stocksCount * (decimal)stockCost;

                    Commision += (decimal)(tradingCommission + bankCommission); //Добавление комиссии

                }
                else if (operationType == "Продажа")
                {

                    FirstCost = 0;

                    StocksCount = 0;

                    Commision = 0;
                }
            }
            else
            {
                if (operationType == "Покупка") //Вычисление количества и стоимости акции
                {
                    StocksCount += stocksCount;

                    FirstCost += stocksCount * (decimal)stockCost;

                    Commision += (decimal)(tradingCommission + bankCommission); //Добавление комиссии
                }
                else if (operationType == "Продажа")
                {
                    FirstCost = FirstCost - (decimal)stocksCount / StocksCount * FirstCost;
                    StocksCount -= stocksCount;
                }
            }
        }

        public void AddOperation(string transactionNumber, DateTime dateOfConclusion, DateTime settlementDate,
            string operationType, int stocksCount, double stockCost, int nkd,
            string currency, int course, double tradingCommission, double bankCommission, string typeOfTransaction)
        {
            Operations.Add(new Operation(transactionNumber,
                    dateOfConclusion,
                    settlementDate,
                    operationType,
                    stocksCount,
                    stockCost,
                    nkd,
                    currency,
                    course,
                    tradingCommission,
                    bankCommission,
                    typeOfTransaction));

            DataComplete(operationType, stocksCount, stockCost,  tradingCommission, bankCommission);
        }

    }
}
