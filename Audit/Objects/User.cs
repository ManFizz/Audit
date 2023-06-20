using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Audit.Objects;

public class User : BaseObject
{

    public User(int id, string login, string password, int? idWroker, TypeUser type)
    {
        Id = id > -1 ? id : GetUniqueId();
        _login = login;
        _password = password;
        _workerId = idWroker ?? -1;
        _type = type;
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)] [Obsolete("Constuctor created for dataGrid", true)]
    public User() {}
    
    #region Id
    private const int MaxId = 99999;

    public int Id { get; }
    
    private static int GetUniqueId()
    {
        var id = ArrUsers.Select(c => c.Id).Prepend(-1).Max() + 1;
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
    
    #region Login
    private const int MinLengthLogin = 3;
    private const int MaxLengthLogin= 32;
    private const string LoginCharacters = " qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюёQWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ01234567890_";
    private string _login;
    
    public string Login
    {
        get => _login;
        set
        {
            try
            {
                CheckLogin(value);
                
                app.FastQuery($"UPDATE users SET login = '{value}' WHERE id = {Id};");
                _login = value;
                NotifyPropertyChanged(nameof(Login));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public static void CheckLogin(string value)
    {
        if (value.Length < MinLengthLogin)
            throw new Exception($"Логин должен содержать минимум {MinLengthLogin} символов");
        
        if (value.Length > MaxLengthLogin)
            throw new Exception($"Логин должен содержать максимум {MaxLengthLogin} символов");
            
        if (value.Any(t => LoginCharacters.All(c => t != c)))
            throw new Exception($"Логин содержит недопустимые символы - '{value.First(t => LoginCharacters.All(c => t != c))}'");
        
        //TODO add check unique
    }
    #endregion
    
    #region Password
    private const int MinLengthPassword = 6;
    private const int MaxLengthPassword = 32;
    private const string PasswordCharacters = "qwertyuiopasdfghjklzxcvvbnmйцукенгшщзххъфывапрролджэячсмитььбюёQWERTYUIOPASDFGHJKLZXCCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬЁ01234567890_";
    private string _password;
    
    public string Password
    {
        get => _password; 
        set
        {
            try
            {
                CheckPassword(value);
                
                app.FastQuery($"UPDATE users SET password = '{value}' WHERE id = {Id};");
                _password = value;
                NotifyPropertyChanged(nameof(Password));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    public static void CheckPassword(string value)
    {
        if (value.Length < MinLengthPassword)
            throw new Exception($"Пароль должен содержать минимум {MinLengthPassword} символов");
        
        if (value.Length > MaxLengthPassword)
            throw new Exception($"Пароль должен содержать максимум {MaxLengthPassword} символов");
            
        if (value.Any(t => PasswordCharacters.All(c => t != c)))
            throw new Exception($"Пароль содержит недопустимые символы - '{value.First(t => PasswordCharacters.All(c => t != c))}'");
    }
    #endregion
    
    #region WorkerId
    private int _workerId;
    public string WorkerName
    {
        get
        {
            var worker = app.ArrWorkers.FirstOrDefault(c => c.Id == _workerId);
            return worker == null ? "" : worker.Name;
        }
    }
    
    public int WorkerId
    {
        get => _workerId; 
        set
        {
            try
            {
                CheckWorkerId(value);
                var sIdW = WorkerId == -1 ? "null" : WorkerId.ToString();
                app.FastQuery($"UPDATE users SET worker_id = {sIdW} WHERE id = {Id};");
                _workerId = value;
                NotifyPropertyChanged(nameof(WorkerId));
                NotifyPropertyChanged(nameof(WorkerName));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public void CheckWorkerId(int value)
    {
        if (Type is TypeUser.worker && value == -1)
            throw new Exception("Нельзя удалить привязку сотрудниика при статусе \"Сотрудник\"");
        
        if (value != -1 && ArrWorkers.All(c => c.Id != value))
            throw new Exception("Заданного сотрудника не существует");
    }
    #endregion
    
    #region Type
    private TypeUser _type;
    
    public TypeUser Type
    {
        get => _type; 
        set
        {
            try
            {
                CheckTypeUser(value);
                    
                app.FastQuery($"UPDATE users SET type = '{value}' WHERE id = {Id};");
                _type = value;
                NotifyPropertyChanged(nameof(Type));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    public void CheckTypeUser(TypeUser value)
    {
        if(value is TypeUser.worker && WorkerId == -1)
            throw new Exception("Нельзя установить статус \"Cотрудник\" без привязки самого сотрудника");
    }
    #endregion
    
    public bool Remove()
    {
        return Remove(this);
    }

    private static bool Remove(User user)
    {
        if (app.ActiveUser.Id == user.Id)
        {
            MessageBox.Show("Нельзя удалить текущего пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return false;
        }
        
        app.FastQuery($"DELETE FROM users WHERE id = {user.Id};");
        app.ArrUsers.Remove(app.ArrUsers.First(c => c.Id == user.Id));
        return true;
    }

    public void Insert()
    {
        var sIdW = WorkerId == -1 ? "null" : WorkerId.ToString();
        app.FastQuery($"INSERT INTO users (id, login, password, worker_id, type) VALUES ({Id},'{Login}','{Password}', {sIdW},'{Type}');");
    }
}