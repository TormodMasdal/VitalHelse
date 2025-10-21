using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using VitalHelse.Models;

namespace VitalHelse.Helpers
{
    public static class SlugHelper
    {
        public static string Slugify(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var normalized = input.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat == UnicodeCategory.NonSpacingMark) continue; // fjern diakritikk

                if (char.IsLetterOrDigit(ch)) sb.Append(ch);
                else if (char.IsWhiteSpace(ch) || ch == '/' || ch == '_' || ch == '-') sb.Append('-');
                // andre tegn droppes
            }

            var slug = Regex.Replace(sb.ToString(), "-{2,}", "-").Trim('-');
            return slug;
        }

        public static string BuildProductPath(Product product)
        {
            var segments = new List<string> { "produkt" };

            // Gå oppover kategori-treet (bruker første kategori som “primær”)
            var cat = product.ProductCategories?.FirstOrDefault()?.Category;
            var stack = new List<string>();
            while (cat != null)
            {
                stack.Insert(0, Slugify(cat.CategoryName));
                cat = cat.ParentCategory;
            }

            segments.AddRange(stack);
            segments.Add(Slugify(product.ProductName));

            return "/" + string.Join("/", segments);
        }
    }
}