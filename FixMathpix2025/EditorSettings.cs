using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FixMathpix2025
{
    public class EditorSettings
    {
        public string FontFamily { get; set; } = "Consolas";
        public double FontSize { get; set; } = 14;
        public int AutoSaveIntervalSeconds { get; set; } = 2;
        public bool IsAutoSaveEnabled { get; set; } = true;
        public Dictionary<string, string> HighlightingColors { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Shortcuts { get; set; } = DefaultEditorShortcuts.GetDefaultShortcuts();

        private static string GetSettingsFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsDir = Path.Combine(appDataPath, "FixMathpix2025");
            return Path.Combine(settingsDir, "settings.json");
        }

        public void Save()
        {
            string filePath = GetSettingsFilePath();
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
        }

        public static EditorSettings Load()
        {
            try
            {
                string filePath = GetSettingsFilePath();
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<EditorSettings>(json) ?? new EditorSettings();
                }
            }
            catch (Exception)
            {
                // Lỗi khi đọc file, trả về cài đặt mặc định
            }

            return new EditorSettings();
        }
    }
}