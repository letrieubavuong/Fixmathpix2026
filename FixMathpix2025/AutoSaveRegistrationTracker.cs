using System;

namespace FixMathpix2025
{
    public class AutoSaveRegistrationTracker
    {
        public bool IsRegistered { get; private set; }
        public bool IsTimerRunning { get; private set; }

        public void Enable(Action subscribeAction, Action startTimerAction)
        {
            if (!IsRegistered)
            {
                subscribeAction?.Invoke();
                IsRegistered = true;
            }
            startTimerAction?.Invoke();
            IsTimerRunning = true;
        }

        public void Disable(Action unsubscribeAction, Action stopTimerAction)
        {
            if (IsRegistered)
            {
                unsubscribeAction?.Invoke();
                IsRegistered = false;
            }
            stopTimerAction?.Invoke();
            IsTimerRunning = false;
        }
    }
}
