using Ganss.Xss;

namespace InventoryApp.Application.Utility
{
    public static class AntiXssUtility
    {
        public static string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(input);
        }

        public static T SanitizeDto<T>(T dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var sanitizer = new HtmlSanitizer();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) && property.CanWrite)
                {
                    var value = property.GetValue(dto) as string;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        property.SetValue(dto, sanitizer.Sanitize(value));
                    }
                }
            }
            return dto;
        }
    }
}
