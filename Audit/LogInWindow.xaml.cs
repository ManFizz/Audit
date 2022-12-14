using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Audit.Objects;
using MySql.Data.MySqlClient;


namespace Audit
{
    public partial class LogInWindow : Window
    {
        
        public LogInWindow()
        {
            InitializeComponent();
            this.WindowStyle+=2;
        }

        private readonly MainWindow? _mainWindow = null;
        public LogInWindow(MainWindow? mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        
        private bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
            
            _shown = true;
            if(_mainWindow != null)
                _mainWindow.Close();
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


            var window = new MainWindow(this);
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