﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AppCore.Attributes;
using AppCore.Models;

namespace AppCore.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enu)
    {
        if (enu == null)
            return null;
        var attr = GetDisplayAttribute(enu);
        return attr != null ? attr.Name : enu.ToString();
    }

    public static string GetDescription(this Enum enu)
    {
        var attr = GetDisplayAttribute(enu);
        return attr != null ? attr.Description : enu.ToString();
    }

    public static EnumValue? GetValue(this Enum? enu)
    {
        if (enu == null)
            return null;
        var attr = GetDisplayAttribute(enu);
        var colorAttr = GetColorAttribute(enu);
        if (attr == null)
            return null;
        
        return new EnumValue
        {
            Value = Convert.ToInt32(enu),
            Name = enu.ToString(),
            DisplayName = attr.Name,
            Color = colorAttr
        };
    }
 
    public static EnumValue? GetValue<T>(int value) where T : Enum
    {
        foreach (var itemType in Enum.GetValues(typeof(T)))
        {
            if ((int)itemType == value)
            {
                var type = itemType.GetType();
                var field = type.GetField(itemType.ToString() ?? string.Empty);
                var displayAttr = field.GetCustomAttribute<DisplayAttribute>();
                var colorAttr = GetColorAttribute((Enum)itemType);
                return new EnumValue
                {
                    Value = (int)itemType,
                    Name = Enum.GetName(typeof(T), itemType),
                    DisplayName = displayAttr?.Name,
                    Color = colorAttr
                };
            }
        }

        return null;
    }

    public static List<EnumValue> GetValues<T>()
    {
        var values = new List<EnumValue>();
        foreach (var itemType in Enum.GetValues(typeof(T)))
        {
            var type = itemType.GetType();
            var field = type.GetField(itemType.ToString() ?? string.Empty);
            values.Add(new EnumValue
            {
                Value = (int)itemType,
                Name = Enum.GetName(typeof(T), itemType),
                DisplayName = field != null ? field.GetCustomAttribute<DisplayAttribute>()?.Name : null
            });
        }

        return values;
    }

    public static List<int> GetIntValues<T>()
    {
        var results = new List<int>();
        foreach (var itemType in Enum.GetValues(typeof(T))) results.Add((int)itemType);
        return results;
    }

    private static DisplayAttribute GetDisplayAttribute(object value)
    {
        if (value == null)
            return null;
        var type = value.GetType();
        if (!type.IsEnum) throw new ArgumentException(string.Format("Type {0} is not an enum", type));

        // Get the enum field.
        var field = type.GetField(value.ToString() ?? string.Empty);
        return field == null ? null : field.GetCustomAttribute<DisplayAttribute>();
    }

    public static object GetEnumValue<TEnum>(this string displayName) where TEnum : Enum
    {
        try
        {
            displayName = displayName.ToLower().RemoveUnicode();
            var enumType = typeof(TEnum);
            var displayAttributeType = typeof(DisplayAttribute);

            var displayNameMapping = new Dictionary<string, TEnum>();
            Enum.GetNames(enumType).ToList().ForEach(name =>
            {
                var member = enumType.GetMember(name).First();
                var displayAttrib =
                    (DisplayAttribute) member.GetCustomAttributes(displayAttributeType, false)
                        .First();
                if (displayAttrib.Name == null) return;
                displayNameMapping.Add(
                    displayAttrib.Name.ToLower().RemoveUnicode() ?? string.Empty,
                    (TEnum) Enum.Parse(enumType, name));
            });
            return displayNameMapping[displayName];
        }
        catch
        {
            return null;
        }
    }
    
    private static string? GetColorAttribute(Enum value)
    {
        var type = value.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException($"Type {type} is not an enum");
        }

        // Get the enum field.
        var field = type.GetField(value.ToString() ?? string.Empty);
        var colorAttribute = field?.GetCustomAttribute<ColorAttribute>();
        return colorAttribute?.Color ?? null; // Return the color or default value
    }

}