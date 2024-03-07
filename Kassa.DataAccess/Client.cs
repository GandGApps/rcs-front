namespace Kassa.DataAccess;

/// <summary>
/// Пока что, все что известно о клиенте
/// </summary>
public class Client : IModel
{
    public Guid Id
    {
        get; set;
    }

    public string FullName
    {
        get; set;
    }

    public string Address
    {
        get; set;
    }

    public string Phone
    {
        get; set;
    }
}