using Microsoft.AspNetCore.Http;

namespace Library.Application.Validators.ValidatorsHelpers
{
    public static class BookValidHelper
    {
        public static bool IsValidISBN(string isbn)
        {
            isbn = isbn.Replace("-", "").Replace(" ", "");

            if (isbn.Length == 10)
            {
                return IsValidISBN10(isbn);
            }
            else if (isbn.Length == 13)
            {
                return IsValidISBN13(isbn);
            }
            return false;
        }

        private static bool IsValidISBN10(string isbn)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^\d{9}[\dX]$"))
                return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (i + 1) * (isbn[i] - '0');
            }

            char checkDigit = isbn[9];
            sum += (checkDigit == 'X') ? 10 * 10 : 10 * (checkDigit - '0');

            return sum % 11 == 0;
        }

        private static bool IsValidISBN13(string isbn)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^\d{13}$"))
                return false;

            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);
            }

            int checkDigit = (10 - (sum % 10)) % 10;

            return checkDigit == (isbn[12] - '0');
        }
        public static bool BeAValidFileName(string? fileName)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            return validExtensions.Any(ext => fileName!.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
        public static bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

    }
}
