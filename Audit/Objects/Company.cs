using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MySql.Data.MySqlClient;


namespace Audit.Objects;

public class Company : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }    
    
    private static ObservableCollection<Company> ArrCompany {
        get {
            return ((App) Application.Current).ArrCompany;
        }
    }

    private int _id;
    private string _name;
    private string _address;
    
    public Company(int id, string name, string address)
    {
        _id = id;
        _name = name;
        _address = address;
    }
    
    public Company(string name, string address)
    {
        _name = "";
        _address = "";
        SetName(name);
        SetAddress(address);
        _id = -1;
    }
    
    //Page constructor --- DON'T USE
    public Company()
    {
        SetUniqueId();
        SetUniqueName();
        SetUniqueAddress();
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
        
        if (ArrCompany.Any(c => (c.Id == value && this != c)))
            throw new Exception("The input string contains a non-unique Id");
        
        ((App) Application.Current).FastQuery($"UPDATE company SET id = {value} WHERE id = {Id};");
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
            

        const string template = "qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюё.,!?" +
                                "QWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ1234567890" + " ";
        if (!value.All(t => template.Any(c => t == c)))
            throw new Exception("The input string contains unresolved characters");

        if (ArrCompany.Any(c => (string.Compare(c.Name, value, StringComparison.Ordinal) == 0 && this != c)))
            throw new Exception("The input string contains a non-unique Name");
            
        ((App) Application.Current).FastQuery($"UPDATE company SET name = '{value}' WHERE id = {Id};");
        _name = value;
        NotifyPropertyChanged(nameof(Name));
    }

    public string Address 
    { 
        get => _address;
        set
        {
            try {
                SetAddress(value);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    private void SetAddress(string value)
    {
        if (value.Length is < 1 or > 100)
            throw new Exception("Invalid input string size");
            
        const string template = "qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюё.,!?" +
                                "QWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ1234567890" + " ";
        if (!value.All(t => template.Any(c => t == c)))
            throw new Exception("The input string contains unresolved characters");

        if (ArrCompany.Any(c => (string.Compare(c.Address, value, StringComparison.Ordinal) == 0 && this != c)))
            throw new Exception("The input string contains a non-unique Adress");
            
        ((App) Application.Current).FastQuery($"UPDATE company SET address = '{value}' WHERE id = {Id};");
        _address = value;
        NotifyPropertyChanged(nameof(Address));
    }

    public void Remove()
    {
        Remove(this);
    }
    
    public static void Remove(Company comp)
    {
        var app = ((App) Application.Current);
        var cmd = new MySqlCommand($"DELETE FROM company WHERE id = {comp.Id}", app.DbCon);
        app.DbCon.Open();
        cmd.ExecuteNonQuery();
        app.DbCon.Close();
        
        app.ArrCompany.Remove(app.ArrCompany.First(c => c.Id == comp.Id));
        //MessageBox.Show("Category removed successfully", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
    }
    
    public void SetUniqueId()
    {
        _id = ArrCompany.Select(c => c._id).Prepend(-1).Max() + 1;
    }
    
    public void SetUniqueAddress()
    {
        var n = "I";
        while (ArrCompany.Any(c => string.CompareOrdinal(c._address, n) == 0))
            n += "I"; //Can br broken on limits
        _address = n;
    }
    
    public void SetUniqueName()
    {
        var n = "I";
        while (ArrCompany.Any(c => string.CompareOrdinal(c._name, n) == 0))
            n += "I"; //Can br broken on limits
        _name = n;
    }
}