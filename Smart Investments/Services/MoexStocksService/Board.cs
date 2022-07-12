using Newtonsoft.Json.Linq;
using Smart_Investments.Services.MoexStocksService.DataMarket.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smart_Investments.Services.MoexStocksService
{
    public class Board : IBoard
    {
        internal Board(string boardid, string engine, string market, string boardTitle, bool isTraded)
        {
            BoardId = boardid;
            Engine = engine;
            Market = market;
            BoardTitle = boardTitle;
            IsTraded = isTraded;
        }

        #region Properties

        public string BoardId { get; internal set; }

        public string Engine { get; internal set; }

        public string Market { get; internal set; }

        public string BoardTitle { get; internal set; }

        public bool IsTraded { get; internal set; }

        public bool IsStarted { get; private set; }

        #endregion

        public async Task GetStocksAsync(Action<Stock> GetStock = null)
        {
            if (IsTraded)
            {
                string url = $"https://iss.moex.com/iss/engines/{Engine}/markets/{Market}/boards/{BoardId}/securities.json?iss.meta=off&iss.only=securities,marketdata&securities.columns=SECID,SHORTNAME,SECNAME,PREVPRICE,LOTSIZE&marketdata.columns=CHANGE,LAST";

                MyFunction.CheckInternet();

                string response = await MyFunction.GetResponse(url);

                var json = JObject.Parse(response);

                var Jsecurities = json["securities"]["data"];
                var Jmarketdata = json["marketdata"]["data"];

                int i = 0;
                foreach (var sec in Jsecurities)
                {
                    var lastcost = MyFunction.ParseDecimal(Jmarketdata[i][1].ToString());

                    if (lastcost is null) lastcost = MyFunction.ParseDecimal(sec[3].ToString());

                    Stock st = new Stock(this, sec[0].ToString(), sec[1].ToString(), sec[2].ToString(), MyFunction.ParseInt(sec[4].ToString()), MyFunction.ParseDecimal(Jmarketdata[i][0].ToString()), lastcost);

                    GetStock?.Invoke(st);
                    i++;
                }
            }
        }
    }
}
