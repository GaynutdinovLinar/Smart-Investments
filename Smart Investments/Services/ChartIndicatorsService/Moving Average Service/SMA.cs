using LiveCharts;
using Smart_Investments.Services.MoexStocksService;
using Smart_Investments.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Smart_Investments.Services.ChartIndicatorsService.Moving_Average_Service
{
    public class SMA : IMovingAverage
    {
        public SMA(string title = "SMA")
        {
            Title = title;
        }

        public string Title { get; set; }

        public ChartValues<ChartPointXY> GetValues(int interval, ICollection<ChartPointXY> data)
        {
            if (interval < data.Count())
            {
                ChartValues<ChartPointXY> SMA = new ChartValues<ChartPointXY>();

                double cost;

                for (int i = 0; i < data.Count() - interval; i++)
                {
                    cost = data.Skip(i).Take(interval).Sum(cost => cost.Y) / interval;
                    SMA.Add(new ChartPointXY(i + interval,cost));
                }

                cost = data.Skip(data.Count() - interval).Take(interval).Sum(dc => dc.Y) / interval;

                SMA.Add(new ChartPointXY(2 * SMA[^1].X - SMA[^2].X, cost));

                //for (int i = 0; i < data.Count() - interval; i++)
                //{
                //    cost = data.Skip(i).Take(interval).Sum(cost => getValue(cost)) / interval;
                //    SMA.Add(new DateCost(
                //        data.ElementAt(i + interval).Day,
                //        cost,
                //        cost,
                //        cost,
                //        cost));
                //}

                //cost = data.Skip(data.Count() - interval).Take(interval).Sum(dc => getValue(dc)) / interval;

                //SMA.Add(new DateCost(new DateTime(2 * SMA[^1].Day.Ticks - SMA[^2].Day.Ticks), cost, cost, cost, cost));

                return SMA;
            }
            else return null;
        }
    }
}
