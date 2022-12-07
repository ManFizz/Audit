using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;


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
        SetName(name);
        SetAddress(address);
        _id = -1;
    }
    public int Id
    {
        get => _id;
    }

    private void SetName(string value)
    {
        if (value.Length is < 1 or > 100)
            throw new Exception("Invalid input string size");
            
        const string template = "qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюё.,!?" +
                                "QWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ";
        if (!value.All(t => template.Any(c => t == c)))
            throw new Exception("The input string contains unresolved characters");

        if (ArrCompany.Any(c => (string.Compare(c.Name, value, StringComparison.Ordinal) == 0 && this != c)))
            throw new Exception("The input string contains a non-unique Name");
            
        _name = value;
        NotifyPropertyChanged(nameof(Name));
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

    private void SetAddress(string value)
    {
        if (value.Length is < 1 or > 100)
            throw new Exception("Invalid input string size");
            
        const string template = "qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюё.,!?" +
                                "QWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ";
        if (!value.All(t => template.Any(c => t == c)))
            throw new Exception("The input string contains unresolved characters");

        if (ArrCompany.Any(c => (string.Compare(c.Adress, value, StringComparison.Ordinal) == 0 && this != c)))
            throw new Exception("The input string contains a non-unique Adress");
            
        _address = value;
        NotifyPropertyChanged(nameof(Adress));
    }
    
    public string Adress 
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
}