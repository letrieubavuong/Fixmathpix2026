using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    /// <summary>
    /// Thuần service xử lý biến đổi văn bản và LaTeX (Pure transformation engine).
    /// Không chứa bất kỳ phụ thuộc UI nào (WPF, AvalonEdit, MessageBox, etc.).
    /// </summary>
    public static class LatexTransformationService
    {
        /// <summary>
        /// Sửa định dạng TeX/MathType (Pure function).
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

        /// <summary>
        /// Thực hiện dọn dẹp văn bản thuần (CleanupText).
        /// </summary>
        public static string CleanupText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // 1. Chuẩn hóa ký tự xuống dòng: Xóa \r\n thành \n
            text = text.Replace("\r\n", "\n");

            // 2. Xóa các tab ở đầu dòng
            text = Regex.Replace(text, @"^\t+", "", RegexOptions.Multiline);

            // 3. Sửa lỗi dấu câu trong tiếng Việt
            // Xóa khoảng trắng thừa trước các dấu câu
            text = Regex.Replace(text, @"\s+([,.;:?!])", "$1");
            text = Regex.Replace(text, @"^\s", "");
            // Đảm bảo có một khoảng trắng sau dấu câu (nếu theo sau là một chữ cái/số)
            text = Regex.Replace(text, @"([,.;:?!])(\w)", "$1 $2");

            // 4. Xóa nhiều khoảng trắng liên tục (chỉ xóa space, không xóa các loại whitespace khác như tab, newline)
            text = Regex.Replace(text, @"[ ]{2,}", " ");

            return text;
        }

        /// <summary>
        /// Thực hiện chuyển đổi TeX sang cấu trúc chuẩn (ChuyenTeX).
        /// </summary>
        public static string ProcessChuyenTex(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string currentContent = input;

            // 1. Chuẩn hóa và dọn dẹp cơ bản
            currentContent = currentContent.Replace("\r\n", "\n");
            currentContent = Regex.Replace(currentContent, @"\n{2,}", "\n"); // Nhiều dòng trống -> 1
            currentContent = currentContent.Replace(@"\frac", @"\dfrac");

            // 2. Loại bỏ các lệnh không cần thiết
            currentContent = Regex.Replace(currentContent, @"\\mathrm{(.*?)}", m => m.Groups[1].Value, RegexOptions.Singleline);
            currentContent = Regex.Replace(currentContent, @"\\text{(.*?)}", m => m.Groups[1].Value, RegexOptions.Singleline);

            // 3. Xử lý "Chọn"
            currentContent = Regex.Replace(currentContent, @"Chọn \$\\mathbf{(.*?)}\$", m => $"Chọn {m.Groups[1].Value}\n", RegexOptions.Singleline);

            // 4. Xử lý các section và câu hỏi để bọc trong \begin{ex}...\end{ex}
            // Thêm \end{ex} ở cuối để đảm bảo block cuối cùng được đóng
            if (!currentContent.TrimEnd().EndsWith("\\end{ex}"))
            {
                currentContent += "\n\\end{ex}";
            }

            // Thay thế các section/subsection/câu hỏi bằng cấu trúc \end{ex}...\begin{ex}
            currentContent = Regex.Replace(currentContent, @"\\section\*{(\d{1,}.*?)}\n", m => $"\\end{{ex}}\n{m.Value.Trim()}\n\\begin{{ex}}\n", RegexOptions.Multiline);
            currentContent = Regex.Replace(currentContent, @"\\section\*{BÀI(.*?)}\n", m => $"\\end{{ex}}\n\\section*{{BÀI{m.Groups[1].Value.Trim()}}}\n\\begin{{ex}}\n", RegexOptions.Multiline);
            currentContent = Regex.Replace(currentContent, @"\\subsection\*{(\d{1,}.*?)}\n", m => $"\\end{{ex}}\n{m.Value.Trim()}\n\\begin{{ex}}\n", RegexOptions.Multiline);
            currentContent = Regex.Replace(currentContent, @"\\section\*{(.*?)}\n", m => $"{m.Groups[1].Value.Trim()}\n", RegexOptions.Multiline); // Xóa các section không có số
            currentContent = Regex.Replace(currentContent, @"^Câu\s\d+[\.:]", m => $"\n\\end{{ex}}\n\\begin{{ex}}\n", RegexOptions.Multiline);

            // 5. Xử lý Lời giải
            currentContent = currentContent.Replace(@"Hướng dẫn (Group Vật lý Physics)", "\\loigiai{\n");
            currentContent = Regex.Replace(currentContent, @"Lời giải([\s\.:])", "\\loigiai{\n");

            // 6. Xử lý các lựa chọn A, B, C, D
            currentContent = Regex.Replace(currentContent, @"A\.(.*?)\nB\.(.*?)\nC\.(.*?)\nD\.(.*?)\n",
                m => $"\\choice\n{{{m.Groups[1].Value.Trim()}}}\n{{{m.Groups[2].Value.Trim()}}}\n{{{m.Groups[3].Value.Trim()}}}\n{{{m.Groups[4].Value.Trim()}}}\n",
                RegexOptions.Singleline);

            // 7. Xử lý các block \loigiai bên trong \begin{ex}
            string regexExBlocks = @"\\begin{ex}([\s\S]*?)\\end{ex}";
            var newFullContent = new StringBuilder();
            int lastIndex = 0;

            foreach (Match match in Regex.Matches(currentContent, regexExBlocks))
            {
                newFullContent.Append(currentContent, lastIndex, match.Index - lastIndex);
                string exBlockContent = match.Groups[1].Value;

                if (exBlockContent.Contains("\\loigiai{"))
                {
                    // Nếu có \loigiai, thêm dấu } vào cuối nội dung của block đó
                    newFullContent.Append("\\begin{ex}");
                    newFullContent.Append(exBlockContent.TrimEnd());
                    newFullContent.Append("\n}\n"); // Thêm dấu đóng cho \loigiai
                    newFullContent.Append("\\end{ex}");
                }
                else
                {
                    // Giữ nguyên block nếu không có \loigiai
                    newFullContent.Append(match.Value);
                }
                lastIndex = match.Index + match.Length;
            }
            newFullContent.Append(currentContent, lastIndex, currentContent.Length - lastIndex);
            currentContent = newFullContent.ToString();

            // 8. Dọn dẹp cuối cùng
            // Xóa \end{ex} thừa ở đầu
            if (currentContent.StartsWith("\\end{ex}"))
            {
                currentContent = currentContent.Substring("\\end{ex}".Length);
            }
            // Đảm bảo bắt đầu bằng \begin{ex}
            if (!currentContent.TrimStart().StartsWith("\\begin{ex}"))
            {
                currentContent = "\\begin{ex}\n" + currentContent;
            }

            currentContent = currentContent.Trim();

            // Dọn dẹp khoảng trắng quanh dấu }
            currentContent = currentContent.Replace(" }", "}")
                                           .Replace("\t}", "}")
                                           .Replace("{ ", "{")
                                           .Replace(".}\n", "}\n");
            currentContent = Regex.Replace(currentContent, @"\n{3,}", "\n\n");

            currentContent = FixMathtype(currentContent);

            return currentContent;
        }

        /// <summary>
        /// Thay thế môi trường enumerate thành listEX trong các môi trường vd, vidu, ex, bt.
        /// </summary>
        public static string FixEnumerate(string currentText, out int replacementsCount)
        {
            replacementsCount = 0;
            if (string.IsNullOrEmpty(currentText)) return currentText;

            string pattern = @"(\\begin\{(?:vd|vidu|ex|bt)\}[\s\S]*?\\loigiai\{)";
            int count = 0;

            string newText = Regex.Replace(currentText, pattern, match =>
            {
                string block = match.Value;
                string enumeratePattern = @"\\begin{enumerate}(\[[^\]]*\])?";

                if (Regex.IsMatch(block, enumeratePattern))
                {
                    count++;
                    string modifiedBlock = Regex.Replace(block, enumeratePattern, @"\begin{listEX}[1]");
                    modifiedBlock = modifiedBlock.Replace(@"\end{enumerate}", @"\end{listEX}");
                    return modifiedBlock;
                }
                return block;
            }, RegexOptions.Multiline);

            replacementsCount = count;
            return newText;
        }

        /// <summary>
        /// Chuẩn hóa tùy chọn trong các khối tikzpicture.
        /// </summary>
        public static string FixTikzpicture(string currentText, out int replacementsCount)
        {
            replacementsCount = 0;
            if (string.IsNullOrEmpty(currentText)) return currentText;

            string pattern = @"(\\begin\{tikzpicture\}\[)([^\]]*)(\])";
            int count = 0;

            string newText = Regex.Replace(currentText, pattern, match =>
            {
                count++;
                string options = match.Groups[2].Value;

                // Remove thickness options
                options = Regex.Replace(options, @"\b(very thick|thick|thin)\b", "", RegexOptions.IgnoreCase);

                // Remove color options
                options = Regex.Replace(options, @"\b(red|green|blue|black|white|cyan|magenta|yellow|gray|darkgray|lightgray)\b", "", RegexOptions.IgnoreCase);

                // Clean up extra commas and whitespace
                options = Regex.Replace(options, @"\s*,\s*", ",", RegexOptions.None).Trim().Trim(',');

                // Construct the new options string
                string newOptions = "draw=Mapcolor, very thick," + (string.IsNullOrEmpty(options) ? "" : " " + options);

                return match.Groups[1].Value + newOptions + match.Groups[3].Value;
            }, RegexOptions.IgnoreCase);

            replacementsCount = count;
            return newText;
        }

        /// <summary>
        /// Chuẩn hóa nội dung khối \heva{...}.
        /// </summary>
        public static string FixHevaContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return content;
            string newContent = content.Replace("&", "");
            newContent = Regex.Replace(newContent, @"\s*\\\\\s*", @" \\ ");
            return newContent;
        }

        /// <summary>
        /// Viết hoa tiêu đề dạng toán, bỏ qua các đoạn công thức math mode ($...$).
        /// </summary>
        public static string TitleCaseMath(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var result = new StringBuilder();
            bool inMath = false;

            foreach (char c in input)
            {
                if (c == '$')
                {
                    inMath = !inMath;
                    result.Append(c);
                }
                else if (inMath)
                {
                    result.Append(c);
                }
                else
                {
                    result.Append(char.ToUpper(c));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Viết hoa nội dung section/subsection/subsubsection, giữ nguyên công thức math mode ($...$) và tên lệnh TeX (\cmd).
        /// </summary>
        public static string UppercaseSectionContent(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            StringBuilder sb = new StringBuilder();
            bool inMath = false;
            bool inCommand = false;

            foreach (char c in input)
            {
                if (inMath)
                {
                    sb.Append(c);
                    if (c == '$') inMath = false;
                }
                else
                {
                    if (c == '$')
                    {
                        inMath = true;
                        sb.Append(c);
                    }
                    else if (c == '\\')
                    {
                        inCommand = true;
                        sb.Append(c);
                    }
                    else if (inCommand)
                    {
                        sb.Append(c);
                        if (!char.IsLetter(c))
                        {
                            inCommand = false;
                        }
                    }
                    else
                    {
                        sb.Append(char.ToUpper(c));
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Tạo môi trường listEX từ các dòng được bôi đen.
        /// </summary>
        public static string FormatListEX(string selectedText, string columnCount = "1")
        {
            if (string.IsNullOrEmpty(selectedText)) return selectedText;

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var newContent = new StringBuilder();

            newContent.AppendLine($"\\begin{{listEX}}[{columnCount}]");
            var regex = new Regex(@"^\s*([a-z][\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                string itemContent = match.Success ? match.Groups[2].Value.Trim() : line.Trim();
                newContent.AppendLine($"\\item {itemContent}");
            }

            newContent.Append("\\end{listEX}");
            return newContent.ToString();
        }

        /// <summary>
        /// Tạo môi trường enumEX từ các dòng được bôi đen.
        /// </summary>
        public static string FormatEnumEX(string selectedText, string columnCount = "1", string listStyle = "a)")
        {
            if (string.IsNullOrEmpty(selectedText)) return selectedText;

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var newContent = new StringBuilder();

            newContent.AppendLine($"\\begin{{enumEX}}[{listStyle}]{{{columnCount}}}");
            var regex = new Regex(@"^\s*([a-z0-9]+[\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                string itemContent = match.Success ? match.Groups[2].Value.Trim() : line.Trim();
                newContent.AppendLine($"\\item {itemContent}");
            }

            newContent.Append("\\end{enumEX}");
            return newContent.ToString();
        }

        /// <summary>
        /// Bọc văn bản trong môi trường align*.
        /// </summary>
        public static string WrapWithAlignStar(string selectedText)
        {
            if (string.IsNullOrEmpty(selectedText)) return "\\begin{align*}\n\n\\end{align*}";

            string processedText = selectedText.Replace("$", "");
            var lines = processedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var resultLines = new StringBuilder();

            foreach (var line in lines)
            {
                int firstEqualSign = line.IndexOf('=');
                if (firstEqualSign != -1)
                {
                    resultLines.AppendLine(Regex.Replace(line, "(?<=.=.*)=", m => @"\\ " + "\n" + m.Value));
                }
                else
                {
                    resultLines.AppendLine(line);
                }
            }

            string content = resultLines.ToString().TrimEnd('\r', '\n');
            var newContent = new StringBuilder();
            newContent.AppendLine("\\begin{align*}");
            newContent.AppendLine(content);
            newContent.Append("\\end{align*}");
            return newContent.ToString();
        }

        /// <summary>
        /// Bọc văn bản trong môi trường equation.
        /// </summary>
        public static string WrapWithEquation(string selectedText)
        {
            if (string.IsNullOrEmpty(selectedText)) return "\\begin{equation}\n\t\n\\end{equation}";

            string processedText = selectedText;
            processedText = Regex.Replace(processedText, @"\\begin\{align\*\}", "", RegexOptions.IgnoreCase);
            processedText = Regex.Replace(processedText, @"\\end\{align\*\}", "", RegexOptions.IgnoreCase);
            processedText = processedText.Replace("$", "");
            processedText = processedText.Replace(@"\tag", @"\label");
            processedText = processedText.Trim();

            return $"\\begin{{equation}}\n\t{processedText}\n\\end{{equation}}";
        }

        /// <summary>
        /// Bọc văn bản trong môi trường choiceTF[t].
        /// </summary>
        public static string WrapWithCauDS(string selectedText)
        {
            if (string.IsNullOrEmpty(selectedText)) return selectedText;

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var newContent = new StringBuilder();

            newContent.AppendLine(@"\choiceTF[t]");
            var regex = new Regex(@"^\s*([a-z][\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                string itemContent = match.Success ? match.Groups[2].Value.Trim() : line.Trim();
                newContent.AppendLine($"{{{itemContent}}}");
            }

            return newContent.ToString();
        }

        /// <summary>
        /// Bọc văn bản trong môi trường immini.
        /// </summary>
        public static string WrapWithImmini(string selectedText, bool withThm)
        {
            if (string.IsNullOrEmpty(selectedText)) return selectedText;

            string processedText = selectedText;

            if (processedText.Contains(@"\begin{center}"))
            {
                processedText = processedText.Replace(@"\begin{center}", "").Trim();
                processedText = processedText.Replace(@"\end{center}", "").Trim();
                processedText = processedText.Replace(@"\begin{tikzpicture}", @"}{" + Environment.NewLine + @"\begin{tikzpicture}");
            }

            string command = withThm ? @"\immini[thm]" : @"\immini";
            return $"{command}{{{processedText}}}";
        }
    }
}
