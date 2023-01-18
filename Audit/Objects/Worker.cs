using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Audit.Objects;

public class Worker : BaseObject
{
    public Worker(int id, string name, string passport, string birthday, string phoneNumber, int categoryId)
    {
        Id = id > -1 ? id : GetUniqueId();
        _name = name;
        _passport = passport;
        _birthday = birthday;
        _phoneNumber = phoneNumber;
        _categoryId = categoryId;
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)] [Obsolete("Constuctor created for dataGrid", true)]
    public Worker() {}
    
    #region Id
    private const int MaxId = 99999;

    public int Id { get; }
    
    private static int GetUniqueId()
    {
        var id = ArrWorkers.Select(c => c.Id).Prepend(-1).Max() + 1;
        if (id > MaxId)
        {
            id = 1;
            while (ArrWorkers.Any(c => c.Id == id))
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
    private const string NameCharacters = " qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюёQWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ";
    private static readonly Regex RegexName = new("^[A-ZА-Я][a-zа-я]+ [A-ZА-Я][a-zа-я]+( [A-ZА-Я][a-zа-я]+)?$");
    private string _name;

    public string Name
    {
        get => _name;
        set 
        {
            try {
                CheckName(value);
                
                app.FastQuery($"UPDATE workers SET name = '{value}' WHERE id = {Id};");
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    public static void CheckName(string value)
    {
        if (value.Length < MinLengthName)
            throw new Exception($"ФИО должно содержать минимум {MinLengthName} символов");
        
        if (value.Length > MaxLengthName)
            throw new Exception($"ФИО должно содержать максимум {MaxLengthName} символов");
            
        if (value.Any(t => NameCharacters.All(c => t != c)))
            throw new Exception($"ФИО содержит недопустимые символы - '{value.First(t => NameCharacters.All(c => t != c))}'");
        
        if(!RegexName.Match(value).Success)
            throw new Exception("ФИО не соответствует формату");
    }
    #endregion

    #region Passport
    private static readonly Regex RegexPassport = new("^[0-9]{4} [0-9]{6}$");
    private string _passport;
    
    public string Passport
    {
        get => _passport;
        set
        {
            try
            {
                CheckPassport(value);
                
                app.FastQuery($"UPDATE workers SET passport = '{value}' WHERE id = {Id};");
                _passport = value;
                NotifyPropertyChanged(nameof(Passport));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public static void CheckPassport(string value)
    {
        if(!RegexPassport.Match(value).Success)
            throw new Exception("Пасспорт не соответствует формату");
    }
    #endregion
    
    #region Birthday
    private const string DateTemplate = "dd.MM.yyyy";
    private const int DateMaxYear = 2050;
    private const int DateMinYear = 1950;
    private string _birthday;
    
    public string Birthday
    {
        get => _birthday;
        set 
        {
            try
            {
                CheckBirthday(value);
                
                app.FastQuery($"UPDATE workers SET birthday = '{value}' WHERE id = {Id};");
                _birthday = value;
                NotifyPropertyChanged(nameof(Birthday));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    public static void CheckBirthday(string value)
    {
        if(!DateTime.TryParseExact(value, DateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            throw new Exception("Дата рождения не соответствует формату");
        
        if(dateTime.Year > DateMaxYear)
            throw new Exception($"Год не может быть больше {DateMaxYear}");
        
        if(dateTime.Year < DateMinYear)
            throw new Exception($"Год не может быть меньше {DateMinYear}");
    }
    #endregion
    
    #region PhoneNumber
    private static readonly Regex RegexPhoneNumber = new("^[1-9]{1}[0-9]{10}$");
    private string _phoneNumber;
    
    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            try
            {
                CheckPhoneNumber(value);
            
                app.FastQuery($"UPDATE workers SET phone_number = '{value}' WHERE id = {Id};");
                _phoneNumber = value;
                NotifyPropertyChanged(nameof(PhoneNumber));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    public static void CheckPhoneNumber(string value)
    {
        if(!RegexPhoneNumber.Match(value).Success)
            throw new Exception("Номер телефона не соответствует формату");
    }
    #endregion
    
    #region CategoryId
    //must be already exists
    private int _categoryId;
    
    public string CategoryName
    {
        get
        {
            var cat = app.ArrCategories.FirstOrDefault(c => c.Id == _categoryId);
            return cat == null ? "" : cat.Name;
        }
    }
    
    public int CategoryId
    {
        get => _categoryId;
        set
        {
            try
            {
                CheckCategoryId(value);
                
                app.FastQuery($"UPDATE workers SET category_id = '{value}' WHERE id = {Id};");
                _categoryId = value;
                NotifyPropertyChanged(nameof(CategoryId));
                NotifyPropertyChanged(nameof(CategoryName));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public static void CheckCategoryId(int value)
    {
        if(ArrCategories.All(category => category.Id != value))
            throw new Exception("Заданной категории не существует");
    }
    #endregion

    public bool Remove()
    {
       return Remove(this);
    }

    private static bool Remove(Worker worker)
    {
        var arr1 = app.ArrUsers.Where(w => w.WorkerId == worker.Id).ToList();
        var arr2 = app.ArrHoursRecords.Where(w => w.WorkerId == worker.Id).ToList();
        if (arr1.Count > 0 || arr2.Count > 0)
        {
            var dialogResult = MessageBox.Show($"При удалении сотрудника будет удалено {arr1.Count} пользователей и {arr2.Count} записей об отработанных часах. " +
                                               "Продолжить?", "Удаление сотрудника", MessageBoxButton.YesNo);
            if (dialogResult != MessageBoxResult.Yes)
            {
                MessageBox.Show("Операция отменена пользователем", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
        }
        
        foreach(var user in arr1)
            user.Remove();
        
        foreach(var hoursRecord in arr2)
            hoursRecord.Remove();
        
        app.FastQuery($"DELETE FROM workers WHERE id = {worker.Id}");
        app.ArrWorkers.Remove(app.ArrWorkers.First(c => c.Id == worker.Id));
        return true;
    }

    public void Insert()
    {
        app.FastQuery($"INSERT INTO workers (id, name, passport, birthday, phone_number, category_id) VALUES ('{Id}','{Name}','{Passport}','{Birthday}','{PhoneNumber}','{CategoryId}')");
    }

}