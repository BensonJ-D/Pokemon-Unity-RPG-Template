using System.ComponentModel;
using System.Reflection;

namespace System.Utlilities
{
    public static class EnumExtensionFunctions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            var name = Enum.GetName(type, value);
            
            if (name == null) return value.ToString();
            
            FieldInfo field = type.GetField(name);
            
            if (field == null) return value.ToString();

            if (!(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)) 
                return value.ToString();
            
            return attr.Description;
        }

        public static bool IsNotDefault(this Enum value) => !value.Equals(default);
    }
}