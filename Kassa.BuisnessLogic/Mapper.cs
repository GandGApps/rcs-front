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
public static partial class OrderMapper
{
    public static partial Order MapDtoToOrder(OrderDto order);
}
