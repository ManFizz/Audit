namespace Audit;

public class User
{
    public enum TypeUser {
        Worker = 0,
        Hr,
        Timekeeper,
        Bookkeeper
    }
    private int _idDb;
    private string _login;
    private string _password;
    private int _idWroker;
    private TypeUser _type;

    public User(int idDb, string login, string password, int idWroker, TypeUser type)
    {
        _idDb = idDb;
        _login = login;
        _password = password;
        _idWroker = idWroker;
        _type = type;
    }
}