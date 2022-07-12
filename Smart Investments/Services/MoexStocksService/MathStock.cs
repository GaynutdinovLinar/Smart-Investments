using Smart_Investments.Services.MoexStocksService;
using Smart_Investments.Services.MoexStocksService.DataMarket.Interface;
using System;
using System.Collections.ObjectModel;

namespace Smart_Investments.Services.DataMarket
{
    public class MathStock : IMathStock
    {
        private Stock _stock;

        public MathStock(Stock stock)
        {
            Stock = stock;
        }

        public Stock Stock { get => _stock; set => _stock = value; }

        
    }
}
