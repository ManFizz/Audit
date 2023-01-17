using System;
using System.Collections.ObjectModel;
using System.IO;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            BuildConnection();
            UpdateAllTables();
            base.OnStartup(e);
        }

        private void UpdateAllTables()
        {
            UpdateCategoriesTable();
            UpdateWorkersTable();
            UpdateCompanyTable();
            UpdateUsersTable();
            UpdateHoursRecordsTable();
        }

        private void UpdateCategoriesTable()
        {
            ArrCategories.Clear();
            var mysqlCmd = new MySqlCommand("SELECT * FROM categories", DbCon);
            DbCon.Open();
            var result = mysqlCmd.ExecuteReader();
            while (result.Read())
                ArrCategories.Add(new Category(result.GetInt32(0),result.GetString(1),result.GetInt32(2)));
            DbCon.Close();
        }
    
        private void UpdateWorkersTable()
        {
            ArrWorkers.Clear();
            var mysqlCmd = new MySqlCommand("SELECT * FROM workers", DbCon);
            DbCon.Open();
            var r = mysqlCmd.ExecuteReader();
            while (r.Read())
                ArrWorkers.Add(new Worker(r.GetInt32(0),r.GetString(1),r.GetString(2), r.GetString(3), r.GetString(4), r.GetInt32(5)));
            DbCon.Close();
        }
        
        private void UpdateCompanyTable()
        {
            var mysqlCmd = new MySqlCommand("SELECT * FROM company", DbCon);
            DbCon.Open();
            var result = mysqlCmd.ExecuteReader();
            ArrCompany.Clear();
            while (result.Read())
                ArrCompany.Add(new Company(result.GetInt32(0),result.GetString(1),result.GetString(2)));
            DbCon.Close();
        }
        
        private void UpdateUsersTable()
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
        
        private void UpdateHoursRecordsTable()
        {
            var mysqlCmd = new MySqlCommand("SELECT * FROM hours_records", DbCon);
            DbCon.Open();
            var r = mysqlCmd.ExecuteReader();
            ArrHoursRecords.Clear();
            while (r.Read())
                ArrHoursRecords.Add(new HoursRecord(r.GetInt32(0),r.GetInt32(1),r.GetInt32(2), r.GetString(3),r.GetInt32(4)));
            DbCon.Close();
        }
        
        public int FastQuery(string query)
        {
            try
            {
                var cmd = new MySqlCommand(query, DbCon);
                DbCon.Open();
                var r = cmd.ExecuteNonQuery();
                DbCon.Close();
                return r;
            }
            catch
            {
                DbCon.Close();
                throw;
            }
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