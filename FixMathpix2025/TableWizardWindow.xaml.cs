using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using ICSharpCode.AvalonEdit;
using System.Windows.Controls;

namespace FixMathpix2025
{
    /// <summary>
    /// Interaction logic for TableWizardWindow.xaml
    /// </summary>
    public partial class TableWizardWindow : Window
    {
        public string GeneratedTableLatex { get; private set; }
        private readonly List<TextBox> cellTextBoxes = new List<TextBox>();
        private readonly TextEditor _textEditor;

        public TableWizardWindow(TextEditor editor)
        {
            InitializeComponent();
            _textEditor = editor;

            // Thiết lập giá trị mặc định
            ColumnCountComboBox.SelectedIndex = 0; // 2 cột
            RowCountComboBox.SelectedIndex = 4;    // 6 hàng (ứng với index 4)
            CreateTableContentGrid(); // Gọi lần đầu để khởi tạo grid
        }


        private void ColumnCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Chỉ thực thi logic khi cửa sổ đã được tải hoàn toàn
            if (!this.IsLoaded) return;

            if (int.TryParse(((ComboBoxItem)ColumnCountComboBox.SelectedItem).Content.ToString(), out int columnCount))
            {
                // Tự động tạo cấu trúc cột mặc định: c|M|M...
                var columnSpec = new StringBuilder("c");
                for (int i = 1; i < columnCount; i++)
                {
                    columnSpec.Append("|M");
                }
                ColumnSpecTextBox.Text = columnSpec.ToString();
            }
            CreateTableContentGrid();
        }

        private void RowCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Chỉ thực thi logic khi cửa sổ đã được tải hoàn toàn
            if (!this.IsLoaded) return;
            CreateTableContentGrid();
        }

        private void CreateTableContentGrid()
        {
            // Đảm bảo các control đã được khởi tạo
            if (TableContentGrid == null || ColumnCountComboBox == null || RowCountComboBox == null) return;

            TableContentGrid.Children.Clear();
            TableContentGrid.RowDefinitions.Clear();
            TableContentGrid.ColumnDefinitions.Clear();
            cellTextBoxes.Clear();

            if (!int.TryParse(((ComboBoxItem)ColumnCountComboBox.SelectedItem).Content.ToString(), out int columnCount) ||
                !int.TryParse(((ComboBoxItem)RowCountComboBox.SelectedItem).Content.ToString(), out int rowCount))
            {
                return;
            }

            // Định nghĩa số hàng và cột cho Grid
            for (int i = 0; i < rowCount; i++)
            {
                TableContentGrid.RowDefinitions.Add(new RowDefinition { MaxHeight = 30 });
            }
            for (int j = 0; j < columnCount; j++)
            {
                TableContentGrid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 80 });
            }

            // Tạo các TextBox và thêm vào Grid
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    var textBox = new TextBox();
                    textBox.Text = ""; // Giá trị mặc định
                    Grid.SetRow(textBox, row);
                    Grid.SetColumn(textBox, col);
                    TableContentGrid.Children.Add(textBox);
                    cellTextBoxes.Add(textBox);
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string columnSpec = ColumnSpecTextBox.Text.Trim();
            if (string.IsNullOrEmpty(columnSpec))
            {
                MessageBox.Show("Cấu trúc cột không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int.TryParse(((ComboBoxItem)ColumnCountComboBox.SelectedItem).Content.ToString(), out int columnCount);
            int.TryParse(((ComboBoxItem)RowCountComboBox.SelectedItem).Content.ToString(), out int rowCount);            

            // Tạo nội dung placeholder cho bảng
            var tableContent = new StringBuilder();
            int cellIndex = 0;

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    string cellText = cellTextBoxes[cellIndex].Text.Replace("&", "\\&");
                    tableContent.Append(cellText);
                    if (col < columnCount - 1) tableContent.Append(" & ");
                    cellIndex++;
                }
                tableContent.AppendLine(" \\\\");
            }

            // Xây dựng chuỗi LaTeX hoàn chỉnh            
            GeneratedTableLatex = $"\\begin{{bang}}[tabularx={columnSpec}]" + @"{}" + $"\n{tableContent}\\end{{bang}}";

            // Chèn trực tiếp vào editor và đóng cửa sổ
            _textEditor.Document.Insert(_textEditor.CaretOffset, GeneratedTableLatex);
            _textEditor.Focus();
            this.Close();
        }
    }
}