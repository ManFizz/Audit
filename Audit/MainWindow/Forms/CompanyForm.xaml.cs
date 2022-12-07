using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.MainWindow.Forms;

public partial class CompanyForm : Page
{
    public CompanyForm()
    {
        InitializeComponent();
    }

    private void AddBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text;
        var address = AddressTextBox.Text;

        Company c;
        try
        {
            c = new Company(name, address);
        } catch (Exception exception){
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }

        var app = (App) Application.Current;
        var cmd = new MySqlCommand($"INSERT INTO `company` (`name`, `address`) VALUES (\"{name}\",\"{address}\")", app.DbCon);
        app.DbCon.Open();
        if (cmd.ExecuteNonQuery() == 0)
        {
            MessageBox.Show("The changes were not applied. Please contact customer support.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            app.DbCon.Close();
            return;
        }

        var id = cmd.LastInsertedId;
        c = new Company((int)id, name, address);
        app.DbCon.Close();
        app.ArrCompany.Add(c);
        MessageBox.Show("Company added successfully",
            "Successfully", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        
        NameTextBox.Text = "";
        AddressTextBox.Text = "";
    }

    private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(IdTextBox.Text, out var id)) 
        {
            MessageBox.Show("Field id is not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        
        
        var app = (App) Application.Current;
        if(app.ArrCompany.All( c => c.Id != id))
        {
            MessageBox.Show("Id not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        
        var cmd = new MySqlCommand($"DELETE FROM company WHERE id = {id}", app.DbCon);
        app.DbCon.Open();
        if (cmd.ExecuteNonQuery() == 0)
        {
            MessageBox.Show("The changes were not applied. Please contact customer support.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            app.DbCon.Close();
            return;
        }

        app.DbCon.Close();
        app.ArrCompany.Remove(app.ArrCompany.First(c => c.Id == id));
        MessageBox.Show("Company removed successfully",
            "Successfully", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        IdTextBox.Text = "";
    }
}