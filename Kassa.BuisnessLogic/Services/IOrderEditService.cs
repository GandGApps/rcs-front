using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Services;

[Obsolete("Remove as soon as posible")]
public interface IOrderEditService : IInitializableService
{

    public bool IsAdditiveAdded(Guid additiveId);
    
    public OrderDto GetOrder();
}