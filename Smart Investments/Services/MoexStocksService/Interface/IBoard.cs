using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Investments.Services.MoexStocksService.DataMarket.Interface
{
    public interface IBoard
    {
        Task GetStocksAsync(Action<Stock> GetStock);
    }
}
