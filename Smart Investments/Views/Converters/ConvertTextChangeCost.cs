using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertTextChangeCost : MarkupExtension, IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null)
            {
                decimal LastCost = (decimal)values[0];
                decimal ChangeCost = (decimal)values[1];

                if(LastCost != 0 || ChangeCost != 0)
                {
                    if (ChangeCost > 0) return string.Format("+{0:0.#####} р. (+{1:P})", ChangeCost, ChangeCost / (LastCost - ChangeCost));
                    else return string.Format("{0:0.#####} р. ({1:P})", ChangeCost, ChangeCost / (LastCost - ChangeCost));
                }
                else return "0";
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
