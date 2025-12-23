using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="service"></param>
/// <param name="overwrite"> Si está activado, sobreescribrá un servicio si ya está registrado </param>
    public static void Register<T>(T service, bool overwrite = false) {
        var type = typeof(T);
        if (_services.ContainsKey(type)) {
            if (!overwrite) {
                Debug.LogWarning($"Service {type} already registered. Register Canceled");
                return;
            }
            Debug.LogWarning($"Service {type} already registered. Register Overwrited");
            _services[type] = service;
        } else {
            _services.Add(type, service);
        }
    }

    public static T Get<T>() {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service)) {
            return (T)service;
        }
        throw new Exception($"Service {type} not found.");
    }

    public static void Reset() => _services.Clear();
    //? Descomentar si se necesita un método que no crashee al no encontrar un servicio.
    // public static bool TryGet<T>(out T service) {
    //     var type = typeof(T);
    //     if (_services.TryGetValue(type, out var result)) {
    //         service = (T)result;
    //         return true;
    //     }

    //     service = default;
    //     return false;
    // }

/// <summary>
/// Limpia la caché de servicios. Usar si está dando errores a la hora de recargar escena
/// </summary>
}
