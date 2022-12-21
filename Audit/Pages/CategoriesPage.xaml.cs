using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.Pages;

public partial class CategoriesPage : Page
{
    public CategoriesPage()
    {
        InitializeComponent();
        UpdateTable();
    }

    private void UpdateTable()
    {
        var app = (App) Application.Current;
        var query = "SELECT * FROM categories";
        /*bool needAnd = false;
        var id =  MySqlHelper.EscapeString(idSearch.Text);
        var name =  MySqlHelper.EscapeString(NameSearch.Text);
        var payment =  MySqlHelper.EscapeString(PaymentSearch.Text);
        if(string.IsNullOrEmpty(id+name+payment))
            query += $"WHERE ";

        if (!string.IsNullOrEmpty(id))
        {
            query += $"WHERE id = \"{id}\" ";
            needAnd = true;
        }
        
        if (!string.IsNullOrEmpty(name))
        {
            query += (needAnd ? "AND " : "") + $"name = \"{name}\" ";
            needAnd = true;
        }
        
        if (!string.IsNullOrEmpty(payment))
            query += (needAnd ? "AND " : "") + $"payment = \"{payment}\" ";*/
        
        var mysqlCmd = new MySqlCommand(query, app.DbCon);
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

    private readonly ObservableCollection<Category> _arr = new ();
    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        //TODO Optimizations https://stackoverflow.com/questions/13815607/find-a-record-in-wpf-datagrid-by-typing
        PaymentSearch.Foreground = Brushes.Black;
        IdSearch.Foreground = Brushes.Black;
        
        var app = (App) Application.Current;
        if (string.IsNullOrWhiteSpace(IdSearch.Text + NameSearch.Text + PaymentSearch.Text))
        {
            CategoriesGrid.ItemsSource = app.ArrCategories;
            return;
        }
        
        _arr.Clear();
        var id = -1;
        if (!string.IsNullOrWhiteSpace(IdSearch.Text) && !int.TryParse(IdSearch.Text, out id))
        {
            IdSearch.Foreground = Brushes.Red;
            CategoriesGrid.ItemsSource = _arr;
            return;
        }
        var sId = id.ToString();

        var name = NameSearch.Text;

        var payment = -1;
        if (!string.IsNullOrWhiteSpace(PaymentSearch.Text) && !int.TryParse(PaymentSearch.Text, out payment))
        {
            PaymentSearch.Foreground = Brushes.Red;
            CategoriesGrid.ItemsSource = _arr;
            return;
        }

        var sPayment = payment.ToString();
        
        foreach (var c in app.ArrCategories)
        {
            if (id != -1 && !c.Id.ToString().Contains(sId))
                continue;

            if (!string.IsNullOrWhiteSpace(name) && !c.Name.Contains(name))
                continue;
            
            if (payment != -1 && !c.Payment.ToString().Contains(sPayment))
                continue;
            
            _arr.Add(c);
        }

        CategoriesGrid.ItemsSource = _arr;
    }

    private void Grid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        var grid = (DataGrid)sender;
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        //if (MessageBox.Show($"Would you like to delete {((Category)grid.SelectedItem).Name}", "Confirm Delete", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
        //  e.Handled = true;
        var c = (Category) grid.SelectedItem;
        c.Remove();
        e.Handled = true;
    }

    private void CategoriesGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;
        
        var c = new Category(-1, "Name", -1);
        c.SetUniqueId();
        c.SetUniquePayment();
        c.SetUniqueName();
        e.NewItem = c;
        
        var app = (App) Application.Current;
        app.FastQuery($"INSERT INTO categories (id, name, payment) VALUES ('{c.Id}','{c.Name}',{c.Payment})");
    }
}