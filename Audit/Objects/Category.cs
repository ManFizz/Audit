using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Audit.Objects;

public class Category : BaseObject
{
    public Category(int id, string name, int payment)
    {
        Id = id > -1 ? id : GetUniqueId();
        
        _name = name;
        _payment = payment;
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)] [Obsolete("Constuctor created for dataGrid", true)]
    public Category() {}

    #region Id

    private const int MaxId = 99999;
    public int Id { get; }

    private static int GetUniqueId()
    {
        var id = ArrCategories.Select(c => c.Id).Prepend(-1).Max() + 1;
        if (id > MaxId)
        {
            id = 1;
            while (ArrCategories.Any(c => c.Id == id))
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
    private string _name;
    public string Name
    {
        get => _name;
        set {
            try {
                CheckName(value);
                
                app.FastQuery($"UPDATE categories SET name = '{value}' WHERE id = {Id};");
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
            
        if (value.Any(t => NameCharacters.All(c => t != c)))
            throw new Exception($"Название содержит недопустимые символы - '{value.First(t => NameCharacters.All(c => t != c))}'");

        if (ArrCategories.Any(category => string.Compare(category.Name, value, StringComparison.Ordinal) == 0 && this != category))
            throw new Exception("Введенное название уже существует");
    }
    #endregion
    
    #region Payment
    private const int MinPayment = 1;
    private const int MaxPayment = 9999;
    private int _payment;
    public int Payment 
    { 
        get => _payment;
        set {
            try {
                CheckPayment(value);

                app.FastQuery($"UPDATE categories SET payment = {value} WHERE id = {Id};");
                _payment = value;
                NotifyPropertyChanged(nameof(Payment));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public void CheckPayment(int value)
    {
        if (value < MinPayment)
            throw new Exception($"Введенная оплата не должна быть меньше {MinPayment}");
        
        if (value > MaxPayment)
            throw new Exception($"Введенная оплата не должна быть больше {MaxPayment}");
            
        if (ArrCategories.Any(c => c.Payment == value && this != c))
            throw new Exception("Введенная оплата уже существует");
    }
    #endregion
    
    public bool Remove()
    {
        return Remove(this);
    }

    private static bool Remove(Category cat)
    {
        var arr = app.ArrWorkers.Where(w => w.CategoryId == cat.Id).ToList();
        if (arr.Count > 0)
        {
            var dialogResult = MessageBox.Show($"При удалении категории будет удалено {arr.Count} сотрудников. Продолжить?", "Удаление категории", MessageBoxButton.YesNo);
            if (dialogResult != MessageBoxResult.Yes)
            {
                MessageBox.Show("Операция отменена пользователем", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
        }

        foreach(var worker in arr)
            worker.Remove();
        
        app.ArrCategories.Remove(app.ArrCategories.First(c => c.Id == cat.Id));
        app.FastQuery($"DELETE FROM categories WHERE id = {cat.Id}");
        
        return true;
    }

    public void Insert()
    {
        app.FastQuery($"INSERT INTO categories (id, name, payment) VALUES ('{Id}','{Name}',{Payment})");
    }
    
}