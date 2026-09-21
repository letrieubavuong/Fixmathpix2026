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

        /// <summary>
        /// Sắp xếp các khối câu hỏi ex/bt theo nhóm \PhanI đến \PhanIV mà KHÔNG làm mất các đoạn nội dung ngoài câu hỏi (\subsection*, comment, v.v.).
        /// </summary>
        public static string ProcessSortQuestions(string textToProcess, string bai, out int sortedCount)
        {
            sortedCount = 0;
            if (string.IsNullOrEmpty(textToProcess)) return textToProcess;

            // Tìm tất cả các khối ex hoặc bt
            string pattern = @"\\begin\s*\{(ex|bt)\}[\s\S]*?\\end\s*\{\1\}";
            var matches = Regex.Matches(textToProcess, pattern, RegexOptions.IgnoreCase);

            if (matches.Count == 0)
            {
                return textToProcess;
            }

            sortedCount = matches.Count;

            // Phân loại câu hỏi
            var questions = new List<QuestionItem>();
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                questions.Add(new QuestionItem
                {
                    Content = match.Value,
                    OriginalIndex = match.Index,
                    Length = match.Length,
                    Type = ClassifyQuestion(match.Value)
                });
            }

            var type1 = questions.Where(q => q.Type == 1).OrderBy(q => q.OriginalIndex).ToList();
            var type2 = questions.Where(q => q.Type == 2).OrderBy(q => q.OriginalIndex).ToList();
            var type3 = questions.Where(q => q.Type == 3).OrderBy(q => q.OriginalIndex).ToList();
            var type4 = questions.Where(q => q.Type == 4).OrderBy(q => q.OriginalIndex).ToList();

            int startIndex = matches[0].Index;
            int endIndex = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;
            string questionRegion = textToProcess.Substring(startIndex, endIndex - startIndex);

            // Thu thập các đoạn văn bản không phải câu hỏi nằm giữa các câu hỏi (bảo vệ \subsection*, comments...)
            var nonQuestionChunks = new List<string>();
            int currentPos = 0;

            foreach (Match match in matches)
            {
                int relIndex = match.Index - startIndex;
                if (relIndex > currentPos)
                {
                    string chunk = questionRegion.Substring(currentPos, relIndex - currentPos);
                    // Lọc bỏ các thẻ phân nhóm cũ nếu có
                    string cleanedChunk = Regex.Replace(chunk, @"\\BTVD[ \t]*\r?\n?", "");
                    cleanedChunk = Regex.Replace(cleanedChunk, @"\\Phan(?:I|II|III|IV)[ \t]*\r?\n?", "");
                    cleanedChunk = Regex.Replace(cleanedChunk, @"\\begin\{cauhoi(?:TN|DS|TLN|TL)\}\{.*?\}[ \t]*\r?\n?", "");
                    cleanedChunk = Regex.Replace(cleanedChunk, @"\\end\{cauhoi(?:TN|DS|TLN|TL)\}[ \t]*\r?\n?", "");

                    if (!string.IsNullOrWhiteSpace(cleanedChunk))
                    {
                        nonQuestionChunks.Add(cleanedChunk.Trim());
                    }
                }
                currentPos = relIndex + match.Length;
            }

            var sb = new StringBuilder();
            sb.AppendLine(@"\BTVD");

            // Bảo toàn các đoạn văn bản ngoài câu hỏi
            foreach (var chunk in nonQuestionChunks)
            {
                sb.AppendLine(chunk);
            }

            if (type1.Count > 0)
            {
                sb.AppendLine(@"\PhanI");
                sb.AppendLine($@"\begin{{cauhoiTN}}{{Bai{bai}}}");
                foreach (var q in type1) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTN}");
            }

            if (type2.Count > 0)
            {
                sb.AppendLine(@"\PhanII");
                sb.AppendLine($@"\begin{{cauhoiDS}}{{Bai{bai}}}");
                foreach (var q in type2) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiDS}");
            }

            if (type3.Count > 0)
            {
                sb.AppendLine(@"\PhanIII");
                sb.AppendLine($@"\begin{{cauhoiTLN}}{{Bai{bai}}}");
                foreach (var q in type3) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTLN}");
            }

            if (type4.Count > 0)
            {
                sb.AppendLine(@"\PhanIV");
                sb.AppendLine($@"\begin{{cauhoiTL}}{{Bai{bai}}}");
                foreach (var q in type4) sb.AppendLine(q.Content);
                sb.AppendLine(@"\end{cauhoiTL}");
            }

            return textToProcess.Remove(startIndex, endIndex - startIndex).Insert(startIndex, sb.ToString().TrimEnd());
        }
    }
}
