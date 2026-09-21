using System.Collections.Generic;

namespace FixMathpix2025
{
    /// <summary>
    /// Model dữ liệu thuần cho các cài đặt của trình soạn thảo.
    /// Không chứa logic nạp/lưu hay đường dẫn file đĩa.
    /// </summary>
    public class EditorSettings
    {
        public string FontFamily { get; set; } = "Consolas";
        public double FontSize { get; set; } = 14;
        public int AutoSaveIntervalSeconds { get; set; } = 2;
        public bool IsAutoSaveEnabled { get; set; } = true;
        public Dictionary<string, string> HighlightingColors { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Shortcuts { get; set; } = DefaultEditorShortcuts.GetDefaultShortcuts();
    }
}