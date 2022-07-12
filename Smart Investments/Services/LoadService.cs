using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Smart_Investments.Services.LocationDataBaseService;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.PageService;
using Smart_Investments.Services.PageService.Interface;
using Smart_Investments.Views.Pages;
using Smart_Investments.ViewsModels;

namespace Smart_Investments.Services
{
    class LoadService
    {
        public LoadService(PagesService pageService, StocksCollection stocksCollection, LocationDataBase locationDataBase,
            TableOperations tableOperations, TableDepositsAndDebits tableDepositsAndDebits)
        {
            _pagesService = pageService;
            loadPage = new LoadPage();

            _stocksCollection = stocksCollection;

            locationDataBase.OnSelectedUserChange += (users) => StocksUpdateStart();

            stocksCollection.StocksStartLoad += () => StartCheck("Stocks", false);

            stocksCollection.StocksLoaded += () => StartCheck("Stocks", true);


            tableOperations.StartCompleteCollection += () => StartCheck("TableOperations", false);

            tableOperations.CollectionCompleted += () => StartCheck("TableOperations", true);


            tableDepositsAndDebits.StartCompleteCollection += () => StartCheck("TableDepositsAndDebits", false);

            tableDepositsAndDebits.CollectionCompleted += () => StartCheck("TableDepositsAndDebits", true);
        }

        #region Values

        private readonly PagesService _pagesService;
        private readonly StocksCollection _stocksCollection;

        private Page _currentPage;

        private readonly Page loadPage;

        private IStackPageService _stackPage = null;

        public readonly Dictionary<string, bool> TableLoaded = new Dictionary<string, bool> {
            {"Stocks", false},
            {"TableOperations", false},
            {"TableDepositsAndDebits", false}
        };

        #endregion


        #region Properties

        public bool IsStart { get; private set; } = false;

        #endregion


        #region Methods

        private void StartCheck(string name, bool b)
        {
            TableLoaded[name] = b;
            CheckLoaded();
        }

        private void CheckLoaded()
        {
            foreach (var load in TableLoaded)
            {
                if (!load.Value)
                {
                    if (!_stocksCollection.StocksComplete) Start();
                    return;
                }
            }

            Stop();
        }

        public void StocksUpdateStart()
        {
            _stocksCollection.Stop();

            _stocksCollection.Start(10000);
        }

        public void Start()
        {
            if (!IsStart)
            {
                IsStart = true;
                _currentPage = _pagesService.CurrentPage;
                _pagesService.ChangePage(loadPage);
            }
        }

        public void Stop()
        {
            if (IsStart)
            {
                IsStart = false;

                if (_stackPage != null)
                {
                    _stackPage.RemovePage();
                    _stackPage = null;
                }
                else
                {
                    _pagesService.ChangePage(_currentPage);
                }
            }
        }

        #endregion

       
    }
}
