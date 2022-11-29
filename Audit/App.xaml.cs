using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit
{
    public partial class App : Application
    {
        private readonly MySqlConnection _dbCon = new();
        public User? ActiveUser = null;
        public readonly List<Category> ArrCategories = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            BuildConnection();
            
            base.OnStartup(e);
        }

        public MySqlDataReader FastQuery(string query)
        {
            if(_dbCon.State == ConnectionState.Open)
                _dbCon.Close();
            
            _dbCon.Open();
            return  (new MySqlCommand(query, _dbCon)).ExecuteReader();
        }

        private void BuildConnection()
        {
            ParseCfgDb(out var host, out var database, out var user, out var pass);
            _dbCon.ConnectionString = $"Server={host}; database={database}; UID={user}; password={pass}";
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