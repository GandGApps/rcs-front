namespace Kassa.DataAccess.Model;
/// <summary>
/// Пока что, все что известно о курьере
/// </summary>
public class Courier : IModel
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