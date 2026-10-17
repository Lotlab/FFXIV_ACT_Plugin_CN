using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.MinIoC;

public class Container : Container.IScope, IDisposable, IServiceProvider
{
	public interface IScope : IDisposable, IServiceProvider
	{
	}

	public interface IRegisteredType
	{
		void AsSingleton();

		void PerScope();
	}

	private interface ILifetime : IScope, IDisposable, IServiceProvider
	{
		object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory);

		object GetServicePerScope(Type type, Func<ILifetime, object> factory);
	}

	private abstract class ObjectCache : IDisposable
	{
		private readonly ConcurrentDictionary<Type, object> _instanceCache = new ConcurrentDictionary<Type, object>();

		protected object GetCached(Type type, Func<ILifetime, object> factory, ILifetime lifetime)
		{
			return _instanceCache.GetOrAdd(type, (Type _) => factory(lifetime));
		}

		public void Dispose()
		{
			foreach (object value in _instanceCache.Values)
			{
				(value as IDisposable)?.Dispose();
			}
			_instanceCache.Clear();
		}
	}

	private class ContainerLifetime : ObjectCache, ILifetime, IScope, IDisposable, IServiceProvider
	{
		public Func<Type, Func<ILifetime, object>> GetFactory { get; private set; }

		public ContainerLifetime(Func<Type, Func<ILifetime, object>> getFactory)
		{
			GetFactory = getFactory;
		}

		public object GetService(Type type)
		{
			return GetFactory(type)(this);
		}

		public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
		{
			return GetCached(type, factory, this);
		}

		public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
		{
			return GetServiceAsSingleton(type, factory);
		}
	}

	private class ScopeLifetime : ObjectCache, ILifetime, IScope, IDisposable, IServiceProvider
	{
		private readonly ContainerLifetime _parentLifetime;

		public ScopeLifetime(ContainerLifetime parentContainer)
		{
			_parentLifetime = parentContainer;
		}

		public object GetService(Type type)
		{
			return _parentLifetime.GetFactory(type)(this);
		}

		public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
		{
			return _parentLifetime.GetServiceAsSingleton(type, factory);
		}

		public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
		{
			return GetCached(type, factory, this);
		}
	}

	private class RegisteredType : IRegisteredType
	{
		private readonly Type _itemType;

		private readonly Action<Func<ILifetime, object>> _registerFactory;

		private readonly Func<ILifetime, object> _factory;

		public RegisteredType(Type itemType, Action<Func<ILifetime, object>> registerFactory, Func<ILifetime, object> factory)
		{
			_itemType = itemType;
			_registerFactory = registerFactory;
			_factory = factory;
			registerFactory(_factory);
		}

		public void AsSingleton()
		{
			_registerFactory((ILifetime lifetime) => lifetime.GetServiceAsSingleton(_itemType, _factory));
		}

		public void PerScope()
		{
			_registerFactory((ILifetime lifetime) => lifetime.GetServicePerScope(_itemType, _factory));
		}
	}

	private readonly Dictionary<Type, Func<ILifetime, object>> _registeredTypes = new Dictionary<Type, Func<ILifetime, object>>();

	private readonly ContainerLifetime _lifetime;

	public Container()
	{
		_lifetime = new ContainerLifetime((Type t) => _registeredTypes[t]);
	}

	public IRegisteredType Register(Type @interface, Func<object> factory)
	{
		return RegisterType(@interface, (ILifetime _) => factory());
	}

	public IRegisteredType Register(Type @interface, Type implementation)
	{
		return RegisterType(@interface, FactoryFromType(implementation));
	}

	private IRegisteredType RegisterType(Type itemType, Func<ILifetime, object> factory)
	{
		return new RegisteredType(itemType, (Func<ILifetime, object> f) =>
		{
			_registeredTypes[itemType] = f;
		}, factory);
	}

	public object GetService(Type type)
	{
		if (!_registeredTypes.TryGetValue(type, out var value))
		{
			return null;
		}
		return value(_lifetime);
	}

	public IScope CreateScope()
	{
		return new ScopeLifetime(_lifetime);
	}

	public void Dispose()
	{
		_lifetime.Dispose();
		_registeredTypes.Clear();
		GC.SuppressFinalize(this);
	}

	private static Func<ILifetime, object> FactoryFromType(Type itemType)
	{
		ConstructorInfo[] constructors = itemType.GetConstructors();
		if (constructors.Length == 0)
		{
			constructors = itemType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
		}
		ConstructorInfo constructorInfo = constructors.First();
		ParameterExpression arg = Expression.Parameter(typeof(ILifetime));
		return (Func<ILifetime, object>)Expression.Lambda(Expression.New(constructorInfo, constructorInfo.GetParameters().Select((ParameterInfo param) =>
		{
			Func<ILifetime, object> func = (ILifetime lifetime) => lifetime.GetService(param.ParameterType);
			return Expression.Convert(Expression.Call(Expression.Constant(func.Target), func.Method, arg), param.ParameterType);
		})), arg).Compile();
	}
}
