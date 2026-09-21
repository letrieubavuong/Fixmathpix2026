using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FixMathpix2025
{
    public static class QuestionBlockParser
    {
        private static readonly Regex TokenRegex = new Regex(@"\\(begin|end)\s*\{(ex|bt)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Extract top-level ex and bt question blocks comment-safely with strict environment matching.
        /// Ignores commented \begin and \end tags.
        /// Mismatched environments (e.g. \begin{ex} ... \end{bt}) are not treated as valid question blocks.
        /// Preserves sub-environments such as chc inside parent ex/bt blocks.
        /// </summary>
        public static List<QuestionItem> ParseBlocks(string input)
        {
            var result = new List<QuestionItem>();
            if (string.IsNullOrEmpty(input)) return result;

            string masked = QuestionClassifier.MaskLatexComments(input);
            var matches = TokenRegex.Matches(masked);
            if (matches.Count == 0) return result;

            int matchIndex = 0;
            var stack = new Stack<(string env, int startIndex)>();
            int currentCandidateStartIndex = -1;
            int currentCandidateStartMatchIndex = -1;

            while (matchIndex < matches.Count)
            {
                Match match = matches[matchIndex];
                bool isBegin = match.Groups[1].Value.Equals("begin", StringComparison.OrdinalIgnoreCase);
                string env = match.Groups[2].Value.ToLower();

                if (isBegin)
                {
                    if (stack.Count == 0)
                    {
                        currentCandidateStartIndex = match.Index;
                        currentCandidateStartMatchIndex = matchIndex;
                    }
                    stack.Push((env, match.Index));
                    matchIndex++;
                }
                else // end
                {
                    if (stack.Count == 0)
                    {
                        // Orphan end tag, ignore and proceed
                        matchIndex++;
                    }
                    else
                    {
                        var top = stack.Peek();
                        if (top.env.Equals(env, StringComparison.OrdinalIgnoreCase))
                        {
                            stack.Pop();
                            if (stack.Count == 0)
                            {
                                // Valid top-level block matched!
                                int blockEndIndex = match.Index + match.Length;
                                int length = blockEndIndex - currentCandidateStartIndex;
                                string blockContent = input.Substring(currentCandidateStartIndex, length);

                                result.Add(new QuestionItem
                                {
                                    Content = blockContent.Trim(),
                                    OriginalIndex = currentCandidateStartIndex,
                                    Length = length,
                                    Kind = QuestionClassifier.Classify(blockContent)
                                });

                                currentCandidateStartIndex = -1;
                                currentCandidateStartMatchIndex = -1;
                            }
                            matchIndex++;
                        }
                        else
                        {
                            // Mismatched end tag!
                            // Invalidate current candidate block and retry scanning from the token after candidate start.
                            int retryMatchIndex = currentCandidateStartMatchIndex + 1;
                            stack.Clear();
                            currentCandidateStartIndex = -1;
                            currentCandidateStartMatchIndex = -1;

                            if (retryMatchIndex > matchIndex)
                            {
                                matchIndex++;
                            }
                            else
                            {
                                matchIndex = retryMatchIndex;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
