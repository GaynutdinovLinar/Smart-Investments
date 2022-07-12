using Smart_Investments.Services.PageService.Interface;
using Smart_Investments.Views.Pages;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Smart_Investments.Services.PageService
{
    class MarketPagesStack : IStackPageService
    {
        public MarketPagesStack(PagesService pageService, StocksCollection stocksCollection, LoadService loadService)
        {
            _pageService = pageService;
            _stackPage = new Stack<Page>();
        }

        public void Init() => _stackPage.Push(new MarketPage());

        private readonly PagesService _pageService;

        private readonly Stack<Page> _stackPage;

        public int CountPages { get => _stackPage.Count; }

        public Page GetPage()
        {
            return _stackPage.Peek();
        }

        public void AddPage(Page page)
        {
            _stackPage.Push(page);
            _pageService.ChangePage(page);
        }

        public void RemovePage()
        {
            if(_stackPage.Count > 1)
            {
                _stackPage.Pop();
                _pageService.ChangePage(GetPage());
            }
        }
    }
}
