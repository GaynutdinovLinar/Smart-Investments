using LiveCharts;
using Smart_Investments.Services.MoexStocksService;
using Smart_Investments.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Smart_Investments.Services.ChartIndicatorsService.Moving_Average_Service
{
    class WMA : IMovingAverage
    {
        public WMA(string title = "WMA")
        {
            Title = title;
        }
        public string Title { get; set; }

        public ChartValues<ChartPointXY> GetValues(int interval, ICollection<ChartPointXY> data)
        {
            if (interval < data.Count())
            {
                ChartValues<ChartPointXY> WMA = new ChartValues<ChartPointXY>();

                double cost;
                double sum = 0;

                List<ChartPointXY> dc_per;
                for (int i = 0; i < data.Count(); i++)
                {
                    dc_per = new List<ChartPointXY>(data.Skip(i).Take(interval));

                    sum = 0;

                    for (int k = 0; k < dc_per.Count; k++)
                    {
                        sum += dc_per[k].Y * (k + 1);
                    }

                    if (i < data.Count() - interval)
                    {
                        cost = sum / ((interval * (1 + interval)) / 2);
                        WMA.Add(new ChartPointXY(i + interval, cost));
                    }
                    else break;

                }

                cost = sum / ((interval * (1 + interval)) / 2);
                WMA.Add(new ChartPointXY(2 * WMA[^1].X - WMA[^2].X, cost));

                //decimal? cost;
                //decimal? sum = 0;
                //for (int i = 0; i < data.Count(); i++)
                //{
                //    List<DateCost> dc_per = new List<DateCost>(data.Skip(i).Take(interval));

                //    sum = 0;

                //    for (int k = 0; k < dc_per.Count; k++)
                //    {
                //        sum += getValue(dc_per[k]) * (k + 1);
                //    }

                //    if (i < data.Count() - interval)
                //    {
                //        cost = sum / ((interval * (1 + interval)) / 2);
                //        WMA.Add(new DateCost(data.ElementAt(i + interval).Day, cost, cost, cost, cost));
                //    }
                //    else break;

                //}

                //cost = sum / ((interval * (1 + interval)) / 2);
                //WMA.Add(new DateCost(new DateTime(2 * WMA[^1].Day.Ticks - WMA[^2].Day.Ticks), cost, cost, cost, cost));

                return WMA;
            }
            else return null;
            
        }
    }
}
