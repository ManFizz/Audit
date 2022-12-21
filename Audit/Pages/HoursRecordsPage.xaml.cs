using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.Pages;

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

    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        
    }

    private void HoursRecordsGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        
    }

    private void HoursRecordsGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        
    }
}