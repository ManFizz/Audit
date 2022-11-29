namespace Audit.Objects;

public class Category
{
    public int Id;
    public string Name;
    public int Payment;

    public Category(int id, string name, int payment)
    {
        Id = id;
        Name = name;
        Payment = payment;
    }
}