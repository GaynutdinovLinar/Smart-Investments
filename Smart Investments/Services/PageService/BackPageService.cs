using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Views.Pages;
using System;
using System.Windows.Input;

namespace Smart_Investments.Services.PageService
{
    class BackPageService
    {
        public BackPageService(BagPagesStack bagPagesStack, MarketPagesStack marketPagesStack)
        {
            _bagPagesStack = bagPagesStack;

            _marketPagesStack = marketPagesStack;
        }

        private readonly BagPagesStack _bagPagesStack;
        private readonly MarketPagesStack _marketPagesStack;

        public int BackButtonBagPagesVisibility
        {
            get
            {
                return _bagPagesStack.CountPages;
            }
        }


        #region Commands

        public ICommand BagPagesStackBack_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (_bagPagesStack.CountPages > 1) _bagPagesStack.RemovePage();
            });
        }

        public ICommand MarketPagesStackBack_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (_marketPagesStack.CountPages > 1) _marketPagesStack.RemovePage();
            });
        }

        #endregion
    }
}
