using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using BaseLibs.Tasks;
using BaseLibs.Types;
using System.Collections.Concurrent;
using StaticProxyInterfaces;

namespace AspNetCore.SignalR.Client.HubProxy
{
    public sealed class HubProxy : IHubProxy
    {
        static readonly ConcurrentDictionary<Type, ConstructorInvoker> proxyCtors = new ConcurrentDictionary<Type, ConstructorInvoker>();

        readonly Dictionary<string, IDisposable> clientMethodDisposers = new Dictionary<string, IDisposable>();

        public HubProxy(HubConnection connection, Type hubType, Type? clientInterface = null, object? clientObject = null)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (hubType == null)
                throw new ArgumentNullException(nameof(hubType));
            if (!hubType.IsInterface)
                throw new ArgumentException($"{nameof(hubType)} must be an interface");
            HubType = hubType;
            ClientType = clientInterface;
            if ((ClientType != null || clientObject != null) && (ClientType == null || clientObject == null))
                throw new ArgumentException("If specifying concrete client interface then type and object must both not be null");
            if (ClientType != null && !ClientType.IsInstanceOfType(clientObject!))
                throw new ArgumentException($"{nameof(clientObject)} must be of type (or derived of) {nameof(clientObject)}");
            ClientObject = clientObject;

            Proxy = ProxyGeneratorHelpers.InstantiateProxy(hubType, InterceptorHandler);

            if (ClientType != null)
            {
                var methods = ClientType.GetMethods();
                foreach (var method in methods)
                {
                    if (clientMethodDisposers.ContainsKey(method.Name))
                        throw new ArgumentException("Don't currently support duplicate method names for clientInterface");
                    var methodCaller = method.DelegateForMethod();
                    Task callback(object[] args, object _)
                    {
                        var result = methodCaller(ClientObject, args);
                        if (result is Task resultTask)
                            return resultTask;
                        return Task.CompletedTask;
                    }
                    var disposer = Connection.On(method.Name,
                        method.GetParameters().Select(p => p.ParameterType).ToArray(), 
                        callback, null);
                    clientMethodDisposers.Add(method.Name, disposer);
                }
            }
        }

        public HubConnection Connection { get; }
        public Type HubType { get; }
        public Type? ClientType { get; }
        public object? ClientObject { get; }
        public object Proxy { get; }

        object? InterceptorHandler(object instance, MethodInfo method, object[] arguments, Type[] genericArguments)
        {
            if (method.Name == nameof(IDisposable.Dispose))
                Dispose();
            else
            {
                var retType = method.ReturnType;
                Type retUnderlyingType = retType;
                var retIsTask = typeof(Task).IsAssignableFrom(retType);
                var retIsGenericTask = retIsTask && retType.IsGenericType && retType.GetGenericTypeDefinition() == typeof(Task<>);
                if (retIsGenericTask)
                    retUnderlyingType = retType.GetGenericArguments()[0];
                try
                {
                    if (retType == typeof(void) || (retIsTask && !retIsGenericTask)) // no results
                    {
                        var task = Connection.InvokeCoreAsync(method.Name, arguments);
                        if (retType == typeof(void))
                        {
                            task.Wait();
                            return null;
                        }
                        else
                            return task;
                    }
                    else // have results
                    {
                        var task = Connection.InvokeCoreAsync(method.Name, retUnderlyingType, arguments);
                        if (!retIsGenericTask)
                            return task.Result;
                        else // Task<T>
                            return task.CastResultAs(retUnderlyingType);
                    }
                }
                catch (Exception ex)
                {
                    if (retType != typeof(void))
                    {
                        if (ex is AggregateException agEx && agEx.InnerExceptions.Count == 1)
                            ex = agEx.InnerException!;
                        if (retIsTask && !retIsGenericTask)
                            return Task.FromException(ex);
                        else if (retIsGenericTask)
                            return ex.AsGenericTaskException(retUnderlyingType);
                        else
                            throw;
                    }
                }
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var method in clientMethodDisposers)
                method.Value?.Dispose();
            clientMethodDisposers.Clear();
        }
    }
}
