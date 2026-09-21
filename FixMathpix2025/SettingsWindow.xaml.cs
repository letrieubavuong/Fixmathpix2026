using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace FixMathpix2025
{
    public class HighlightingColorViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private string _colorHex;
        public string ColorHex
        {
            get => _colorHex;
            set
            {
                if (_colorHex != value)
                {
                    _colorHex = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ShortcutSetting : INotifyPropertyChanged
    {
        public string CommandName { get; set; }
        public string FunctionName { get; set; }

        private string _keyGesture;
        public string KeyGesture
        {
            get => _keyGesture;
            set
            {
                if (_keyGesture != value)
                {
                    _keyGesture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeyGesture)));
                }
            }
        }

        private bool _isDuplicate;
        public bool IsDuplicate
        {
            get => _isDuplicate;
            set
            {
                if (_isDuplicate != value)
                {
                    _isDuplicate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDuplicate)));
                }
            }
        }

        private string _duplicateMessage;
        public string DuplicateMessage
        {
            get => _duplicateMessage;
            set
            {
                if (_duplicateMessage != value)
                {
                    _duplicateMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DuplicateMessage)));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class SettingsWindow : Window
    {
        private EditorSettings _currentSettings;
        private readonly MainWindow _mainWindow;
        private readonly SettingsRepository _settingsRepository;
        public ObservableCollection<HighlightingColorViewModel> HighlightingColors { get; set; }
        public ObservableCollection<ShortcutSetting> Shortcuts { get; set; }

        public SettingsWindow(MainWindow mainWindow, SettingsRepository settingsRepository = null)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _settingsRepository = settingsRepository ?? new SettingsRepository();
            this.Owner = mainWindow;
            LoadSettings();
            PopulateControls();
        }

        private void LoadSettings()
        {
            _currentSettings = _settingsRepository.Load();

            // Lấy danh sách màu từ định nghĩa tô sáng hiện tại
            var currentHighlighting = _mainWindow.textEditor.SyntaxHighlighting;
            if (currentHighlighting != null)
            {
                HighlightingColors = new ObservableCollection<HighlightingColorViewModel>();
                foreach (var color in currentHighlighting.NamedHighlightingColors)
                {
                    // Lấy màu đã lưu hoặc màu mặc định
                    if (!_currentSettings.HighlightingColors.TryGetValue(color.Name, out var savedColorHex))
                    {
                        var defaultBrush = color.Foreground?.GetBrush(null) as SolidColorBrush;
                        savedColorHex = defaultBrush?.Color.ToString() ?? "#FFFFFF";
                    }

                    HighlightingColors.Add(new HighlightingColorViewModel
                    {
                        Name = color.Name,
                        ColorHex = savedColorHex
                    });
                }
            }
            LoadShortcuts();
        }

        private void PopulateControls()
        {
            // Font Family
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.SelectedItem = new FontFamily(_currentSettings.FontFamily);

            // Font Size
            FontSizeComboBox.ItemsSource = new List<double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32 };
            FontSizeComboBox.Text = _currentSettings.FontSize.ToString();

            // Colors
            ColorItemsControl.ItemsSource = HighlightingColors;

            // Auto-save
            AutoSaveCheckBox.IsChecked = _currentSettings.IsAutoSaveEnabled;

            // Auto-save interval
            AutoSaveIntervalComboBox.ItemsSource = new List<int> { 1, 2, 5, 10, 30, 60 };
            AutoSaveIntervalComboBox.Text = _currentSettings.AutoSaveIntervalSeconds.ToString();

            // Shortcuts
            ShortcutListView.ItemsSource = Shortcuts;
        }

        private void LoadShortcuts()
        {
            Shortcuts = new ObservableCollection<ShortcutSetting>(GetDefaultShortcuts());

            // Ghi đè bằng các phím tắt đã lưu của người dùng
            foreach (var shortcut in Shortcuts)
            {
                if (_currentSettings.Shortcuts.TryGetValue(shortcut.CommandName, out var savedGesture))
                {
                    shortcut.KeyGesture = savedGesture;
                }
                shortcut.PropertyChanged += Shortcut_PropertyChanged;
            }
            ValidateAndHighlightDuplicates();
        }
        private void Shortcut_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShortcutSetting.KeyGesture))
            {
                ValidateAndHighlightDuplicates();
            }
        }

        private static readonly Dictionary<string, string> FunctionNames = new Dictionary<string, string>
        {
            { "ApplicationCommands.Open", "Mở tệp" },
            { "ApplicationCommands.Save", "Lưu tệp" },
            { "ApplicationCommands.Find", "Tìm kiếm" },
            { "ApplicationCommands.Replace", "Thay thế" },
            { "local:CustomCommands.Bold", "In đậm" },
            { "local:CustomCommands.Italic", "In nghiêng" },
            { "local:CustomCommands.Underline", "Gạch chân" },
            { "avalonedit:AvalonEditCommands.ConvertToUppercase", "Viết hoa" },
            { "local:CustomCommands.ToggleComment", "Chuyển đổi Ghi chú" },
            { "local:CustomCommands.MathMode", "Chế độ toán học ($...$)" },
            { "local:CustomCommands.InsertLoigiai", "Chèn Lời giải" },
            { "local:CustomCommands.SpellCheck", "Soát lỗi chính tả" },
            { "local:CustomCommands.CleanupText", "Dọn dẹp văn bản" }
        };

        private List<ShortcutSetting> GetDefaultShortcuts()
        {
            var list = new List<ShortcutSetting>();
            foreach (var kvp in DefaultEditorShortcuts.GetDefaultShortcuts())
            {
                string fnName = FunctionNames.TryGetValue(kvp.Key, out string name) ? name : kvp.Key;
                list.Add(new ShortcutSetting { CommandName = kvp.Key, FunctionName = fnName, KeyGesture = kvp.Value });
            }
            return list;
        }

        private bool TryApplySettings()
        {
            var candidate = new EditorSettings
            {
                FontFamily = (FontFamilyComboBox.SelectedItem is FontFamily selectedFont) ? selectedFont.Source : _currentSettings.FontFamily,
                FontSize = double.TryParse(FontSizeComboBox.Text, out double newSize) ? newSize : _currentSettings.FontSize,
                IsAutoSaveEnabled = AutoSaveCheckBox.IsChecked ?? true,
                AutoSaveIntervalSeconds = (int.TryParse(AutoSaveIntervalComboBox.Text, out int newInterval) && newInterval > 0) ? newInterval : _currentSettings.AutoSaveIntervalSeconds,
                HighlightingColors = HighlightingColors != null ? HighlightingColors.ToDictionary(c => c.Name, c => c.ColorHex) : new Dictionary<string, string>(_currentSettings.HighlightingColors),
                Shortcuts = Shortcuts != null ? Shortcuts.ToDictionary(s => s.CommandName, s => s.KeyGesture ?? "") : new Dictionary<string, string>(_currentSettings.Shortcuts)
            };

            // Validate colors
            if (!SettingsValidator.ValidateHighlightingColors(candidate.HighlightingColors, out string colorError))
            {
                MessageBox.Show(this, colorError, "Lỗi màu sắc", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate shortcuts
            if (!SettingsValidator.ValidateShortcuts(candidate.Shortcuts, out string shortcutError))
            {
                MessageBox.Show(this, shortcutError, "Lỗi phím tắt trùng lặp", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            ValidateAndHighlightDuplicates();
            if (Shortcuts != null && Shortcuts.Any(s => s.IsDuplicate))
            {
                var duplicates = Shortcuts.Where(s => s.IsDuplicate)
                                          .GroupBy(s => s.KeyGesture)
                                          .Where(g => g.Count() > 1);

                var errorBuilder = new System.Text.StringBuilder();
                errorBuilder.AppendLine("Không thể lưu cài đặt do có phím tắt bị trùng lặp:");
                foreach (var group in duplicates)
                {
                    var functionNames = group.Select(g => $"'{g.FunctionName}'");
                    errorBuilder.AppendLine($"  - Phím tắt '{group.Key}' được gán cho các chức năng: {string.Join(", ", functionNames)}");
                }
                MessageBox.Show(this, errorBuilder.ToString(), "Lỗi phím tắt trùng lặp", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validated! Commit candidate to current settings
            _currentSettings = candidate;
            _mainWindow.ApplySettings(_currentSettings);
            return true;
        }

        private bool SaveSettings()
        {
            if (!TryApplySettings()) return false;
            _settingsRepository.Save(_currentSettings);
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SaveSettings())
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu cài đặt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TryApplySettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng cài đặt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ShortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Lấy các phím bổ trợ (Ctrl, Shift, Alt)
            ModifierKeys modifiers = Keyboard.Modifiers;
            Key key = e.Key;

            // Xử lý các phím hệ thống đặc biệt
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Bỏ qua nếu chỉ nhấn phím bổ trợ
            if (key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Tạo chuỗi phím tắt và cập nhật TextBox
            var converter = new KeyGestureConverter();
            textBox.Text = converter.ConvertToString(new KeyGesture(key, modifiers));
        }

        private void ValidateAndHighlightDuplicates()
        {
            if (Shortcuts == null) return;

            // Trước tiên, đặt lại trạng thái trùng lặp cho tất cả các mục
            foreach (var s in Shortcuts)
            {
                s.IsDuplicate = false;
                s.DuplicateMessage = null;
            }

            // Nhóm các phím tắt theo tổ hợp phím để tìm các phím bị trùng
            var gestureGroups = Shortcuts
                .Where(s => !string.IsNullOrWhiteSpace(s.KeyGesture))
                .GroupBy(s => s.KeyGesture);

            foreach (var group in gestureGroups)
            {
                if (group.Count() > 1)
                {
                    // Tổ hợp phím này bị trùng lặp
                    var allFunctionNames = group.Select(s => $"'{s.FunctionName}'").ToList();
                    foreach (var shortcut in group)
                    {
                        shortcut.IsDuplicate = true;
                        var otherFunctionNames = allFunctionNames.Where(name => name != $"'{shortcut.FunctionName}'");
                        shortcut.DuplicateMessage = $"Phím tắt này cũng được sử dụng cho: {string.Join(", ", otherFunctionNames)}";
                    }
                }
            }
        }
    }

    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hexColor)
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
                }
                catch
                {
                    return Brushes.Transparent;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public static class SettingsValidator
    {
        public static bool ValidateHighlightingColors(IEnumerable<KeyValuePair<string, string>> colors, out string errorMessage)
        {
            errorMessage = null;
            if (colors != null)
            {
                foreach (var kvp in colors)
                {
                    try
                    {
                        ColorConverter.ConvertFromString(kvp.Value);
                    }
                    catch
                    {
                        errorMessage = $"Mã màu không hợp lệ cho '{kvp.Key}': {kvp.Value}";
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ValidateShortcuts(IEnumerable<KeyValuePair<string, string>> shortcuts, out string errorMessage)
        {
            errorMessage = null;
            if (shortcuts != null)
            {
                var gestureMap = new Dictionary<string, List<string>>();
                foreach (var kvp in shortcuts)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        if (!gestureMap.ContainsKey(kvp.Value))
                        {
                            gestureMap[kvp.Value] = new List<string>();
                        }
                        gestureMap[kvp.Value].Add(kvp.Key);
                    }
                }

                var duplicates = gestureMap.Where(kvp => kvp.Value.Count > 1).ToList();
                if (duplicates.Any())
                {
                    var sb = new StringBuilder("Phím tắt trùng lặp: ");
                    foreach (var dup in duplicates)
                    {
                        sb.Append($"[{dup.Key}: {string.Join(", ", dup.Value)}] ");
                    }
                    errorMessage = sb.ToString().Trim();
                    return false;
                }
            }
            return true;
        }
    }
}