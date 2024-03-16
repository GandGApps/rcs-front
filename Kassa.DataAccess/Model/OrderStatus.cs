namespace Kassa.DataAccess.Model;

public enum OrderStatus
{
    Unconfirmed,
    New,
    InCooking,
    Ready,
    OnTheWay,
    Completed,
    Canceled
}