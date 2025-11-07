using System.ComponentModel;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Helpers;

public static class EnumExtensions
{
    public static string GetStringValue(this Enum enumValue)
    {
        var attributes = (DescriptionAttribute[])enumValue
            .GetType()
            .GetField(enumValue.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
}