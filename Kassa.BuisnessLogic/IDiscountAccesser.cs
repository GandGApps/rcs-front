namespace Kassa.BuisnessLogic;

public interface IDiscountAccesser
{
    public bool HasTotalDiscount
    {
        get;
    }

    /// <summary>
    /// If has not total discount, return 1
    /// </summary>
    public double TotalDiscount
    {
        get;
    }


    /// <summary>
    /// If has not discount, return 1
    /// </summary>
    /// <returns></returns>
    public double AccessDicsount(Guid id);

    internal class MockDiscountAccesser(Dictionary<Guid, double> discountAccessers, double discount = 1) : IDiscountAccesser
    {
        public bool HasTotalDiscount => !double.IsNaN(TotalDiscount);
        public double TotalDiscount => discount;

        public double AccessDicsount(Guid id) => discountAccessers.TryGetValue(id, out var discont) ? discont : 1;
    }

    public static IDiscountAccesser CreateMock(Dictionary<Guid, double> discountAccessers, double discount = 1) => new MockDiscountAccesser(discountAccessers, discount);
}
