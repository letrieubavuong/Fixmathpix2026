using System.Collections.Generic;
using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class SpellCheckTests
    {
        [Fact]
        public void ApplySpellingCorrections_ShouldNotCascadeReplacements()
        {
            // Arrange
            // Rule 1: teh -> the
            // Rule 2: the -> tên
            // Input: "teh"
            // Non-cascading expected result: "the" (Rule 1 result must NOT be further replaced by Rule 2)
            var corrections = new Dictionary<string, string>
            {
                { "teh", "the" },
                { "the", "tên" }
            };

            string input = "teh cat and the dog";

            // Act
            string result = SpellingService.ApplySpellingCorrections(input, corrections, out int replacementsCount);

            // Assert
            Assert.Equal("the cat and tên dog", result);
            Assert.Equal(2, replacementsCount);
        }

        [Fact]
        public void ApplySpellingCorrections_ShouldPrioritizeLongerRulesOverShorterRules()
        {
            // Arrange
            var corrections = new Dictionary<string, string>
            {
                { "bài tập", "Bài tập lớn" },
                { "bài", "Bài học" }
            };

            string input = "đây là bài tập mới";

            // Act
            string result = SpellingService.ApplySpellingCorrections(input, corrections, out int replacementsCount);

            // Assert
            Assert.Equal("đây là Bài tập lớn mới", result);
            Assert.Equal(1, replacementsCount);
        }

        [Fact]
        public void ApplySpellingCorrections_ShouldEnforceWholeWordMatching()
        {
            var corrections = new Dictionary<string, string>
            {
                { "bài", "Bài" }
            };

            string input = "bài tập và bài_học và tam_bài";

            string result = SpellingService.ApplySpellingCorrections(input, corrections, out int count);

            // Should replace standalone "bài", but not inside "bài_học" or "tam_bài"
            Assert.Contains("Bài tập", result);
        }
    }
}
