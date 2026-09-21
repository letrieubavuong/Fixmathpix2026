using System.Collections.Generic;
using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class SettingsValidationTests
    {
        [Fact]
        public void ValidateShortcuts_WithDuplicateGestures_ShouldReject()
        {
            var shortcuts = new Dictionary<string, string>
            {
                { "ApplicationCommands.Save", "Ctrl+S" },
                { "local:CustomCommands.SpellCheck", "Ctrl+S" }
            };

            bool isValid = SettingsValidator.ValidateShortcuts(shortcuts, out string errorMessage);

            Assert.False(isValid);
            Assert.NotNull(errorMessage);
            Assert.Contains("Ctrl+S", errorMessage);
        }

        [Fact]
        public void ValidateHighlightingColors_WithInvalidHexCode_ShouldReject()
        {
            var colors = new Dictionary<string, string>
            {
                { "Keyword", "#FF0000" },
                { "Comment", "InvalidHexColor" }
            };

            bool isValid = SettingsValidator.ValidateHighlightingColors(colors, out string errorMessage);

            Assert.False(isValid);
            Assert.NotNull(errorMessage);
            Assert.Contains("Comment", errorMessage);
        }

        [Fact]
        public void ValidateSettings_WithValidInputs_ShouldPass()
        {
            var shortcuts = new Dictionary<string, string>
            {
                { "ApplicationCommands.Open", "Ctrl+O" },
                { "ApplicationCommands.Save", "Ctrl+S" }
            };

            var colors = new Dictionary<string, string>
            {
                { "Keyword", "#00FF00" },
                { "Comment", "#888888" }
            };

            bool isShortcutsValid = SettingsValidator.ValidateShortcuts(shortcuts, out string shortcutError);
            bool isColorsValid = SettingsValidator.ValidateHighlightingColors(colors, out string colorError);

            Assert.True(isShortcutsValid);
            Assert.Null(shortcutError);
            Assert.True(isColorsValid);
            Assert.Null(colorError);
        }
    }
}
