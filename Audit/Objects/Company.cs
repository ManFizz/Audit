using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;


namespace Audit.Objects;

public class Company : BaseObject
{
    public Company(int id, string name, string address)
    {
        Id = id > -1 ? id : GetUniqueId();
        _name = name;
        _address = address;
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)] [Obsolete("Constuctor created for dataGrid", true)]
    public Company() {}
    
    #region Id
    private const int MaxId = 99999;
    public int Id { get; }

    private static int GetUniqueId()
    {
        var id = ArrCompany.Select(c => c.Id).Prepend(-1).Max() + 1;
        if (id > MaxId)
        {
            id = 1;
            while (ArrCompany.Any(c => c.Id == id))
                id++;
            
            if(id > MaxId)
                throw new Exception("Таблица категорий заполнена, удалите не нужные записи");
        }

        return id;
    }
    #endregion
    
    #region Name
    private const int MinLengthName = 6;
    private const int MaxLengthName = 64;
    private const string NameCharacters = " qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюёQWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ1234567890";
    private string _name;
    
    public string Name
    {
        get => _name;
        set
        {
            try {
                CheckName(value);
            
                app.FastQuery($"UPDATE company SET name = '{value}' WHERE id = {Id};");
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    public void CheckName(string value)
    {
        if (value.Length < MinLengthName)
            throw new Exception($"Название должно содержать минимум {MinLengthName} символов");
        
        if (value.Length > MaxLengthName)
            throw new Exception($"Название должно содержать максимум {MaxLengthName} символов");
        
        if (!value.All(t => NameCharacters.Any(c => t == c)))
            throw new Exception($"Название содержит недопустимые символы - '{value.First(t => NameCharacters.All(c => t != c))}'");

        if (ArrCompany.Any(c => string.Compare(c.Name, value, StringComparison.Ordinal) == 0 && this != c))
            throw new Exception("Введенное название уже существует");
    }
    #endregion

    #region Address
    private const int MinLengthAddress = 6;
    private const int MaxLengthAddress  = 128;
    private const string AddressCharacters = " qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюёQWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ.,!?1234567890";
    private string _address;
    
    public string Address 
    { 
        get => _address;
        set
        {
            try {
                CheckAddress(value);
            
                app.FastQuery($"UPDATE company SET address = '{value}' WHERE id = {Id};");
                _address = value;
                NotifyPropertyChanged(nameof(Address));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public void CheckAddress(string value)
    {
        if (value.Length < MinLengthAddress)
            throw new Exception($"Адрес должен содержать минимум {MinLengthAddress} символов");
        
        if (value.Length > MaxLengthAddress)
            throw new Exception($"Адрес должендолжен содержать максимум {MaxLengthAddress} символов");
            
        if (!value.All(t => AddressCharacters.Any(c => t == c)))
            throw new Exception($"Адрес содержит недопустимые символы - '{value.First(t => AddressCharacters.All(c => t != c))}'");

    }
    #endregion

    public bool Remove()
    {
        return Remove(this);
    }

    private static bool Remove(Company comp)
    {
        var arr = app.ArrHoursRecords.Where(hr => hr.CompanyId == comp.Id).ToList();
        var dialogResult = MessageBox.Show($"При удалении компании будет удалено {arr.Count} записей об отработанных часах. Продолжить?", "Удаление компании", MessageBoxButton.YesNo);
        if (dialogResult != MessageBoxResult.Yes)
        {
            MessageBox.Show("Операция отменена пользователем", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return false;
        }

        foreach(var hoursRecord in arr)
            hoursRecord.Remove();
        
        app.FastQuery($"DELETE FROM company WHERE id = {comp.Id};");
        app.ArrCompany.Remove(app.ArrCompany.First(c => c.Id == comp.Id));
        return true;
    }

    public void Insert()
    {
        app.FastQuery($"INSERT INTO company (id, name, address) VALUES ('{Id}','{Name}','{Address}');");
    }
}