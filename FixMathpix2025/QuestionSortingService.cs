using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public class QuestionItem
    {
        public string Content { get; set; }
        public int OriginalIndex { get; set; }
        public int Length { get; set; }
        public int Type { get; set; }
    }

    public class NonQuestionChunk
    {
        public string Content { get; set; }
        public int OriginalIndex { get; set; }
    }

    public static class QuestionSortingService
    {
        /// <summary>
        /// Phân loại câu hỏi dựa trên nội dung bên trong môi trường ex hoặc bt.
        /// Type 1: Trắc nghiệm 4 lựa chọn (\choice)
        /// Type 2: Đúng/Sai (\choiceTF)
        /// Type 3: Trả lời ngắn (\shortans)
        /// Type 4: Tự luận (còn lại)
        /// </summary>
        public static int ClassifyQuestion(string content)
        {
            if (string.IsNullOrEmpty(content)) return 4;

            if (content.IndexOf(@"\choiceTF", StringComparison.OrdinalIgnoreCase) >= 0) return 2;
            if (content.IndexOf(@"\choice", StringComparison.OrdinalIgnoreCase) >= 0) return 1;
            if (content.IndexOf(@"\shortans", StringComparison.OrdinalIgnoreCase) >= 0) return 3;

            return 4;
        }

        /// <summary>
        /// Loại bỏ các thẻ bao ngoài cũ (\BTVD, \PhanI..\PhanIV, \begin{cauhoi...}, \end{cauhoi...}) để chuẩn hóa văn bản trước khi sắp xếp lại.
        /// </summary>
        public static string NormalizeWrappers(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;
            result = Regex.Replace(result, @"\\BTVD\b\s*", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\\Phan(?:III|II|IV|I)\b\s*", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\\begin\{cauhoi(?:TN|DS|TLN|TL)\}\{[^}]*\}\s*", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\\end\{cauhoi(?:TN|DS|TLN|TL)\}\s*", "", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// Sắp xếp các khối câu hỏi ex/bt theo nhóm \PhanI đến \PhanIV mà KHÔNG làm mất các đoạn nội dung ngoài câu hỏi (\subsection*, comment, v.v.) và ĐẢM BẢO IDEMPOTENT.
        /// </summary>
        public static string ProcessSortQuestions(string textToProcess, string bai, out int sortedCount)
        {
            sortedCount = 0;
            if (string.IsNullOrEmpty(textToProcess)) return textToProcess;

            // Bước 1: Chuẩn hóa bóc tách các thẻ bao bọc cũ
            string normalizedText = NormalizeWrappers(textToProcess);

            // Bước 2: Tìm tất cả các khối ex hoặc bt
            string pattern = @"\\begin\s*\{(ex|bt)\}[\s\S]*?\\end\s*\{\1\}";
            var matches = Regex.Matches(normalizedText, pattern, RegexOptions.IgnoreCase);

            if (matches.Count == 0)
            {
                return textToProcess; // Không có khối câu hỏi nào để sắp xếp
            }

            sortedCount = matches.Count;

            // Trích xuất các câu hỏi với vị trí ban đầu
            var questions = new List<QuestionItem>();
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                questions.Add(new QuestionItem
                {
                    Content = match.Value.Trim(),
                    OriginalIndex = match.Index,
                    Length = match.Length,
                    Type = ClassifyQuestion(match.Value)
                });
            }

            int firstMatchPos = matches[0].Index;
            int lastMatchEndPos = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;

            // Trích xuất các đoạn văn bản ngoài câu hỏi nằm giữa các khối câu hỏi
            var nonQuestionChunks = new List<NonQuestionChunk>();
            int currentPos = firstMatchPos;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                if (match.Index > currentPos)
                {
                    string chunkText = normalizedText.Substring(currentPos, match.Index - currentPos);
                    if (!string.IsNullOrWhiteSpace(chunkText))
                    {
                        nonQuestionChunks.Add(new NonQuestionChunk
                        {
                            Content = chunkText.Trim(),
                            OriginalIndex = currentPos
                        });
                    }
                }
                currentPos = match.Index + match.Length;
            }

            // Phân nhóm câu hỏi theo loại
            var type1 = questions.Where(q => q.Type == 1).OrderBy(q => q.OriginalIndex).ToList();
            var type2 = questions.Where(q => q.Type == 2).OrderBy(q => q.OriginalIndex).ToList();
            var type3 = questions.Where(q => q.Type == 3).OrderBy(q => q.OriginalIndex).ToList();
            var type4 = questions.Where(q => q.Type == 4).OrderBy(q => q.OriginalIndex).ToList();

            // Chuẩn hóa tên bài (ví dụ "01" hoặc "Bai01" -> "01")
            string baiNum = string.IsNullOrEmpty(bai) ? "01" : bai.Trim();
            if (baiNum.StartsWith("Bai", StringComparison.OrdinalIgnoreCase))
            {
                baiNum = baiNum.Substring(3);
            }

            // Xây dựng kết quả
            var sb = new StringBuilder();
            sb.AppendLine(@"\BTVD");

            // Phân bố các nonQuestionChunks:
            // Chunks trước tất cả câu hỏi
            var leadingChunks = nonQuestionChunks.Where(c => c.OriginalIndex < firstMatchPos).OrderBy(c => c.OriginalIndex).ToList();
            foreach (var c in leadingChunks) sb.AppendLine(c.Content);

            // Chunks xuất hiện giữa/sau các nhóm
            int maxType1Idx = type1.Count > 0 ? type1.Max(q => q.OriginalIndex) : -1;
            int maxType2Idx = type2.Count > 0 ? type2.Max(q => q.OriginalIndex) : -1;
            int maxType3Idx = type3.Count > 0 ? type3.Max(q => q.OriginalIndex) : -1;

            if (type1.Count > 0)
            {
                sb.AppendLine(@"\PhanI");
                sb.AppendLine($@"\begin{{cauhoiTN}}{{Bai{baiNum}}}");
                foreach (var q in type1) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTN}");

                // Chunks nằm sau Type 1 nhưng trước Type 2/3/4
                var t1Chunks = nonQuestionChunks
                    .Where(c => c.OriginalIndex > maxType1Idx && c.OriginalIndex < (type2.Count > 0 ? type2.Min(q => q.OriginalIndex) : (type3.Count > 0 ? type3.Min(q => q.OriginalIndex) : (type4.Count > 0 ? type4.Min(q => q.OriginalIndex) : int.MaxValue))))
                    .OrderBy(c => c.OriginalIndex).ToList();
                foreach (var c in t1Chunks) sb.AppendLine(c.Content);
            }

            if (type2.Count > 0)
            {
                sb.AppendLine(@"\PhanII");
                sb.AppendLine($@"\begin{{cauhoiDS}}{{Bai{baiNum}}}");
                foreach (var q in type2) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiDS}");

                // Chunks nằm sau Type 2 nhưng trước Type 3/4
                var t2Chunks = nonQuestionChunks
                    .Where(c => c.OriginalIndex > maxType2Idx && c.OriginalIndex < (type3.Count > 0 ? type3.Min(q => q.OriginalIndex) : (type4.Count > 0 ? type4.Min(q => q.OriginalIndex) : int.MaxValue)))
                    .OrderBy(c => c.OriginalIndex).ToList();
                foreach (var c in t2Chunks) sb.AppendLine(c.Content);
            }

            if (type3.Count > 0)
            {
                sb.AppendLine(@"\PhanIII");
                sb.AppendLine($@"\begin{{cauhoiTLN}}{{Bai{baiNum}}}");
                foreach (var q in type3) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTLN}");

                // Chunks nằm sau Type 3 nhưng trước Type 4
                var t3Chunks = nonQuestionChunks
                    .Where(c => c.OriginalIndex > maxType3Idx && c.OriginalIndex < (type4.Count > 0 ? type4.Min(q => q.OriginalIndex) : int.MaxValue))
                    .OrderBy(c => c.OriginalIndex).ToList();
                foreach (var c in t3Chunks) sb.AppendLine(c.Content);
            }

            if (type4.Count > 0)
            {
                sb.AppendLine(@"\PhanIV");
                sb.AppendLine($@"\begin{{cauhoiTL}}{{Bai{baiNum}}}");
                foreach (var q in type4) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTL}");
            }

            // Tất cả các chunks còn lại nằm sau cùng
            var remainingChunks = nonQuestionChunks
                .Where(c => c.OriginalIndex > lastMatchEndPos || (type4.Count > 0 && c.OriginalIndex > type4.Max(q => q.OriginalIndex)))
                .OrderBy(c => c.OriginalIndex).ToList();
            foreach (var c in remainingChunks) sb.AppendLine(c.Content);

            string sortedContent = sb.ToString().TrimEnd();

            // Ghép lại vào normalizedText
            string prefix = normalizedText.Substring(0, firstMatchPos);
            string suffix = normalizedText.Substring(lastMatchEndPos);

            return (prefix + sortedContent + "\n" + suffix).TrimEnd();
        }
    }
}
