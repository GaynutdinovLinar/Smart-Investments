using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace Smart_Investments.Services
{
    public class ExcelReaderService
    {

        public DataTableCollection GetTablesExcel(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                    });

                    return result.Tables;
                }
            }
        }

        public void ReadRowsTable(DataTable table, Action<object[]> itemArray = null)
        {
            foreach (DataRow dataRows in table.Rows)
            {
                var item = dataRows.ItemArray;

                itemArray?.Invoke(item);
            }
        }
 
    }
}
