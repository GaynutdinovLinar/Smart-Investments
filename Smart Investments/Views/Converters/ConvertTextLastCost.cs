using Smart_Investments.Services.MoexStocksService;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertTextLastCost : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                decimal cost1;
                double cost2;

                if (decimal.TryParse(value.ToString(), out cost1))
                {
                    NumberFormatInfo frmt = new NumberFormatInfo { NumberGroupSeparator = " " };
                    return string.Format("{0} р.", cost1.ToString("N4", frmt).TrimEnd('0').TrimEnd('.'));
                }
                else if(double.TryParse(value.ToString(), out cost2))
                {
                    NumberFormatInfo frmt = new NumberFormatInfo { NumberGroupSeparator = " " };
                    return string.Format("{0} р.", cost2.ToString("N4", frmt).TrimEnd('0').TrimEnd('.'));
                } 
            }
            return "";

            throw new ApplicationException();
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
