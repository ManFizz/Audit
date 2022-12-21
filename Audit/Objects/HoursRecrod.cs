using System;

namespace Audit.Objects;

public class HoursRecrod
{
    private int _id;
    private int _companyId;
    private int _workerId;
    private string _date;
    private int _hours;

    public HoursRecrod(int id, int companyId, int workerId, string date, int hours)
    {
        _id = id;
        _companyId = companyId;
        _workerId = workerId;
        _date = date;
        _hours = hours;
    }
    
    public HoursRecrod(int companyId, int workerId, string date, int hours)
    {
        _id = -1;
        SetCompanyId(companyId);
        SetWorkerId(workerId);
        SetDate(date);
        SetHours(hours);
    }
    
    
    //Page constructor --- DON'T USE
    public HoursRecrod()
    {
        
    }

    public int Id
    {
        get => _id;
        set => SetId(value);
    }

    private void SetId(int id)
    {
        _id = id;
    }

    public int CompanyId
    {
        get => _companyId;
        set => SetCompanyId(value);
    }
    
    private void SetCompanyId(int companyId)
    {
        _companyId = companyId;
    }

    public int WorkerId
    {
        get => _workerId;
        set => SetWorkerId(value);
    }
    
    private void SetWorkerId(int workerId)
    {
        _workerId = workerId;
    }

    public string Date
    {
        get => _date;
        set => SetDate(value);
    }
    private void SetDate(string date)
    {
        _date = date;
    }

    public int Hours
    {
        get => _hours;
        set => SetHours(value);
    }
    private void SetHours(int hours)
    {
        _hours = hours;
    }
}