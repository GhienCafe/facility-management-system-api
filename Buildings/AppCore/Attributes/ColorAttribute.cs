namespace AppCore.Attributes;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class ColorAttribute : Attribute
{
    public string Color { get; }

    public ColorAttribute(string color)
    {
        Color = color;
    }
}

