using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace Smart_Investments.Services.LocationDataBaseService
{
    public class LocationDataBase
    {

        public LocationDataBase()
        {
            Users = new List<User>();
        }

        #region Values

        private readonly string _path = @"Data Source=|DataDirectory|\local.db;";

        public Dictionary<string, bool> FilesReady = new Dictionary<string, bool>
        {
            ["Операции"] = false,
            ["Зачисления и сделки"] = false
        };

        private Dictionary<string, bool> FilesComplete = new Dictionary<string, bool>
        {
            ["Операции"] = false,
            ["Зачисления и сделки"] = false
        };

        #endregion

        #region Events

        public event Action<List<User>> OnUsersChange;

        public event Action OnFilesReady;

        public event Action<User> OnSelectedUserChange;

        #endregion

        #region Properties

        public List<User> Users { get; set; }

        public User SelectedUser { get; set; }

        public bool DBExists
        {
            get => File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\local.db");
        }

        public string PathDB { get => _path; }

        public bool TableOperationsReady
        {
            set
            {
                FilesReady["Операции"] = value;
                CheckReady();
            }
        }

        public bool TableDepositsAndDebitsReady
        {
            set
            {
                FilesReady["Зачисления и сделки"] = value;
                CheckReady();
            }
        }

        public void TableOperationsComplete()
        {
            FilesComplete["Операции"] = true;
            CheckComplete();
        }

        public void TableDepositsAndDebitsComplete()
        {
            FilesComplete["Зачисления и сделки"] = true;
            CheckComplete();
        }


        #endregion

        #region Methods

        public void ChangeSelectedUser(User user)
        {
            SelectedUser = user;

            OnSelectedUserChange?.Invoke(SelectedUser);
        }

        public void CheckReady()
        {
            foreach (var file in FilesReady)
            {
                if (!file.Value) return;
            }

            Users.Clear();
            OnFilesReady?.Invoke();

            foreach (var file in FilesReady.Keys)
            {
                FilesReady[file] = false;
            }
        }

        public void CheckComplete()
        {
            lock (FilesComplete)
            {
                foreach (var file in FilesComplete)
                {
                    if (!file.Value) return;
                }

                OnUsersChange?.Invoke(Users);

                if (Users != null && Users.Count > 0)
                {
                    bool have = false;

                    foreach (var u in Users)
                    {
                        if (SelectedUser != null && u.UserId == SelectedUser.UserId)
                        {
                            ChangeSelectedUser(u);
                            have = true;
                        }
                    }

                    if (!have) ChangeSelectedUser(Users[0]);
                }
                else ChangeSelectedUser(null);


                foreach (var file in FilesComplete.Keys)
                {
                    FilesComplete[file] = false;
                }
            }
            
        }

        #endregion

    }
}
