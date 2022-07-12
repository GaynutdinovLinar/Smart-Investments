using System;

namespace Smart_Investments.Services.LocationDataBaseService.UserStocks
{
    public class Dividend
    {
        public Dividend(DateTime date, string shortname, int count, double amount)
        {
            Date = date;

            Shortname = shortname;

            Count = count;

            Amount = amount;
        }


        public DateTime Date { get;}

        public string Shortname { get; }

        public int Count { get; }

        public double Amount { get; }
    }
}
