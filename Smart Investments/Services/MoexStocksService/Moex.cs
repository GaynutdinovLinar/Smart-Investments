using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_Investments.Services.MoexStocksService
{
    public class Moex
    {
        public async Task GetBoardsAsync(ICollection<Board> boards, params string[] ListId )
        {
            string url = "http://iss.moex.com/iss/index.json?iss.meta=off&iss.only=engines,markets,boards&engines.columns=id,name&markets.columns=id,market_name&boards.columns=engine_id,market_id,boardid,board_title,is_traded";

            MyFunction.CheckInternet();

            string response = await MyFunction.GetResponse(url);

            var json = JObject.Parse(response);

            var Jengines = json["engines"]["data"];
            var Jmarkets = json["markets"]["data"];
            var Jboards = json["boards"]["data"];

            if (boards.Count > 0) boards.Clear();

            foreach (var sec in Jboards)
            {
                bool isTraded;
                if (sec[4].ToString() == "1") isTraded = true;
                else isTraded = false;

                var boardid = sec[2].ToString();

                if (ListId.Contains(boardid)) 
                    boards.Add(new Board(boardid, MyFunction.SearchInJToken(Jengines, sec[0].ToString(), 0, 1), MyFunction.SearchInJToken(Jmarkets, sec[1].ToString(), 0, 1), sec[3].ToString(), isTraded));
            }
        }

        public Stock SearchStock(ICollection<Stock> ls, string secid)
        {
            if (ls != null)
            {
                Stock st = null;

                foreach (var l in ls)
                {
                    if (l.Secid == secid)
                    {
                        st = l;
                        break;
                    }
                }

                return st;
            }
            else return null;
        }

        public Board SearchBoard(ICollection<Board> ls, string boardid)
        {
            if (ls != null)
            {
                Board st = null;

                foreach (var l in ls)
                {

                    if (l.BoardId == boardid)
                    {
                        st = l;
                        break;
                    }
                }

                return st;
            }
            else return null;
        }
    }
}
