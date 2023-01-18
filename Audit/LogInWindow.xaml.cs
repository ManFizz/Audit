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

        private readonly MainWindow? _mainWindow;
        public LogInWindow(MainWindow? mainWindow)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
            _mainWindow?.Close();
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

            Enum.TryParse(reader.GetString(2), false, out TypeUser type);
            int? idWorker = reader.IsDBNull(1) ? null : reader.GetInt32(1);
            app.ActiveUser = new User(reader.GetInt32(0), login, password, idWorker, type);
            reader.Close();
            app.DbCon.Close();

            Hide();
            var window = new MainWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
            Close();
        }

        private static bool IsValidLogin(string login)
        {
            try
            {
                User.CheckLogin(login);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
            
            return true;
        }

        private static bool IsValidPassword(string password)
        {
            try
            {
                User.CheckPassword(password);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
            
            return true;
        }
    }
}