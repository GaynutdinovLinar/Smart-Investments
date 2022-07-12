using LiveCharts;
using Smart_Investments.Services.MoexStocksService;
using Smart_Investments.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Smart_Investments.Services.ChartIndicatorsService.Moving_Average_Service
{
    public class EMA : IMovingAverage
    {
        public EMA(string title = "EMA")
        {
            Title = "EMA";
        }

        public string Title { get; set; }

        public ChartValues<ChartPointXY> GetValues(int interval, ICollection<ChartPointXY> data)
        {
            if (interval < data.Count())
            {
                ChartValues<ChartPointXY> EMA = new ChartValues<ChartPointXY>();

                double cost = data.Take(interval).Sum(cost => cost.Y) / interval;

                EMA.Add(new ChartPointXY(interval, cost));

                double a = 2 / (double)(interval + 1); ;

                for (int i = interval + 1, k = 1; i < data.Count() + 1; i++, k++)
                {
                    cost = a * data.ElementAt(i - 1).Y + (1 - a) * EMA[k - 1].Y;

                    EMA.Add(new ChartPointXY(i, cost));
                }

                //EMA.Add(new DateCost(
                //       data.ElementAt(0).Day,
                //       cost, cost, cost, cost));

                //decimal a = 0;

                //for (int i = 1; i < data.Count() - 1; i++)
                //{
                //    a = 2 / (decimal)(interval + 1);

                //    cost = a * getValue(data.ElementAt(i)) + (1 - a) * getValue(EMA[i - 1]);

                //    EMA.Add(new DateCost(data.ElementAt(i).Day, cost, cost, cost, cost));
                //}

                return EMA;
            }
            else return null;
        }
    }
}
