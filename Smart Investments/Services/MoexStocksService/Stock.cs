using Newtonsoft.Json.Linq;
using Smart_Investments.Services.MoexStocksService.DataMarket.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smart_Investments.Services.MoexStocksService
{
    public class Stock
    {
        internal Stock() { }

        internal Stock(Board board, string secid, string shortname, string secname, int? lotsize, decimal? changeCost, decimal? lastCost)
        {
            Secid = secid;
            Shortname = shortname;
            Secname = secname;
            LotSize = lotsize;
            ChangeCost = changeCost;
            LastCost = lastCost;

            if (lastCost != null)
            {
                if (changeCost != null) StartCost = lastCost - changeCost;
                else StartCost = lastCost;
            }

            BoardStock = board;
        }

        #region Properties

        public string Secid { get; internal set; }
        public string Shortname { get; internal set; }
        public string Secname { get; internal set; }
        public int? LotSize { get; internal set; }
        public decimal? StartCost { get; internal set; }
        public decimal? LastCost { get; internal set; }
        public decimal? ChangeCost { get; internal set; }
        public decimal? ProcentChangeCostDay { get => ChangeCost / StartCost; }

        public decimal? LotCost { get => LotSize * LastCost; }
        public Board BoardStock { get; internal set; }


        #endregion

        public async Task GetCostPeriodStockAsync(ICollection<DateCost> daycost, Interval interval, DateTime? startDate = null)
        {
            string url = "";

            if (startDate is null)
            {
                url = $"http://iss.moex.com/iss/engines/{BoardStock.Engine}/markets/{BoardStock.Market}/boards/{BoardStock.BoardId}/securities/{Secid}/candles.json?iss.meta=off&candles.columns=open,close,low,high,end&interval={(int)interval}";
            }
            else
            {
                DateTime date = (DateTime)startDate;

                string st = date.ToString("yyyy-MM-dd-HH-mm");

                url = $"http://iss.moex.com/iss/engines/{BoardStock.Engine}/markets/{BoardStock.Market}/boards/{BoardStock.BoardId}/securities/{Secid}/candles.json?iss.meta=off&candles.columns=open,close,low,high,end&interval={(int)interval}&from={st}";
            }

            MyFunction.CheckInternet();

            string response = await MyFunction.GetResponse(url);

            var json = JObject.Parse(response);

            var Jhistory = json["candles"]["data"];

            int i = 0;

            DateCost dc = null;

            foreach (var sec in Jhistory)
            {
                dc = new DateCost((DateTime)sec[4], MyFunction.ParseDecimal(sec[0].ToString()), MyFunction.ParseDecimal(sec[1].ToString()), MyFunction.ParseDecimal(sec[2].ToString()), MyFunction.ParseDecimal(sec[3].ToString()));

                daycost.Add(dc);

                i++;
            }

            if (i == 500)
            {
                while (i == 500)
                {
                    string st = dc.Day.ToString("yyyy-MM-dd");

                    url = $"http://iss.moex.com/iss/engines/{BoardStock.Engine}/markets/{BoardStock.Market}/boards/{BoardStock.BoardId}/securities/{Secid}/candles.json?iss.meta=off&candles.columns=open,close,low,high,end&interval={(int)interval}&from={st}";

                    MyFunction.CheckInternet();

                    response = await MyFunction.GetResponse(url);

                    json = JObject.Parse(response);

                    Jhistory = json["candles"]["data"];

                    i = 0;

                    foreach (var sec in Jhistory)
                    {
                        dc = new DateCost((DateTime)sec[4], MyFunction.ParseDecimal(sec[0].ToString()), MyFunction.ParseDecimal(sec[1].ToString()), MyFunction.ParseDecimal(sec[2].ToString()), MyFunction.ParseDecimal(sec[3].ToString()));

                        daycost.Add(dc);

                        i++;
                    }
                }
            }
        }

    }

    public enum Interval
    {
        OneMinute = 1,
        TenMinutes = 10,
        OneHour = 60,
        OneDay = 24,
        OneWeek = 7,
        OneMonth = 31,
        OneQuarter = 4
    }
}
