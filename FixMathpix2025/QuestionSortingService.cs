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

            if (Regex.IsMatch(trimmed, @"^\\begin\{cauhoi(?:TN|DS|TLN|TL)\}\{[^}]*\}$", RegexOptions.IgnoreCase)) return true;
            if (Regex.IsMatch(trimmed, @"^\\end\{cauhoi(?:TN|DS|TLN|TL)\}$", RegexOptions.IgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// Loại bỏ các thẻ bao ngoài cũ (\BTVD, \PhanI..\PhanIV, \begin{cauhoi...}, \end{cauhoi...}) một cách AN TOÀN THEO DÒNG.
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

            string pattern = @"\\begin\s*\{(ex|bt)\}[\s\S]*?\\end\s*\{\1\}";
            var matches = Regex.Matches(textToProcess, pattern, RegexOptions.IgnoreCase);

            if (matches.Count == 0)
            {
                return textToProcess;
            }

            sortedCount = matches.Count;

            string baiNum = string.IsNullOrEmpty(bai) ? "01" : bai.Trim();
            if (baiNum.StartsWith("Bai", StringComparison.OrdinalIgnoreCase))
            {
                baiNum = baiNum.Substring(3);
            }

            // Xây dựng chuỗi các chunk (câu hỏi và đoạn văn bản phân cách)
            var chunks = new List<DocumentChunk>();
            int lastIndex = 0;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                if (match.Index > lastIndex)
                {
                    string textBetween = textToProcess.Substring(lastIndex, match.Index - lastIndex);
                    chunks.Add(new DocumentChunk { IsQuestion = false, Text = textBetween });
                }

                chunks.Add(new DocumentChunk
                {
                    IsQuestion = true,
                    Question = new QuestionItem
                    {
                        Content = match.Value.Trim(),
                        OriginalIndex = match.Index,
                        Length = match.Length,
                        Type = ClassifyQuestion(match.Value)
                    }
                });

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < textToProcess.Length)
            {
                string trailingText = textToProcess.Substring(lastIndex);
                chunks.Add(new DocumentChunk { IsQuestion = false, Text = trailingText });
            }

            // Nhóm các câu hỏi vào từng Region.
            // Đoạn văn bản ngoài câu hỏi là Boundary nếu sau khi bóc wrapper cũ nó chứa nội dung khác khoảng trắng.
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
            var type1 = questions.Where(q => q.Type == 1).ToList();
            var type2 = questions.Where(q => q.Type == 2).ToList();
            var type3 = questions.Where(q => q.Type == 3).ToList();
            var type4 = questions.Where(q => q.Type == 4).ToList();

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
