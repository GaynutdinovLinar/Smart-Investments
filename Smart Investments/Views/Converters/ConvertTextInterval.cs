using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertTextInterval : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string period)
            {
                string res = "Manual";

                switch (period)
                {
                    case "Всё время":
                        res = "Months";
                        break;
                    case "Год":
                        res = "Months";
                        break;
                    case "Квартал":
                        res = "Months";
                        break;
                    case "Месяц":
                        res = "Weeks";
                        break;
                    case "Неделя":
                        res = "Days";
                        break;
                    case "День":
                        res = "Hours";
                        break;
                    case "Час":
                        res = "Minutes";
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
