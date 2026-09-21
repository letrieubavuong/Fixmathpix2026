using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class AutoSaveLifecycleTests
    {
        [Fact]
        public void AutoSaveTracker_StartupDisabled_IsSafeAndNotRegistered()
        {
            var tracker = new AutoSaveRegistrationTracker();

            Assert.False(tracker.IsRegistered);
            Assert.False(tracker.IsTimerRunning);
        }

        [Fact]
        public void AutoSaveTracker_Enable_RegistersAndStartsTimerOnlyOnce()
        {
            var tracker = new AutoSaveRegistrationTracker();
            int subscribeCount = 0;
            int startTimerCount = 0;

            tracker.Enable(() => subscribeCount++, () => startTimerCount++);

            Assert.True(tracker.IsRegistered);
            Assert.True(tracker.IsTimerRunning);
            Assert.Equal(1, subscribeCount);
            Assert.Equal(1, startTimerCount);

            // Enable second time
            tracker.Enable(() => subscribeCount++, () => startTimerCount++);

            Assert.True(tracker.IsRegistered);
            Assert.True(tracker.IsTimerRunning);
            Assert.Equal(1, subscribeCount); // Must NOT duplicate subscription
            Assert.Equal(2, startTimerCount); // Timer restart is allowed
        }

        [Fact]
        public void AutoSaveTracker_Disable_UnregistersAndStopsTimerSafely()
        {
            var tracker = new AutoSaveRegistrationTracker();
            int unsubscribeCount = 0;
            int stopTimerCount = 0;

            // Enable first
            tracker.Enable(null, null);
            Assert.True(tracker.IsRegistered);

            // Disable
            tracker.Disable(() => unsubscribeCount++, () => stopTimerCount++);

            Assert.False(tracker.IsRegistered);
            Assert.False(tracker.IsTimerRunning);
            Assert.Equal(1, unsubscribeCount);
            Assert.Equal(1, stopTimerCount);

            // Disable second time
            tracker.Disable(() => unsubscribeCount++, () => stopTimerCount++);

            Assert.False(tracker.IsRegistered);
            Assert.False(tracker.IsTimerRunning);
            Assert.Equal(1, unsubscribeCount); // Must NOT duplicate unsubscription
            Assert.Equal(2, stopTimerCount);
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
