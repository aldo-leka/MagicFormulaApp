using System.Text.Json;

namespace Updater
{
    public static class CompanyDocument
    {
        public static bool FindProperty(this JsonElement element, string key, out JsonElement result)
        {
            var i = 0;
            var properties = key.Split(".");
            bool propertyFound = false;
            result = element;

            if (properties.Length > 0)
            {
                do
                {
                    propertyFound = result.TryGetProperty(properties[i++], out result);
                }
                while (i < properties.Length && propertyFound);
            }

            return propertyFound;
        }

        public static bool TryGetInt(this JsonElement element, string key, out int result)
        {
            if (element.FindProperty(key, out var property))
            {
                return property.TryGetInt32(out result);
            }

            result = default;
            return false;
        }

        public static bool TryGetString(this JsonElement element, string key, out string result)
        {
            if (element.FindProperty(key, out var property))
            {
                result = element.GetString();
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryGetLast(this JsonElement element, string propertyName, string key, out JsonElement result)
        {
            if (element.FindProperty(key, out result))
            {
                result = result.EnumerateArray().Last().EnumerateObject().FirstOrDefault(c => c.Name == propertyName).Value;
                return true;
            }

            result = default;
            return false;
        }

        public static JsonElement GetValue(this JsonElement element, string propertyName)
        {
            return element.EnumerateObject().FirstOrDefault(c => c.Name == propertyName).Value;
        }

        public static bool TryGetLastDecimal(this JsonElement element, string propertyName, string key, out decimal? result)
        {
            if (element.TryGetLast(propertyName, key, out var property))
            {
                result = property.GetDecimal();
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryGetLastDate(this JsonElement element, string propertyName, string key, out DateTime? result)
        {
            if (element.TryGetLast(propertyName, key, out var property))
            {
                result = property.GetDateTime();
                return true;
            }

            result = default;
            return false;
        }
    }
}
