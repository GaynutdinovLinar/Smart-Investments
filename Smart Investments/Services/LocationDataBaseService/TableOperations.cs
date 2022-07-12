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
    public class TableOperations
    {
        public TableOperations(LocationDataBase locationDataBase, ExcelReaderService excelReaderService)
        {

            _locationDataBase = locationDataBase;
            _excelReaderService = excelReaderService;

            _locationDataBase.OnFilesReady += () =>
            {
                new Thread(CompleteTableFromExcel).Start();
            };
        }


        #region Events

        public event Action StartCompleteCollection;
        public event Action CollectionCompleted;

        #endregion

        #region Values

        private DataTable _excelTable;

        private readonly LocationDataBase _locationDataBase;
        private readonly ExcelReaderService _excelReaderService;

        public readonly string NameTable = "Операции";

        public readonly Dictionary<int, string> TableColumn = new Dictionary<int, string> {
            {0, "Номер_договора"},
            {1, "Номер_сделки"},
            {2, "Дата_заключения"},
            {3, "Дата_расчётов"},
            {4, "Код_финансового_инструмента"},
            {5, "Тип_финансового_инструмента"},
            {6, "Тип_рынка"},
            {7, "Операция"},
            {8, "Количество"},
            {9, "Цена"},
            {10, "НКД"},
            {11, "Валюта"},
            {12, "Курс"},
            {13, "Комиссия_торговой_системы"},
            {14, "Комиссия_банка"},
            {15, "Тип_сделки"}};

        #endregion

        #region Methods

        public bool DropFile(string path)
        {
            var tables = _excelReaderService.GetTablesExcel(path);

            bool ready = false;

            if (tables != null && tables.Count == 1)
            {
                foreach (DataRow row in tables[0].Rows)
                {
                    try
                    {
                        row.Field<string>("Номер договора");
                        row.Field<string>("Номер сделки");
                        row.Field<DateTime>("Дата заключения");
                        row.Field<DateTime>("Дата расчётов");
                        row.Field<string>("Код финансового инструмента");
                        row.Field<string>("Тип финансового инструмента");
                        row.Field<string>("Тип рынка");
                        row.Field<string>("Операция");
                        row.Field<double>("Количество");
                        row.Field<double>("Цена");
                        row.Field<double>("НКД");
                        row.Field<string>("Валюта");
                        row.Field<double>("Курс");
                        row.Field<double>("Комиссия торговой системы");
                        row.Field<double>("Комиссия банка");
                        row.Field<string>("Тип сделки");
                    }
                    catch
                    {
                        _locationDataBase.TableOperationsReady = ready;
                        return ready;
                    }

                }
            }
            else
            {
                _locationDataBase.TableOperationsReady = ready;
                return ready;
            }

            ready = true;

            _locationDataBase.TableOperationsReady = ready;
            _excelTable = tables[0];


            return ready;
        }

        public async void GetAndReadTableAsync()
        {
            StartCompleteCollection?.Invoke();

            await Task.Run(() =>
            {
                lock (_locationDataBase.Users)
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
                    else _locationDataBase.TableOperationsComplete();
                }              
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
	                {TableColumn[4]}   TEXT NOT NULL,
	                {TableColumn[5]} TEXT NOT NULL,
	                {TableColumn[6]}    TEXT NOT NULL,
                    {TableColumn[7]}  TEXT NOT NULL,
	                {TableColumn[8]}  INTEGER NOT NULL,
	                {TableColumn[9]}   REAL NOT NULL,
	                {TableColumn[10]} INTEGER NOT NULL,
	                {TableColumn[11]} TEXT NOT NULL,
	                {TableColumn[12]}    INTEGER NOT NULL,
                    {TableColumn[13]} REAL NOT NULL,
	                {TableColumn[14]}    REAL NOT NULL,
	                {TableColumn[15]}    TEXT NOT NULL); ";

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

                            newUser.AddInfoOperation(
                                row.Field<string>(TableColumn[1]),
                                DateTime.Parse(row.Field<string>(TableColumn[2])),
                                DateTime.Parse(row.Field<string>(TableColumn[3])),
                                row.Field<string>(TableColumn[4]),
                                row.Field<string>(TableColumn[7]),
                                (int)row.Field<Int64>(TableColumn[8]),
                                 row.Field<double>(TableColumn[9]),
                                (int)row.Field<Int64>(TableColumn[10]),
                                 row.Field<string>(TableColumn[11]),
                                (int)row.Field<Int64>(TableColumn[12]),
                                row.Field<double>(TableColumn[13]),
                                row.Field<double>(TableColumn[14]),
                                row.Field<string>(TableColumn[15])
                                );

                            _locationDataBase.Users.Add(newUser);
                        }
                        else
                        {
                            user.AddInfoOperation(
                                row.Field<string>(TableColumn[1]),
                                DateTime.Parse(row.Field<string>(TableColumn[2])),
                                DateTime.Parse(row.Field<string>(TableColumn[3])),
                                row.Field<string>(TableColumn[4]),
                                row.Field<string>(TableColumn[7]),
                                (int)row.Field<Int64>(TableColumn[8]),
                                 row.Field<double>(TableColumn[9]),
                                (int)row.Field<Int64>(TableColumn[10]),
                                 row.Field<string>(TableColumn[11]),
                                (int)row.Field<Int64>(TableColumn[12]),
                                row.Field<double>(TableColumn[13]),
                                row.Field<double>(TableColumn[14]),
                                row.Field<string>(TableColumn[15])
                                );
                        }
                    }

                    _locationDataBase.TableOperationsComplete();
                }
            }
        }
        

        public  void CompleteTableFromExcel()
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
                            $"{TableColumn[5]}, {TableColumn[6]}, {TableColumn[7]}, {TableColumn[8]},{TableColumn[9]},{TableColumn[10]},{TableColumn[11]},{TableColumn[12]},{TableColumn[13]},{TableColumn[14]}, {TableColumn[15]}) " +
                            $"VALUES (:{TableColumn[0]}, :{TableColumn[1]}, :{TableColumn[2]}, :{TableColumn[3]}, :{TableColumn[4]}, :{TableColumn[5]}, :{TableColumn[6]}, :{TableColumn[7]}," +
                            $":{TableColumn[8]}, :{TableColumn[9]}, :{TableColumn[10]}, :{TableColumn[11]}, :{TableColumn[12]}, :{TableColumn[13]}," +
                            $":{TableColumn[14]}, :{TableColumn[15]})";

                        try
                        {
                            foreach (DataRow row in _excelTable.Rows)
                            {
                                command.Parameters.AddWithValue(TableColumn[0], row.Field<string>("Номер договора"));
                                command.Parameters.AddWithValue(TableColumn[1], row.Field<string>("Номер сделки"));
                                command.Parameters.AddWithValue(TableColumn[2], row.Field<DateTime>("Дата заключения"));
                                command.Parameters.AddWithValue(TableColumn[3], row.Field<DateTime>("Дата расчётов"));
                                command.Parameters.AddWithValue(TableColumn[4], row.Field<string>("Код финансового инструмента"));
                                command.Parameters.AddWithValue(TableColumn[5], row.Field<string>("Тип финансового инструмента"));
                                command.Parameters.AddWithValue(TableColumn[6], row.Field<string>("Тип рынка"));
                                command.Parameters.AddWithValue(TableColumn[7], row.Field<string>("Операция"));
                                command.Parameters.AddWithValue(TableColumn[8], (int)row.Field<double>("Количество"));
                                command.Parameters.AddWithValue(TableColumn[9], row.Field<double>("Цена"));
                                command.Parameters.AddWithValue(TableColumn[10], (int)row.Field<double>("НКД"));
                                command.Parameters.AddWithValue(TableColumn[11], row.Field<string>("Валюта"));
                                command.Parameters.AddWithValue(TableColumn[12], (int)row.Field<double>("Курс"));
                                command.Parameters.AddWithValue(TableColumn[13], row.Field<double>("Комиссия торговой системы"));
                                command.Parameters.AddWithValue(TableColumn[14], row.Field<double>("Комиссия банка"));
                                command.Parameters.AddWithValue(TableColumn[15], row.Field<string>("Тип сделки"));
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
