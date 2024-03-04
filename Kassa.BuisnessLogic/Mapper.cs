using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;
using Riok.Mapperly.Abstractions;

namespace Kassa.BuisnessLogic;

[Mapper]
public static partial class Mapper
{
    public static partial Order MapDtoToOrder(OrderDto order);
    public static partial OrderDto MapOrderToDto(Order order);

    public static partial OrderedProduct MapDtoToOrderedProduct(OrderedProductDto product);
    public static partial OrderedProductDto MapOrderedProductToDto(OrderedProduct product);

    public static partial OrderedAdditive MapDtoToOrderedAdditive(OrderedAdditiveDto additive);
    public static partial OrderedAdditiveDto MapOrderedAdditiveToDto(OrderedAdditive additive);

    public static partial Client MapDtoToClient(ClientDto client);
    public static partial Client MapClientToDto(ClientDto client);

    public static partial Courier MapDtoToCourier(CourierDto courier);
    public static partial CourierDto MapCourierToDto(CourierDto courier);
}
