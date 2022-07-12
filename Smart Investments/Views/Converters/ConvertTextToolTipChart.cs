using LiveCharts;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertTextToolTipChart : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ChartPoint series = (ChartPoint)value;

                if (series.High == 0) return ConvertToString(series.Y);
                else return $"High: {ConvertToString(series.High)} \n" +
                        $"Open: {ConvertToString(series.Open)} \n"+
                        $"Close: {ConvertToString(series.Close)} \n" +
                        $"Low: {ConvertToString(series.Low)}";
            }
            else return "";

            throw new ApplicationException();
        }

        private string ConvertToString(double d)
        {
            NumberFormatInfo frmt = new NumberFormatInfo { NumberGroupSeparator = " " };
            return string.Format("{0} р.", d.ToString("N4", frmt).TrimEnd('0').TrimEnd('.'));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ApplicationException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
