using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;


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
    
    public Category(int id, string name, int payment)
    {
        _id = id;
        _name = name;
        _payment = payment;
    }
    
    public Category(string name, int payment)
    {
        SetName(name);
        SetPayment(payment);
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
            
        const string template = "qwertyuiopasdfghjklzxcvvbnm\tйцукенгшщзххъфывапрролджэячсмитььбюё.,!?" +
                                "QWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ";
        if (!value.All(t => template.Any(c => t == c)))
            throw new Exception("The input string contains unresolved characters");

        if (ArrCategories.Any(c => (string.Compare(c.Name, value, StringComparison.Ordinal) == 0 && this != c)))
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

    private void SetPayment(int value)
    {
        
        if (value is < 1 or > 10000)
            throw new Exception("Invalid input string size");
            
        if (ArrCategories.Any(c => (c.Payment == value && this != c)))
            throw new Exception("The input string contains a non-unique Payment");

        ((App) Application.Current).FastQuery($"UPDATE categories SET payment = {value} WHERE id = {Id};");
        /*
         * поиск
         * сохранить изменения
         * undo / redo
         * цвет чередует строки
         * дисейблить кнопки
         * 
         */
        _payment = value;
        NotifyPropertyChanged(nameof(Payment));
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


}