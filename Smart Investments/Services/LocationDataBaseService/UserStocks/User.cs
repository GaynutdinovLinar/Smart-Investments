using Smart_Investments.Services.MoexStocksService;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Smart_Investments.Services.LocationDataBaseService.UserStocks
{
    public class User
    {
        public User(string userid)
        {
            Enrollments = new ObservableCollection<Enrollment>();

            UserStocks = new ObservableCollection<UserStock>();
            Dividends = new ObservableCollection<Dividend>();

            UserId = userid;
        }

        #region Events

        public event Action OnUpdateInfo;

        #endregion

        #region Properties

        public string UserId { get; private set; }

        public double DividendsAmount { get; private set; } = 0;

        public double InvestedAmount { get; private set; } = 0;

        public decimal Commision { get; private set; } = 0; //Комиссия с операций

        public decimal FirstStocksCost { get; private set; } = 0; //Первоначальная стоимость акций

        public decimal ChangeCostAll { get => CurrentStocksCost - FirstStocksCost - Commision; }//Изменение стоимости акций за все время

        public decimal ChangeCostToday { get; set; } = 0;  //Изменение стоимости акций сегодня

        public decimal CurrentStocksCost { get; set; } = 0; //Текущая стоимость акций

        public ObservableCollection<Enrollment> Enrollments { get; private set; }

        public ObservableCollection<UserStock> UserStocks { get; private set; }

        public ObservableCollection<Dividend> Dividends { get; private set; }

        #endregion

        #region Methods

        public void CompleteUserStock(UserStock findedStock, Stock stocks)
        {
            findedStock.Stock = stocks;

            if (stocks.LastCost != null)
            {

                findedStock.CurrentCost = (decimal)stocks.LastCost * findedStock.StocksCount; // Подсчет текущей стоимости акций
                CurrentStocksCost += findedStock.CurrentCost;
            }

            if (stocks.ChangeCost != null)
            {
                findedStock.ChangeCostToday = (decimal)stocks.ChangeCost * findedStock.StocksCount; // Подсчет изменения стоимости акций за день
                ChangeCostToday += findedStock.ChangeCostToday;
            }    
        }

        public void InfoUpdate()
        {
            OnUpdateInfo?.Invoke();
        }

        public void AddInfoOperation( string transactionNumber, DateTime dateOfConclusion, DateTime settlementDate, string secid,
            string operationType, int stocksCount, double stockCost, int nkd,
            string currency, int course, double tradingCommission, double bankCommission, string typeOfTransaction)
        {
            var stock = UserStocks.FirstOrDefault(s => s.Stock.Secid == secid);

            if (stock != null && (stock.StocksCount - stocksCount == 0 || stock.StocksCount + stocksCount == 0)) //Проверка на продажу последней акции
            {
                if (operationType == "Покупка") //Вычисление количества и стоимости акции
                {
                    FirstStocksCost += stocksCount * (decimal)stockCost;

                    Commision += (decimal)(tradingCommission + bankCommission);
                }
                else if (operationType == "Продажа")
                {
                    FirstStocksCost -= stock.FirstCost + stock.Commision;
                }
            }
            else
            {
                if (operationType == "Покупка") //Вычисление количества и стоимости акции
                {
                    FirstStocksCost += stocksCount * (decimal)stockCost;

                    Commision += (decimal)(tradingCommission + bankCommission);
                }
                else if (operationType == "Продажа")
                {
                    FirstStocksCost -= (decimal)stocksCount / stock.StocksCount * stock.FirstCost;
                }
            }

            if (stock == null)
            {
                UserStock userStock = new UserStock();

                userStock.Stock.Secid = secid;                

                userStock.AddOperation(transactionNumber, 
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
                    typeOfTransaction);

                UserStocks.Add(userStock);
            }
            else
            {

                    stock.AddOperation(transactionNumber,
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
                   typeOfTransaction);
            }
        }

        public void AddInfoDepositsAndDebits(DateTime dateOfConclusion, DateTime settlementDate, string operationType, double amount,
            string currency, string debitingFrom, string creditingTo, string operationContent, string status)
        {
            

            switch (operationType)
            {
                case "Ввод ДС":
                    InvestedAmount += amount;
                    break;
                case "Зачисление дивидендов":
                    var mass = operationContent.Remove(0, "Зачисление дивидендов по бумаге  ".Length).Split(' ');
                    var shortname = string.Join(' ', mass.Take(mass.Length - 2));
                    Dividends.Add(new Dividend(settlementDate, shortname, int.Parse(mass[^2]), amount));
                    DividendsAmount += amount;
                    break;
            }

            //if(operationType == "Зачисление дивидендов")
            //{


            //    var mass = operationContent.Remove(0, "Зачисление дивидендов по бумаге  ".Length).Split(' ');
            //    var shortname = string.Join(' ', mass.Take(mass.Length - 2));

            //    var stock = UserStocks.FirstOrDefault(s => string.Equals(s.Stock.Shortname, shortname, StringComparison.InvariantCultureIgnoreCase));

            //    if(stock != null)
            //    {
            //        stock.DividendAdd(settlementDate, int.Parse(mass[^2]), amount); //Добавление информации о дивидендах
            //    }
            //}

            //if (operationType == "Ввод ДС")
            //{
            //    InvestedAmount += amount;
            //}

            //if (operationType != "Списание комиссии")
            //{


            //    if (operationType == "Ввод ДС")
            //    {
            //        InvestedAmount += amount;
            //    }

            //    if (operationType == "Зачисление дивидендов")
            //    {
            //        var mass = operationContent.Remove(0, "Зачисление дивидендов по бумаге  ".Length).Split(' ');

            //        Enrollments.Add(new Enrollment(dateOfConclusion, settlementDate, operationType, amount, currency, debitingFrom, creditingTo, string.Join(' ', mass.Take(mass.Length - 2)), int.Parse(mass[^2]), status));
            //    }
            //}
        }

        #endregion

    }
}
