using System.Windows;
using System.Windows.Controls;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.MainWindow.Pages;

public partial class WorkersPage : Page
{
    public WorkersPage()
    {
        InitializeComponent();
        UpdateTable();
    }
    
    public void UpdateTable()
    {
        var app = (App) Application.Current;
        var mysqlCmd = new MySqlCommand("SELECT * FROM workers", app.DbCon);
        app.DbCon.Open();
        var r = mysqlCmd.ExecuteReader();
        app.ArrWorkers.Clear();
        while (r.Read())
        {
            app.ArrWorkers.Add(new Worker(r.GetInt32(0),r.GetString(1),r.GetString(2), r.GetString(3), r.GetString(4), r.GetInt32(5)));
        }
        WorkersGrid.ItemsSource = app.ArrWorkers;
        app.DbCon.Close();
    }
}