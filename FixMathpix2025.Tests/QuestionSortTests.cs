using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class QuestionSortTests
    {
        [Fact]
        public void ProcessSortQuestions_ShouldPreserveNonQuestionContentBetweenQuestions()
        {
            // Arrange
            string input = @"\begin{ex}
  Câu 1 \choice{A}{B}{C}{D}
\end{ex}
\subsection*{Ghi chú quan trọng}
% Comment phải giữ nguyên không bị mất
\begin{ex}
  Câu 2 \choiceTF{Đ}{S}
\end{ex}";

            // Act
            string result = QuestionSortingService.ProcessSortQuestions(input, "01", out int sortedCount);

            // Assert
            Assert.Equal(2, sortedCount);
            Assert.Contains(@"\subsection*{Ghi chú quan trọng}", result);
            Assert.Contains(@"% Comment phải giữ nguyên không bị mất", result);
            Assert.Contains(@"\PhanI", result);
            Assert.Contains(@"\PhanII", result);
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
