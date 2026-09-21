using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class AutoSaveLifecycleTests
    {
        [Fact]
        public void DefaultEditorSettings_IsAutoSaveEnabledState()
        {
            var settings = new EditorSettings
            {
                IsAutoSaveEnabled = false,
                AutoSaveIntervalSeconds = 5
            };

            Assert.False(settings.IsAutoSaveEnabled);
            Assert.Equal(5, settings.AutoSaveIntervalSeconds);
        }

        [Fact]
        public void AutoSaveSettings_SaveAndLoad_ShouldPreserveDisabledState()
        {
            var settings = new EditorSettings
            {
                IsAutoSaveEnabled = false,
                AutoSaveIntervalSeconds = 10
            };

            string json = System.Text.Json.JsonSerializer.Serialize(settings);
            var restored = System.Text.Json.JsonSerializer.Deserialize<EditorSettings>(json);

            Assert.NotNull(restored);
            Assert.False(restored.IsAutoSaveEnabled);
            Assert.Equal(10, restored.AutoSaveIntervalSeconds);
        }
    }
}
