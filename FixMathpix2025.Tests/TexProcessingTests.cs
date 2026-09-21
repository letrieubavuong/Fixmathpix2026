using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class TexProcessingTests
    {
        [Fact]
        public void FixMathtype_ShouldCleanWhitespaceAndExponentSubscriptBrackets()
        {
            // Arrange
            string input = @"  \^{2} + _{x} + {  abc  } + [  xyz  ] .}
  \s+ test";

            // Act
            string result = TexProcessingService.FixMathtype(input);

            // Assert
            Assert.Contains("^2", result);
            Assert.Contains("_x", result);
            Assert.Contains("{abc", result);
            Assert.Contains("[xyz", result);
            Assert.DoesNotContain(@"\^{", result);
            Assert.DoesNotContain(@"_{", result);
        }

        [Fact]
        public void FixMathtype_ShouldHandleEmptyAndNullStringsSafely()
        {
            Assert.Null(TexProcessingService.FixMathtype(null));
            Assert.Equal("", TexProcessingService.FixMathtype(""));
        }
    }
}
