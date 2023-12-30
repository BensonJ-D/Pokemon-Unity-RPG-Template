using System;
using System.ComponentModel;
using UnityEngine;

namespace GameSystem.Utilities
{
    public static class EnumExtensionFunctions
    {
        public static string GetDescription(this Enum value) {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name == null) return value.ToString();

            var field = type.GetField(name);

            if (field == null) return value.ToString();

            if (!(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr))
                return value.ToString();

            return attr.Description;
        }

        public static bool IsNotDefault(this Enum value) {
            return !value.Equals(default);
        }
    }

    public static class VectorExtensionFunctions
    {
        public static Vector2Int ToVector2Int(this Vector3Int vector) {
            return new Vector2Int(vector.x, vector.y);
        }
        
        public static Vector3Int ToVector3Int(this Vector2Int vector) {
            return new Vector3Int(vector.x, vector.y, 0);
        }
        
        public static Vector2Int ToVector2Int(this Vector3 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }
    }
}