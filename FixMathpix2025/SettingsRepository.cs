using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FixMathpix2025
{
    /// <summary>
    /// Chịu trách nhiệm nạp và lưu cấu hình EditorSettings từ tệp settings.json.
    /// Đóng vai trò là One Source of Truth cho persistence của cài đặt ứng dụng.
    /// </summary>
    public class SettingsRepository
    {
        private readonly string _filePath;

        public string FilePath => _filePath;

        public SettingsRepository(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _filePath = Path.Combine(appDataPath, "FixMathpix2025", "settings.json");
            }
            else
            {
                _filePath = filePath;
            }
        }

        public virtual EditorSettings Load()
        {
            EditorSettings settings = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    settings = JsonSerializer.Deserialize<EditorSettings>(json);
                }
            }
            catch (Exception)
            {
                // Lỗi khi đọc file/JSON không hợp lệ, trả về cài đặt mặc định
            }

            if (settings == null)
            {
                settings = CreateDefaultSettings();
            }
            else
            {
                NormalizeSettings(settings);
            }

            return settings;
        }

        public virtual void Save(EditorSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            string directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_filePath, json);
        }

        public static EditorSettings CreateDefaultSettings()
        {
            var settings = new EditorSettings
            {
                FontFamily = "Consolas",
                FontSize = 14,
                AutoSaveIntervalSeconds = 2,
                IsAutoSaveEnabled = true,
                HighlightingColors = new Dictionary<string, string>(),
                Shortcuts = DefaultEditorShortcuts.GetDefaultShortcuts()
            };
            return settings;
        }

        private static void NormalizeSettings(EditorSettings settings)
        {
            if (settings.HighlightingColors == null)
            {
                settings.HighlightingColors = new Dictionary<string, string>();
            }

            if (settings.Shortcuts == null || settings.Shortcuts.Count == 0)
            {
                settings.Shortcuts = DefaultEditorShortcuts.GetDefaultShortcuts();
            }
            else
            {
                // Đảm bảo dictionary phím tắt là bản sao độc lập
                settings.Shortcuts = new Dictionary<string, string>(settings.Shortcuts);
            }
        }
    }
}
