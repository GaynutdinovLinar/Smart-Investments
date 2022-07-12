using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Smart_Investments.Views.Converters
{
    public class ConvertColorChangeCost : MarkupExtension, IValueConverter
    {
        public System.Windows.DynamicResourceExtension BrushFalling { get; set; }

        public System.Windows.DynamicResourceExtension BrushGrowing { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? cost = (decimal?)value;

            if (cost != null )
            {
                if (cost >= 0) return Application.Current.Resources[BrushGrowing.ResourceKey];
                else if (cost < 0) return Application.Current.Resources[BrushFalling.ResourceKey];
            }
            return new SolidColorBrush(Colors.Black);

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
