using System;
using System.Globalization;
using UnityEngine;

namespace LOK1game.Utility
{
    public static class ConsoleArgumentConverter
    {
        public static bool TryConvert(string rawArg, Type targetType, out object converted, out string error)
        {
            converted = null;
            error = null;

            if (targetType == typeof(string))
            {
                converted = rawArg;
                return true;
            }

            if (targetType == typeof(Vector2))
                return TryParseVector(rawArg, 2, out converted, out error);

            if (targetType == typeof(Vector3))
                return TryParseVector(rawArg, 3, out converted, out error);

            if (targetType == typeof(Vector4))
                return TryParseVector(rawArg, 4, out converted, out error);

            if (targetType == typeof(Quaternion))
                return TryParseQuaternion(rawArg, out converted, out error);

            if (targetType.IsEnum)
            {
                try
                {
                    converted = Enum.Parse(targetType, rawArg, true);
                    return true;
                }
                catch (Exception ex)
                {
                    error = $"Failed to convert \"{rawArg}\" to {targetType.Name}: {ex.Message}";
                    return false;
                }
            }

            try
            {
                converted = Convert.ChangeType(rawArg, targetType, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex)
            {
                error = $"Failed to convert \"{rawArg}\" to {targetType.Name}: {ex.Message}";
                return false;
            }
        }

        private static bool TryParseVector(string raw, int dimension, out object converted, out string error)
        {
            converted = null;
            error = null;

            if (!TryParseFloatComponents(raw, dimension, out var components, out error))
                return false;

            switch (dimension)
            {
                case 2:
                    converted = new Vector2(components[0], components[1]);
                    break;
                case 3:
                    converted = new Vector3(components[0], components[1], components[2]);
                    break;
                case 4:
                    converted = new Vector4(components[0], components[1], components[2], components[3]);
                    break;
            }

            return true;
        }

        private static bool TryParseQuaternion(string raw, out object converted, out string error)
        {
            converted = null;
            error = null;

            if (!TryParseFloatComponents(raw, 4, out var components, out error))
                return false;

            converted = new Quaternion(components[0], components[1], components[2], components[3]);
            return true;
        }

        private static bool TryParseFloatComponents(string raw, int expectedCount, out float[] components, out string error)
        {
            components = null;
            error = null;

            if (string.IsNullOrWhiteSpace(raw))
            {
                error = $"Empty value cannot be converted to a {expectedCount}D vector.";
                return false;
            }

            var normalized = raw.Trim().Trim('(', ')').Replace(";", " ").Replace(",", " ");
            var parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != expectedCount)
            {
                error = $"Expected {expectedCount} components but got {parts.Length} ({raw}).";
                return false;
            }

            components = new float[expectedCount];

            for (int i = 0; i < expectedCount; i++)
            {
                if (!float.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out components[i]))
                {
                    error = $"Unable to parse \"{parts[i]}\" as a number.";
                    return false;
                }
            }

            return true;
        }
    }
}

