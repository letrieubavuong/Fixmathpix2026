using System.Collections.Generic;
using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class SettingsValidationTests
    {
        [Fact]
        public void DefaultEditorShortcuts_SingleSourceOfTruth_ShouldMatchDefaultEditorSettings()
        {
            var defaultShortcuts = DefaultEditorShortcuts.GetDefaultShortcuts();
            var settings = new EditorSettings();

            Assert.NotNull(defaultShortcuts);
            Assert.NotEmpty(defaultShortcuts);
            Assert.Equal(defaultShortcuts.Count, settings.Shortcuts.Count);
            foreach (var kvp in defaultShortcuts)
            {
                Assert.True(settings.Shortcuts.ContainsKey(kvp.Key));
                Assert.Equal(kvp.Value, settings.Shortcuts[kvp.Key]);
            }
        }

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

        [Fact]
        public void AtomicSettingsValidation_InvalidCandidate_MustNotMutateOriginal()
        {
            var originalSettings = new EditorSettings
            {
                FontFamily = "Consolas",
                FontSize = 14,
                IsAutoSaveEnabled = true,
                AutoSaveIntervalSeconds = 2
            };
            originalSettings.HighlightingColors["Keyword"] = "#FF0000";

            var invalidColors = new Dictionary<string, string>
            {
                { "Keyword", "NOT_A_COLOR" }
            };

            bool isValid = SettingsValidator.ValidateHighlightingColors(invalidColors, out _);

            Assert.False(isValid);
            // Verify original settings remained untouched
            Assert.Equal("Consolas", originalSettings.FontFamily);
            Assert.Equal(14, originalSettings.FontSize);
            Assert.True(originalSettings.IsAutoSaveEnabled);
            Assert.Equal("#FF0000", originalSettings.HighlightingColors["Keyword"]);
        }

        [Fact]
        public void ShortcutManagement_EmptyShortcut_ClearsGestureMapping()
        {
            var shortcuts = new Dictionary<string, string>
            {
                { "local:CustomCommands.ToggleComment", "" }
            };

            bool isValid = SettingsValidator.ValidateShortcuts(shortcuts, out string error);

            Assert.True(isValid);
            Assert.Null(error);
            Assert.Equal("", shortcuts["local:CustomCommands.ToggleComment"]);
        }
    }
}
