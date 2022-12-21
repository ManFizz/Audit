using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.Pages;

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

    private readonly ObservableCollection<Company> _arr = new ();
    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        IdSearch.Foreground = Brushes.Black;
        
        var app = (App) Application.Current;
        if (string.IsNullOrWhiteSpace(IdSearch.Text + NameSearch.Text + AddressSearch.Text))
        {
            CompanyGrid.ItemsSource = app.ArrCompany;
            return;
        }
        
        _arr.Clear();
        var id = -1;
        if (!string.IsNullOrWhiteSpace(IdSearch.Text) && !int.TryParse(IdSearch.Text, out id))
        {
            IdSearch.Foreground = Brushes.Red;
            CompanyGrid.ItemsSource = _arr;
            return;
        }
        var sId = id.ToString();

        var name = NameSearch.Text;
        var address = AddressSearch.Text;
        
        
        foreach (var c in app.ArrCompany)
        {
            if (id != -1 && !c.Id.ToString().Contains(sId))
                continue;

            if (!string.IsNullOrWhiteSpace(name) && !c.Name.Contains(name))
                continue;
            
            if (!string.IsNullOrWhiteSpace(address) && !c.Address.Contains(address))
                continue;
            
            _arr.Add(c);
        }

        CompanyGrid.ItemsSource = _arr;
    }

    private void CompanyGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        var grid = (DataGrid)sender;
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        //if (MessageBox.Show($"Would you like to delete {((Company)grid.SelectedItem).Name}", "Confirm Delete", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
        //  e.Handled = true;
        var c = (Company) grid.SelectedItem;
        c.Remove();
        e.Handled = true;
    }

    private void CompanyGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;
        
        var c = new Company(-1, "Name", "Adress");
        c.SetUniqueId();
        c.SetUniqueAddress();
        c.SetUniqueName();
        e.NewItem = c;
        
        var app = (App) Application.Current;
        app.FastQuery($"INSERT INTO company (id, name, address) VALUES ('{c.Id}','{c.Name}','{c.Address}')");
    }
}