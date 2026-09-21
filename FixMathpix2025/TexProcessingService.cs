using System;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public static class TexProcessingService
    {
        /// <summary>
        /// Thực hiện dọn dẹp và chuẩn hóa định dạng TeX/MathType (Pure function, không phụ thuộc UI).
        /// </summary>
        public static string FixMathtype(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text = text.Replace(@"\^{", @"^").Replace(@"_{", @"_");

            // Dọn dẹp khoảng trắng
            text = Regex.Replace(text, @"([{\[])\s+", "$1"); // Xóa khoảng trắng sau [ hoặc {
            text = Regex.Replace(text, @"\s+([}\\.,\?;!])", "$1"); // Xóa khoảng trắng trước các dấu câu
            text = Regex.Replace(text, @"^[ \t]+", "", RegexOptions.Multiline); // Xóa khoảng trắng/tab ở đầu mỗi dòng

            return text;
        }
    }
}
