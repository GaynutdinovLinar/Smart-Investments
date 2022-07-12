using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Smart_Investments.Services
{
    class ThemeSwitchService
    {
        public Action Update;
        public void ChangeTheme(Theme th)
        {
            string style = "";

            switch (th)
            {
                case Theme.Light:
                    style = "LightTheme";
                    break;
                case Theme.Dark:
                    style = "DarkTheme";
                    break;
            }

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri($"Themes/{style}.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries[0] = dictionary;

            Update?.Invoke();
        }
    }

    public enum Theme
    {
        Light,
        Dark
    }
}
