// d:\OneDrive\2025 - 2026\FixMathpix\FixMathpix2025\FixMathpix2025\InsertImageWindow.xaml.cs
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace FixMathpix2025
{
    public partial class InsertImageWindow : Window
    {
        private string _basePath;
        public string GeneratedLatex { get; private set; }
        public event Action<string> ImageInserted;

        public InsertImageWindow(string basePath = null)
        {
            InitializeComponent();
            _basePath = basePath;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.pdf;*.eps)|*.png;*.jpg;*.jpeg;*.pdf;*.eps|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;

                // Ưu tiên tìm thư mục "images" và lấy đường dẫn tương đối từ đó
                string imagesFolderName = "images";
                string normalizedPath = selectedPath.Replace('\\', '/');
                int imagesFolderIndex = normalizedPath.IndexOf($"/{imagesFolderName}/", StringComparison.OrdinalIgnoreCase);

                if (imagesFolderIndex != -1)
                {
                    // Trích xuất đường dẫn từ thư mục "images" trở đi
                    selectedPath = normalizedPath.Substring(imagesFolderIndex + 1);
                }
                else if (!string.IsNullOrEmpty(_basePath)) // Nếu không, dùng logic đường dẫn tương đối cũ
                {
                    // Cố gắng tạo đường dẫn tương đối nếu có thể
                    try
                    {
                        // Đảm bảo đường dẫn thư mục kết thúc bằng dấu phân cách
                        string folderPath = _basePath.EndsWith(Path.DirectorySeparatorChar.ToString())
                            ? _basePath
                            : _basePath + Path.DirectorySeparatorChar;

                        Uri pathUri = new Uri(selectedPath);
                        Uri folderUri = new Uri(folderPath);
                        
                        Uri relativeUri = folderUri.MakeRelativeUri(pathUri);
                        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

                        selectedPath = relativePath;
                    }
                    catch { }
                }
                // LaTeX sử dụng dấu gạch chéo '/' cho đường dẫn
                ImagePathTextBox.Text = selectedPath.Replace('\\', '/');
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            string path = ImagePathTextBox.Text.Trim();
            string options = OptionsTextBox.Text.Trim();
            string caption = CaptionTextBox.Text.Trim();
            string label = LabelTextBox.Text.Trim();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Tạo lệnh includegraphics
            string includeGraphics = $"\\includegraphics[{options}]{{{path}}}";

            if (RadioHinhanh.IsChecked == true)
            {
                string width = HinhanhWidthTextBox.Text.Trim();
                sb.AppendLine($"\\begin{{hinhanh}}{{{width}}}");
                sb.AppendLine(includeGraphics + @"\\");
                sb.Append("\\centering");
                if (!string.IsNullOrEmpty(caption)) sb.AppendLine($"\\captionof{{figure}}{{{caption}}}");
                if (!string.IsNullOrEmpty(label)) sb.AppendLine($"\\label{{fig:H{label}}}");
                sb.Append("\\end{hinhanh}");
            }
            else if (RadioCenter.IsChecked == true)
            {
                sb.AppendLine("\\begin{center}");
                sb.AppendLine("\t" + includeGraphics + @"\\");
                if (!string.IsNullOrEmpty(caption)) sb.AppendLine($"\\captionof{{figure}}{{{caption}}}");
                if (!string.IsNullOrEmpty(label)) sb.AppendLine($"\\label{{fig:H{label}}}");
                sb.Append("\\end{center}");
            }
            else
            {
                sb.Append(includeGraphics);
                // Nếu không có môi trường bao quanh, caption và label có thể không hoạt động như mong đợi
                // nhưng vẫn thêm vào nếu người dùng nhập
                if (!string.IsNullOrEmpty(caption)) sb.Append($"\\captionof{{figure}}{{{caption}}}");
                if (!string.IsNullOrEmpty(label)) sb.Append($"\\label{{fig:H{label}}}");
            }

            GeneratedLatex = sb.ToString();
            ImageInserted?.Invoke(GeneratedLatex);
            try { DialogResult = true; } catch { } // Bỏ qua lỗi nếu cửa sổ mở bằng .Show()
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try { DialogResult = false; } catch { } // Bỏ qua lỗi nếu cửa sổ mở bằng .Show()
            Close();
        }
    }
}
