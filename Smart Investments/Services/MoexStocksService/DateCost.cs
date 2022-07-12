using System;

namespace Smart_Investments.Services.MoexStocksService
{
    public class DateCost
    {
        public DateCost(DateTime day, decimal? open = null, decimal? close = null, decimal? low = null, decimal? high = null)
        {
            Day = day;
            Open = open;
            Close = close;
            Low = low;
            High = high;
        }

        public DateTime Day{ get; internal set;}

        public decimal? Open { get; internal set;}

        public decimal? Close { get; internal set; }

        public decimal? Low { get; internal set; }

        public decimal? High { get; internal set; }
    }
}
