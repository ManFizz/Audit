namespace Audit.Objects;

public class User
{
    public enum TypeUser {
        Worker = 0,
        Hr,
        Timekeeper,
        Bookkeeper
    }
    
    private int _id;
    private string _login;
    private string _password;
    private int _idWroker;
    private TypeUser _type;

    public User(int id, string login, string password, int idWroker, TypeUser type)
    {
        _id = id;
        _login = login;
        _password = password;
        _idWroker = idWroker;
        _type = type;
    }

    public User(string login, string password, int idWroker, TypeUser type)
    {
        _id = -1;
        SetLogin(login);
        SetPassword(password);
        SetIdWorker(idWroker);
        SetTypeUser(type);
    }
    
    //Page constructor --- DON'T USE
    public User()
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
    
    public string Login
    {
        get => _login; 
        set => SetLogin(value);
    }
    private void SetLogin(string login)
    {
        _login = login;
    }
    
    public string Password
    {
        get => _password; 
        set => SetPassword(value);
    }
    private void SetPassword(string password)
    {
        _password = password;
    }
    
    public int IdWorker
    {
        get => _idWroker; 
        set => SetIdWorker(value);
    }
    private void SetIdWorker(int idWorker)
    {
        _idWroker = idWorker;
    }
    
    public TypeUser Type
    {
        get => _type; 
        set => SetTypeUser(value);
    }
    
    private void SetTypeUser(TypeUser type)
    {
        _type = type;
    }
}