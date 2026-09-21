using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public static class QuestionSortingService
    {
        /// <summary>
        /// Phân loại câu hỏi dựa trên nội dung bên trong môi trường ex hoặc bt.
        /// </summary>
        public static int ClassifyQuestion(string content)
        {
            return (int)QuestionClassifier.Classify(content);
        }

        public static bool IsKnownStandaloneWrapperLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return false;
            string trimmed = line.Trim();
            if (trimmed.StartsWith("%")) return false; // KHÔNG BAO GIỜ xóa comment!

            if (string.Equals(trimmed, @"\BTVD", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(trimmed, @"\PhanI", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(trimmed, @"\PhanII", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(trimmed, @"\PhanIII", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(trimmed, @"\PhanIV", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(trimmed, @"\PhanTL", StringComparison.OrdinalIgnoreCase)) return true;

            if (Regex.IsMatch(trimmed, @"^\\begin\{cauhoi(?:TN|DS|TLN|TL)\}\{[^}]*\}$", RegexOptions.IgnoreCase)) return true;
            if (Regex.IsMatch(trimmed, @"^\\end\{cauhoi(?:TN|DS|TLN|TL)\}$", RegexOptions.IgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// Loại bỏ các thẻ bao ngoài cũ (\BTVD, \PhanI..\PhanIV, \PhanTL, \begin{cauhoi...}, \end{cauhoi...}) một cách AN TOÀN THEO DÒNG.
        /// CHỈ xóa khi cả dòng (sau khi Trim) là wrapper do hệ thống tạo. Giữ nguyên 100% comment và text.
        /// </summary>
        public static string NormalizeWrappers(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var resultLines = new List<string>();

            foreach (var line in lines)
            {
                if (IsKnownStandaloneWrapperLine(line))
                {
                    continue; // Bóc dòng wrapper độc lập
                }
                resultLines.Add(line);
            }

            return string.Join("\n", resultLines);
        }

        private class DocumentChunk
        {
            public bool IsQuestion { get; set; }
            public QuestionItem Question { get; set; }
            public string Text { get; set; }
        }

        /// <summary>
        /// Sắp xếp các khối câu hỏi ex/bt trong từng vùng (region) ngăn cách bởi các boundary (nội dung ngoài ex/bt).
        /// Đảm bảo tuyệt đối không di chuyển câu hỏi vượt qua boundary và hoàn toàn Idempotent.
        /// </summary>
        public static string ProcessSortQuestions(string textToProcess, string bai, out int sortedCount)
        {
            sortedCount = 0;
            if (string.IsNullOrEmpty(textToProcess)) return textToProcess;

            var questions = QuestionBlockParser.ParseBlocks(textToProcess);

            if (questions.Count == 0)
            {
                return textToProcess;
            }

            sortedCount = questions.Count;

            string baiNum = string.IsNullOrEmpty(bai) ? "01" : bai.Trim();
            if (baiNum.StartsWith("Bai", StringComparison.OrdinalIgnoreCase))
            {
                baiNum = baiNum.Substring(3);
            }

            var chunks = new List<DocumentChunk>();
            int lastIndex = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                var q = questions[i];
                if (q.OriginalIndex > lastIndex)
                {
                    string textBetween = textToProcess.Substring(lastIndex, q.OriginalIndex - lastIndex);
                    chunks.Add(new DocumentChunk { IsQuestion = false, Text = textBetween });
                }

                chunks.Add(new DocumentChunk
                {
                    IsQuestion = true,
                    Question = q
                });

                lastIndex = q.OriginalIndex + q.Length;
            }

            if (lastIndex < textToProcess.Length)
            {
                string trailingText = textToProcess.Substring(lastIndex);
                chunks.Add(new DocumentChunk { IsQuestion = false, Text = trailingText });
            }

            var sb = new StringBuilder();
            var currentRegionQuestions = new List<QuestionItem>();

            void FlushRegion()
            {
                if (currentRegionQuestions.Count > 0)
                {
                    string formattedRegion = FormatQuestionRegion(currentRegionQuestions, baiNum);
                    sb.AppendLine(formattedRegion);
                    currentRegionQuestions.Clear();
                }
            }

            foreach (var chunk in chunks)
            {
                if (chunk.IsQuestion)
                {
                    currentRegionQuestions.Add(chunk.Question);
                }
                else
                {
                    string normalized = NormalizeWrappers(chunk.Text);
                    if (string.IsNullOrWhiteSpace(normalized))
                    {
                        // Khoảng trắng thuần túy hoặc wrapper đã bóc -> ở lại trong region hiện tại
                    }
                    else
                    {
                        // Chứa nội dung boundary ngoài câu hỏi -> flush region hiện tại
                        FlushRegion();

                        string trimmedBoundary = normalized.Trim('\r', '\n');
                        if (!string.IsNullOrWhiteSpace(trimmedBoundary))
                        {
                            sb.AppendLine(trimmedBoundary);
                        }
                    }
                }
            }

            FlushRegion();

            return sb.ToString().TrimEnd();
        }

        private static string FormatQuestionRegion(List<QuestionItem> questions, string baiNum)
        {
            var type1 = questions.Where(q => q.Kind == QuestionKind.MultipleChoice).ToList();
            var type2 = questions.Where(q => q.Kind == QuestionKind.TrueFalse).ToList();
            var type3 = questions.Where(q => q.Kind == QuestionKind.ShortAnswer).ToList();
            var type4 = questions.Where(q => q.Kind == QuestionKind.Essay).ToList();

            var sb = new StringBuilder();
            sb.AppendLine(@"\BTVD");

            if (type1.Count > 0)
            {
                sb.AppendLine(@"\PhanI");
                sb.AppendLine($@"\begin{{cauhoiTN}}{{Bai{baiNum}}}");
                foreach (var q in type1) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTN}");
            }

            if (type2.Count > 0)
            {
                sb.AppendLine(@"\PhanII");
                sb.AppendLine($@"\begin{{cauhoiDS}}{{Bai{baiNum}}}");
                foreach (var q in type2) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiDS}");
            }

            if (type3.Count > 0)
            {
                sb.AppendLine(@"\PhanIII");
                sb.AppendLine($@"\begin{{cauhoiTLN}}{{Bai{baiNum}}}");
                foreach (var q in type3) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTLN}");
            }

            if (type4.Count > 0)
            {
                sb.AppendLine(@"\PhanIV");
                sb.AppendLine($@"\begin{{cauhoiTL}}{{Bai{baiNum}}}");
                foreach (var q in type4) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTL}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
