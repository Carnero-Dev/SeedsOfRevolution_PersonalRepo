using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class DeepCloner
{
    public static T Clone<T>(T source)
    {
        return (T)CloneObject(source);
    }

    private static object CloneObject(object source)
    {
        if (source == null) return null;

        Type type = source.GetType();

        if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
            return source;

        if (type.IsArray)
        {
            Array array = (Array)source;
            Array copied = Array.CreateInstance(type.GetElementType(), array.Length);
            for (int i = 0; i < array.Length; i++)
                copied.SetValue(CloneObject(array.GetValue(i)), i);
            return copied;
        }

        if (typeof(IList).IsAssignableFrom(type))
        {
            IList sourceList = (IList)source;
            IList clonedList = (IList)Activator.CreateInstance(type);
            foreach (var item in sourceList)
                clonedList.Add(CloneObject(item));
            return clonedList;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            IDictionary sourceDict = (IDictionary)source;
            IDictionary clonedDict = (IDictionary)Activator.CreateInstance(type);
            foreach (DictionaryEntry entry in sourceDict)
                clonedDict.Add(CloneObject(entry.Key), CloneObject(entry.Value));
            return clonedDict;
        }

        object clone = Activator.CreateInstance(type);
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            object fieldValue = field.GetValue(source);
            field.SetValue(clone, CloneObject(fieldValue));
        }

        return clone;
    }
}
