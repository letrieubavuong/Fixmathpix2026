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
            return LatexTransformationService.FixMathtype(text);
        }
    }
}
