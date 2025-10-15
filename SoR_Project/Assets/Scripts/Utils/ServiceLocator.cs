using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());
    private static ServiceLocator _instance;
    private readonly Dictionary<Type, object> _services;

    private ServiceLocator(){
        _services = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Registra un servicio
    /// </summary>
    /// <typeparam name="T">Tipo del servicio</typeparam>
    /// <param name="service">Referencia al servicio</param>
    public void RegisterService<T>(T service){
        var type = typeof(T);
        if(_services.ContainsKey(type)){
            Debug.Log($"Service {type} already registered");
            return;
        }

        _services.Add(type, service);
    }

    /// <summary>
    /// Devuelve, si existe, el servicio especificado por el tipo.
    /// </summary>
    /// <typeparam name="T">Tipo del servicio a obtener</typeparam>
    /// <returns>El servicio si está registrado</returns>
    /// <exception cref="Exception">El servicio no se encuentra registrado</exception>
    public T GetService<T>(){
        var type = typeof(T);
        if(!_services.TryGetValue(type, out var service)){
            throw new Exception($"Service {type} not found");
        }

        return (T) service;
    }
}
