using System.Text.Encodings.Web;

namespace InventoryApp.Application.Utility
{
    public static class AntiXssUtility
    {
        private static readonly HtmlEncoder _htmlEncoder = HtmlEncoder.Default;
        public static string Encode(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? input : _htmlEncoder.Encode(input);
        }

        public static T EncodeDto<T>(T dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) && property.CanWrite)
                {
                    var value = property.GetValue(dto) as string;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        property.SetValue(dto, _htmlEncoder.Encode(value));
                    }
                }
            }
            return dto;
        }
    }
}
