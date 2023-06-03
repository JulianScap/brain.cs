using System.Reflection;

namespace Brain.Utils;

public static class MergeHelper
{
    public static T Merge<T>(this T? main,
        T? secondary)
        where T : class, new()
    {
        var newInstance = new T();

        if (main == null)
        {
            return secondary ?? newInstance;
        }

        if (secondary == null)
        {
            return main;
        }

        PropertyInfo[] properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object? defaultValue = GetDefaultValue(property.PropertyType);
            object? mainValue = property.GetValue(main);
            object? secondaryValue = property.GetValue(secondary);

            if (mainValue == defaultValue)
            {
                property.SetValue(newInstance, secondaryValue);
            }
            else if (secondaryValue == defaultValue)
            {
                property.SetValue(newInstance, mainValue);
            }
        }

        return newInstance;
    }

    private static object? GetDefaultValue(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }
}
