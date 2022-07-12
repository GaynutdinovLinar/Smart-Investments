using Smart_Investments.Services.LocationDataBaseService;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.PageService.Interface;
using Smart_Investments.Views.Pages;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Smart_Investments.Services.PageService
{
    class BagPagesStack : IStackPageService
    {
        public BagPagesStack(PagesService pageService, LocationDataBase locationDataBase, LoadService loadService)
        {
            _pageService = pageService;
            _stackPage = new Stack<Page>();

            locationDataBase.OnUsersChange += (users) => DragDropOpen(users);

            locationDataBase.OnFilesReady += () =>
            {
                if (GetPage() is DragDropExcelPage)
                {
                    _stackPage.Pop();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_stackPage.Count == 0)
                        {
                            var bp = new BagPage();
                            _stackPage.Push(bp);
                        }                   

                        if (!loadService.IsStart) pageService.ChangePage(GetPage());
                    });
                }
            };
        }

        private readonly PagesService _pageService;

        private readonly Stack<Page> _stackPage;

        public int CountPages { get => _stackPage.Count; }

        public Page GetPage()
        {
            return  _stackPage.Peek();
        }

        public void AddPage(Page page)
        {
            _stackPage.Push(page);
            _pageService.ChangePage(page);
        }

        public void RemovePage()
        {
            if (_stackPage.Count > 0)
            {
                _stackPage.Pop();
                _pageService.ChangePage(GetPage());
            }
        }

        private void DragDropOpen(List<User> users)
        {
            if(users.Count == 0)
            {
                if (_stackPage.Count == 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var dp = new DragDropExcelPage();
                        _stackPage.Push(dp);
                    });
                }
            }
            else
            {
                if (_stackPage.Count == 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var bp = new BagPage();
                        _stackPage.Push(bp);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _stackPage.Pop();

                        var bp = new BagPage();

                        _pageService.CurrentPage = bp;

                        _stackPage.Push(bp);
                    });
                }
            }
        }

    }
}
