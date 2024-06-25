namespace Kassa.DataAccess.Model;
/// <summary>
/// Пока что, все что известно о курьере
/// </summary>
public class Courier : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string FirstName
    {
        get; set;
    }

    public string LastName
    {
        get; set;
    }

    public string MiddleName
    {
        get; set;
    }

    public string Phone
    {
        get; set;
    }
}