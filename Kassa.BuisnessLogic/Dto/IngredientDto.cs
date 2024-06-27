namespace Kassa.BuisnessLogic.Dto;

public class IngredientDto
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = string.Empty;

    public double Count
    {
        get; set;
    }

    public string? Measure
    {
        get; set;
    }

    public bool IsSellRemainder
    {
        get; set;
    }
}