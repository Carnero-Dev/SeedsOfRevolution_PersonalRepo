using System;
using System.Reflection;

public static class DataFactory
{
    public static T Create<T>() where T : class, IData, new() {
        T data = new T();

        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            if (field.FieldType.IsClass && field.FieldType.GetConstructor(Type.EmptyTypes) != null) {
                field.SetValue(data, Activator.CreateInstance(field.FieldType));
            }
        }
        return data;
    }
}
