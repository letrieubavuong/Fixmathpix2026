using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FixMathpix2025
{
    public partial class GoToLineDialog : Window
    {
        public int LineNumber { get; private set; }

        public GoToLineDialog(int currentLine, int maxLine)
        {
            InitializeComponent();
            lineNumberTextBox.Text = currentLine.ToString();
            lineNumberTextBox.Focus();
            lineNumberTextBox.SelectAll();
            this.Title = $"Đi tới dòng (1 - {maxLine})";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(lineNumberTextBox.Text, out int line))
            {
                LineNumber = line;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }

        private void LineNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Chỉ cho phép nhập số
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]+");
        }
    }
}
