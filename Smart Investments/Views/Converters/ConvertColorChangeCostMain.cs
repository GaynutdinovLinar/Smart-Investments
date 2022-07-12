using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertColorChangeCostMain : MarkupExtension, IMultiValueConverter
    {
        public DynamicResourceExtension ColorFalling { get; set; }

        public DynamicResourceExtension ColorGrowing { get; set; }

        public DynamicResourceExtension ColorConst { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null)
            {
                if (values[0] is User u)
                {
                    string period = values[1].ToString();

                    decimal ChangeCost = 0;

                    if (period == "Today")
                    {
                        ChangeCost = u.ChangeCostToday;
                    }
                    else if (period == "All")
                    {
                        ChangeCost = u.ChangeCostAll;
                    }


                    if (ChangeCost != 0)
                    {
                        if (ChangeCost > 0) return Application.Current.Resources[ColorGrowing.ResourceKey];
                        else if (ChangeCost < 0) return Application.Current.Resources[ColorFalling.ResourceKey];
                    }
                    return Application.Current.Resources[ColorConst.ResourceKey];
                }
                else if (values[0] is UserStock ustock)
                {
                    string period = values[1].ToString();

                    decimal ChangeCost = 0;

                    if (period == "Today")
                    {
                        ChangeCost = ustock.ChangeCostToday;
                    }
                    else if (period == "All")
                    {
                        ChangeCost = ustock.ChangeCostAll;
                    }


                    if (ChangeCost != 0)
                    {
                        if (ChangeCost > 0) return Application.Current.Resources[ColorGrowing.ResourceKey];
                        else if (ChangeCost < 0) return Application.Current.Resources[ColorFalling.ResourceKey];
                    }
                    return Application.Current.Resources[ColorConst.ResourceKey];
                }

            }

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
