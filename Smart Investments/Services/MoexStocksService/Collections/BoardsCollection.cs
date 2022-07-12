using Smart_Investments.Services.MoexStocksService;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_Investments.Services.DataMarket
{
    internal class BoardsCollection
    {
        public BoardsCollection(ExceptionService exceptionService)
        {
            _exceptionService = exceptionService;
            Boards = new ObservableCollection<Board>();

            BoardsCompleted = false;

            _boardsStr = new Dictionary<string, string>
            {
                { "TQBR", "Росс. акции" },
                { "FQBR", "Ин. акции" },
                { "TQTF", "ETF" }
            };
        }

        #region Values

        private readonly ExceptionService _exceptionService;

        private Dictionary<string, string> _boardsStr;

        #endregion

        #region Properties 

        public ObservableCollection<Board> Boards { get; set; }

        public Dictionary<string, string> BoardsStr
        {
            get => _boardsStr;
            set => _boardsStr = value;
        }

        public bool BoardsCompleted { get; private set; }

        #endregion

        public async Task BoardsCollectionComplete()
        {
            Moex moex = new Moex();

            try
            {
                await moex.GetBoardsAsync(Boards, _boardsStr.Keys.ToArray());
                BoardsCompleted = true;
            }
            catch(System.Exception e)
            {
                _exceptionService.NewException(e, TypeException.Ethernet);
            }
        }
    }
}
