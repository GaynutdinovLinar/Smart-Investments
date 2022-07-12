using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConverterErrorValidation : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = (IEnumerable<ValidationError>)value;

            int i = 0;

            foreach (var el in res)
            {
                return el.ErrorContent;
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
