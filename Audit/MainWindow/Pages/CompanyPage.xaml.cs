using System.Windows;
using System.Windows.Controls;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.MainWindow.Pages;

public partial class CompanyPage : Page
{
    public CompanyPage()
    {
        InitializeComponent();
        UpdateTable();
    }
    public void UpdateTable()
    {
        var app = (App) Application.Current;
        var mysqlCmd = new MySqlCommand("SELECT * FROM company", app.DbCon);
        app.DbCon.Open();
        var result = mysqlCmd.ExecuteReader();
        app.ArrCompany.Clear();
        while (result.Read())
        {
            app.ArrCompany.Add(new Company(result.GetInt32(0),result.GetString(1),result.GetString(2)));
        }
        CompanyGrid.ItemsSource = app.ArrCompany;
        app.DbCon.Close();
    }
}