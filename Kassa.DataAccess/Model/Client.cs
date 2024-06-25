namespace Kassa.DataAccess.Model;

/// <summary>
/// Пока что, все что известно о клиенте
/// </summary>
public class Client : IGuidId
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

    public string Address
    {
        get; set;
    }

    public string House
    {
        get; set;
    }

    public string Building
    {
        get; set;
    }

    public string Entrance
    {
        get; set;
    }

    public string Floor
    {
        get; set;
    }

    public string Apartment
    {
        get; set;
    }

    public string Intercom
    {
        get; set;
    }

    public string Phone
    {
        get; set;
    }

    public string Card
    {
        get; set;
    }

    public string AddressNote
    {
        get; set;
    }

    public string Miscellaneous
    {
        get; set;
    }

}