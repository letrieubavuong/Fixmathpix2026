using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Xml;
using ICSharpCode.AvalonEdit.Snippets;
using Microsoft.Win32;

namespace FixMathpix2025
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FindAndReplace _findReplaceWindow;
        private EditorSettings _editorSettings;
        private CompletionWindow _completionWindow;
        private string _currentFilePath;

        private DispatcherTimer _autoSaveTimer;
        private readonly string _autoSaveFilePath;

        private DispatcherTimer _bracketCheckTimer;
        private ITextMarkerService _textMarkerService;
        private readonly List<ITextMarker> _bracketErrorMarkers = new List<ITextMarker>();

        // Thêm các biến để theo dõi tệp
        private FileSystemWatcher _fileWatcher;
        private DateTime _lastFileWriteTime;
        private bool _isSaving = false; // Cờ để tránh tự kích hoạt sự kiện khi ứng dụng lưu tệp
        private readonly AutoSaveRegistrationTracker _autoSaveTracker = new AutoSaveRegistrationTracker();
        private DispatcherTimer _fileWatcherDebounceTimer;
        private DateTime _expectedSaveWriteTimeUtc;
        private string _expectedContentHash;

        // Định nghĩa lệnh tùy chỉnh để đóng tệp
        public static readonly RoutedCommand CloseTexCommand = new RoutedCommand("CloseTex", typeof(MainWindow));

        // Thêm các biến cho chức năng folding
        private FoldingManager _foldingManager;
        private LatexFoldingStrategy _foldingStrategy;
        private DispatcherTimer _foldingUpdateTimer;

        public MainWindow()
        {
            InitializeComponent();
            LoadLaTeXHighlighting();

            _autoSaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FixMathpix2025", "autosave.tex");
            SetupAutoSaveTimer();
            LoadAndApplySettings();
            CheckForAutoSave();

            // Đăng ký sự kiện để xử lý tự động hoàn thành
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;

            // Khởi tạo các thành phần cho việc kiểm tra ngoặc
            SetupBracketChecking();

            // Khởi tạo FileSystemWatcher
            InitializeFileWatcher();

            // Khởi tạo chức năng folding
            InitializeFolding();

            // Đăng ký sự kiện cập nhật trạng thái con trỏ và tệp trên StatusBar
            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            UpdateStatusBarFile();
        }

        private void AutoCloseEnvironment()
        {
            var document = textEditor.Document;
            int offset = textEditor.CaretOffset;

            // Chỉ thực hiện nếu offset đủ lớn để chứa một lệnh \begin{}
            if (offset < 8) return;

            // Lấy một đoạn văn bản trước con trỏ để kiểm tra
            // Độ dài 256 là đủ lớn cho hầu hết các tên môi trường
            int start = Math.Max(0, offset - 256);
            string textBeforeCaret = document.GetText(start, offset - start);

            // Regex để tìm `\begin{env_name}` ngay trước con trỏ
            var match = Regex.Match(textBeforeCaret, @"\\begin\{([a-zA-Z0-9\*]+)\}\s*$", RegexOptions.RightToLeft);

            if (match.Success && !textEditor.TextArea.Selection.IsMultiline)
            {
                string envName = match.Groups[1].Value;

                // Sử dụng Dispatcher để đảm bảo hành động được thực hiện sau khi việc nhập ký tự '}' hoàn tất
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var snippet = new Snippet();
                    snippet.Elements.Add(new SnippetTextElement { Text = "\n\t" });
                    snippet.Elements.Add(new SnippetCaretElement()); // $caret$
                    snippet.Elements.Add(new SnippetTextElement { Text = $"\n\\end{{{envName}}}" });

                    snippet.Insert(textEditor.TextArea);
                }), DispatcherPriority.Background);
            }
        }

        private void InsertEnvironment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;

            var snippet = new Snippet();
            var replaceableEnv = new SnippetReplaceableTextElement { Text = "môi trường" };

            snippet.Elements.Add(new SnippetTextElement { Text = "\\begin{" });
            snippet.Elements.Add(replaceableEnv);

            if (string.IsNullOrEmpty(selectedText))
            {
                // Không có văn bản nào được chọn, chèn snippet trống với con trỏ ở giữa
                snippet.Elements.Add(new SnippetTextElement { Text = "}\n\t" });
                snippet.Elements.Add(new SnippetCaretElement()); // $caret$
            }
            else
            {
                // Bọc văn bản đã chọn
                snippet.Elements.Add(new SnippetTextElement { Text = "}\n" });
                snippet.Elements.Add(new SnippetTextElement { Text = selectedText });
            }

            snippet.Elements.Add(new SnippetTextElement { Text = "\n\\end{" });
            snippet.Elements.Add(new SnippetBoundElement { TargetElement = replaceableEnv }); // Liên kết với placeholder 'môi trường'
            snippet.Elements.Add(new SnippetTextElement { Text = "}" });

            snippet.Insert(textEditor.TextArea);
        }

        private void InsertFigure_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var snippet = new Snippet();
            snippet.Elements.Add(new SnippetTextElement { Text = "\\begin{figure}[h!]\n\t\\centering\n\t\\includegraphics[width=0.8\\textwidth]{" });
            snippet.Elements.Add(new SnippetReplaceableTextElement { Text = "path" });
            snippet.Elements.Add(new SnippetTextElement { Text = "}\n\t\\caption{" });
            snippet.Elements.Add(new SnippetReplaceableTextElement { Text = "caption" });
            snippet.Elements.Add(new SnippetTextElement { Text = "}\n\t\\label{fig:" });
            snippet.Elements.Add(new SnippetReplaceableTextElement { Text = "label" });
            snippet.Elements.Add(new SnippetTextElement { Text = "}\n\\end{figure}\n" });
            snippet.Elements.Add(new SnippetCaretElement());

            snippet.Insert(textEditor.TextArea);
        }

        private void InitializeFolding()
        {
            // Tạo và cài đặt FoldingManager trước
            _foldingManager = FoldingManager.Install(textEditor.TextArea);

            // Cài đặt FoldingMargin vào lề trái của editor
            var foldingMargin = new FoldingMargin { FoldingManager = _foldingManager };
            textEditor.TextArea.LeftMargins.Insert(0, foldingMargin); // Thêm vào trước LineNumberMargin
            _foldingStrategy = new LatexFoldingStrategy();

            // Cập nhật folding lần đầu
            UpdateFolding();

            // Tạo timer để cập nhật folding sau khi người dùng gõ
            _foldingUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2) // Cập nhật sau 2 giây không gõ
            };
            _foldingUpdateTimer.Tick += (sender, e) =>
            {
                _foldingUpdateTimer.Stop(); // Stop timer inside Tick (debounce)
                UpdateFolding();
            };

            textEditor.TextChanged += (sender, e) =>
            {
                _foldingUpdateTimer.Stop();
                _foldingUpdateTimer.Start();
            };
        }

        private void InitializeFileWatcher()
        {
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;

            _fileWatcherDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _fileWatcherDebounceTimer.Tick += (sender, e) =>
            {
                _fileWatcherDebounceTimer.Stop();
                ProcessFileChangeNotification();
            };
        }

        private void StartWatchingFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                _fileWatcher.EnableRaisingEvents = false;
                return;
            }

            try
            {
                _fileWatcher.Path = Path.GetDirectoryName(filePath);
                _fileWatcher.Filter = Path.GetFileName(filePath);
                _lastFileWriteTime = File.GetLastWriteTimeUtc(filePath);
                _fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                _fileWatcher.EnableRaisingEvents = false;
                Console.WriteLine("Không thể bắt đầu theo dõi tệp: " + ex.Message);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath) || !e.FullPath.Equals(_currentFilePath, StringComparison.OrdinalIgnoreCase))
                return;

            Dispatcher.Invoke(() =>
            {
                _fileWatcherDebounceTimer.Stop();
                _fileWatcherDebounceTimer.Start();
            });
        }

        private void ProcessFileChangeNotification()
        {
            if (string.IsNullOrEmpty(_currentFilePath) || !File.Exists(_currentFilePath))
                return;

            try
            {
                DateTime currentWriteTime = File.GetLastWriteTimeUtc(_currentFilePath);

                if (_isSaving || currentWriteTime == _expectedSaveWriteTimeUtc)
                {
                    return;
                }

                string diskContent = File.ReadAllText(_currentFilePath);
                string diskHash = ComputeHash(diskContent);

                if (_expectedContentHash != null && diskHash == _expectedContentHash)
                {
                    return;
                }

                string editorHash = ComputeHash(textEditor.Text);
                if (!textEditor.IsModified && diskHash == editorHash)
                {
                    return;
                }

                _lastFileWriteTime = currentWriteTime;
                _expectedSaveWriteTimeUtc = currentWriteTime;

                var result = MessageBox.Show(this, "Tệp đã bị thay đổi bởi một chương trình khác. Bạn có muốn tải lại nội dung mới không?", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    textEditor.Text = diskContent;
                    textEditor.IsModified = false;
                    StatusMessageTextBlock.Text = "Đã tải lại tệp thành công";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kiểm tra thay đổi tệp: " + ex.Message);
            }
        }

        private string ComputeHash(string text)
        {
            if (text == null) return string.Empty;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        private void LoadLaTeXHighlighting()
        {
            try
            {
                // Get the current assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Build the resource name (Namespace.FileName.extension)
                // Make sure this matches your project's default namespace and the xshd file name
                string resourceName = "FixMathpix2025.LaTeXHighlighting.xshd";

                // Load the XSHD definition from the embedded resource
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        MessageBox.Show("Resource not found: " + resourceName + ". Please ensure Build Action is set to Embedded Resource.");
                        return;
                    }

                    using (XmlTextReader reader = new XmlTextReader(stream))
                    {
                        IHighlightingDefinition latexHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        HighlightingManager.Instance.RegisterHighlighting("LaTeX", new string[] { ".tex" }, latexHighlighting);
                        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("LaTeX");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading LaTeX highlighting: " + ex.Message);
            }
        }

        private void ApplyCommandButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy item đang được chọn từ ComboBox
            if (CommandComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string command = selectedItem.Content.ToString();
                string selectedText = textEditor.SelectedText;

                // Kiểm tra xem có lệnh và có văn bản được chọn hay không
                if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(selectedText))
                {
                    // Xây dựng chuỗi thay thế
                    string replacementText = $"\\{command}{{{selectedText}}}";

                    // Thay thế văn bản đang chọn trong editor bằng chuỗi mới
                    textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
                }
            }
        }

        private void ListEXButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                return;
            }

            // Lấy số cột từ ComboBox
            string columnCount = (ListColumnCountComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "1";

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var newContent = new StringBuilder();

            newContent.AppendLine($"\\begin{{listEX}}[{columnCount}]");

            // Regex để tìm các mục trong danh sách.
            // Hỗ trợ các định dạng: "a.", "a)", "a/", "-", "•" theo sau là khoảng trắng.
            var regex = new Regex(@"^\s*([a-z][\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                string itemContent;
                if (match.Success)
                {
                    // Lấy nội dung sau dấu đầu dòng
                    itemContent = match.Groups[2].Value.Trim();
                }
                else
                {
                    // Nếu không khớp, coi cả dòng là nội dung
                    itemContent = line.Trim();
                }
                newContent.AppendLine($"\\item {itemContent}");
            }

            newContent.Append("\\end{listEX}");

            // Thay thế văn bản đã chọn bằng nội dung mới
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
        }

        private void EnumEXButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                return;
            }

            // Lấy số cột từ ComboBox
            string columnCount = (ListColumnCountComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "1";
            // Lấy kiểu danh sách từ ComboBox
            string listStyle = (EnumStyleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "a)";

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var newContent = new StringBuilder();

            newContent.AppendLine($"\\begin{{enumEX}}[{listStyle}]{{{columnCount}}}");

            // Regex để tìm các mục trong danh sách.
            var regex = new Regex(@"^\s*([a-z0-9]+[\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                string itemContent;
                if (match.Success)
                {
                    // Lấy nội dung sau dấu đầu dòng
                    itemContent = match.Groups[2].Value.Trim();
                }
                else
                {
                    // Nếu không khớp, coi cả dòng là nội dung
                    itemContent = line.Trim();
                }
                newContent.AppendLine($"\\item {itemContent}");
            }

            newContent.Append("\\end{enumEX}");

            // Thay thế văn bản đã chọn bằng nội dung mới
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
        }

        private void WrapSelectionWithCommand(string command)
        {
            string selectedText = textEditor.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                // Xây dựng chuỗi thay thế
                string replacementText = $"\\{command}{{{selectedText}}}";

                // Thay thế văn bản đang chọn trong editor bằng chuỗi mới
                textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCommand("textbf");
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCommand("textit");
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCommand("underline");
        }

        private void LabelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCommand("label");
        }

        private void RefMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCommand("ref");
        }

        private void WrapSelectionWithEnvironment(string environmentName)
        {
            string selectedText = textEditor.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                // Xây dựng chuỗi thay thế
                var newContent = new StringBuilder();
                newContent.AppendLine($"\\begin{{{environmentName}}}");
                newContent.AppendLine(selectedText);
                newContent.Append($"\\end{{{environmentName}}}");

                // Thay thế văn bản đang chọn trong editor bằng chuỗi mới
                textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
            }
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithEnvironment("flushleft");
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithEnvironment("center");
        }

        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithEnvironment("flushright");
        }

        private void WrapSelectionInListEnvironment(string environmentName, string itemCommand)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                // Nếu không có gì được chọn, chỉ chèn cấu trúc cơ bản
                string emptyStructure = $"\\begin{{{environmentName}}}\n\t{itemCommand} \n\\end{{{environmentName}}}";
                textEditor.Document.Insert(textEditor.CaretOffset, emptyStructure);
                // Di chuyển con trỏ vào trong
                textEditor.CaretOffset -= ($"\n\\end{{{environmentName}}}").Length;
                return;
            }

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var newContent = new StringBuilder();

            newContent.AppendLine($"\\begin{{{environmentName}}}");

            // Regex để tìm các mục trong danh sách (tùy chọn)
            var regex = new Regex(@"^\s*([a-z0-9]+[\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    newContent.AppendLine(line); // Giữ lại các dòng trống
                    continue;
                }
                var match = regex.Match(trimmedLine);
                string itemContent = match.Success ? match.Groups[2].Value.Trim() : trimmedLine;

                newContent.AppendLine($"\t{itemCommand} {itemContent}");
            }

            newContent.Append($"\\end{{{environmentName}}}");

            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
        }

        private void ItemizeButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionInListEnvironment("itemize", "\\item");
        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionInListEnvironment("enumerate", "\\item");
        }

        private void ItemChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelectionInListEnvironment("itemchoice", "\\itemch");
        }

        private void WrapSelection(string prefix, string suffix)
        {
            string selectedText = textEditor.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                string replacementText = $"{prefix}{selectedText}{suffix}";
                textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
            }
        }

        private void MathModeButton_Click(object sender, RoutedEventArgs e)
        {
            WrapSelection("$", "$");
        }

        private void AlignStarButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                WrapSelectionWithEnvironment("align*");
                return;
            }

            // 1. Xóa các ký tự $
            string processedText = selectedText.Replace("$", "");

            // 2. Xử lý các dòng có nhiều dấu '='
            var lines = processedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var resultLines = new StringBuilder();

            foreach (var line in lines)
            {
                int firstEqualSign = line.IndexOf('=');
                if (firstEqualSign != -1)
                {
                    // Thay thế tất cả các dấu '=' từ vị trí thứ hai trở đi
                    resultLines.AppendLine(Regex.Replace(line, "(?<=.=.*)=", m => @"\\ " + "\n" + m.Value));
                }
                else
                {
                    resultLines.AppendLine(line);
                }
            }
            WrapSelectionWithEnvironment("align*", resultLines.ToString().TrimEnd('\r', '\n'));
        }

        private void ShortAnsButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                // Cấu trúc này khá đặc biệt nên ta xử lý riêng
                string replacementText = $"\\shortans[oly]{{{selectedText}}}";
                textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
            }
            else
            {
                textEditor.Document.Insert(textEditor.CaretOffset, "\\shortans[oly]{}");
            }
        }

        private void EquationButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                // If nothing is selected, just insert an empty environment
                textEditor.Document.Insert(textEditor.CaretOffset, "\\begin{equation}\n\t\n\\end{equation}");
                textEditor.CaretOffset -= "\n\\end{equation}".Length;
                return;
            }

            string processedText = selectedText;

            // - xóa begin, end align*
            processedText = Regex.Replace(processedText, @"\\begin\{align\*\}", "", RegexOptions.IgnoreCase);
            processedText = Regex.Replace(processedText, @"\\end\{align\*\}", "", RegexOptions.IgnoreCase);

            // - xóa dấu $
            processedText = processedText.Replace("$", "");

            // - chuyển \tag thành \label
            processedText = processedText.Replace(@"\tag", @"\label");

            // Trim whitespace from the start and end of the content
            processedText = processedText.Trim();

            // Wrap in equation environment
            string replacementText = $"\\begin{{equation}}\n\t{processedText}\n\\end{{equation}}";

            // Replace the selected text
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
        }

        private void LoigiaiButton_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Document.Insert(textEditor.CaretOffset, "\\loigiai{}");
            textEditor.CaretOffset -= 1; // Di chuyển con trỏ vào trong cặp ngoặc
        }

        private void WrapSelectionWithEnvironment(string environmentName, string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            var newContent = new StringBuilder();
            newContent.AppendLine($"\\begin{{{environmentName}}}");
            newContent.AppendLine(content);
            newContent.Append($"\\end{{{environmentName}}}");

            // Thay thế văn bản đang chọn trong editor bằng chuỗi mới
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
        }

        private void TrueButton_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Document.Insert(textEditor.CaretOffset, "\\True{}");
            textEditor.CaretOffset -= 1; // Di chuyển con trỏ vào trong cặp ngoặc
        }

        private void CauDSButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                return;
            }

            var lines = selectedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var newContent = new StringBuilder();

            newContent.AppendLine(@"\choiceTF[t]");

            // Regex để tìm các mục trong danh sách.
            // Hỗ trợ các định dạng: "a.", "a)", "a/", "-", "•" theo sau là khoảng trắng.
            var regex = new Regex(@"^\s*([a-z][\.\)\/]|[-•])\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                string itemContent = match.Success ? match.Groups[2].Value.Trim() : line.Trim();

                newContent.AppendLine($"{{{itemContent}}}");
            }

            // Thay thế văn bản đã chọn bằng nội dung mới
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
        }

        private void BangButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo một instance mới của wizard, truyền vào textEditor
            var tableWizard = new TableWizardWindow(textEditor)
            {
                Owner = this // Đặt cửa sổ chính làm chủ sở hữu để wizard luôn ở trên
            };

            // Hiển thị cửa sổ wizard mà không khóa cửa sổ chính
            tableWizard.Show();
        }
        private void Ghepfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "TeX files (*.tex)|*.tex|All files (*.*)|*.*",
                DefaultExt = ".tex",
                Title = "Chọn các tệp TeX để ghép",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var contentToAppend = new StringBuilder();

                    // Nếu trình soạn thảo đã có nội dung, hãy thêm dòng mới để phân tách.
                    if (textEditor.Document.TextLength > 0)
                    {
                        contentToAppend.AppendLine();
                        contentToAppend.AppendLine();
                    }

                    string combinedContent = string.Join(Environment.NewLine + Environment.NewLine,
                        openFileDialog.FileNames.Select(File.ReadAllText));

                    textEditor.Document.Insert(textEditor.Document.TextLength, contentToAppend.ToString() + combinedContent);
                    MessageBox.Show($"Đã ghép thành công {openFileDialog.FileNames.Length} tệp.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi ghép tệp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CauhoiButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                MessageBox.Show("Vui lòng bôi đen đoạn văn bản cần chuyển đổi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string bai = (BaiComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "01";
            string phan = (PhanComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "I";

            string environmentName;
            switch (phan)
            {
                case "I":
                    environmentName = "cauhoiTN";
                    break;
                case "II":
                    environmentName = "cauhoiDS";
                    break;
                case "III":
                    environmentName = "cauhoiTLN";
                    break;
                case "IV":
                    environmentName = "cauhoiTL";
                    break;
                default:
                    environmentName = "cauhoiTN";
                    break;
            }

            string replacementText = $"\\Phan{phan}\n\\begin{{{environmentName}}}{{Bai{bai}}}\n{selectedText}\n\\end{{{environmentName}}}";
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
        }

        private void ChuyenTeXButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textEditor.Document.Text)) return;

            try
            {
                string currentContent = textEditor.Document.Text;

                // 1. Chuẩn hóa và dọn dẹp cơ bản
                currentContent = currentContent.Replace("\r\n", "\n");
                currentContent = Regex.Replace(currentContent, @"\n{2,}", "\n"); // Nhiều dòng trống -> 1
                currentContent = currentContent.Replace(@"\frac", @"\dfrac");

                // 2. Loại bỏ các lệnh không cần thiết
                currentContent = Regex.Replace(currentContent, @"\\mathrm{(.*?)}", m => m.Groups[1].Value, RegexOptions.Singleline);
                currentContent = Regex.Replace(currentContent, @"\\text{(.*?)}", m => m.Groups[1].Value, RegexOptions.Singleline);

                // 3. Xử lý "Chọn"
                currentContent = Regex.Replace(currentContent, @"Chọn \$\\mathbf{(.*?)}\$", m => $"Chọn {m.Groups[1].Value}\n", RegexOptions.Singleline);

                // 4. Xử lý các section và câu hỏi để bọc trong \begin{ex}...\end{ex}
                // Thêm \end{ex} ở cuối để đảm bảo block cuối cùng được đóng
                if (!currentContent.TrimEnd().EndsWith("\\end{ex}"))
                {
                    currentContent += "\n\\end{ex}";
                }

                // Thay thế các section/subsection/câu hỏi bằng cấu trúc \end{ex}...\begin{ex}
                currentContent = Regex.Replace(currentContent, @"\\section\*{(\d{1,}.*?)}\n", m => $"\\end{{ex}}\n{m.Value.Trim()}\n\\begin{{ex}}\n", RegexOptions.Multiline);
                currentContent = Regex.Replace(currentContent, @"\\section\*{BÀI(.*?)}\n", m => $"\\end{{ex}}\n\\section*{{BÀI{m.Groups[1].Value.Trim()}}}\n\\begin{{ex}}\n", RegexOptions.Multiline);
                currentContent = Regex.Replace(currentContent, @"\\subsection\*{(\d{1,}.*?)}\n", m => $"\\end{{ex}}\n{m.Value.Trim()}\n\\begin{{ex}}\n", RegexOptions.Multiline);
                currentContent = Regex.Replace(currentContent, @"\\section\*{(.*?)}\n", m => $"{m.Groups[1].Value.Trim()}\n", RegexOptions.Multiline); // Xóa các section không có số
                currentContent = Regex.Replace(currentContent, @"^Câu\s\d+[\.:]", m => $"\n\\end{{ex}}\n\\begin{{ex}}\n", RegexOptions.Multiline);


                // 5. Xử lý Lời giải
                currentContent = currentContent.Replace(@"Hướng dẫn (Group Vật lý Physics)", "\\loigiai{\n");
                currentContent = Regex.Replace(currentContent, @"Lời giải([\s\.:])", "\\loigiai{\n");

                // 6. Xử lý các lựa chọn A, B, C, D
                currentContent = Regex.Replace(currentContent, @"A\.(.*?)\nB\.(.*?)\nC\.(.*?)\nD\.(.*?)\n",
                    m => $"\\choice\n{{{m.Groups[1].Value.Trim()}}}\n{{{m.Groups[2].Value.Trim()}}}\n{{{m.Groups[3].Value.Trim()}}}\n{{{m.Groups[4].Value.Trim()}}}\n",
                    RegexOptions.Singleline);

                // 7. Xử lý các block \loigiai bên trong \begin{ex}
                string regexExBlocks = @"\\begin{ex}([\s\S]*?)\\end{ex}";
                var newFullContent = new StringBuilder();
                int lastIndex = 0;

                foreach (Match match in Regex.Matches(currentContent, regexExBlocks))
                {
                    newFullContent.Append(currentContent, lastIndex, match.Index - lastIndex);
                    string exBlockContent = match.Groups[1].Value;

                    if (exBlockContent.Contains("\\loigiai{"))
                    {
                        // Nếu có \loigiai, thêm dấu } vào cuối nội dung của block đó
                        newFullContent.Append("\\begin{ex}");
                        newFullContent.Append(exBlockContent.TrimEnd());
                        newFullContent.Append("\n}\n"); // Thêm dấu đóng cho \loigiai
                        newFullContent.Append("\\end{ex}");
                    }
                    else
                    {
                        // Giữ nguyên block nếu không có \loigiai
                        newFullContent.Append(match.Value);
                    }
                    lastIndex = match.Index + match.Length;
                }
                newFullContent.Append(currentContent, lastIndex, currentContent.Length - lastIndex);
                currentContent = newFullContent.ToString();

                // 8. Dọn dẹp cuối cùng
                // Xóa \end{ex} thừa ở đầu
                if (currentContent.StartsWith("\\end{ex}"))
                {
                    currentContent = currentContent.Substring("\\end{ex}".Length);
                }
                // Đảm bảo bắt đầu bằng \begin{ex}
                if (!currentContent.TrimStart().StartsWith("\\begin{ex}"))
                {
                    currentContent = "\\begin{ex}\n" + currentContent;
                }

                currentContent = currentContent.Trim();

                // Dọn dẹp khoảng trắng quanh dấu }
                currentContent = currentContent.Replace(" }", "}")
                                               .Replace("\t}", "}")
                                               .Replace("{ ", "{")
                                               .Replace(".}\n", "}\n");
                currentContent = Regex.Replace(currentContent, @"\n{3,}", "\n\n");

                currentContent = FixMathtype(currentContent);

                textEditor.Document.Text = currentContent;
                StatusMessageTextBlock.Text = "Đã chuyển đổi TeX thành công";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình chuyển đổi TeX: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Thực hiện các thay thế và dọn dẹp cuối cùng cho văn bản LaTeX.
        /// </summary>
        /// <param name="text">Văn bản cần xử lý.</param>
        /// <returns>Văn bản đã được dọn dẹp.</returns>
        private string FixMathtype(string text)
        {
            return TexProcessingService.FixMathtype(text);
        }

        private void FindReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_findReplaceWindow == null || !_findReplaceWindow.IsLoaded)
            {
                _findReplaceWindow = new FindAndReplace(textEditor);
                _findReplaceWindow.Closed += (s, args) => _findReplaceWindow = null;
            }
            _findReplaceWindow.Show();
            _findReplaceWindow.Activate();
        }

        private void FindReplace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplaceButton_Click(sender, e);
            if (_findReplaceWindow != null)
            {
                if (e.Command == ApplicationCommands.Replace)
                {
                    _findReplaceWindow.ReplaceComboBox.Focus();
                }
                else
                {
                    _findReplaceWindow.FindComboBox.Focus();
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.ShowDialog();
        }

        private void LoadAndApplySettings()
        {
            _editorSettings = EditorSettings.Load();
            ApplySettings(_editorSettings);
        }

        public void ApplySettings(EditorSettings settings)
        {
            _editorSettings = settings;

            // Apply font settings
            textEditor.FontFamily = new FontFamily(settings.FontFamily);
            textEditor.FontSize = settings.FontSize;

            // Apply highlighting colors
            var highlighting = textEditor.SyntaxHighlighting;
            if (highlighting != null)
            {
                foreach (var colorSetting in settings.HighlightingColors)
                {
                    var namedColor = highlighting.GetNamedColor(colorSetting.Key);
                    if (namedColor != null)
                        namedColor.Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString(colorSetting.Value));
                }
            }

            // Apply auto-save settings
            if (_autoSaveTimer != null)
            {
                _autoSaveTimer.Stop(); // Dừng timer trước khi thay đổi khoảng thời gian
                _autoSaveTimer.Interval = TimeSpan.FromSeconds(_editorSettings.AutoSaveIntervalSeconds);
            }
            UpdateAutoSaveRegistration();

            // Apply dynamic shortcuts to InputBindings
            var shortcutsToApply = (settings.Shortcuts == null || settings.Shortcuts.Count == 0)
                ? DefaultEditorShortcuts.GetDefaultShortcuts()
                : settings.Shortcuts;
            if (shortcutsToApply != null && shortcutsToApply.Count > 0)
            {
                var converter = new KeyGestureConverter();
                foreach (var kvp in shortcutsToApply)
                {
                    ICommand targetCmd = GetCommandByName(kvp.Key);
                    if (targetCmd != null)
                    {
                        var oldBindings = textEditor.InputBindings.OfType<KeyBinding>().Where(kb => kb.Command == targetCmd).ToList();
                        foreach (var oldB in oldBindings)
                        {
                            textEditor.InputBindings.Remove(oldB);
                        }

                        if (!string.IsNullOrWhiteSpace(kvp.Value))
                        {
                            try
                            {
                                var gesture = (KeyGesture)converter.ConvertFromString(kvp.Value);
                                textEditor.InputBindings.Add(new KeyBinding(targetCmd, gesture));
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private ICommand GetCommandByName(string commandName)
        {
            switch (commandName)
            {
                case "ApplicationCommands.Open": return ApplicationCommands.Open;
                case "ApplicationCommands.Save": return ApplicationCommands.Save;
                case "ApplicationCommands.Find": return ApplicationCommands.Find;
                case "ApplicationCommands.Replace": return ApplicationCommands.Replace;
                case "local:CustomCommands.Bold": return CustomCommands.Bold;
                case "local:CustomCommands.Italic": return CustomCommands.Italic;
                case "local:CustomCommands.Underline": return CustomCommands.Underline;
                case "avalonedit:AvalonEditCommands.ConvertToUppercase": return ICSharpCode.AvalonEdit.AvalonEditCommands.ConvertToUppercase;
                case "local:CustomCommands.ToggleComment": return CustomCommands.ToggleComment;
                case "local:CustomCommands.MathMode": return CustomCommands.MathMode;
                case "local:CustomCommands.InsertLoigiai": return CustomCommands.InsertLoigiai;
                case "local:CustomCommands.SpellCheck": return CustomCommands.SpellCheck;
                case "local:CustomCommands.CleanupText": return CustomCommands.CleanupText;
                default: return null;
            }
        }

        private void FixHevaButton_Click(object sender, RoutedEventArgs e)
        {
            // Regex để tìm khối \heva{...}
            var regex = new Regex(@"\\heva\{([\s\S]*?)\}", RegexOptions.Compiled);
            Match match = regex.Match(textEditor.Text, textEditor.SelectionStart);

            // Nếu con trỏ không nằm trong khối heva, hãy tìm khối gần nhất
            if (!match.Success || textEditor.SelectionStart > match.Index + match.Length)
            {
                match = regex.Match(textEditor.Text, textEditor.CaretOffset);
            }
            if (!match.Success)
            {
                match = regex.Match(textEditor.Text); // Thử tìm từ đầu
            }

            if (match.Success)
            {
                // Lấy nội dung bên trong dấu ngoặc
                var group = match.Groups[1];
                string content = group.Value;

                // Thực hiện thay thế
                string newContent = content.Replace("&", "");
                newContent = Regex.Replace(newContent, @"\s*\\\\\s*", @" \\ "); // Chuẩn hóa khoảng trắng quanh \\

                // Chỉ thay thế nếu có sự thay đổi
                if (content != newContent)
                {
                    textEditor.Document.Replace(group.Index, group.Length, newContent);
                }

                // Tìm và chọn khối heva tiếp theo
                FindNextHeva();
            }
            else
            {
                MessageBox.Show("Không tìm thấy khối `\\heva{...}` nào.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FindNextHevaButton_Click(object sender, RoutedEventArgs e)
        {
            FindNextHeva();
        }

        private void FindNextHeva()
        {
            var regex = new Regex(@"\\heva\{[\s\S]*?\}", RegexOptions.Compiled);
            Match match = regex.Match(textEditor.Text, textEditor.SelectionStart + textEditor.SelectionLength);

            if (!match.Success) // Nếu không tìm thấy, tìm lại từ đầu
                match = regex.Match(textEditor.Text);

            if (match.Success)
            {
                textEditor.Select(match.Index, match.Length);
                textEditor.ScrollTo(textEditor.Document.GetLineByOffset(match.Index).LineNumber, 0);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khối `\\heva{...}`.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void SetupAutoSaveTimer()
        {
            if (_autoSaveTimer == null)
            {
                _autoSaveTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(_editorSettings?.AutoSaveIntervalSeconds ?? 5)
                };
                _autoSaveTimer.Tick += AutoSaveTimer_Tick;
                this.Closing += MainWindow_Closing;
            }
        }

        private void UpdateAutoSaveRegistration()
        {
            if (_editorSettings != null && _editorSettings.IsAutoSaveEnabled)
            {
                EnableAutoSave();
            }
            else
            {
                DisableAutoSave();
            }
        }

        private void EnableAutoSave()
        {
            _autoSaveTracker.Enable(
                () => textEditor.TextChanged += TextEditor_TextChanged,
                () => _autoSaveTimer?.Start());
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            _autoSaveTimer?.Stop();
            _autoSaveTimer?.Start();
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            if (_editorSettings == null || !_editorSettings.IsAutoSaveEnabled)
            {
                _autoSaveTimer?.Stop();
                return;
            }
            _autoSaveTimer?.Stop();
            try
            {
                string directory = Path.GetDirectoryName(_autoSaveFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(_autoSaveFilePath, textEditor.Document.Text);
                StatusMessageTextBlock.Text = "Đã tự động lưu nháp lúc " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception)
            {
                // Âm thầm xử lý ngoại lệ tự động lưu
            }
        }

        private void DisableAutoSave()
        {
            _autoSaveTracker.Disable(
                () => textEditor.TextChanged -= TextEditor_TextChanged,
                () => _autoSaveTimer?.Stop());
        }

        private void CheckForAutoSave()
        {
            if (File.Exists(_autoSaveFilePath))
            {
                var result = MessageBox.Show("Phát hiện có một phiên làm việc chưa được lưu. Bạn có muốn khôi phục lại không?", "Khôi phục phiên làm việc", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    textEditor.Document.Text = File.ReadAllText(_autoSaveFilePath);
                }
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Chỉ xóa tệp sao lưu nếu chức năng được bật
            if (_editorSettings.IsAutoSaveEnabled)
            {
                if (File.Exists(_autoSaveFilePath))
                {
                    File.Delete(_autoSaveFilePath);
                }
            }
        }


        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            // Logic tự động đóng / nhảy qua ngoặc
            if (e.Text.Length == 1)
            {
                char typedChar = e.Text[0];

                // Nhảy con trỏ qua ngoặc đóng có sẵn khi gõ ngoặc đóng (Overtype)
                if (typedChar == ')' || typedChar == '}' || typedChar == ']')
                {
                    var textArea = textEditor.TextArea;
                    if (textArea.Selection.Length == 0 && textArea.Caret.Offset < textEditor.Document.TextLength)
                    {
                        char nextChar = textEditor.Document.GetCharAt(textArea.Caret.Offset);
                        if (nextChar == typedChar)
                        {
                            textArea.Caret.Offset++;
                            e.Handled = true;
                            return;
                        }
                    }
                }

                char closingChar = '\0';

                switch (typedChar)
                {
                    case '(':
                        closingChar = ')';
                        break;
                    case '{':
                        closingChar = '}';
                        break;
                    case '[':
                        closingChar = ']';
                        break;
                }

                if (closingChar != '\0')
                {
                    var textArea = textEditor.TextArea;
                    if (textArea.Selection.Length > 0)
                    {
                        // Nếu có văn bản được chọn, bọc nó bằng cặp ngoặc
                        string selectedText = textArea.Selection.GetText();
                        textArea.Selection.ReplaceSelectionWithText(typedChar + selectedText + closingChar);
                    }
                    else
                    {
                        // Nếu không, chèn cặp ngoặc và di chuyển con trỏ vào giữa
                        textArea.Document.Insert(textArea.Caret.Offset, typedChar.ToString() + closingChar.ToString());
                        textArea.Caret.Offset--;
                    }

                    // Đánh dấu sự kiện đã được xử lý để ngăn ký tự gốc được chèn lại
                    e.Handled = true;
                    return; // Thoát để không ảnh hưởng đến logic của cửa sổ gợi ý
                }
            }

            // Logic của cửa sổ tự động hoàn thành (giữ nguyên)
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Bất cứ khi nào một ký tự không phải chữ/số được nhập,
                    // hãy chèn mục đang được chọn.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Không đóng cửa sổ ở đây.
            // Nếu người dùng không chọn gì, cửa sổ sẽ tự đóng khi mất focus.
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\\")
            {
                // Mở cửa sổ gợi ý khi người dùng gõ '\'
                _completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;

                // Tải danh sách lệnh từ tệp XML
                LoadCompletionDataFromXml(data);

                if (data.Any())
                {
                    _completionWindow.Show();
                    _completionWindow.Closed += delegate {
                        _completionWindow = null;
                    };
                }
                else
                {
                    _completionWindow = null;
                }
            }
            else if (e.Text == "{" && _completionWindow != null)
            {
                // Nếu người dùng gõ '{' ngay sau một lệnh, hãy đóng cửa sổ gợi ý
                _completionWindow.Close();
            }
            else if (e.Text == "}")
            {
                // Tự động đóng môi trường LaTeX
                AutoCloseEnvironment();
            }
        }

        private void LoadCompletionDataFromXml(IList<ICompletionData> completionData)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = "FixMathpix2025.LatexCommands.xml";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(stream);

                    foreach (XmlNode node in doc.SelectNodes("//Command"))
                    {
                        string name = node.Attributes["name"]?.Value;
                        string description = node.Attributes["description"]?.Value;
                        if (!string.IsNullOrEmpty(name))
                        {
                            completionData.Add(new LatexCompletionData(name, description));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Âm thầm bỏ qua lỗi để không làm phiền người dùng,
                // nhưng bạn có thể thêm log ở đây để gỡ lỗi.
                Console.WriteLine("Error loading completion data: " + ex.Message);
            }
        }

        private void SpellCheckButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var corrections = LoadSpellingCorrections();
                if (!corrections.Any())
                {
                    MessageBox.Show("Không tìm thấy hoặc không có quy tắc sửa lỗi nào trong thư viện.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string currentText = textEditor.Text;
                string newText = SpellingService.ApplySpellingCorrections(currentText, corrections, out int replacementsCount);

                if (replacementsCount > 0)
                {
                    textEditor.Text = newText;
                    StatusMessageTextBlock.Text = $"Đã soát lỗi chính tả ({replacementsCount} thay thế)";
                    MessageBox.Show($"Đã hoàn tất! Thực hiện {replacementsCount} thay thế.", "Soát lỗi hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy lỗi chính tả nào cần sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình soát lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageSpellingButton_Click(object sender, RoutedEventArgs e)
        {
            SpellingCorrectionManagerWindow managerWindow = new SpellingCorrectionManagerWindow();
            managerWindow.Owner = this; // Set owner to keep it on top of main window
            managerWindow.ShowDialog();
        }

        private Dictionary<string, string> LoadSpellingCorrections()
        {
            var corrections = new Dictionary<string, string>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "FixMathpix2025.SpellingCorrections.xml";

            // Define user-specific path for corrections
            string userSpellingCorrectionsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FixMathpix2025",
                "SpellingCorrections.xml"
            );

            XmlDocument doc = new XmlDocument();
            bool loaded = false;

            // Try to load from user-specific file first
            if (File.Exists(userSpellingCorrectionsFilePath))
            {
                doc.Load(userSpellingCorrectionsFilePath);
                loaded = true;
            }
            else
            {
                // Fallback to embedded resource if user file doesn't exist
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        doc.Load(stream);
                        loaded = true;
                    }
                }
            }

            if (loaded)
            {
                foreach (XmlNode node in doc.SelectNodes("//Correction"))
                {
                    string find = node.Attributes["find"]?.Value;
                    string replace = node.Attributes["replace"]?.Value;
                    if (!string.IsNullOrEmpty(find) && replace != null && !corrections.ContainsKey(find))
                    {
                        corrections.Add(find, replace);
                    }
                }
            }
            return corrections;
        }

        private void HinhanhButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy độ rộng từ ComboBox, giá trị mặc định là 5 nếu không chọn được
            string width = (Doronghinhanh.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "5";

            string selectedText = textEditor.SelectedText;

            var newContent = new StringBuilder();
            newContent.AppendLine($"\\begin{{hinhanh}}{{{width}}}");

            if (string.IsNullOrEmpty(selectedText))
            {
                // Nếu không có văn bản nào được chọn, chèn cấu trúc rỗng và đặt con trỏ vào giữa
                newContent.AppendLine("\t"); // Thêm một dòng trống với tab
                newContent.Append($"\\end{{hinhanh}}");
                textEditor.Document.Insert(textEditor.CaretOffset, newContent.ToString());
                textEditor.CaretOffset -= ($"\n\\end{{hinhanh}}").Length;
            }
            else
            {
                // Nếu có văn bản được chọn, bọc nó bằng môi trường
                newContent.AppendLine(selectedText);
                newContent.Append($"\\end{{hinhanh}}");
                textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, newContent.ToString());
            }
        }

        private void ImminiButton_Click(object sender, RoutedEventArgs e)
        {
            WrapWithImmini(withThm: false);
        }

        private void ImminiThmButton_Click(object sender, RoutedEventArgs e)
        {
            WrapWithImmini(withThm: true);
        }

        private void WrapWithImmini(bool withThm)
        {
            string selectedText = textEditor.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                MessageBox.Show("Vui lòng bôi đen một đoạn văn bản để chèn lệnh.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string processedText = selectedText;

            // Kiểm tra và xử lý nếu có môi trường center
            if (processedText.Contains(@"\begin{center}"))
            {
                processedText = processedText.Replace(@"\begin{center}", "").Trim();
                processedText = processedText.Replace(@"\end{center}", "").Trim();

                // Thêm "}{" trước \begin{tikzpicture}
                processedText = processedText.Replace(@"\begin{tikzpicture}", @"}{" + Environment.NewLine + @"\begin{tikzpicture}");
            }

            string command = withThm ? @"\immini[thm]" : @"\immini";
            string replacementText = $"{command}{{{processedText}}}";

            // Thay thế văn bản đã chọn bằng nội dung mới
            textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, replacementText);
        }

        private void FixEnumerateButton_Click(object sender, RoutedEventArgs e)
        {
            string pattern = @"(\\begin\{(?:vd|vidu|ex|bt)\}[\s\S]*?\\loigiai\{)";
            string currentText = textEditor.Text;
            int replacementsCount = 0;

            string newText = Regex.Replace(currentText, pattern, match =>
            {
                string block = match.Value;
                // Regex để tìm \begin{enumerate} có hoặc không có tham số tùy chọn, ví dụ: \begin{enumerate}[i.]
                string enumeratePattern = @"\\begin{enumerate}(\[[^\]]*\])?";

                if (Regex.IsMatch(block, enumeratePattern))
                {
                    replacementsCount++;
                    // Thay thế \begin{enumerate} hoặc \begin{enumerate}[...] bằng \begin{listEX}[1]
                    string modifiedBlock = Regex.Replace(block, enumeratePattern, @"\begin{listEX}[1]");
                    // Thay thế \end{enumerate} bằng \end{listEX}
                    modifiedBlock = modifiedBlock.Replace(@"\end{enumerate}", @"\end{listEX}");
                    return modifiedBlock;
                }
                return block; // No change
            }, RegexOptions.Multiline);

            if (replacementsCount > 0)
            {
                textEditor.Text = newText;
                MessageBox.Show($"Đã hoàn tất! Sửa {replacementsCount} khối enumerate.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khối enumerate nào cần sửa trong các môi trường vd, vidu, ex, bt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FixTikzpictureButton_Click(object sender, RoutedEventArgs e)
        {
            string pattern = @"(\\begin\{tikzpicture\}\[)([^\]]*)(\])";
            string currentText = textEditor.Text;
            int replacementsCount = 0;

            string newText = Regex.Replace(currentText, pattern, match =>
            {
                replacementsCount++;
                string options = match.Groups[2].Value;

                // Remove thickness options
                options = Regex.Replace(options, @"\b(very thick|thick|thin)\b", "", RegexOptions.IgnoreCase);

                // Remove color options
                options = Regex.Replace(options, @"\b(red|green|blue|black|white|cyan|magenta|yellow|gray|darkgray|lightgray)\b", "", RegexOptions.IgnoreCase);

                // Clean up extra commas and whitespace
                options = Regex.Replace(options, @"\s*,\s*", ",", RegexOptions.None).Trim().Trim(',');

                // Construct the new options string
                string newOptions = "draw=Mapcolor, very thick," + (string.IsNullOrEmpty(options) ? "" : " " + options);

                return match.Groups[1].Value + newOptions + match.Groups[3].Value;
            }, RegexOptions.IgnoreCase);

            if (replacementsCount > 0)
            {
                textEditor.Text = newText;
                MessageBox.Show($"Đã hoàn tất! Sửa {replacementsCount} khối tikzpicture.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khối tikzpicture nào để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FixMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        #region Bracket Checking

        private void SetupBracketChecking()
        {
            _textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(_textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(_textMarkerService);

            _bracketCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // Chờ 0.5 giây sau lần gõ cuối cùng
            };
            _bracketCheckTimer.Tick += (sender, args) =>
            {
                _bracketCheckTimer.Stop();
                CheckBrackets();
            };

            // Đăng ký sự kiện để kích hoạt timer
            textEditor.TextChanged += BracketCheck_TextChanged;

            // Thiết lập sự kiện để hiển thị tooltip
            textEditor.TextArea.MouseMove += TextArea_MouseMove;
            textEditor.TextArea.MouseLeave += TextArea_MouseLeave;

            // Tạo một tooltip và ẩn nó ban đầu
            _bracketErrorToolTip = new ToolTip { Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom, IsOpen = false };
        }

        private void BracketCheck_TextChanged(object sender, EventArgs e)
        {
            _bracketCheckTimer.Stop();
            _bracketCheckTimer.Start();
        }

        private void CheckBrackets()
        {
            // Xóa các đánh dấu lỗi cũ
            foreach (var marker in _bracketErrorMarkers)
            {
                _textMarkerService.Remove(marker);
            }
            _bracketErrorMarkers.Clear();

            var errors = FindBracketMismatches(textEditor.Text);

            foreach (var error in errors)
            {
                var marker = _textMarkerService.Create(error.Offset, error.Length);
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
                marker.ToolTip = error.Message; // Gán thông báo lỗi vào tooltip của marker
                _bracketErrorMarkers.Add(marker);
            }
        }

        private List<BracketMismatchError> FindBracketMismatches(string text)
        {
            var errors = new List<BracketMismatchError>();
            var stack = new Stack<Tuple<char, int>>(); // Lưu trữ ký tự và vị trí

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // Bỏ qua các comment của LaTeX (từ '%' đến cuối dòng)
                if (c == '%')
                {
                    // Tìm vị trí cuối dòng
                    int endOfLine = text.IndexOf('\n', i);
                    if (endOfLine == -1)
                    {
                        break; // Nếu là dòng cuối cùng, kết thúc việc kiểm tra
                    }
                    i = endOfLine; // Di chuyển con trỏ đến cuối dòng
                    continue;
                }

                // Bỏ qua các ký tự được escape, ví dụ: \{
                if (c == '\\' && i + 1 < text.Length)
                {
                    i++; // Bỏ qua ký tự tiếp theo
                    continue;
                }

                if (c == '(' || c == '{' || c == '[')
                {
                    stack.Push(new Tuple<char, int>(c, i));
                }
                else if (c == ')' || c == '}' || c == ']')
                {
                    if (stack.Count == 0)
                    {
                        // Thừa ngoặc đóng
                        errors.Add(new BracketMismatchError(i, 1, $"Dư dấu ngoặc đóng '{c}'."));
                    }
                    else
                    {
                        var openBracket = stack.Pop();
                        char expectedClosing;
                        switch (openBracket.Item1)
                        {
                            case '(': expectedClosing = ')'; break;
                            case '{': expectedClosing = '}'; break;
                            case '[': expectedClosing = ']'; break;
                            default: continue;
                        }

                        if (c != expectedClosing)
                        {
                            // Lỗi không khớp loại ngoặc
                            errors.Add(new BracketMismatchError(openBracket.Item2, 1, $"Lỗi không khớp ngoặc. Mong đợi dấu '{expectedClosing}' nhưng lại là '{c}'."));
                            errors.Add(new BracketMismatchError(i, 1, $"Lỗi không khớp ngoặc. Dấu '{c}' không khớp với dấu '{openBracket.Item1}'."));
                        }
                    }
                }
            }

            // Các ngoặc mở còn lại trong stack là lỗi thiếu ngoặc đóng
            while (stack.Count > 0)
            {
                var unclosedBracket = stack.Pop();
                errors.Add(new BracketMismatchError(unclosedBracket.Item2, 1, $"Thiếu dấu ngoặc đóng cho '{unclosedBracket.Item1}'."));
            }

            return errors;
        }

        private ToolTip _bracketErrorToolTip;

        private void TextArea_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
            if (pos != null)
            {
                int offset = textEditor.Document.GetOffset(pos.Value.Line, pos.Value.Column);

                // Tìm marker tại vị trí con trỏ
                var markerAtOffset = _textMarkerService.TextMarkers
                    .FirstOrDefault(m => m.StartOffset <= offset && (m.StartOffset + m.Length) >= offset);

                if (markerAtOffset != null && markerAtOffset.ToolTip != null)
                {
                    _bracketErrorToolTip.Content = markerAtOffset.ToolTip;
                    _bracketErrorToolTip.PlacementTarget = textEditor; // Hiển thị tooltip liên quan đến editor
                    _bracketErrorToolTip.IsOpen = true;
                    e.Handled = true;
                }
                else
                {
                    _bracketErrorToolTip.IsOpen = false;
                }
            }
        }

        private void TextArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_bracketErrorToolTip != null && _bracketErrorToolTip.IsOpen)
            {
                _bracketErrorToolTip.IsOpen = false;
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (textEditor.IsModified)
            {
                var result = MessageBox.Show(
                    "Tệp hiện tại có thay đổi chưa được lưu. Bạn có muốn lưu trước khi mở tệp khác không?",
                    "Lưu thay đổi",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (!SaveFileInternal()) return;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "TeX files (*.tex)|*.tex|All files (*.*)|*.*",
                DefaultExt = ".tex",
                Title = "Mở tệp TeX"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    textEditor.Text = File.ReadAllText(openFileDialog.FileName);
                    _currentFilePath = openFileDialog.FileName;
                    textEditor.IsModified = false;
                    StartWatchingFile(_currentFilePath);
                    this.Title = $"FixMathpix 2025 - {_currentFilePath}";
                    UpdateStatusBarFile();
                    StatusMessageTextBlock.Text = "Đã mở tệp thành công";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể mở tệp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileInternal();
        }

        private bool SaveFileInternal()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                return SaveFileAsInternal();
            }
            else
            {
                try
                {
                    _isSaving = true;
                    string content = textEditor.Text;
                    _expectedContentHash = ComputeHash(content);
                    File.WriteAllText(_currentFilePath, content);
                    _lastFileWriteTime = File.GetLastWriteTimeUtc(_currentFilePath);
                    _expectedSaveWriteTimeUtc = _lastFileWriteTime;
                    textEditor.IsModified = false;
                    StatusMessageTextBlock.Text = "Đã lưu tệp thành công";
                    MessageBox.Show("Đã lưu tệp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể lưu tệp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    _isSaving = false;
                }
            }
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileAsInternal();
        }

        private bool SaveFileAsInternal()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "TeX files (*.tex)|*.tex|All files (*.*)|*.*",
                DefaultExt = ".tex",
                Title = "Lưu tệp TeX"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _isSaving = true;
                    string content = textEditor.Text;
                    _expectedContentHash = ComputeHash(content);
                    File.WriteAllText(saveFileDialog.FileName, content);
                    _currentFilePath = saveFileDialog.FileName;
                    textEditor.IsModified = false;
                    StartWatchingFile(_currentFilePath);
                    this.Title = $"FixMathpix 2025 - {_currentFilePath}";
                    _lastFileWriteTime = File.GetLastWriteTimeUtc(saveFileDialog.FileName);
                    _expectedSaveWriteTimeUtc = _lastFileWriteTime;
                    UpdateStatusBarFile();
                    StatusMessageTextBlock.Text = "Đã lưu tệp mới thành công";
                    MessageBox.Show("Đã lưu tệp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể lưu tệp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    _isSaving = false; // Bỏ cờ sau khi lưu xong
                }
            }
            return false;
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem có thay đổi chưa được lưu không
            if (textEditor.IsModified)
            {
                var result = MessageBox.Show(
                    "Tệp hiện tại có thay đổi chưa được lưu. Bạn có muốn lưu lại không?",
                    "Lưu thay đổi",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (SaveFileInternal())
                    {
                        PerformFileClose();
                    }
                    return;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            PerformFileClose();
        }

        private void PerformFileClose()
        {
            // Dừng theo dõi tệp
            _fileWatcher.EnableRaisingEvents = false;

            // Xóa nội dung và đặt lại trạng thái
            textEditor.Clear();
            _currentFilePath = null;
            textEditor.IsModified = false; // Đặt lại trạng thái đã sửa đổi

            // Cập nhật tiêu đề cửa sổ
            this.Title = "FixMathpix 2025";
            UpdateStatusBarFile();
            StatusMessageTextBlock.Text = "Đã đóng tệp";
        }

        private void UpdateStatusBarFile()
        {
            if (FilePathTextBlock != null)
            {
                FilePathTextBlock.Text = string.IsNullOrEmpty(_currentFilePath) ? "Chưa mở tệp nào" : _currentFilePath;
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            if (CaretPositionTextBlock != null && textEditor != null && textEditor.TextArea != null && textEditor.TextArea.Caret != null)
            {
                CaretPositionTextBlock.Text = $"Ln {textEditor.TextArea.Caret.Line}, Col {textEditor.TextArea.Caret.Column}";
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleCaseDang_Click(object sender, RoutedEventArgs e)
        {
            string text = textEditor.Text;
            string pattern = @"\\begin\{dang\}\{([^}]+)\}";

            var matches = System.Text.RegularExpressions.Regex.Matches(text, pattern);

            if (matches.Count == 0)
            {
                MessageBox.Show("Không tìm thấy tiêu đề dạng toán nào.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            textEditor.Document.BeginUpdate();
            try
            {
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    var match = matches[i];
                    var titleGroup = match.Groups[1];
                    string originalTitle = titleGroup.Value;
                    string newTitle = TitleCaseMath(originalTitle);

                    if (originalTitle != newTitle)
                    {
                        textEditor.Document.Replace(titleGroup.Index, titleGroup.Length, newTitle);
                    }
                }
            }
            finally
            {
                textEditor.Document.EndUpdate();
            }
            MessageBox.Show($"Đã viết hoa {matches.Count} tiêu đề dạng toán.", "Hoàn thành", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UppercaseSection_Click(object sender, RoutedEventArgs e)
        {
            string text = textEditor.Text;
            // Regex tìm các lệnh section, subsection, subsubsection (có thể có *)
            // Sử dụng balancing groups để xử lý đúng các dấu ngoặc nhọn lồng nhau
            string pattern = @"\\(sub){0,2}section\*?\{(?<content>(?>[^{}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))\}";

            var matches = Regex.Matches(text, pattern);

            if (matches.Count == 0)
            {
                MessageBox.Show("Không tìm thấy mục (section) nào.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            textEditor.Document.BeginUpdate();
            try
            {
                // Duyệt ngược để thay thế không làm sai lệch chỉ số
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    var match = matches[i];
                    var contentGroup = match.Groups["content"];
                    string originalContent = contentGroup.Value;

                    string newContent = UppercaseSectionContent(originalContent);

                    if (originalContent != newContent)
                    {
                        textEditor.Document.Replace(contentGroup.Index, contentGroup.Length, newContent);
                    }
                }
            }
            finally
            {
                textEditor.Document.EndUpdate();
            }
            MessageBox.Show($"Đã viết hoa {matches.Count} mục.", "Hoàn thành", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string UppercaseSectionContent(string input)
        {
            StringBuilder sb = new StringBuilder();
            bool inMath = false;
            bool inCommand = false;

            foreach (char c in input)
            {
                if (inMath)
                {
                    sb.Append(c);
                    if (c == '$') inMath = false;
                }
                else
                {
                    if (c == '$')
                    {
                        inMath = true;
                        sb.Append(c);
                    }
                    else if (c == '\\')
                    {
                        inCommand = true;
                        sb.Append(c);
                    }
                    else if (inCommand)
                    {
                        sb.Append(c);
                        // Lệnh kết thúc khi gặp ký tự không phải chữ cái (ví dụ: khoảng trắng, {, }, số...)
                        if (!char.IsLetter(c))
                        {
                            inCommand = false;
                        }
                    }
                    else
                    {
                        sb.Append(char.ToUpper(c));
                    }
                }
            }
            return sb.ToString();
        }

        private void SortQuestions_Click(object sender, RoutedEventArgs e)
        {
            bool isSelection = textEditor.SelectionLength > 0;
            string textToProcess = isSelection ? textEditor.SelectedText : textEditor.Text;
            int baseOffset = isSelection ? textEditor.SelectionStart : 0;
            string bai = (BaiComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "01";

            textEditor.Document.BeginUpdate();
            try
            {
                string newText = QuestionSortingService.ProcessSortQuestions(textToProcess, bai, out int sortedCount);

                if (sortedCount == 0)
                {
                    MessageBox.Show("Không tìm thấy câu hỏi nào để sắp xếp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (isSelection)
                {
                    textEditor.Document.Replace(baseOffset, textEditor.SelectionLength, newText);
                }
                else
                {
                    textEditor.Document.Text = newText;
                }

                StatusMessageTextBlock.Text = $"Đã sắp xếp {sortedCount} câu hỏi";
                MessageBox.Show($"Đã sắp xếp các câu hỏi theo cấu trúc mới.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                textEditor.Document.EndUpdate();
            }
        }

        private int ClassifyQuestion(string content)
        {
            // 1: Trắc nghiệm (\choice)
            // 2: Đúng sai (\choiceTF)
            // 3: Trả lời ngắn (\shortans)
            // 4: Tự luận (Còn lại)

            // Kiểm tra \choiceTF trước vì nó chứa từ "choice"
            if (content.IndexOf(@"\choiceTF", StringComparison.OrdinalIgnoreCase) >= 0) return 2;
            if (content.IndexOf(@"\choiceTF[t]", StringComparison.OrdinalIgnoreCase) >= 0) return 2;
            if (content.IndexOf(@"\choice", StringComparison.OrdinalIgnoreCase) >= 0) return 1;
            if (content.IndexOf(@"\shortans", StringComparison.OrdinalIgnoreCase) >= 0) return 3;
            return 4;
        }

        private string TitleCaseMath(string input)
        {
            var result = new System.Text.StringBuilder();
            bool inMath = false;

            foreach (char c in input)
            {
                if (c == '$')
                {
                    inMath = !inMath;
                    result.Append(c); 
                }
                else if (inMath)
                {
                    result.Append(c);
                }
                else
                {
                    result.Append(char.ToUpper(c));
                }
            }

            return result.ToString();
        }

        private void CleanupText_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textEditor.Text)) return;

            try
            {
                string text = textEditor.Text;

                // 1. Chuẩn hóa ký tự xuống dòng: Xóa \r\n thành \n
                text = text.Replace("\r\n", "\n");

                // 2. Xóa các tab ở đầu dòng
                text = Regex.Replace(text, @"^\t+", "", RegexOptions.Multiline);

                // 3. Sửa lỗi dấu câu trong tiếng Việt
                // Xóa khoảng trắng thừa trước các dấu câu
                text = Regex.Replace(text, @"\s+([,.;:?!])", "$1");
                text = Regex.Replace(text, @"^\s", "");
                // Đảm bảo có một khoảng trắng sau dấu câu (nếu theo sau là một chữ cái/số)
                text = Regex.Replace(text, @"([,.;:?!])(\w)", "$1 $2");

                // 4. Xóa nhiều khoảng trắng liên tục (chỉ xóa space, không xóa các loại whitespace khác như tab, newline)
                text = Regex.Replace(text, @"[ ]{2,}", " ");

                textEditor.Text = text;
                MessageBox.Show("Đã dọn dẹp văn bản thành công!", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi dọn dẹp văn bản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleComment_Click(object sender, RoutedEventArgs e)
        {
            var document = textEditor.Document;
            int selectionStart = textEditor.SelectionStart;
            int selectionEnd = textEditor.SelectionStart + textEditor.SelectionLength;

            if (selectionStart == selectionEnd) // No selection, toggle current line
            {
                var line = document.GetLineByOffset(selectionStart);
                ToggleLineComment(document, line);
            }
            else // Toggle all lines in selection
            {
                var startLine = document.GetLineByOffset(selectionStart);
                var endLine = document.GetLineByOffset(selectionEnd);

                // If the selection ends exactly at the beginning of a new line, don't include that line.
                if (endLine.Offset == selectionEnd && startLine.LineNumber != endLine.LineNumber)
                {
                    endLine = endLine.PreviousLine;
                }

                document.BeginUpdate();
                try
                {
                    for (int i = startLine.LineNumber; i <= endLine.LineNumber; i++)
                    {
                        ToggleLineComment(document, document.GetLineByNumber(i));
                    }
                }
                finally
                {
                    document.EndUpdate();
                }
            }
        }

        private void ToggleLineComment(IDocument document, IDocumentLine line)
        {
            string lineText = document.GetText(line);
            if (lineText.TrimStart().StartsWith("%"))
            {
                // Uncomment: remove the first '%'
                document.Replace(line.Offset + lineText.IndexOf('%'), 1, "");
            }
            else
            {
                // Comment: add '%' at the beginning
                document.Insert(line.Offset, "%");
            }
        }

        private void GoToLine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new GoToLineDialog(textEditor.TextArea.Caret.Line, textEditor.Document.LineCount)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                int line = dialog.LineNumber;
                if (line >= 1 && line <= textEditor.Document.LineCount)
                {
                    textEditor.ScrollTo(line, -1);
                    var documentLine = textEditor.Document.GetLineByNumber(line);
                    textEditor.CaretOffset = documentLine.Offset;
                    textEditor.Focus();
                }
                else
                {
                    MessageBox.Show(this, $"Số dòng phải nằm trong khoảng từ 1 đến {textEditor.Document.LineCount}.", "Số dòng không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }



        #endregion

        private void UpdateFolding()
        {
            if (_foldingManager != null && _foldingStrategy != null)
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
        }

        private void CollapseAllFoldings_Click(object sender, RoutedEventArgs e)
        {
            if (_foldingManager != null)
            {
                foreach (FoldingSection folding in _foldingManager.AllFoldings)
                {
                    folding.IsFolded = true;
                }
            }
        }

        private void ExpandAllFoldings_Click(object sender, RoutedEventArgs e)
        {
            if (_foldingManager != null)
            {
                foreach (FoldingSection folding in _foldingManager.AllFoldings)
                {
                    folding.IsFolded = false;
                }
            }
        }

        private void InsertImageWizardButton_Click(object sender, RoutedEventArgs e)
        {
            string basePath = null;
            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                basePath = Path.GetDirectoryName(_currentFilePath);
            }

            var window = new InsertImageWindow(basePath);
            window.Owner = this;
            window.ImageInserted += (latex) =>
            {
                textEditor.Document.Insert(textEditor.CaretOffset, latex);
            };
            window.Show();
        }
    }

    /// <summary>
    /// Chiến lược để tìm các khối môi trường LaTeX (\begin{...}...\end{...}) để thu gọn.
    /// </summary>
    public class LatexFoldingStrategy
    {
        // Danh sách các môi trường được phép thu gọn
        private readonly HashSet<string> _foldableEnvironments = new HashSet<string>
        {
            "cauhoiTN",
            "cauhoiDS",
            "cauhoiTL", 
            "cauhoiTLN",
            "vidu",
            "dang",
            "dang*",
            "ex",
            "bt"
        };

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document);
            manager.UpdateFoldings(newFoldings, -1);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document)
        {
            var newFoldings = new List<NewFolding>();
            var startTags = new Stack<Tuple<int, string>>();

            // Regex để tìm \begin{env} và \end{env}
            var regex = new Regex(@"\\(begin|end)\{([a-zA-Z0-9\*]+)\}", RegexOptions.Compiled);

            foreach (Match match in regex.Matches(document.Text))
            {
                string type = match.Groups[1].Value;
                string name = match.Groups[2].Value;

                if (type == "begin")
                {
                    // Chỉ thêm vào stack nếu môi trường này được phép thu gọn
                    if (_foldableEnvironments.Contains(name))
                    {
                        startTags.Push(Tuple.Create(match.Index, name));
                    }
                }
                else if (type == "end" && startTags.Count > 0 && startTags.Peek().Item2 == name)
                {
                    var startTag = startTags.Pop();
                    newFoldings.Add(new NewFolding(startTag.Item1, match.Index + match.Length)
                    {
                        // Đặt tên cho vùng folding để hiển thị khi thu gọn
                        Name = $"... {name} ..."
                    });
                }
            }
            // Sắp xếp các vùng gấp theo vị trí bắt đầu để tránh lỗi
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}
