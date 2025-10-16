using System;
using System.Globalization;
using System.Text;

namespace BusinessLayer.Helpers
{
    public static class SearchHelper
    {
        /// <summary>
        /// Normalizes a string for search by:
        /// 1. Converting to lowercase
        /// 2. Removing diacritics (accents)
        /// 3. Trimming whitespace
        /// </summary>
        public static string NormalizeForSearch(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Convert to lowercase
            var normalized = input.ToLowerInvariant();

            // Remove diacritics (accents)
            normalized = RemoveDiacritics(normalized);

            // Trim whitespace
            return normalized.Trim();
        }

        /// <summary>
        /// Removes diacritics (accents) from a string
        /// Example: "Đại lý" -> "Dai ly"
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Normalize to FormD (decomposed form) to separate base characters from diacritics
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                // Skip non-spacing marks (diacritics)
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Normalize back to FormC (composed form)
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Checks if a text contains a search term (case-insensitive, diacritics-insensitive)
        /// </summary>
        public static bool Contains(string text, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            var normalizedText = NormalizeForSearch(text);
            var normalizedSearch = NormalizeForSearch(searchTerm);

            return normalizedText.Contains(normalizedSearch);
        }

        /// <summary>
        /// Checks if a text starts with a search term (case-insensitive, diacritics-insensitive)
        /// Useful for searching by first letter
        /// </summary>
        public static bool StartsWithLetter(string text, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            var normalizedText = NormalizeForSearch(text);
            var normalizedSearch = NormalizeForSearch(searchTerm);

            return normalizedText.StartsWith(normalizedSearch);
        }

        /// <summary>
        /// Checks if a text starts with a search term (case-insensitive, diacritics-insensitive)
        /// </summary>
        public static bool StartsWith(string text, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            var normalizedText = NormalizeForSearch(text);
            var normalizedSearch = NormalizeForSearch(searchTerm);

            return normalizedText.StartsWith(normalizedSearch);
        }

        /// <summary>
        /// Checks if a text equals a search term (case-insensitive, diacritics-insensitive)
        /// </summary>
        public static bool Equals(string text, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(searchTerm))
                return true;

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchTerm))
                return false;

            var normalizedText = NormalizeForSearch(text);
            var normalizedSearch = NormalizeForSearch(searchTerm);

            return normalizedText == normalizedSearch;
        }
    }
}

