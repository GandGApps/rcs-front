namespace Kassa.DataAccess;

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