using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.Pages;

public partial class UsersPage : Page
{
    public UsersPage()
    {
        InitializeComponent();
        UpdateTable();
    }
    
    public void UpdateTable()
    {
        var app = (App) Application.Current;
        var mysqlCmd = new MySqlCommand("SELECT * FROM users", app.DbCon);
        app.DbCon.Open();
        var r = mysqlCmd.ExecuteReader();
        app.ArrUsers.Clear();
        while (r.Read())
        {
            Enum.TryParse(r.GetString(4), false, out User.TypeUser type);
            app.ArrUsers.Add(new User(r.GetInt32(0),r.GetString(1),r.GetString(2), r.GetInt32(3), type));
        }
        UsersGrid.ItemsSource = app.ArrUsers;
        app.DbCon.Close();
    }

    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        
    }

    private void UsersGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        
    }

    private void UsersGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        
    }
}