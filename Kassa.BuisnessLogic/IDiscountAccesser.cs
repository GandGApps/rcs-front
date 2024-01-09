namespace Kassa.BuisnessLogic;

public interface IDiscountAccesser
{
    public bool HasTotalDiscount
    {
        get;
    }

    /// <summary>
    /// If has not total discount, return <see cref="double.NaN"/>
    /// </summary>
    public double TotalDiscount
    {
        get;
    }


    /// <summary>
    /// If has not discount, return <see cref="double.NaN"/>
    /// </summary>
    /// <returns></returns>
    public double AccessDicsount(int id);

    internal class MockDiscountAccesser(Dictionary<int, double> discountAccessers, double discount = double.NaN) : IDiscountAccesser
    {
        public bool HasTotalDiscount => !double.IsNaN(TotalDiscount);
        public double TotalDiscount => discount;

        public double AccessDicsount(int id) => discountAccessers.TryGetValue(id, out var discont) ? discont : double.NaN;
    }

    public static IDiscountAccesser CreateMock(Dictionary<int, double> discountAccessers, double discount = double.NaN) => new MockDiscountAccesser(discountAccessers, discount);
}
