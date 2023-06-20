using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Audit.Objects;

public class HoursRecord : BaseObject
{
    public HoursRecord(int id, int companyId, int workerId, string date, int hours)
    {
        Id = id > -1 ? id : GetUniqueId();
        _companyId = companyId;
        _workerId = workerId;
        _date = date;
        _hours = hours;
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)] [Obsolete("Constuctor created for dataGrid", true)]
    public HoursRecord() {}

    
    #region Id
    private const int MaxId = 99999;

    public int Id { get; }
    
    private static int GetUniqueId()
    {
        var id = ArrHoursRecords.Select(c => c.Id).Prepend(-1).Max() + 1;
        if (id > MaxId)
        {
            id = 1;
            while (ArrHoursRecords.Any(c => c.Id == id))
                id++;
            
            if(id > MaxId)
                throw new Exception("Таблица категорий заполнена, удалите не нужные записи");
        }

        return id;
    }
    #endregion

    #region CompanyId
    //must be exists
    private int _companyId;
    public string CompanyName
    {
        get
        {
            var company = app.ArrCompany.FirstOrDefault(c => c.Id == CompanyId);
            return company == null ? "" : company.Name;
        }
    }

    public int CompanyId
    {
        get => _companyId;
        set
        {
            try
            {
                CheckCompanyId(value);
                
                app.FastQuery($"UPDATE hours_records SET company_id = '{value}' WHERE id = {Id};");
                _companyId = value;
                NotifyPropertyChanged(nameof(CompanyId));
                NotifyPropertyChanged(nameof(CompanyName));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public static void CheckCompanyId(int value)
    {
        if (ArrCompany.All(c => c.Id != value))
            throw new Exception("Заданной компании не существует");
    }
    #endregion
    
    #region WorkerId
    //must be exists
    private int _workerId;
    public string WorkerName
    {
        get
        {
            var worker = app.ArrWorkers.FirstOrDefault(c => c.Id == WorkerId);
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
                
                app.FastQuery($"UPDATE hours_records SET worker_id = '{value}' WHERE id = {Id};");
                _workerId = value;
                NotifyPropertyChanged(nameof(WorkerId));
                NotifyPropertyChanged(nameof(WorkerName));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public static void CheckWorkerId(int value)
    {
        if (ArrWorkers.All(c => c.Id != value))
            throw new Exception("Заданного сотрудника не существует");
    }
    #endregion
    
    #region Date
    private const string DateTemplate = "dd.MM.yyyy";
    private const int DateMaxYear = 2050;
    private const int DateMinYear = 1950;
    private string _date;
    public string Date
    {
        get => _date;
        set
        {
            try
            {
                CheckDate(value);
                
                app.FastQuery($"UPDATE hours_records SET date = '{value}' WHERE id = {Id};");
                _date = value;
                NotifyPropertyChanged(nameof(Date));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }
    
    public static void CheckDate(string value)
    {
        if(!DateTime.TryParseExact(value, DateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            throw new Exception("Дата не соответствует формату");
        
        if(dateTime.Year > DateMaxYear)
            throw new Exception($"Год не может быть больше {DateMaxYear}");
        
        if(dateTime.Year < DateMinYear)
            throw new Exception($"Год не может быть меньше {DateMinYear}");
    }
    #endregion

    #region Hours

    private const int HoursMin = 1;
    private const int HoursMax = 24;
    private int _hours;
    public int Hours
    {
        get => _hours;
        set
        {
            try
            {
                CheckHours(value);
                
                app.FastQuery($"UPDATE hours_records SET hours = '{value}' WHERE id = {Id};");
                _hours = value;
                NotifyPropertyChanged(nameof(Hours));
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }
    }

    public static void CheckHours(int value)
    {
        if(value <  HoursMin)
            throw new Exception($"Количество часов не может быть меньше {HoursMin}");
        
        if(value >  HoursMax)
            throw new Exception($"Количество часов не может быть больше {HoursMax}");
    }

    #endregion
    
    public bool Remove()
    {
        return Remove(this);
    }

    public static bool Remove(HoursRecord hoursRecord)
    {
        app.FastQuery($"DELETE FROM hours_records WHERE id = {hoursRecord.Id};");
        app.ArrHoursRecords.Remove(app.ArrHoursRecords.First(c => c.Id == hoursRecord.Id));
        return true;
    }
    public void Insert()
    {
        app.FastQuery($"INSERT INTO hours_records (id, company_id, worker_id, date, hours) VALUES ('{Id}','{CompanyId}','{WorkerId}', '{Date}','{Hours}');");
    }
}