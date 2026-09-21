using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FixMathpix2025
{
    public class ExternalFileChangedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public string NewContent { get; }

        public ExternalFileChangedEventArgs(string filePath, string newContent)
        {
            FilePath = filePath;
            NewContent = newContent;
        }
    }

    public class ExternalFileChangeMonitor : IDisposable
    {
        private readonly FileSystemWatcher _fileWatcher;
        private readonly System.Timers.Timer _debounceTimer;
        private string _watchedFilePath;
        private DateTime _lastFileWriteTime;
        private DateTime _expectedSaveWriteTimeUtc;
        private string _expectedContentHash;
        private bool _isSaving;

        public event EventHandler<ExternalFileChangedEventArgs> ExternalFileChanged;
        public Action<Action> DispatcherInvoke { get; set; }

        public bool IsSaving
        {
            get => _isSaving;
            set => _isSaving = value;
        }

        public string ExpectedContentHash => _expectedContentHash;
        public DateTime ExpectedSaveWriteTimeUtc => _expectedSaveWriteTimeUtc;
        public DateTime LastFileWriteTime => _lastFileWriteTime;

        public ExternalFileChangeMonitor()
        {
            _fileWatcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            _fileWatcher.Changed += OnFileChanged;

            _debounceTimer = new System.Timers.Timer(300)
            {
                AutoReset = false
            };
            _debounceTimer.Elapsed += (s, e) => ProcessFileChangeNotification();
        }

        public void StartWatching(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                StopWatching();
                return;
            }

            try
            {
                _watchedFilePath = filePath;
                _fileWatcher.Path = Path.GetDirectoryName(filePath);
                _fileWatcher.Filter = Path.GetFileName(filePath);
                _lastFileWriteTime = File.GetLastWriteTimeUtc(filePath);
                _fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                _fileWatcher.EnableRaisingEvents = false;
                Console.WriteLine("Không thể bắt đầu theo dõi tệp: " + ex.Message);
            }
        }

        public void StopWatching()
        {
            _fileWatcher.EnableRaisingEvents = false;
            _watchedFilePath = null;
        }

        public void NotifySaveStarting(string content)
        {
            _isSaving = true;
            _expectedContentHash = ComputeHash(content);
        }

        public void NotifySaveCompleted(string filePath, DateTime writeTimeUtc)
        {
            _lastFileWriteTime = writeTimeUtc;
            _expectedSaveWriteTimeUtc = writeTimeUtc;
            _isSaving = false;
        }

        public void ClearSaveFlags()
        {
            _isSaving = false;
            _expectedContentHash = null;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(_watchedFilePath) || !e.FullPath.Equals(_watchedFilePath, StringComparison.OrdinalIgnoreCase))
                return;

            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        public bool EvaluateExternalChange(string currentEditorText, bool isEditorModified, out string diskContent)
        {
            diskContent = null;
            if (string.IsNullOrEmpty(_watchedFilePath) || !File.Exists(_watchedFilePath))
                return false;

            DateTime currentWriteTime = File.GetLastWriteTimeUtc(_watchedFilePath);
            if (_isSaving || currentWriteTime == _expectedSaveWriteTimeUtc)
            {
                return false;
            }

            diskContent = File.ReadAllText(_watchedFilePath);
            string diskHash = ComputeHash(diskContent);

            if (_expectedContentHash != null && diskHash == _expectedContentHash)
            {
                return false;
            }

            string editorHash = ComputeHash(currentEditorText);
            if (!isEditorModified && diskHash == editorHash)
            {
                return false;
            }

            _lastFileWriteTime = currentWriteTime;
            _expectedSaveWriteTimeUtc = currentWriteTime;
            return true;
        }

        public Func<string> GetEditorTextCallback { get; set; }
        public Func<bool> GetIsModifiedCallback { get; set; }

        public void ProcessFileChangeNotification()
        {
            if (DispatcherInvoke != null)
            {
                DispatcherInvoke(() => ExecuteProcessChange());
            }
            else
            {
                ExecuteProcessChange();
            }
        }

        private void ExecuteProcessChange()
        {
            try
            {
                string editorText = GetEditorTextCallback?.Invoke() ?? string.Empty;
                bool isModified = GetIsModifiedCallback?.Invoke() ?? false;

                if (EvaluateExternalChange(editorText, isModified, out string diskContent))
                {
                    ExternalFileChanged?.Invoke(this, new ExternalFileChangedEventArgs(_watchedFilePath, diskContent));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kiểm tra thay đổi tệp: " + ex.Message);
            }
        }

        public static string ComputeHash(string text)
        {
            if (text == null) return string.Empty;
            using (var md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                byte[] hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public void Dispose()
        {
            _fileWatcher?.Dispose();
            _debounceTimer?.Dispose();
        }
    }
}
