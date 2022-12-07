using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Audit.Objects;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Audit
{

    public partial class App : Application
    {
        public readonly MySqlConnection DbCon = new();
        public User? ActiveUser = null;
        public readonly ObservableCollection<Category> ArrCategories = new();
        public readonly ObservableCollection<Company> ArrCompany = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            BuildConnection();
            base.OnStartup(e);
        }

        public int FastQuery(string query)
        {
            var cmd = new MySqlCommand(query, DbCon);
            DbCon.Open();
            var r = cmd.ExecuteNonQuery();
            DbCon.Close();
            return r;
        }

        private void BuildConnection()
        {
            ParseCfgDb(out var host, out var database, out var user, out var pass);
            DbCon.ConnectionString = $"Server={host}; database={database}; UID={user}; password={pass}";
        }

        private static bool ParseCfgDb(out string host, out string database, out string user, out string pass)
        {
            using var fs = new StreamReader("C:\\Users\\Mak\\RiderProjects\\Audit\\Audit\\cfg\\db.ini");
            var cfg = fs.ReadToEnd();
            var s = cfg.Split("\r\n");
            host = s[0].Split(":")[1];
            database = s[1].Split(":")[1];
            user = s[2].Split(":")[1];
            pass = s[03].Split(":")[1];
            return true;
        }
    }
}