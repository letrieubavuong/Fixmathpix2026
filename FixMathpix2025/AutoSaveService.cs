using System;
using System.IO;

namespace FixMathpix2025
{
    /// <summary>
    /// Quản lý vòng đời lưu tự động (AutoSave) và khôi phục (Recovery).
    /// Sử dụng AutoSaveRegistrationTracker để quản lý trạng thái đăng ký.
    /// </summary>
    public class AutoSaveService
    {
        private readonly AutoSaveRegistrationTracker _tracker = new AutoSaveRegistrationTracker();
        private readonly string _autoSaveFilePath;

        public string AutoSaveFilePath => _autoSaveFilePath;
        public bool IsRegistered => _tracker.IsRegistered;
        public bool IsTimerRunning => _tracker.IsTimerRunning;

        public AutoSaveService(string autoSaveFilePath = null)
        {
            _autoSaveFilePath = autoSaveFilePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FixMathpix2025",
                "autosave.tex");
        }

        public void Enable(Action subscribeTextChanged, Action startTimer)
        {
            _tracker.Enable(subscribeTextChanged, startTimer);
        }

        public void Disable(Action unsubscribeTextChanged, Action stopTimer)
        {
            _tracker.Disable(unsubscribeTextChanged, stopTimer);
        }

        public bool SaveSnapshot(string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(_autoSaveFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(_autoSaveFilePath, content ?? string.Empty);
                return true;
            }
            catch (Exception)
            {
                // Âm thầm xử lý ngoại lệ tự động lưu
                return false;
            }
        }

        public bool HasAutoSaveSnapshot()
        {
            return File.Exists(_autoSaveFilePath);
        }

        public string ReadAutoSaveSnapshot()
        {
            if (HasAutoSaveSnapshot())
            {
                return File.ReadAllText(_autoSaveFilePath);
            }
            return null;
        }

        public void ClearAutoSaveSnapshot()
        {
            if (HasAutoSaveSnapshot())
            {
                try
                {
                    File.Delete(_autoSaveFilePath);
                }
                catch (Exception)
                {
                }
            }
        }

        public void OnApplicationClosing(bool isAutoSaveEnabled)
        {
            if (isAutoSaveEnabled)
            {
                ClearAutoSaveSnapshot();
            }
        }
    }
}
