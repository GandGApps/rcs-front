namespace Kassa.DataAccess.Model;

public enum OrderStatus
{
    /// <summary>
    /// Неподтвержден
    /// </summary>
    Unconfirmed, 
    /// <summary>
    /// Новый
    /// </summary>
    New,
    /// <summary>
    /// Готовится 
    /// </summary>
    InCooking,
    /// <summary>
    /// Готов
    /// </summary>
    Ready,
    /// <summary>
    /// В пути
    /// </summary>
    OnTheWay,
    /// <summary>
    /// Закрытый
    /// </summary>
    Completed,
    /// <summary>
    /// Отменен
    /// </summary>
    Canceled
}