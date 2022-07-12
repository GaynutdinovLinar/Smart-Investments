using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using Smart_Investments.Models;
using Smart_Investments.Services;
using Smart_Investments.Services.ChartIndicatorsService.Moving_Average_Service;
using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.MoexStocksService;
using Smart_Investments.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Smart_Investments.ViewsModels
{
    class StockPageViewModel : INotifyPropertyChanged
    {
        public StockPageViewModel(StockPageModel stockPageModel, LoadService loadService, ThemeSwitchService themeSwitchService)
        {
            _stockPageModel = stockPageModel;

            _loadService = loadService;

            UserStock = new UserStock();

            CompleteIntervalsCollection();

            Periods = new ObservableCollection<string>
            {
                "Год",
                "Квартал",
                "Месяц",
                "Неделя",
                "День",
                //"Час",
                "Всё время"
            };

            Values = new ObservableCollection<string>
            {
                "open",
                "close",
                "low",
                "high"
            };
            
            _formatterY = value => Math.Round(value, 4).ToString();

            themeSwitchService.Update += () => OnPropertyChanged(nameof(UserStock));

            AllVariantsMA = new ObservableCollection<IMovingAverage>()
            {
                new SMA(),
                new WMA(),
                new EMA()
            };

            SeriesMA = new ObservableCollection<SeriesMA>();

            _labelsAxisX = new ObservableCollection<string>();
        }

        #region Value

        private readonly StockPageModel _stockPageModel;

        private readonly LoadService _loadService;

        private Func<double, string> _formatterY;

        private ObservableCollection<string> _labelsAxisX;

        private SeriesCollection _seriesCollection;

        private UserStock _userStock;

        public double _xAxisMinValue = default;
        public double _xAxisMaxValue = default;
        private double _xAxisMaxRange = default;

        public double _yAxisMinValue = default;
        public double _yAxisMaxValue = default;
        private double _yAxisMaxRange = default;

        private ObservableCollection<string> _period;

        private string _selectedPeriod;

        private ObservableCollection<string> _interval;

        private string _selectedInterval;

        private ObservableCollection<string> _value;

        private string _selectedValue;

        private readonly ObservableCollection<string> _allInterval = new ObservableCollection<string>
        {
             "Квартал",
             "Месяц",
             "Неделя",
             "День",
             "Час",
             "10 минут",
             "1 минута"
        };

        private ObservableCollection<string> _intervalAllTime;
        private ObservableCollection<string> _intervalYear;
        private ObservableCollection<string> _intervalQuarter;
        private ObservableCollection<string> _intervalMonth;
        private ObservableCollection<string> _intervalWeek;
        private ObservableCollection<string> _intervalDay;
        private ObservableCollection<string> _intervalHour;

        private ObservableCollection<ChartPoint> selectedChartPoint;

        private DateTime? _minCostDate;
        private DateTime? _maxCostDate;

        private ISeriesView _mainSeries;

        private bool _isLineChart = true;

        private Func<DateTime, string> _dateFormat;


        #endregion

        #region Properties

        #region Chart

        public bool LineChartVisible
        {
            get => _isLineChart;
            set
            {
                _isLineChart = value;

                if (value)
                {
                    MainSeries = MainLineSeries;
                    MainLineSeries.Values = _stockPageModel.GetDataPointCollection();
                }
                else
                {
                    MainSeries = MainCandleSeries;
                    MainCandleSeries.Values = _stockPageModel.GetDataPointCollection();                   
                }

                OnPropertyChanged();
            }
        }

        public DateTime? MinCostDate 
        {
            get
            {
                return _minCostDate;
            }
            set
            {
                _minCostDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? MaxCostDate
        {
            get
            {
                return _maxCostDate;
            }
            set
            {
                _maxCostDate = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChartPoint> SelectedChartPoint
        {
            get => selectedChartPoint;
            set
            {
                selectedChartPoint = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> FormatterY
        {
            get => _formatterY;
            set
            {
                _formatterY = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged();
            }
        }

        public ISeriesView MainSeries 
        { 
            get => _mainSeries;
            set
            {
                _mainSeries = value;
                if (SeriesCollection != null && SeriesCollection.Count > 3)
                {
                    
                    SeriesCollection.RemoveAt(3);
                    SeriesCollection.Insert(3, value);
                }
            }
        }

        public CandleSeries MainCandleSeries { get; set; }

        public LineSeries MainLineSeries { get; set; }

        public LineSeries TrendSeries { get; set; }

        public LineSeries MaxSeries { get; set; }

        public LineSeries MinSeries { get; set; }


        public ObservableCollection<IMovingAverage> AllVariantsMA { get; }

        public ObservableCollection<SeriesMA> SeriesMA { get; set; }

        public ObservableCollection<string> LabelsAxisX 
        { 
            get => _labelsAxisX;
            set
            {
                OnPropertyChanged();
            }
        }


        public double XAxisMaxValue
        {
            get
            {
                return _xAxisMaxValue;
            }
            set
            {
                _xAxisMaxValue = value;
                OnPropertyChanged();
            }
        }

        public double XAxisMinValue
        {
            get
            {
                return _xAxisMinValue;
            }
            set
            {
                _xAxisMinValue = value;
                OnPropertyChanged();
            }
        }

        public double XAxisMaxRange
        {
            get
            {
                return _xAxisMaxRange;
            }
            set
            {
                _xAxisMaxRange = value;
            }
        }

        public double YAxisMaxValue
        {
            get
            {
                return _yAxisMaxValue * 1.2;
            }
            set
            {
                _yAxisMaxValue = value;
                OnPropertyChanged();
            }
        }

        public double YAxisMinValue
        {
            get
            {
                return _yAxisMinValue * 0.8;
            }
            set
            {
                _yAxisMinValue = value;
                OnPropertyChanged();
            }
        }

        public double YAxisMaxRange
        {
            get
            {
                return _yAxisMaxRange;
            }
            set
            {
                _yAxisMaxRange = value;
            }
        }

        #endregion

        #region ChartParametrs

        public ObservableCollection<string> Periods
        {
            get
            {
                return _period;
            }
            set
            {
                _period = value;
                SelectedPeriod = _period[0];
                OnPropertyChanged();
            }
        }

        public string SelectedPeriod
        {
            get
            {
                return _selectedPeriod;
            }
            set
            {
                _selectedPeriod = value;

                switch (value)
                {
                    case "Всё время":
                        Intervals = _intervalAllTime;
                        break;
                    case "Год":
                        Intervals = _intervalYear;
                        break;
                    case "Квартал":
                        Intervals = _intervalQuarter;
                        break;
                    case "Месяц":
                        Intervals = _intervalMonth;
                        break;
                    case "Неделя":
                        Intervals = _intervalWeek;
                        break;
                    case "День":
                        Intervals = _intervalDay;
                        break;
                    case "Час":
                        Intervals = _intervalHour;
                        break;
                }

                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Intervals
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;

                SelectedInterval = _interval[0];

                OnPropertyChanged();
            }
        }

        public string SelectedInterval
        {
            get
            {
                return _selectedInterval;
            }
            set
            {
                _selectedInterval = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Values
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                SelectedValue = _value[0];
                OnPropertyChanged();
            }
        }

        public string SelectedValue
        {
            get
            {
                return _selectedValue;
            }
            set
            {
                switch (value)
                {
                    case "open":
                        _stockPageModel.TypeValue = TypeValueChart.Open;
                        break;
                    case "close":
                        _stockPageModel.TypeValue = TypeValueChart.Close;
                        break;
                    case "low":
                        _stockPageModel.TypeValue = TypeValueChart.Low;
                        break;
                    case "high":
                        _stockPageModel.TypeValue = TypeValueChart.High;
                        break;
                }

                _selectedValue = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public int IntroIntervalMA { get; set; }

        public IMovingAverage IntroMA { get; set; }

        public Color IntroColor { get; set; }

        public Action BackPageClick { get; set; }

        public UserStock UserStock
        {
            get => _userStock;
            set
            {
                _userStock = value;

                var maMaooer = Mappers.Xy<ChartPointXY>()
                    .X(value => value.X)
                    .Y(value => value.Y);

                Charting.For<ChartPointXY>(maMaooer);

                var mainMapper = Mappers.Xy<DateCost>()
                    .X((value,index) => index)
                    .Y(value => (double)_stockPageModel.GetValue(value));

                MainLineSeries = new LineSeries(mainMapper)
                {
                    Title = _userStock.Stock.Secid,
                    Style = Application.Current.Resources["MainLineSeriesStyle"] as Style
                };

                var candleMapper = Mappers.Financial<DateCost>()
                    .Open(value => (double)value.Open)
                    .High(value => (double)value.High)
                    .Low(value => (double)value.Low)
                    .Close(value => (double)value.Close);

                MainCandleSeries = new CandleSeries(candleMapper)
                {
                    Title = _userStock.Stock.Secid,
                    LabelPoint = x => string.Format("H: {0}, O: {1}, C: {2} L: {3}", x.High, x.Open, x.Close, x.Low),
                    Style = Application.Current.Resources["MainCandleSeriesStyle"] as Style
                };


                TrendSeries = new LineSeries()
                {
                    Title = "Тренд",
                    Style = Application.Current.Resources["MALineSeriesStyle"] as Style,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0))
                };

                MaxSeries = new LineSeries()
                {
                    Style = Application.Current.Resources["MaxSeriesStyle"] as Style
                };

                MinSeries = new LineSeries()
                {
                    Style = Application.Current.Resources["MinSeriesStyle"] as Style
                };

                MainSeries = MainLineSeries;

                SeriesCollection = new SeriesCollection()
                {
                    TrendSeries,
                    MaxSeries,
                    MinSeries,
                    MainSeries
                };

                OnPropertyChanged();
            }
        }

       

        #endregion

        #region Methods

        private void CompleteIntervalsCollection()
        {
            _intervalAllTime = new ObservableCollection<string>
            {
                _allInterval[0],
                _allInterval[1]
            };

            _intervalYear = new ObservableCollection<string>
            {
                _allInterval[1],
                _allInterval[2],
                _allInterval[3]
            };

            _intervalQuarter = new ObservableCollection<string>
            {
                _allInterval[2],
                _allInterval[3]
            };

            _intervalMonth = new ObservableCollection<string>
            {
                _allInterval[3],
                _allInterval[4]
            };

            _intervalWeek = new ObservableCollection<string>
            {
                _allInterval[4]
            };

            _intervalDay = new ObservableCollection<string>
            {
                _allInterval[4],
                _allInterval[5]
            };

            _intervalHour = new ObservableCollection<string>
            {
                _allInterval[5],
                _allInterval[6]
            };
        }

        private DateTime? GetStartDate(string period)
        {
            DateTime? startDate = null;

            switch (period)
            {
                case "Всё время":
                    startDate = null;
                    break;
                case "Год":
                    startDate = DateTime.Now.AddDays(-365);
                    break;
                case "Квартал":
                    startDate = DateTime.Now.AddDays(-90);
                    break;
                case "Месяц":
                    startDate = DateTime.Now.AddDays(-30);
                    break;
                case "Неделя":
                    startDate = DateTime.Now.AddDays(-7);
                    break;
                case "День":
                    startDate = DateTime.Now.AddDays(-1);
                    break;
                case "Час":
                    startDate = DateTime.Now.AddHours(-1);
                    break;
            }

            return startDate;
        }

        private Interval GetStartInterval(string interval_str)
        {
            Interval interval = Interval.OneQuarter;

            switch (interval_str)
            {
                case "Квартал":
                    interval = Interval.OneQuarter;
                    _dateFormat = (date) => date.ToString("MMM yyyy");
                    break;
                case "Месяц":
                    interval = Interval.OneMonth;
                    _dateFormat = (date) => date.ToString("MMM yyyy");
                    break;
                case "Неделя":
                    interval = Interval.OneWeek;
                    _dateFormat = (date) => date.ToString("MMM yyyy");
                    break;
                case "День":
                    interval = Interval.OneDay;
                    _dateFormat = (date) => date.ToString("dd MMM");
                    break;
                case "Час":
                    interval = Interval.OneHour;
                    _dateFormat = (date) => date.ToString("H часов ddd");
                    break;
                case "10 минут":
                    interval = Interval.TenMinutes;
                    _dateFormat = (date) => date.ToString("mm минут H часов");
                    break;
                case "1 минута":
                    interval = Interval.OneMinute;
                    break;
            }

            return interval;
        }

        private async Task ChangeDataGraphAsync()
        {
            await _stockPageModel.CompleteAllCollections(UserStock.Stock, GetStartInterval(SelectedInterval), GetStartDate(SelectedPeriod));

            if (LineChartVisible) MainLineSeries.Values = _stockPageModel.GetDataPointCollection(LabelsAxisX, _dateFormat);
            else MainCandleSeries.Values = _stockPageModel.GetDataPointCollection(LabelsAxisX, _dateFormat);

            var MinMax = _stockPageModel.GetXAxisMinAndMaxValue();
            XAxisMinValue = MinMax.Min;
            XAxisMaxValue = MinMax.Max;
            XAxisMaxRange = XAxisMaxValue - XAxisMinValue;

            var dateCostMinMax = _stockPageModel.GetCostMinMaxValues();

            if (dateCostMinMax.min != null && dateCostMinMax.max != null)
            {
                MaxSeries.Values = new ChartValues<ChartPointXY> { dateCostMinMax.max };
                MinSeries.Values = new ChartValues<ChartPointXY> { dateCostMinMax.min };
                MaxCostDate = dateCostMinMax.maxCostDate;
                MinCostDate = dateCostMinMax.minCostDate;
            }
            else
            {
                MaxCostDate = null;
                MinCostDate = null;
                MaxSeries.Values = null;
                MinSeries.Values = null;
            }

            OnPropertyChanged(nameof(MinSeries));
            OnPropertyChanged(nameof(MaxSeries));

            TrendSeries.Values = _stockPageModel.GetTrendLine();

            foreach (var ma in SeriesMA)
            {
                ma.Series.Values = _stockPageModel.GetValuesMA(ma.MA, ma.Interval);
            }

        }

        private async void UpdateDataAsync()
        {
            _loadService.Start();

            await ChangeDataGraphAsync();

            _loadService.Stop();
        }

        public void Initialize(UserStock ustock)
        {
            UserStock = ustock;

            UpdateDataAsync();
        }

        #endregion

        #region Commands

        public ICommand Period_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (obj is string period)
                {
                    SelectedPeriod = period;

                    UpdateDataAsync();
                }
            });
        }

        public ICommand Interval_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (obj is string interval)
                {
                    SelectedInterval = interval;

                    UpdateDataAsync();
                }
            });
        }

        public ICommand Value_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (obj is string value)
                {
                    SelectedValue = value;

                    UpdateDataAsync();
                }
            });
        }

        public ICommand BackButtonClick
        {
            get => new DelegateCommand((obj) =>
            {
                BackPageClick?.Invoke();
            });
        }

        public ICommand OpenWindowAddIndicatorClick
        {
            get => new DelegateCommand((obj) =>
            {
                IntroColor = default;
                IntroMA = AllVariantsMA[0];
                IntroIntervalMA = default;

                SetIntervalDialogWindiw setIntervalDialogWindiw = new SetIntervalDialogWindiw();

                setIntervalDialogWindiw.DataContext = this;

                setIntervalDialogWindiw.ShowDialog();
            });
        }

        public ICommand AddIndicatorClick
        {
            get => new DelegateCommand((obj) =>
            {
                var series = new LineSeries()
                {
                    Values = _stockPageModel.GetValuesMA(IntroMA, IntroIntervalMA),
                    Title = IntroMA.Title + IntroIntervalMA.ToString(),
                    Style = Application.Current.Resources["MALineSeriesStyle"] as Style,
                };

                if (IntroColor != default) series.Stroke = new SolidColorBrush(IntroColor);

                SeriesMA.Add(new SeriesMA(series, IntroIntervalMA, IntroMA));

                SeriesCollection.Add(series);

            }, (obj) => IntroIntervalMA > 0 && IntroMA != default && SeriesMA.Count <= 6);
        }

        public ICommand ItemLegendChartClick
        {
            get => new DelegateCommand((obj) =>
            {
                MessageBox.Show("fsdfdsfsfd");
            });
        }

        #endregion


        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }


    class SeriesMA
    {
        public SeriesMA(LineSeries series, int interval, IMovingAverage ma)
        {
            Series = series;
            Interval = interval;
            MA = ma;
        }

        public LineSeries Series { get; set; }

        public int Interval { get; set; }

        public IMovingAverage MA { get; set; }

    }

    public class ChartPointXY
    {
        public ChartPointXY(int x, double y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public double Y { get; set; }

    }
}
