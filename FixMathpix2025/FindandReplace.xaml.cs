using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace FixMathpix2025
{
    public partial class FindAndReplace : Window
    {
        private readonly TextEditor _textEditor;
        private readonly ObservableCollection<SearchReplaceItem> _searchHistory;
        private readonly SearchHistoryRepository _searchHistoryRepository;

        public FindAndReplace(TextEditor textEditor, SearchHistoryRepository searchHistoryRepository = null)
        {
            InitializeComponent();
            _textEditor = textEditor;
            _searchHistoryRepository = searchHistoryRepository ?? new SearchHistoryRepository();
            this.Owner = Application.Current.MainWindow;

            _searchHistory = LoadHistory();
            SearchHistoryGrid.ItemsSource = _searchHistory;

            // Thêm sự kiện để xử lý độ mờ khi cửa sổ mất/nhận focus
            this.Activated += FindAndReplace_Activated;
            this.Deactivated += FindAndReplace_Deactivated;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private ISearchStrategy CreateSearchStrategy()
        {
            try
            {
                return SearchStrategyFactory.Create(
                    FindComboBox.Text,
                    !MatchCaseCheckBox.IsChecked.GetValueOrDefault(),
                    WholeWordCheckBox.IsChecked.GetValueOrDefault(),
                    RegexCheckBox.IsChecked.GetValueOrDefault() ? SearchMode.RegEx : SearchMode.Normal
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi biểu thức tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            var strategy = CreateSearchStrategy();
            if (strategy == null) return;

            // Logic tìm kiếm không thay đổi
            var result = strategy.FindNext(_textEditor.Document, _textEditor.SelectionStart + _textEditor.SelectionLength, _textEditor.Document.TextLength - (_textEditor.SelectionStart + _textEditor.SelectionLength));
            if (result == null) // wrap around
                result = strategy.FindNext(_textEditor.Document, 0, _textEditor.SelectionStart);

            if (result != null)
            {
                _textEditor.Select(result.Offset, result.Length);
                var loc = _textEditor.Document.GetLocation(result.Offset);
                _textEditor.ScrollTo(loc.Line, loc.Column);
            }
            else
            {
                MessageBox.Show("Không tìm thấy kết quả.", "Thông báo");
            }
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            var strategy = CreateSearchStrategy();
            if (strategy == null) return;

            // If something is selected and it matches the search text, replace it
            if (_textEditor.SelectionLength > 0)
            {
                var result = strategy.FindNext(_textEditor.Document, _textEditor.SelectionStart, _textEditor.SelectionLength);
                if (result != null && result.Offset == _textEditor.SelectionStart && result.Length == _textEditor.SelectionLength)
                {
                    string replacementPattern = ConvertToDotNetRegexReplacement(ReplaceComboBox.Text);
                    string actualReplacement = result.ReplaceWith(replacementPattern);
                    _textEditor.Document.Replace(result.Offset, result.Length, actualReplacement);
                }
            }
            // Find the next occurrence
            FindNextButton_Click(sender, e); // Lưu ý: Gọi lại hàm async từ một hàm async khác
        }

        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            var strategy = CreateSearchStrategy();
            if (strategy == null) return;

            int count = 0;
            try
            {
                _textEditor.Document.BeginUpdate();
                var results = strategy.FindAll(_textEditor.Document, 0, _textEditor.Document.TextLength).ToList();
                // Replace from end to start to keep offsets valid
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    var result = results[i];
                    string replacementPattern = ConvertToDotNetRegexReplacement(ReplaceComboBox.Text);
                    string actualReplacement = result.ReplaceWith(replacementPattern);
                    _textEditor.Document.Replace(result.Offset, result.Length, actualReplacement);
                    count++;
                }
            }
            finally
            {
                _textEditor.Document.EndUpdate();
            }
            MessageBox.Show($"Đã thay thế {count} kết quả.", "Hoàn tất");
        }
        
        /// <summary>
        /// Chuyển đổi các tham chiếu ngược từ dạng \1, \2 sang dạng $1, $2 của .NET.
        /// </summary>
        private string ConvertToDotNetRegexReplacement(string pattern)
        {
            // Regex này tìm một dấu \ theo sau là một chữ số, nhưng không phải là \\ (hai dấu)
            // và thay thế nó bằng $ và chữ số đó.
            return Regex.Replace(pattern, @"(?<!\\)\\(?<digit>\d)", "${digit}");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FindComboBox.Text)) return;

            var newItem = new SearchReplaceItem
            {
                FindText = FindComboBox.Text,
                ReplaceText = ReplaceComboBox.Text,
                MatchCase = MatchCaseCheckBox.IsChecked.GetValueOrDefault(),
                WholeWord = WholeWordCheckBox.IsChecked.GetValueOrDefault(),
                UseRegex = RegexCheckBox.IsChecked.GetValueOrDefault()
            };

            // Avoid duplicates
            if (!_searchHistory.Any(i => i.FindText == newItem.FindText && i.ReplaceText == newItem.ReplaceText))
            {
                _searchHistory.Add(newItem);
                SaveHistory();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is SearchReplaceItem item)
            {
                _searchHistory.Remove(item);
                SaveHistory();
            }
        }

        private void SearchHistoryGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchHistoryGrid.SelectedItem is SearchReplaceItem item)
            {
                FindComboBox.Text = item.FindText;
                ReplaceComboBox.Text = item.ReplaceText;
                MatchCaseCheckBox.IsChecked = item.MatchCase;
                WholeWordCheckBox.IsChecked = item.WholeWord;
                RegexCheckBox.IsChecked = item.UseRegex;
            }
        }

        private ObservableCollection<SearchReplaceItem> LoadHistory()
        {
            try
            {
                var items = _searchHistoryRepository.Load();
                return new ObservableCollection<SearchReplaceItem>(items);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải lịch sử tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<SearchReplaceItem>();
            }
        }

        private void SaveHistory()
        {
            try
            {
                _searchHistoryRepository.Save(_searchHistory);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu lịch sử tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FindAndReplace_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1.0; // Đặt độ mờ về 100% khi cửa sổ nhận focus
        }

        private void FindAndReplace_Deactivated(object sender, EventArgs e)
        {
            this.Opacity = 0.7; // Giảm độ mờ xuống 70% khi cửa sổ mất focus
        }
    }
}