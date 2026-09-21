using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public static class SpellingService
    {
        /// <summary>
        /// Thực hiện thay thế soát lỗi chính tả không cascade (Single-pass regex replacement, ưu tiên rule dài trước rule ngắn).
        /// </summary>
        public static string ApplySpellingCorrections(string text, Dictionary<string, string> corrections, out int replacementsCount)
        {
            replacementsCount = 0;
            if (string.IsNullOrEmpty(text) || corrections == null || corrections.Count == 0)
            {
                return text;
            }

            // Sắp xếp các từ cần sửa theo độ dài giảm dần để ưu tiên cụm từ dài trước
            var sortedKeys = corrections.Keys.Where(k => !string.IsNullOrEmpty(k))
                                            .OrderByDescending(k => k.Length)
                                            .ThenBy(k => k)
                                            .ToList();

            if (!sortedKeys.Any()) return text;

            // Hợp nhất các quy tắc thành một Regex duy nhất với ranh giới từ \b
            string combinedPattern = @"\b(" + string.Join("|", sortedKeys.Select(Regex.Escape)) + @")\b";
            Regex regex = new Regex(combinedPattern, RegexOptions.IgnoreCase);

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in corrections)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    dict[kvp.Key] = kvp.Value;
                }
            }

            int count = 0;
            string result = regex.Replace(text, match =>
            {
                count++;
                if (dict.TryGetValue(match.Value, out string replacement))
                {
                    return replacement;
                }
                return match.Value;
            });

            replacementsCount = count;
            return result;
        }
    }
}
