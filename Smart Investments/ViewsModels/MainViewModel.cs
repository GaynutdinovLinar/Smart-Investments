using Smart_Investments.Services;
using Smart_Investments.Services.Commands.Base;
using Smart_Investments.Services.LocationDataBaseService;
using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using Smart_Investments.Services.PageService;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace Smart_Investments.ViewsModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel (PagesService pageService, ThemeSwitchService themeSwitchService, BagPagesStack bagPagesStack, LoadService loadService,
            MarketPagesStack marketPagesStack, LocationDataBase locationDataBase, TableOperations tableOperations, TableDepositsAndDebits tableDepositsAndDebits)
        {
            _pageService = pageService;
            _themeSwitchService = themeSwitchService;
            _loadService = loadService;
            _locationDataBase = locationDataBase;

            _pageService.OnPageChanged += (page) => CurrentPage = page;

            _marketPagesStack = marketPagesStack;
            _marketPagesStack.Init();

            _bagPagesStack = bagPagesStack;

            tableOperations.GetAndReadTableAsync();
            tableDepositsAndDebits.GetAndReadTableAsync();

            locationDataBase.OnSelectedUserChange += (user) => SelectedUser = user;
            locationDataBase.OnUsersChange += (users) => Users = new ObservableCollection<User>(users);
        }

        #region Value

        private readonly PagesService _pageService;
        private readonly ThemeSwitchService _themeSwitchService;
        private readonly LoadService _loadService;
        private readonly LocationDataBase _locationDataBase;

        private readonly MarketPagesStack _marketPagesStack;
        private readonly BagPagesStack _bagPagesStack;

        private Page _currentPage;

        private bool _isLigthTheme = true;


        #endregion

        #region Properties

        private User _selectedUser;

        public User SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<User> _users;

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public bool IsLigthTheme
        {
            get
            {
                return _isLigthTheme;
            }
            set
            {
                _isLigthTheme = value;

                if (_isLigthTheme) _themeSwitchService.ChangeTheme(Theme.Light);
                else _themeSwitchService.ChangeTheme(Theme.Dark);

            }
        }

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        public ICommand Bag_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _pageService.ChangePage(_bagPagesStack.GetPage());
            },
                (obj) => !_loadService.IsStart);
        }

        public ICommand Market_Click
        {
            get => new DelegateCommand((obj) =>
            {
                _pageService.ChangePage(_marketPagesStack.GetPage());
            },
                (obj) => !_loadService.IsStart);
        }

        public ICommand User_Click
        {
            get => new DelegateCommand((obj) =>
            {
                if (obj is User u)
                {
                    _locationDataBase.ChangeSelectedUser(u);
                }
            });
        }


        #endregion

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
