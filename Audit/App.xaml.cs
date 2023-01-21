using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit
{

    public partial class App : Application
    {
        public readonly MySqlConnection DbCon = new();
        
        public User ActiveUser;
        public DataGrid LastDataGrid;
        
        public readonly ObservableCollection<Category> ArrCategories = new();
        public readonly ObservableCollection<Company> ArrCompany = new();
        public readonly ObservableCollection<HoursRecord> ArrHoursRecords = new();
        public readonly ObservableCollection<User> ArrUsers = new();
        public readonly ObservableCollection<Worker> ArrWorkers = new();
        public readonly List<string> QueryList = new();
        public readonly List<int> AvailableYears = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            BuildConnection();
            UpdateAllTables();
            base.OnStartup(e);
        }

        public void UpdateAllTables()
        {
            UpdateCategoriesTable();
            UpdateWorkersTable();
            UpdateCompanyTable();
            UpdateUsersTable();
            UpdateHoursRecordsTable();
        }

        public void UpdateCategoriesTable()
        {
            ArrCategories.Clear();
            var mysqlCmd = new MySqlCommand("SELECT * FROM categories", DbCon);
            DbCon.Open();
            var result = mysqlCmd.ExecuteReader();
            while (result.Read())
                ArrCategories.Add(new Category(result.GetInt32(0),result.GetString(1),result.GetInt32(2)));
            DbCon.Close();
        }
    
        public void UpdateWorkersTable()
        {
            ArrWorkers.Clear();
            var mysqlCmd = new MySqlCommand("SELECT * FROM workers", DbCon);
            DbCon.Open();
            var r = mysqlCmd.ExecuteReader();
            while (r.Read())
                ArrWorkers.Add(new Worker(r.GetInt32(0),r.GetString(1),r.GetString(2), r.GetString(3), r.GetString(4), r.GetInt32(5)));
            DbCon.Close();
        }
        
        public void UpdateCompanyTable()
        {
            var mysqlCmd = new MySqlCommand("SELECT * FROM company", DbCon);
            DbCon.Open();
            var result = mysqlCmd.ExecuteReader();
            ArrCompany.Clear();
            while (result.Read())
                ArrCompany.Add(new Company(result.GetInt32(0),result.GetString(1),result.GetString(2)));
            DbCon.Close();
        }
        
        public void UpdateUsersTable()
        {
            var mysqlCmd = new MySqlCommand("SELECT * FROM users", DbCon);
            DbCon.Open();
            var r = mysqlCmd.ExecuteReader();
            ArrUsers.Clear();
            while (r.Read())
            {
                Enum.TryParse(r.GetString(4), false, out TypeUser type);
                int? idWorker = r.IsDBNull(3) ? null : r.GetInt32(3);
                ArrUsers.Add(new User(r.GetInt32(0),r.GetString(1),r.GetString(2), idWorker, type));
            }
            DbCon.Close();
        }
        
        public void UpdateHoursRecordsTable()
        {
            var mysqlCmd = new MySqlCommand("SELECT * FROM hours_records", DbCon);
            DbCon.Open();
            var r = mysqlCmd.ExecuteReader();
            ArrHoursRecords.Clear();
            AvailableYears.Clear();
            while (r.Read())
            {
                ArrHoursRecords.Add(new HoursRecord(r.GetInt32(0), r.GetInt32(1), r.GetInt32(2), r.GetString(3), r.GetInt32(4)));
                var dt = DateTime.Parse(r.GetString(3));
                if(!AvailableYears.Contains(dt.Year)) 
                    AvailableYears.Add(dt.Year);
            }

            DbCon.Close();
        }
        
        public void FastQuery(string query)
        {
            QueryList.Add(query);
        }

        public void SaveQuery()
        {
            var query = QueryList.Aggregate("START TRANSACTION;", (current, q) => current + q) + "COMMIT;";
            try
            {
                var cmd = new MySqlCommand(query, DbCon);
                DbCon.Open();
                cmd.ExecuteNonQuery();
                DbCon.Close();
            }
            catch
            {
                DbCon.Close();
                throw;
            }
            QueryList.Clear();
            MessageBox.Show("Изменения успешно сохранены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        }

        private void BuildConnection()
        {
            ParseCfgDb(out var host, out var database, out var user, out var pass);
            DbCon.ConnectionString = $"Server={host}; database={database}; UID={user}; password={pass}";
        }

        private static void ParseCfgDb(out string host, out string database, out string user, out string pass)
        {
            using var fs = new StreamReader("cfg\\db.ini");
            var cfg = fs.ReadToEnd();
            var s = cfg.Split("\r\n");
            host = s[0].Split(":")[1];
            database = s[1].Split(":")[1];
            user = s[2].Split(":")[1];
            pass = s[03].Split(":")[1];
        }
    }
}