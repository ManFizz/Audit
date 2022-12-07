using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.MainWindow;

public partial class CategoriesPage : Page
{
    public CategoriesPage()
    {
        InitializeComponent();
        UpdateTable();
        //CategoriesGrid.BeginEdit();
    }

    public void UpdateTable()
    {
        var app = (App) Application.Current;
        var mysqlCmd = new MySqlCommand("SELECT * FROM categories", app.DbCon);
        app.DbCon.Open();
        var result = mysqlCmd.ExecuteReader();
        app.ArrCategories.Clear();
        while (result.Read())
        {
            app.ArrCategories.Add(new Category(result.GetInt32(0),result.GetString(1),result.GetInt32(2)));
        }
        CategoriesGrid.ItemsSource = app.ArrCategories;
        app.DbCon.Close();
    }
}