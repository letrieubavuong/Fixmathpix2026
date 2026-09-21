using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class QuestionSortTests
    {
        [Fact]
        public void ProcessSortQuestions_ShouldPreserveContentOrderAndRelativeIndex()
        {
            // Test 1: Giữ nội dung và vị trí tương đối
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

            // Thứ tự các đoạn ngoài câu hỏi không bị phá: subsection < comment < center
            Assert.True(idxSub < idxComment, "\\subsection phải xuất hiện trước comment");
            Assert.True(idxComment < idxCenter, "comment phải xuất hiện trước center");
        }

        [Fact]
        public void ProcessSortQuestions_ShouldBeIdempotent()
        {
            // Test 2: Idempotence (Sort(Sort(x)) == Sort(x))
            string input = @"\begin{ex} Q1 \choice{A}{B}{C}{D} \end{ex}
\subsection*{Nội dung}
\begin{ex} Q2 \choiceTF{A}{B}{C}{D} \end{ex}";

            string once = QuestionSortingService.ProcessSortQuestions(input, "01", out _);
            string twice = QuestionSortingService.ProcessSortQuestions(once, "01", out _);

            Assert.Equal(once, twice);
        }

        [Fact]
        public void ProcessSortQuestions_ShouldNotDuplicateExistingWrappers()
        {
            // Test 3: Document đã có wrapper không bị lồng thêm wrapper
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

            // Kiểm tra số lần xuất hiện của các thẻ bao bọc (không bị trùng lặp/lồng nhau)
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
