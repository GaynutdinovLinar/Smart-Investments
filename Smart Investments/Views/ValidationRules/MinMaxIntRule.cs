using System;
using System.Globalization;
using System.Windows.Controls;

namespace Smart_Investments.Views.ValidationRules
{
    class MinMaxIntRule : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        { 
            if(value != null)
            {
                var s = (string)value;

                if (s != "")
                {
                    int d;
                    if (!int.TryParse(s, out d)) return new ValidationResult(false, "Недопустимые символы");
                    else if (d > int.MaxValue) return new ValidationResult(false, "Слишком большое значение");
                    else if (d < 0) return new ValidationResult(false, "Число должно быть положительным");
                    else return ValidationResult.ValidResult;
                }
                else return null;
            }
            else return ValidationResult.ValidResult;

        }
    }
}
