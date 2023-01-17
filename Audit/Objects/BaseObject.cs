using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Audit.Objects;

public class BaseObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }    
    protected static readonly App app = (App) Application.Current;
    protected static  ObservableCollection<HoursRecord> ArrHoursRecords => app.ArrHoursRecords;
    protected static ObservableCollection<Company> ArrCompany => app.ArrCompany;
    protected static ObservableCollection<Worker> ArrWorkers => app.ArrWorkers;
    protected static ObservableCollection<User> ArrUsers => app.ArrUsers;
    protected static ObservableCollection<Category> ArrCategories => app.ArrCategories;
    
    //kludge - for dataGrid 
    //https://stackoverflow.com/questions/4484256/how-to-use-a-factory-for-datagrid-canuseraddrows-true
    //[EditorBrowsable(EditorBrowsableState.Never)] [Obsolete("Constuctor created for dataGrid", true)]
    public BaseObject()
    { 
        //throw new Exception("Don't use this constructor");
    }
}