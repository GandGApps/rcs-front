namespace Kassa.DataAccess;

public class Client : IModel
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }
}