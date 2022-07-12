using Smart_Investments.Views.Pages;
using System;
using System.Windows.Controls;

namespace Smart_Investments.Services.PageService
{
    public class PagesService
    {
        public PagesService()
        {
            CurrentPage = null;
        }

        public event Action<Page> OnPageChanged;

        public Page CurrentPage { get; set; }

        public void ChangePage(Page page)
        {
            CurrentPage = page;
            OnPageChanged?.Invoke(page);
        }
    }
}
