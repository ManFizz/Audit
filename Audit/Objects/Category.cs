using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MySql.Data.MySqlClient;


namespace Audit.Objects;

public class Category : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private static ObservableCollection<Category> ArrCategories {
        get {
            return ((App) Application.Current).ArrCategories;
        }
    }

    private int _id;
    private string _name;
    private int _payment;
    
    //Force constructor --- DON'T USE
    public Category(int id, string name, int payment)
    {
        _id = id;
        _name = name;
        _payment = payment;
    }
    
    //Main constructor
    public Category(string name, int payment)
    {
        _name = "";
        SetName(name);
        SetPayment(payment);
        SetUniqueId();
    }

    //Page constructor --- DON'T USE
    public Category()
    {
        SetUniqueId();
        _name = "Name";
        SetUniquePayment();
    }

    public int Id
    {
        get => _id;
        set
        {
            try {
                SetId(value);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    private void SetId(int value)
    {
        if (value is < 0 or > 99999)
            throw new Exception("Invalid input string size");
        
        if (ArrCategories.Any(c => (c.Id == value && this != c)))
            throw new Exception("The input string contains a non-unique Id");
        
        ((App) Application.Current).FastQuery($"UPDATE categories SET id = {value} WHERE id = {Id};");
        _id = value;
        NotifyPropertyChanged(nameof(Id));
    }
    
    public string Name
    {
        get => _name;
        set
        {
            try {
                SetName(value);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    private void SetName(string value)
    {
        if (value.Length is < 1 or > 100)
            throw new Exception("Invalid input string size");
            
        const string template = "qwertyuiopasdfghjklzxcvvbnm\tйцукенгшщзххъфывапрролджэячсмитььбюё.,!?" +
                                "QWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ";
        if (!value.All(t => template.Any(c => t == c)))
            throw new Exception("The input string contains unresolved characters");

        if (ArrCategories.Any(c => (string.Compare(c.Name, value, StringComparison.Ordinal) == 0 && this != c)))
            throw new Exception("The input string contains a non-unique Name");
            
        ((App) Application.Current).FastQuery($"UPDATE categories SET name = '{value}' WHERE id = {Id};");
        _name = value;
        NotifyPropertyChanged(nameof(Name));
    }
    
    public int Payment 
    { 
        get => _payment;
        set
        {
            try {
                SetPayment(value);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    private void SetPayment(int value)
    {
        
        if (value is < 1 or > 10000)
            throw new Exception("Invalid string int size");
            
        if (ArrCategories.Any(c => (c.Payment == value && this != c)))
            throw new Exception("The input string contains a non-unique Payment");

        ((App) Application.Current).FastQuery($"UPDATE categories SET payment = {value} WHERE id = {Id};");
        _payment = value;
        NotifyPropertyChanged(nameof(Payment));
    }

    public void Remove()
    {
        Remove(this);
    }

    public static void Remove(Category cat)
    {
        var app = ((App) Application.Current);
        var cmd = new MySqlCommand($"DELETE FROM categories WHERE id = {cat.Id}", app.DbCon);
        app.DbCon.Open();
        cmd.ExecuteNonQuery();
        app.DbCon.Close();
        app.ArrCategories.Remove(app.ArrCategories.First(c => c.Id == cat.Id));
        //MessageBox.Show("Category removed successfully", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
    }

    public void SetUniqueId()
    {
        _id = ArrCategories.Select(c => c._id).Prepend(-1).Max() + 1;
    }
    public void SetUniquePayment()
    {
        _payment = ArrCategories.Select(c => c._payment).Prepend(-1).Max() + 1;
    }
    public void SetUniqueName()
    {
        var n = "I";
        while (ArrCategories.Any(c => string.CompareOrdinal(c._name, n) == 0))
            n += "I"; //Can br broken on limits
        _name = n;
    }
    
}