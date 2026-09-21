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
    /// <summary>
    /// Interaction logic for SpellingCorrectionManagerWindow.xaml
    /// </summary>
    public partial class SpellingCorrectionManagerWindow : Window
    {
        private ObservableCollection<CorrectionRule> _corrections;
        private readonly SpellingRepository _spellingRepository;

        public SpellingCorrectionManagerWindow(SpellingRepository spellingRepository = null)
        {
            InitializeComponent();
            _spellingRepository = spellingRepository ?? new SpellingRepository();
            _corrections = new ObservableCollection<CorrectionRule>();
            LoadCorrections();

            // Sử dụng ICollectionView để lọc
            ICollectionView correctionsView = CollectionViewSource.GetDefaultView(_corrections);
            CorrectionsDataGrid.ItemsSource = correctionsView;
        }

        private void LoadCorrections()
        {
            _corrections.Clear();
            try
            {
                var rules = _spellingRepository.Load();
                foreach (var rule in rules)
                {
                    _corrections.Add(rule);
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
                _spellingRepository.Save(_corrections);
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