using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class QuestionEngineTests
    {
        [Fact]
        public void Classify_MultipleChoiceMarkers_ShouldClassifyAsMultipleChoice()
        {
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(@"\begin{ex} \choice{A}{B}{C}{D} \end{ex}"));
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(@"\begin{ex} \choiceN{A}{B}{C}{D} \end{ex}"));
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(@"\begin{ex} \motcot \item A \item B \end{ex}"));
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(@"\begin{ex} \haicot \item A \item B \end{ex}"));
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(@"\begin{ex} \boncot \item A \item B \end{ex}"));
        }

        [Fact]
        public void Classify_TrueFalseMarkers_ShouldClassifyAsTrueFalse()
        {
            Assert.Equal(QuestionKind.TrueFalse, QuestionClassifier.Classify(@"\begin{ex} \choiceTF{A}{B}{C}{D} \end{ex}"));
            Assert.Equal(QuestionKind.TrueFalse, QuestionClassifier.Classify(@"\begin{ex} \choiceTF[t]{A}{B}{C}{D} \end{ex}"));
            Assert.Equal(QuestionKind.TrueFalse, QuestionClassifier.Classify(@"\begin{ex} \choiceTFt{A}{B}{C}{D} \end{ex}"));
            Assert.Equal(QuestionKind.TrueFalse, QuestionClassifier.Classify(@"\begin{ex} \choiceTFn{A}{B}{C}{D} \end{ex}"));
        }

        [Fact]
        public void Classify_ShortAnswerMarkers_ShouldClassifyAsShortAnswer()
        {
            Assert.Equal(QuestionKind.ShortAnswer, QuestionClassifier.Classify(@"\begin{ex} \shortans{2} \end{ex}"));
            Assert.Equal(QuestionKind.ShortAnswer, QuestionClassifier.Classify(@"\begin{ex} \SA[oly]{2} \end{ex}"));
        }

        [Fact]
        public void Classify_EssayOrDapsoOnly_ShouldClassifyAsEssay()
        {
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify(@"\begin{ex} Câu tự luận không có marker \end{ex}"));
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify(@"\begin{ex} Câu tự luận \dapso{10} \end{ex}"));
        }

        [Fact]
        public void Classify_PrefixSafety_CommandTokensMustNotMatchSubstrings()
        {
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify(@"\begin{ex} \choicew{5cm} \end{ex}"));
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify(@"\begin{ex} \choiceSomething \end{ex}"));
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify(@"\begin{ex} \SAmething \end{ex}"));
        }

        [Fact]
        public void Classify_CommentSafety_MarkersInsideCommentsMustBeIgnored()
        {
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify("\\begin{ex} Câu tự luận % \\choice{A}{B}{C}{D} \\end{ex}"));
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify("\\begin{ex} Câu tự luận % \\shortans{2} \\end{ex}"));
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify("\\begin{ex} Câu tự luận % \\choiceTF{A}{B} \\end{ex}"));
        }

        [Fact]
        public void Classify_EscapedPercent_ShouldNotBeTreatedAsComment()
        {
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(@"\begin{ex} Tỉ lệ 50\% \choice{A}{B}{C}{D} \end{ex}"));
        }

        [Fact]
        public void Classify_Precedence_TrueFalseOverMultipleChoice()
        {
            string input = @"\begin{ex} \choice{A}{B}{C}{D} \choiceTF{a}{b}{c}{d} \end{ex}";
            Assert.Equal(QuestionKind.TrueFalse, QuestionClassifier.Classify(input));
        }

        [Fact]
        public void Classify_Precedence_MultipleChoiceOverShortAnswer()
        {
            string input = @"\begin{ex} \shortans{2} \choice{A}{B}{C}{D} \end{ex}";
            Assert.Equal(QuestionKind.MultipleChoice, QuestionClassifier.Classify(input));
        }

        [Fact]
        public void Classify_Precedence_TrueFalseOverShortAnswer()
        {
            string input = @"\begin{ex} \shortans{2} \choiceTF{a}{b}{c}{d} \end{ex}";
            Assert.Equal(QuestionKind.TrueFalse, QuestionClassifier.Classify(input));
        }

        [Fact]
        public void Classify_Precedence_EssayFallback()
        {
            string input = @"\begin{ex} Trình bày lời giải chi tiết. \loigiai{...} \end{ex}";
            Assert.Equal(QuestionKind.Essay, QuestionClassifier.Classify(input));
        }

        [Fact]
        public void BlockParser_MatchedEx_ShouldEmitValidBlock()
        {
            string input = @"\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Single(blocks);
            Assert.Equal(QuestionKind.MultipleChoice, blocks[0].Kind);
        }

        [Fact]
        public void BlockParser_MatchedBt_ShouldEmitValidBlock()
        {
            string input = @"\begin{bt} Q1 \end{bt}";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Single(blocks);
            Assert.Equal(QuestionKind.Essay, blocks[0].Kind);
        }

        [Fact]
        public void BlockParser_MismatchedExToBt_ShouldEmitZeroBlocks()
        {
            string input = @"\begin{ex} Broken \end{bt}";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Empty(blocks);
        }

        [Fact]
        public void BlockParser_MismatchedBtToEx_ShouldEmitZeroBlocks()
        {
            string input = @"\begin{bt} Broken \end{ex}";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Empty(blocks);
        }

        [Fact]
        public void BlockParser_UnmatchedBegin_ShouldEmitZeroBlocks()
        {
            string input = @"\begin{ex} Broken";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Empty(blocks);
        }

        [Fact]
        public void BlockParser_UnmatchedEnd_ShouldEmitZeroBlocks()
        {
            string input = @"Text \end{ex}";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Empty(blocks);
        }

        [Fact]
        public void BlockParser_RecoveryAfterInvalidBlock_ShouldParseValidBlockFollowingBrokenBlock()
        {
            string input = @"\begin{ex} Broken \end{bt} \begin{ex} Valid \choice{A}{B}{C}{D} \end{ex}";
            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Single(blocks);
            Assert.Contains("Valid", blocks[0].Content);
            Assert.DoesNotContain("Broken", blocks[0].Content);
        }

        [Fact]
        public void BlockParser_CommentedBeginEnd_MustBeIgnored()
        {
            string input = @"% \begin{ex} comment block \end{ex}
\begin{ex} Real question \choice{A}{B}{C}{D} \end{ex}";

            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Single(blocks);
            Assert.Contains("Real question", blocks[0].Content);
        }

        [Fact]
        public void BlockParser_CommentedEndInsideBlock_MustFindMatchingRealEnd()
        {
            string input = @"\begin{ex} Real question % \end{ex}
\choice{A}{B}{C}{D} \end{ex}";

            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Single(blocks);
            Assert.Contains(@"\choice{A}{B}{C}{D}", blocks[0].Content);
        }

        [Fact]
        public void BlockParser_ChcInsideEx_MustRemainInParentExBlock()
        {
            string input = @"\begin{ex} Dữ kiện chung
\begin{chc} Câu con 1 \choice{A}{B}{C}{D} \end{chc}
\end{ex}";

            var blocks = QuestionBlockParser.ParseBlocks(input);

            Assert.Single(blocks);
            Assert.Contains(@"\begin{chc}", blocks[0].Content);
        }

        [Fact]
        public void ProcessSortQuestions_NonSortableEnvironments_ShouldReturnZeroSortedCount()
        {
            string input = @"\begin{vd} Ví dụ 1 \choice{A}{B}{C}{D} \end{vd}
\begin{vidu} Ví dụ 2 \end{vidu}
\begin{printex} Print 1 \end{printex}";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(0, sortedCount);
            Assert.Equal(input, result);
        }

        [Fact]
        public void ProcessSortQuestions_LegacyPhanTL_ShouldNormalizeToPhanIV()
        {
            string input = @"\BTVD
\PhanTL
\begin{cauhoiTL}{Bai01}
\begin{ex} Câu tự luận \end{ex}
\end{cauhoiTL}";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(1, sortedCount);
            Assert.DoesNotContain(@"\PhanTL", result);
            Assert.Contains(@"\PhanIV", result);
            Assert.Contains(@"\begin{cauhoiTL}{Bai01}", result);

            string resultTwice = QuestionSortingService.ProcessSortQuestions(result, "01", out _);
            Assert.Equal(result, resultTwice);
        }
    }
}
