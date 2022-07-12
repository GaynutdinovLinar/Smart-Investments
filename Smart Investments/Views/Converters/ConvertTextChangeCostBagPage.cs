using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Smart_Investments.Views.Converters
{
    class ConvertTextChangeCostBagPage : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null)
            {
                if (values[0] is User u)
                {
                    string period = values[1].ToString();

                    decimal LastCost = u.CurrentStocksCost;
                    decimal ChangeCost = 0;

                    if (period == "Today")
                    {
                        ChangeCost = u.ChangeCostToday;
                    }
                    else if (period == "All")
                    {
                        ChangeCost = u.ChangeCostAll;
                    }

                    if (LastCost != 0 || ChangeCost != 0)
                    {
                        if (ChangeCost > 0) return string.Format("+{0:0.#####} р. (+{1:P})", ChangeCost, ChangeCost / (LastCost - ChangeCost));
                        else return string.Format("{0:0.#####} р. ({1:P})", ChangeCost, ChangeCost / (LastCost - ChangeCost));
                    }
                    else return "0";
                }
                else if (values[0] is UserStock ustock)
                {
                    string period = values[1].ToString();

                    decimal LastCost = ustock.CurrentCost;
                    decimal ChangeCost = 0;

                    if (period == "Today")
                    {
                        ChangeCost = ustock.ChangeCostToday;
                    }
                    else if (period == "All")
                    {
                        ChangeCost = ustock.ChangeCostAll;
                    }

                    if (LastCost != 0 || ChangeCost != 0)
                    {
                        if (ChangeCost > 0) return string.Format("+{0:0.#####} р. (+{1:P})", ChangeCost, ChangeCost / (LastCost - ChangeCost));
                        else return string.Format("{0:0.#####} р. ({1:P})", ChangeCost, ChangeCost / (LastCost - ChangeCost));
                    }
                    else return "0";
                }

            }
            else return "";

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
