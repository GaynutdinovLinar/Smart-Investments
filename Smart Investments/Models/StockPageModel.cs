using LiveCharts;
using Smart_Investments.Services;
using Smart_Investments.Services.ChartIndicatorsService.Moving_Average_Service;
using Smart_Investments.Services.MoexStocksService;
using System;
using System.Collections.ObjectModel;
using Smart_Investments.ViewsModels;

namespace Smart_Investments.Models
{
    class StockPageModel
    {
        public StockPageModel(ExceptionService exceptionService)
        {
            _exceptionService = exceptionService;

            _dayCostCollection = new ObservableCollection<DateCost>();
        }

        #region Events

        public event Action DayCostComplete;

        #endregion

        #region Values

        private readonly ExceptionService _exceptionService;

        private ObservableCollection<DateCost> _dayCostCollection;

        #endregion

        #region Properties

        public ObservableCollection<DateCost> DateCostCollection
        {
            get
            {
                return _dayCostCollection;
            }
            set
            {
                _dayCostCollection = value;
                DayCostComplete?.Invoke();
            }
        }

        public ObservableCollection<ChartPointXY> ChartPointXYCollection { get; set; }

        public TypeValueChart TypeValue { get; set; } = TypeValueChart.Open;

        #endregion

        #region Methods

        public decimal? GetValue(DateCost dc)
        {
            switch (TypeValue)
            {
                case TypeValueChart.Open:
                    return dc?.Open;
                case TypeValueChart.Close:
                    return dc?.Close;
                case TypeValueChart.Low:
                    return dc?.Low;
                case TypeValueChart.High:
                    return dc?.High;
            }

            return null;
        }

        public ChartValues<ChartPointXY> GetTrendLine()
        {
            ChartValues<ChartPointXY> Trend = new ChartValues<ChartPointXY>();

            if (DateCostCollection != null && DateCostCollection.Count > 0)
            {
                double sX = 0;
                double sY = 0;

                double XY = 0;
                double X2 = 0;

                for (int i = 0; i < DateCostCollection.Count; i++)
                {
                    double cost = (double)GetValue(DateCostCollection[i]);

                    sX += i;
                    sY += cost;

                    XY += i * cost;
                    X2 += i * i;
                }


                //foreach (var dc in DateCostCollection)
                //{
                //    double cost = (double)GetValue(dc);
                //    double currentDate = dc.Day.Ticks;

                //    sX += currentDate;
                //    sY += cost;

                //    XY += currentDate * cost;
                //    X2 += currentDate * currentDate;
                //}

                sX /= DateCostCollection.Count;
                sY /= DateCostCollection.Count;

                double b = (XY - DateCostCollection.Count * sX * sY) / (X2 - DateCostCollection.Count * sX * sX);
                double a = sY - b * sX;

                Trend.Add(new ChartPointXY(0, a + b * 0));
                Trend.Add(new ChartPointXY(DateCostCollection.Count - 1, a + b * DateCostCollection.Count - 1));
            }

            return Trend;
        }

        public ChartValues<DateCost> GetDataPointCollection(ObservableCollection<string> labelsX, Func<DateTime,string> dateformat)
        {
            ChartValues<DateCost> values = new ChartValues<DateCost>(DateCostCollection);

            labelsX.Clear();

            foreach (var v in values) labelsX.Add(dateformat(v.Day));

            return values;
        }

        public ChartValues<DateCost> GetDataPointCollection()
        {
            ChartValues<DateCost> values = new ChartValues<DateCost>(DateCostCollection);

            return values;
        }


        public ChartValues<ChartPointXY> GetValuesMA(IMovingAverage movingAverage, int interval)
        {
            return movingAverage.GetValues(interval, ChartPointXYCollection);
        }

        public (double Min, double Max) GetXAxisMinAndMaxValue()
        {
            DateTime startDate = DateTime.Now;

            if (DateCostCollection != null && DateCostCollection.Count > 0) startDate = DateCostCollection[0].Day;
            return (Min: startDate.Ticks, Max: DateTime.Now.Ticks);
        }

        public async System.Threading.Tasks.Task CompleteAllCollections(Stock ms, Interval interval, DateTime? startDate = null)
        {
            var dc = new ObservableCollection<DateCost>();

            try
            {
                if (startDate is null) await ms.GetCostPeriodStockAsync(dc, interval);
                else await ms.GetCostPeriodStockAsync(dc, interval, startDate);
            }
            catch (Exception e)
            {
                _exceptionService.NewException(e, TypeException.Ethernet);
            }

            ObservableCollection<ChartPointXY> cp = new ObservableCollection<ChartPointXY>();

            for (int i = 0; i < dc.Count; i++)
            {
                cp.Add(new ChartPointXY(i, (double)GetValue(dc[i])));
            }

            ChartPointXYCollection = cp;

            DateCostCollection = dc;
        }


        public (ChartPointXY min, DateTime minCostDate, ChartPointXY max, DateTime maxCostDate) GetCostMinMaxValues() 
        {
            int indexMin = 0;
            int indexMax = 0;

            if (ChartPointXYCollection != null && ChartPointXYCollection.Count > 0)
            {
                for (int j = 0; j < ChartPointXYCollection.Count; j++)
                {
                    if (ChartPointXYCollection[indexMin].Y > ChartPointXYCollection[j].Y)
                    {
                        indexMin = j;
                    }

                    if (ChartPointXYCollection[indexMax].Y < ChartPointXYCollection[j].Y)
                    {
                        indexMax = j;
                    }
                }
            }

            return (min: ChartPointXYCollection[indexMin], minCostDate:  DateCostCollection[indexMin].Day, max: ChartPointXYCollection[indexMax], maxCostDate: DateCostCollection[indexMax].Day);
        }

        #endregion   
    }

    public enum TypeValueChart
    {
        Open, Close, Low, High
    }
}
