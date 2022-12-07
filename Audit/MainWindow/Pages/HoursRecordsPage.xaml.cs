using System.Windows;
using System.Windows.Controls;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.MainWindow.Pages;

public partial class HoursRecordsPage : Page
{
    public HoursRecordsPage()
    {
        InitializeComponent();
        UpdateTable();
    }
    
    public void UpdateTable()
    {
        var app = (App) Application.Current;
        var mysqlCmd = new MySqlCommand("SELECT * FROM hours_records", app.DbCon);
        app.DbCon.Open();
        var r = mysqlCmd.ExecuteReader();
        app.ArrHoursRecords.Clear();
        while (r.Read())
        {
            app.ArrHoursRecords.Add(new HoursRecrod(r.GetInt32(0),r.GetInt32(1),r.GetInt32(2), r.GetString(3),r.GetInt32(4)));
        }
        HoursRecordsGrid.ItemsSource = app.ArrHoursRecords;
        app.DbCon.Close();
    }
}