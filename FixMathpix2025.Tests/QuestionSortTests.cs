using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class QuestionSortTests
    {
        [Fact]
        public void ProcessSortQuestions_ShouldPreserveContentOrderAndRelativeIndex()
        {
            string input = @"\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}
\subsection*{Nội dung phải giữ}
% comment phải giữ
\begin{center} Nội dung center \end{center}
\begin{ex} Q2 \choiceTF{A}{B}{C}{D} \end{ex}";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(2, sortedCount);

            int idxSub = result.IndexOf(@"\subsection*{Nội dung phải giữ}");
            int idxComment = result.IndexOf(@"% comment phải giữ");
            int idxCenter = result.IndexOf(@"\begin{center} Nội dung center \end{center}");

            Assert.True(idxSub >= 0, "\\subsection phải tồn tại");
            Assert.True(idxComment >= 0, "comment phải tồn tại");
            Assert.True(idxCenter >= 0, "center phải tồn tại");

            Assert.True(idxSub < idxComment, "\\subsection phải xuất hiện trước comment");
            Assert.True(idxComment < idxCenter, "comment phải xuất hiện trước center");
        }

        [Fact]
        public void ProcessSortQuestions_Type2BoundaryType1_ShouldNotMoveBoundary()
        {
            string input = @"\begin{ex} Q2 \choiceTF{A}{B}{C}{D} \end{ex}
\subsection*{BOUNDARY}
\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(2, sortedCount);

            int idxQ2 = result.IndexOf("Q2");
            int idxBoundary = result.IndexOf(@"\subsection*{BOUNDARY}");
            int idxQ1 = result.IndexOf("Q1");

            Assert.True(idxQ2 >= 0, "Q2 phải tồn tại");
            Assert.True(idxBoundary >= 0, "Boundary phải tồn tại");
            Assert.True(idxQ1 >= 0, "Q1 phải tồn tại");

            Assert.True(idxQ2 < idxBoundary, "Q2 phải nằm trước Boundary");
            Assert.True(idxBoundary < idxQ1, "Boundary phải nằm trước Q1");
        }

        [Fact]
        public void ProcessSortQuestions_Type4BoundaryType1_ShouldNotMoveBoundary()
        {
            string input = @"\begin{ex} Câu tự luận \end{ex}
% GIỮ ĐÚNG VỊ TRÍ
\begin{ex} Câu trắc nghiệm \choice{A}{B}{C}{D} \end{ex}";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(2, sortedCount);

            int idxTL = result.IndexOf("Câu tự luận");
            int idxComment = result.IndexOf("% GIỮ ĐÚNG VỊ TRÍ");
            int idxTN = result.IndexOf("Câu trắc nghiệm");

            Assert.True(idxTL < idxComment, "Câu tự luận phải nằm trước comment");
            Assert.True(idxComment < idxTN, "Comment phải nằm trước câu trắc nghiệm");
        }

        [Fact]
        public void ProcessSortQuestions_SameRegion_ShouldSortType1BeforeType2()
        {
            string input = @"\begin{ex} Type 2 \choiceTF{A}{B}{C}{D} \end{ex}
\begin{ex} Type 1 \choice{A}{B}{C}{D} \end{ex}";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(2, sortedCount);

            int idxT1 = result.IndexOf("Type 1");
            int idxT2 = result.IndexOf("Type 2");

            Assert.True(idxT1 < idxT2, "Trong cùng region, Type 1 phải lên trước Type 2");
        }

        [Fact]
        public void ProcessSortQuestions_StrongIdempotence_1x_2x_3x_ShouldBeIdentical()
        {
            string[] testCases = new string[]
            {
                @"\begin{ex} Q2 \choiceTF{A}{B}{C}{D} \end{ex}
\subsection*{BOUNDARY}
\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}",

                @"\begin{ex} Câu tự luận \end{ex}
% GIỮ ĐÚNG VỊ TRÍ
\begin{ex} Câu trắc nghiệm \choice{A}{B}{C}{D} \end{ex}",

                @"\begin{ex} Q2 \choiceTF{A}{B}{C}{D} \end{ex}
\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}
\subsection*{MIDDLE}
\begin{ex} Q4 tự luận \end{ex}
\begin{ex} Q1b \choice{A}{B}{C}{D} \end{ex}"
            };

            foreach (var input in testCases)
            {
                string once = QuestionSortingService.ProcessSortQuestions(input, "01", out _);
                string twice = QuestionSortingService.ProcessSortQuestions(once, "01", out _);
                string threeTimes = QuestionSortingService.ProcessSortQuestions(twice, "01", out _);

                Assert.Equal(once, twice);
                Assert.Equal(twice, threeTimes);
            }
        }

        [Fact]
        public void ProcessSortQuestions_ShouldNotDuplicateExistingWrappers()
        {
            string inputWithWrapper = @"\BTVD
\PhanI
\begin{cauhoiTN}{Bai01}
\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}
\end{cauhoiTN}
\PhanII
\begin{cauhoiDS}{Bai01}
\begin{ex} Q2 \choiceTF{A}{B}{C}{D} \end{ex}
\end{cauhoiDS}";

            string result = QuestionSortingService.ProcessSortQuestions(inputWithWrapper, "01", out int sortedCount);

            Assert.Equal(2, sortedCount);

            int btvdCount = System.Text.RegularExpressions.Regex.Matches(result, @"\\BTVD\b").Count;
            int phanICount = System.Text.RegularExpressions.Regex.Matches(result, @"\\PhanI\b").Count;
            int cauhoiTNCount = System.Text.RegularExpressions.Regex.Matches(result, @"\\begin\{cauhoiTN\}").Count;

            Assert.Equal(1, btvdCount);
            Assert.Equal(1, phanICount);
            Assert.Equal(1, cauhoiTNCount);
        }

        [Fact]
        public void ProcessSortQuestions_ShouldReturnOriginalTextIfNoQuestionsFound()
        {
            string input = @"Chỉ là văn bản thường không có ex hay bt";

            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            Assert.Equal(0, sortedCount);
            Assert.Equal(input, result);
        }
    }
}
