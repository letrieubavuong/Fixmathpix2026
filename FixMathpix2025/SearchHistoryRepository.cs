using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FixMathpix2025
{
    /// <summary>
    /// Chịu trách nhiệm nạp và lưu lịch sử tìm kiếm/thay thế từ search_history.json.
    /// Đóng vai trò là One Source of Truth cho persistence của lịch sử tìm kiếm.
    /// </summary>
    public class SearchHistoryRepository
    {
        private readonly string _filePath;

        public string FilePath => _filePath;

        public SearchHistoryRepository(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _filePath = Path.Combine(appDataPath, "FixMathpix2025", "search_history.json");
            }
            else
            {
                _filePath = filePath;
            }
        }

        public virtual List<SearchReplaceItem> Load()
        {
            if (!File.Exists(_filePath))
            {
                return new List<SearchReplaceItem>();
            }

            string json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
            {
                return new List<SearchReplaceItem>();
            }

            var items = JsonSerializer.Deserialize<List<SearchReplaceItem>>(json);
            return items ?? new List<SearchReplaceItem>();
        }

        public virtual void Save(IEnumerable<SearchReplaceItem> history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            string directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(history, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
