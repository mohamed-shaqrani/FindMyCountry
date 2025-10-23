namespace Main.Helpers;
public static class GetDescriptionExtension
{
    public static string GetDescription(this object obj)
    {
        return DescriptionAttribute.GetDescription(obj);
    }
}

