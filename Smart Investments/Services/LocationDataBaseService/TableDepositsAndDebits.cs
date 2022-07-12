using Smart_Investments.Services.LocationDataBaseService.UserStocks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Smart_Investments.Services.LocationDataBaseService
{
    class TableDepositsAndDebits
    {
        public TableDepositsAndDebits(LocationDataBase locationDataBase, ExcelReaderService excelReaderService)
        {
            _locationDataBase = locationDataBase;
            _excelReaderService = excelReaderService;

            _locationDataBase.OnFilesReady += () => new Thread(CompleteTableFromExcel).Start();
        }

        #region Events

        public event Action StartCompleteCollection;
        public event Action CollectionCompleted;

        #endregion

        #region Values

        private DataTable _excelTable;

        private readonly LocationDataBase _locationDataBase;
        private readonly ExcelReaderService _excelReaderService;

        public readonly string NameTable = "Зачисления_и_списания";

        public readonly Dictionary<int, string> TableColumn = new Dictionary<int, string> {
            {0, "Номер_договора"},
            {1, "Дата_подачи_поручения"},
            {2, "Дата_заключения"},
            {3, "Операция"},
            {4, "Сумма"},
            {5, "Валюта_операции"},
            {6, "Списание_с"},
            {7, "Зачисление_на"},
            {8, "Содержание_операции"},
            {9, "Статус"}};

        #endregion

        #region Properties


        #endregion

        #region Methods

        public bool DropFile(string path)
        {
            var tables = _excelReaderService.GetTablesExcel(path);

            bool ready = false;

            if (tables != null && tables.Count == 2)
            {
                foreach (DataRow row in tables[0].Rows)
                {
                    try
                    {
                        row.Field<string>("Номер договора");
                        row.Field<DateTime>("Дата подачи поручения");
                        row.Field<DateTime>("Дата исполнения поручения");
                        row.Field<string>("Операция");
                        row.Field<double>("Сумма");
                        row.Field<string>("Валюта операции");
                        row.Field<string>("Списание с");
                        row.Field<string>("Зачисление на");
                        row.Field<string>("Содержание операции");
                        row.Field<string>("Статус");
                    }
                    catch
                    {
                        _locationDataBase.TableDepositsAndDebitsReady = ready;
                        return ready;
                    }

                }
            }
            else
            {
                _locationDataBase.TableDepositsAndDebitsReady = ready;
                return ready;
            }

            ready = true;

            _locationDataBase.TableDepositsAndDebitsReady = ready;
            _excelTable = tables[0];


            return ready;
        }

        public async void GetAndReadTableAsync()
        {
            StartCompleteCollection?.Invoke();

            await Task.Run(() =>
            {
                if (_locationDataBase.DBExists)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(_locationDataBase.PathDB))
                    {
                        connection.Open();

                        CreateTable(connection);

                        ReadTable(GetTable(connection));

                        connection.Close();
                    }
                }
                else _locationDataBase.TableDepositsAndDebitsComplete();
            });

            CollectionCompleted?.Invoke();
        }

        public DataTable GetTable(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = $"SELECT * FROM {NameTable}";
                DataTable data = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);

                return data;
            }
        }

        private void CreateTable(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = $@"CREATE TABLE IF NOT EXISTS {NameTable} (
                    {TableColumn[0]}  TEXT NOT NULL,
	                {TableColumn[1]}  TEXT NOT NULL,
	                {TableColumn[2]}   TEXT NOT NULL,
	                {TableColumn[3]} TEXT NOT NULL,
	                {TableColumn[4]}   REAL NOT NULL,
                    {TableColumn[5]}   TEXT NOT NULL,
	                {TableColumn[6]} TEXT,
	                {TableColumn[7]}   TEXT,
                    {TableColumn[8]}   TEXT NOT NULL,
	                {TableColumn[9]} TEXT NOT NULL);";

                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        public void ReadTable(DataTable data)
        {
            lock (_locationDataBase.Users)
            {
                if (data != null && data.Rows.Count > 0)
                {
                    for (int i = data.Rows.Count - 1; i >= 0; i--)
                    {
                        var row = data.Rows[i];

                        string userName = row.Field<string>(TableColumn[0]);

                        var user = _locationDataBase.Users.FirstOrDefault(u => u.UserId == userName);

                        if (user == null)
                        {
                            User newUser = new User(userName);

                            newUser.AddInfoDepositsAndDebits(
                                DateTime.Parse(row.Field<string>(TableColumn[1])),
                                DateTime.Parse(row.Field<string>(TableColumn[2])),
                                row.Field<string>(TableColumn[3]),
                                row.Field<double>(TableColumn[4]),
                                row.Field<string>(TableColumn[5]),
                                row.Field<string>(TableColumn[6]),
                                row.Field<string>(TableColumn[7]),
                                row.Field<string>(TableColumn[8]),
                                row.Field<string>(TableColumn[9])
                                );

                            _locationDataBase.Users.Add(newUser);
                        }
                        else
                        {
                            user.AddInfoDepositsAndDebits(
                                DateTime.Parse(row.Field<string>(TableColumn[1])),
                                DateTime.Parse(row.Field<string>(TableColumn[2])),
                                row.Field<string>(TableColumn[3]),
                                row.Field<double>(TableColumn[4]),
                                row.Field<string>(TableColumn[5]),
                                row.Field<string>(TableColumn[6]),
                                row.Field<string>(TableColumn[7]),
                                row.Field<string>(TableColumn[8]),
                                row.Field<string>(TableColumn[9])
                                );
                        }
                    }
                    _locationDataBase.TableDepositsAndDebitsComplete();
                }
            }    
        }

        public void CompleteTableFromExcel()
        {
            //lock (_locationDataBase.Users)
            {
                using (SQLiteConnection connection = new SQLiteConnection(_locationDataBase.PathDB))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Restart();

                        var transaction = connection.BeginTransaction();//запускаем транзакцию

                        CreateTable(connection);

                        ClearTable(connection);

                        command.CommandText = $"INSERT INTO {NameTable} ({TableColumn[0]}, {TableColumn[1]}, {TableColumn[2]}, {TableColumn[3]}, {TableColumn[4]}, " +
                            $"{TableColumn[5]}, {TableColumn[6]}, {TableColumn[7]}, {TableColumn[8]}, {TableColumn[9]}) " +
                            $"VALUES (:{TableColumn[0]}, :{TableColumn[1]}, :{TableColumn[2]}, :{TableColumn[3]}, :{TableColumn[4]}, :{TableColumn[5]}, :{TableColumn[6]}, :{TableColumn[7]}," +
                            $":{TableColumn[8]}, :{TableColumn[9]})";

                        try
                        {
                            DataRow row;

                            for(int i = _excelTable.Rows.Count - 1; i >= 0; i--)
                            {
                                row = _excelTable.Rows[i];

                                command.Parameters.AddWithValue(TableColumn[0], row.Field<string>("Номер договора"));
                                command.Parameters.AddWithValue(TableColumn[1], row.Field<DateTime>("Дата подачи поручения"));
                                command.Parameters.AddWithValue(TableColumn[2], row.Field<DateTime>("Дата исполнения поручения"));
                                command.Parameters.AddWithValue(TableColumn[3], row.Field<string>("Операция"));
                                command.Parameters.AddWithValue(TableColumn[4], row.Field<double>("Сумма"));
                                command.Parameters.AddWithValue(TableColumn[5], row.Field<string>("Валюта операции"));
                                command.Parameters.AddWithValue(TableColumn[6], row.Field<string>("Списание с"));
                                command.Parameters.AddWithValue(TableColumn[7], row.Field<string>("Зачисление на"));
                                command.Parameters.AddWithValue(TableColumn[8], row.Field<string>("Содержание операции"));
                                command.Parameters.AddWithValue(TableColumn[9], row.Field<string>("Статус"));
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit(); //применяем изменения

                            sw.Stop();

                        }
                        catch
                        {
                            transaction.Rollback(); //откатываем изменения, если произошла ошибка
                            throw;
                        }
                        finally
                        {
                            connection.Close();
                        }

                        var table = GetTable(connection);

                        ReadTable(table);
                    }
                }
            }
            
        }

        public void ClearTable(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = $"DELETE FROM {NameTable}";

                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
