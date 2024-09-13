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
    Canceled,
    /// <summary>
    /// Доставлен
    /// ПРИМЕЧАНИЕ, БЫЛО РЕШЕНО УДАЛИТЬ ЭТОТ СТАТУС, ТАК КАК ТЕХНИЧЕСКИ 
    /// ЭТО ТОЖЕ САМОЕ ЧТО И ЗАКРЫТЫЙ. А ОПРЕДЕЛИТЬ БЫЛА ЛИ ЭТО ДОСТАВКА ИЛИ 
    /// САМОВЫВОЗ МОЖНО ПО CВОЙСТВУ <see cref="Order.IsDelivery"/> и <see cref="Order.IsForHere"/>.
    /// </summary>
    [Obsolete("Use Completed")]
    Delivered,
}