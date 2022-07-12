using System;

namespace Smart_Investments.Services.LocationDataBaseService.UserStocks
{
    public class Operation
    {
        public Operation(string transactionNumber, DateTime dateOfConclusion, DateTime settlementDate, string operationType, int stocksCount, double stockCost, int nkd,
            string currency, int course, double tradingCommission, double bankCommission, string typeOfTransaction)
        {
            TransactionNumber = transactionNumber;
            DateOfConclusion = dateOfConclusion;
            SettlementDate = settlementDate;
            OperationType = operationType;
            StocksCount = stocksCount;
            StockCost = stockCost;
            NKD = nkd;
            Currency = currency;
            Course = course;
            TradingCommission = tradingCommission;
            BankCommission = bankCommission;
            TypeOfTransaction = typeOfTransaction;
        }


        public string TransactionNumber { get; private set; }

        public DateTime DateOfConclusion { get; private set; }

        public DateTime SettlementDate { get; private set; }

        public string OperationType { get; private set; }

        public int StocksCount { get; private set; }

        public double StockCost { get; private set; }

        public int NKD { get; private set; }

        public string Currency { get; private set; }

        public int Course { get; private set; }

        public double TradingCommission { get; private set; }

        public double BankCommission { get; private set; }
        
        public string TypeOfTransaction { get; private set; }

    }
}
