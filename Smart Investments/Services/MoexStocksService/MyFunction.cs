using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Smart_Investments.Services.MoexStocksService
{
    internal static class MyFunction
    {
        internal static void CheckInternet()
        {
            var status = new Ping().Send(@"www.google.com").Status;

            if (status != IPStatus.Success) throw new Exception("Отсутствует подключение к интернету или сервис недоступен");
        }

        internal static string SearchInJToken(JToken jtok, string id, int columnSearch, int columnShow)
        {
            string res = "";

            foreach (var j in jtok)
            {
                if (j[columnSearch].ToString() == id)
                {
                    res = j[columnShow].ToString();
                    break;
                }
            }

            return res;
        }

        internal static async Task<string> GetResponse(string url)
        {
            using (HttpClient hc = new HttpClient())
            {
                var mess = await hc.GetStringAsync(url);
                return mess;
            }
        }

        internal static int? ParseInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            else return null;
        }

        internal static decimal? ParseDecimal(string s)
        {
            decimal i;
            if (decimal.TryParse(s, out i)) return i;
            else return null;
        }
    }
}
