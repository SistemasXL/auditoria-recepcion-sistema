using System.Text.RegularExpressions;

namespace AuditoriaRecepcion.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidCuit(this string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                return false;

            var regex = new Regex(@"^\d{2}-\d{8}-\d{1}$");
            return regex.IsMatch(cuit);
        }

        public static string ToSafeFileName(this string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeFileName = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return safeFileName;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string FormatCuit(this string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                return cuit;

            var digits = new string(cuit.Where(char.IsDigit).ToArray());
            
            if (digits.Length != 11)
                return cuit;

            return $"{digits.Substring(0, 2)}-{digits.Substring(2, 8)}-{digits.Substring(10, 1)}";
        }
    }
}