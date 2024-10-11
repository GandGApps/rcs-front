using Kassa.Shared;

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

    public required string FirstName
    {
        get; set;
    }

    public required string LastName
    {
        get; set;
    }

    public required string MiddleName
    {
        get; set;
    }

    public required string Address
    {
        get; set;
    }

    public required string House
    {
        get; set;
    }

    public required string Building
    {
        get; set;
    }

    public required string Entrance
    {
        get; set;
    }

    public required string Floor
    {
        get; set;
    }

    public required string Apartment
    {
        get; set;
    }

    public required string Intercom
    {
        get; set;
    }

    public required string Phone
    {
        get; set;
    }

    public required string Card
    {
        get; set;
    }

    public string? AddressNote
    {
        get; set;
    }

    public required string Miscellaneous
    {
        get; set;
    }

}