using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertTextFormatInterval : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string period)
            {
                string res = "g";

                switch (period)
                {
                    case "Всё время":
                        res = "MMM yyyy";
                        break;
                    case "Год":
                        res = "MMM yyyy";
                        break;
                    case "Квартал":
                        res = "MMM yyyy";
                        break;
                    case "Месяц":
                        res = "m";
                        break;
                    case "Неделя":
                        res = "m";
                        break;
                    case "День":
                        res = "d MMM hh:mm";
                        break;
                    case "Час":
                        res = "d MMM hh:mm";
                        break;
                }

                return res;
            }

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
