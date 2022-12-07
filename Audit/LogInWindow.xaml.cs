using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Audit
{
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            var app = ((App) Application.Current);
            var login = MySqlHelper.EscapeString(Login.Text);
            var password = MySqlHelper.EscapeString(Password.Password);
            if (!IsValidPassword(login) || !IsValidLogin(password))
                return;
            var mysqlCmd = new MySqlCommand($"SELECT id, worker_id, type FROM users WHERE login='{login}' AND password='{password}'", app.DbCon);
            app.DbCon.Open();
            var reader = mysqlCmd.ExecuteReader();
            if (!reader.Read())
            {
                MessageBox.Show("Invalid login or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                reader.Close();
                app.DbCon.Close();
                return;
            }

            Enum.TryParse(reader.GetString(2), false, out User.TypeUser type);
            app.ActiveUser = new User(reader.GetInt32(0), login, password, reader.GetInt32(1), type);
            reader.Close();
            app.DbCon.Close();


            var window = new MainWindow.MainWindow(this);
            window.Show();
            //this.Close(); //ERROR Display mainwindow - fix by constructor mainwindow
        }

        private static bool IsValidLogin(string login)
        {
            if (login.Length is < 6 or > 32)
                return false;
            
            return true;
        }

        private static bool IsValidPassword(string password)
        {
            if (password.Length is < 6 or > 32)
                return false;
            
            return true;
        }
    }
}