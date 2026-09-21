using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public static class QuestionBlockParser
    {
        private static readonly Regex TokenRegex = new Regex(@"\\(begin|end)\s*\{(ex|bt)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Extract top-level ex and bt question blocks comment-safely.
        /// Commented \begin or \end tags are ignored.
        /// Sub-environments like chc inside ex/bt are preserved inside their parent question block.
        /// </summary>
        public static List<QuestionItem> ParseBlocks(string input)
        {
            var result = new List<QuestionItem>();
            if (string.IsNullOrEmpty(input)) return result;

            string masked = QuestionClassifier.MaskLatexComments(input);
            var matches = TokenRegex.Matches(masked);

            int currentDepth = 0;
            int blockStartIndex = -1;

            foreach (Match match in matches)
            {
                bool isBegin = match.Groups[1].Value.Equals("begin", StringComparison.OrdinalIgnoreCase);

                if (isBegin)
                {
                    if (currentDepth == 0)
                    {
                        currentDepth = 1;
                        blockStartIndex = match.Index;
                    }
                    else
                    {
                        currentDepth++;
                    }
                }
                else // end
                {
                    if (currentDepth > 0)
                    {
                        currentDepth--;
                        if (currentDepth == 0)
                        {
                            int blockEndIndex = match.Index + match.Length;
                            int length = blockEndIndex - blockStartIndex;
                            string blockContent = input.Substring(blockStartIndex, length);

                            result.Add(new QuestionItem
                            {
                                Content = blockContent.Trim(),
                                OriginalIndex = blockStartIndex,
                                Length = length,
                                Kind = QuestionClassifier.Classify(blockContent)
                            });

                            blockStartIndex = -1;
                        }
                    }
                }
            }

            return result;
        }
    }
}
