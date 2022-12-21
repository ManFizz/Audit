namespace Audit.Objects;

public class Worker
{
    private int _id;
    private string _name;
    private string _passport;
    private string _birthday;
    private string _phoneNumber;
    private int _categoryId;

    public Worker(int id, string name, string passport, string birthday, string phoneNumber, int categoryId)
    {
        _id = id;
        _name = name;
        _passport = passport;
        _birthday = birthday;
        _phoneNumber = phoneNumber;
        _categoryId = categoryId;
    }
    
    public Worker(string name, string passport, string birthday, string phoneNumber, int categoryId)
    {
        _id = -1;
        SetName(name);
        SetPassport(passport);
        SetBirthday(birthday);
        SetPhoneNumber(phoneNumber);
        SetCategoryId(categoryId);
    }
    
    //Page constructor --- DON'T USE
    public Worker()
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

    public string Name
    {
        get => _name;
        set => SetName(value);
    }
    
    private void SetName(string name)
    {
        _name = name;
    }
    
    public string Passport
    {
        get => _passport;
        set => SetPassport(value);
    }
    
    private void SetPassport(string passport)
    {
        _passport = passport;
    }
    
    public string Birthday
    {
        get => _birthday;
        set => SetBirthday(value);
    }
    
    private void SetBirthday(string birthday)
    {
        _birthday = birthday;
    }
    
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetPhoneNumber(value);
    }
    
    private void SetPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
    }
    
    
    public int CategoryId
    {
        get => _categoryId;
        set => SetCategoryId(value);
    }
    
    private void SetCategoryId(int categoryId)
    {
        _categoryId = categoryId;
    }
}