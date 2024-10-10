using Kassa.Shared;

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

    public required string Phone
    {
        get; set;
    }
}