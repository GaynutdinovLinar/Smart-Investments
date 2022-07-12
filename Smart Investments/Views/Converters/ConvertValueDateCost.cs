using LiveCharts.Wpf;
using Smart_Investments.Services.MoexStocksService;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertValueDateCost : MarkupExtension, IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null)
            {
                LineSeries ls = (LineSeries)values[0];

                if (ls.Values != null && ls.Values.Count > 0)
                {
                    DateCost dc = (DateCost)ls.Values[0];
                    string valueType = values[1].ToString();

                    switch (valueType)
                    {
                        case "open":
                            return dc.Open.ToString();
                        case "close":
                            return dc.Close.ToString();
                        case "low":
                            return dc.Low.ToString();
                        case "high":
                            return dc.High.ToString();
                    }
                }
                else return "";
            }
            else return "";

            throw new ApplicationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ApplicationException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
