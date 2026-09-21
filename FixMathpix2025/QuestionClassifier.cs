using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public static class QuestionClassifier
    {
        /// <summary>
        /// Masks LaTeX comments (% to end of line) preserving original text length and newline structure.
        /// Escaped percent (\%) is NOT treated as a comment character.
        /// </summary>
        public static string MaskLatexComments(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder(input.Length);
            bool inComment = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (inComment)
                {
                    if (c == '\n')
                    {
                        inComment = false;
                        sb.Append('\n');
                    }
                    else if (c == '\r')
                    {
                        sb.Append('\r');
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }
                else
                {
                    if (c == '%')
                    {
                        // Check if escaped by preceding odd number of backslashes
                        int backslashCount = 0;
                        for (int j = i - 1; j >= 0 && input[j] == '\\'; j--)
                        {
                            backslashCount++;
                        }

                        bool isEscaped = (backslashCount % 2 != 0);

                        if (!isEscaped)
                        {
                            inComment = true;
                            sb.Append(' ');
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Classify question content into QuestionKind based on domain markers.
        /// Precedence: TrueFalse -> MultipleChoice -> ShortAnswer -> Essay.
        /// Ignores commands inside comments and requires exact token boundary (e.g. \choicew is not \choice).
        /// </summary>
        public static QuestionKind Classify(string content)
        {
            if (string.IsNullOrEmpty(content)) return QuestionKind.Essay;

            string masked = MaskLatexComments(content);

            // TrueFalse markers: \choiceTF, \choiceTFt, \choiceTFn (and \choiceTF[t])
            if (Regex.IsMatch(masked, @"\\choiceTF(t|n)?(?![a-zA-Z])", RegexOptions.IgnoreCase))
            {
                return QuestionKind.TrueFalse;
            }

            // MultipleChoice markers: \choice, \choiceN, \motcot, \haicot, \boncot
            if (Regex.IsMatch(masked, @"\\(choiceN?|motcot|haicot|boncot)(?![a-zA-Z])", RegexOptions.IgnoreCase))
            {
                return QuestionKind.MultipleChoice;
            }

            // ShortAnswer markers: \shortans, \SA
            if (Regex.IsMatch(masked, @"\\(shortans|SA)(?![a-zA-Z])", RegexOptions.IgnoreCase))
            {
                return QuestionKind.ShortAnswer;
            }

            return QuestionKind.Essay;
        }
    }
}
