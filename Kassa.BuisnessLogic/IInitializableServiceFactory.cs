﻿
namespace Kassa.BuisnessLogic;

public interface IInitializableServiceFactory<T> where T : class, IInitializableService
{
    public T GetNotInitializedService();
    public ValueTask<T> GetService();
}