using System;
using System.Collections.Generic;

namespace RVT_WinSchema_re_wpf.Services
{
    public interface IService { }

    public class ServiceLocator
    {
        private ServiceLocator() => _container = new Dictionary<Type, IService>();

        public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());
        private static ServiceLocator _instance;

        private Dictionary<Type, IService> _container;

        public void RegisterSingle<Type, TService>(TService implementation) where TService : class, IService =>
            _container.Add(typeof(Type), implementation);

        public TService Single<Type, TService>() where TService : class, IService =>
            _container.TryGetValue(key: typeof(TService), out IService implementation)
            ? implementation as TService
            : null;

        public TService Single<TService>() where TService : class, IService =>
            Single<TService, TService>();
    }
}
