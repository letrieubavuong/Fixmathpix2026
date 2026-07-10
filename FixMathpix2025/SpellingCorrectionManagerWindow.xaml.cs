using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows;
using System.Xml;

namespace FixMathpix2025
{
    // Data model for a single correction rule
    public class CorrectionRule : INotifyPropertyChanged
    {
        private string _findText;
        public string FindText
        {
            get => _findText;
            set
            {
                if (_findText != value)
                {
                    _findText = value;
                    OnPropertyChanged(nameof(FindText));
                }
            }
        }

        private string _replaceText;
        public string ReplaceText
        {
            get => _replaceText;
            set
            {
                if (_replaceText != value)
                {
                    _replaceText = value;
                    OnPropertyChanged(nameof(ReplaceText));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaction logic for SpellingCorrectionManagerWindow.xaml
    /// </summary>
    public partial class SpellingCorrectionManagerWindow : Window
    {
        private ObservableCollection<CorrectionRule> _corrections;
        private readonly string _userSpellingCorrectionsFilePath;

        public SpellingCorrectionManagerWindow()
        {
            InitializeComponent();
            _userSpellingCorrectionsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FixMathpix2025",
                "SpellingCorrections.xml"
            );
            _corrections = new ObservableCollection<CorrectionRule>();
            LoadCorrections();

            // Sử dụng ICollectionView để lọc
            ICollectionView correctionsView = CollectionViewSource.GetDefaultView(_corrections);
            CorrectionsDataGrid.ItemsSource = correctionsView;
        }

        private void LoadCorrections()
        {
            _corrections.Clear();
            XmlDocument doc = new XmlDocument();

            try
            {
                // Try to load from user-specific file first
                if (File.Exists(_userSpellingCorrectionsFilePath))
                {
                    doc.Load(_userSpellingCorrectionsFilePath);
                }
                else
                {
                    // Fallback to embedded resource if user file doesn't exist
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string resourceName = "FixMathpix2025.SpellingCorrections.xml";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                        {
                            MessageBox.Show("Không tìm thấy tệp SpellingCorrections.xml nhúng.", "Lỗi tải", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        doc.Load(stream);
                    }
                }

                foreach (XmlNode node in doc.SelectNodes("//Correction"))
                {
                    string find = node.Attributes["find"]?.Value;
                    string replace = node.Attributes["replace"]?.Value;
                    if (!string.IsNullOrEmpty(find) && replace != null)
                    {
                        _corrections.Add(new CorrectionRule { FindText = find, ReplaceText = replace });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải quy tắc sửa lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveCorrections()
        {
            try
            {
                string directory = Path.GetDirectoryName(_userSpellingCorrectionsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                XmlDocument doc = new XmlDocument();
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(xmlDeclaration);

                XmlElement root = doc.CreateElement("Corrections");
                doc.AppendChild(root);

                foreach (var rule in _corrections)
                {
                    XmlElement correctionNode = doc.CreateElement("Correction");
                    correctionNode.SetAttribute("find", rule.FindText);
                    correctionNode.SetAttribute("replace", rule.ReplaceText);
                    root.AppendChild(correctionNode);
                }

                doc.Save(_userSpellingCorrectionsFilePath);
                MessageBox.Show("Đã lưu các quy tắc sửa lỗi thành công!", "Lưu thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi lưu quy tắc sửa lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string find = FindTextBox.Text.Trim();
            string replace = ReplaceTextBox.Text; // Giữ nguyên, không Trim()

            if (string.IsNullOrEmpty(find))
            {
                MessageBox.Show("Ô 'Từ sai' không được để trống.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check for duplicates
            if (_corrections.Any(r => r.FindText.Equals(find, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Quy tắc này đã tồn tại. Vui lòng sửa hoặc xóa quy tắc hiện có.", "Lỗi trùng lặp", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _corrections.Add(new CorrectionRule { FindText = find, ReplaceText = replace });
            FindTextBox.Clear();
            ReplaceTextBox.Clear();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (CorrectionsDataGrid.SelectedItem is CorrectionRule selectedRule)
            {
                string find = FindTextBox.Text.Trim();
                string replace = ReplaceTextBox.Text;

                if (string.IsNullOrEmpty(find))
                {
                    MessageBox.Show("Ô 'Từ sai' không được để trống.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for duplicates, excluding the rule being edited
                if (_corrections.Any(r => r != selectedRule && r.FindText.Equals(find, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Quy tắc này đã tồn tại. Vui lòng sửa hoặc xóa quy tắc hiện có.", "Lỗi trùng lặp", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selectedRule.FindText = find;
                selectedRule.ReplaceText = replace;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một quy tắc để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CorrectionsDataGrid.SelectedItem is CorrectionRule selectedRule)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa quy tắc '{selectedRule.FindText}' không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _corrections.Remove(selectedRule);
                    FindTextBox.Clear();
                    ReplaceTextBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một quy tắc để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCorrections();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CorrectionsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CorrectionsDataGrid.SelectedItem is CorrectionRule selectedRule)
            {
                FindTextBox.Text = selectedRule.FindText;
                ReplaceTextBox.Text = selectedRule.ReplaceText;
            }
            else
            {
                FindTextBox.Clear();
                ReplaceTextBox.Clear();
            }
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Lấy ICollectionView từ ItemsSource của DataGrid
            ICollectionView view = CollectionViewSource.GetDefaultView(CorrectionsDataGrid.ItemsSource);
            if (view != null)
            {
                // Gán hoặc xóa bộ lọc
                if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    view.Filter = null; // Xóa bộ lọc nếu ô tìm kiếm trống
                }
                else
                {
                    view.Filter = item =>
                    {
                        var rule = item as CorrectionRule;
                        return rule.FindText.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                               rule.ReplaceText.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            }
        }
    }
}