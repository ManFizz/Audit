using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Audit.Objects;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace Audit.MainWindow.Forms;

public partial class InputCategories : Page
{
    public InputCategories()
    {
        InitializeComponent();
    }

    private void AddBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text;
        if (!int.TryParse(PaymentTextBox.Text, out var payment)) 
        {
            MessageBox.Show("Field payment is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }

        Category c;
        try
        {
            c = new Category(name, payment);
        } catch (Exception exception){
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }

        var app = (App) Application.Current;
        var cmd = new MySqlCommand($"INSERT INTO categories (name, payment) VALUES ('{name}',{payment})", app.DbCon);
        app.DbCon.Open();
        if (cmd.ExecuteNonQuery() == 0)
        {
            MessageBox.Show("The changes were not applied. Please contact customer support.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            app.DbCon.Close();
            return;
        }

        var id = cmd.LastInsertedId;
        c = new Category((int)id, name, payment);
        app.DbCon.Close();
        app.ArrCategories.Add(c);
        MessageBox.Show("Category added successfully",
            "Successfully", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        
        NameTextBox.Text = "";
        PaymentTextBox.Text = "";
    }

    private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(IdTextBox.Text, out var id)) 
        {
            MessageBox.Show("Field id is not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        
        
        var app = (App) Application.Current;
        if(app.ArrCategories.All( c => c.Id != id))
        {
            MessageBox.Show("Id not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        
        var cmd = new MySqlCommand($"DELETE FROM categories WHERE id = {id}", app.DbCon);
        app.DbCon.Open();
        if (cmd.ExecuteNonQuery() == 0)
        {
            MessageBox.Show("The changes were not applied. Please contact customer support.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            app.DbCon.Close();
            return;
        }

        app.DbCon.Close();
        app.ArrCategories.Remove(app.ArrCategories.First(c => c.Id == id));
        MessageBox.Show("Category removed successfully",
            "Successfully", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        IdTextBox.Text = "";
    }
}