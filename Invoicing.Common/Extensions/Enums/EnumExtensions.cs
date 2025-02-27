using System.ComponentModel;

namespace Invoicing.Common.Extensions.Enums
{
    public static class EnumExtensions
    {
        public static T? GetEnumTypeByDescription<T>(string description) where T : Enum
		{
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                string enumDescription = GetEnumDescription(enumValue);

                if (enumDescription.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }

            return default;
        }

        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
