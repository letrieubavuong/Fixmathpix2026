using System.Windows.Input;

namespace FixMathpix2025
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Bold = new RoutedUICommand("Bold", "Bold", typeof(CustomCommands));
        public static readonly RoutedUICommand Italic = new RoutedUICommand("Italic", "Italic", typeof(CustomCommands));
        public static readonly RoutedUICommand Underline = new RoutedUICommand("Underline", "Underline", typeof(CustomCommands));
        public static readonly RoutedUICommand ToggleComment = new RoutedUICommand("ToggleComment", "ToggleComment", typeof(CustomCommands));
        public static readonly RoutedUICommand CleanupText = new RoutedUICommand("CleanupText", "CleanupText", typeof(CustomCommands));
        public static readonly RoutedUICommand MathMode = new RoutedUICommand("MathMode", "MathMode", typeof(CustomCommands));
        public static readonly RoutedUICommand SpellCheck = new RoutedUICommand("SpellCheck", "SpellCheck", typeof(CustomCommands));
        public static readonly RoutedUICommand InsertLoigiai = new RoutedUICommand("InsertLoigiai", "InsertLoigiai", typeof(CustomCommands));
        public static readonly RoutedUICommand ChuyenTeX = new RoutedUICommand("ChuyenTeX", "ChuyenTeX", typeof(CustomCommands));

        // Thêm lệnh mới cho việc chèn môi trường
        public static readonly RoutedUICommand InsertEnvironment = new RoutedUICommand(
            "Insert Environment",
            "InsertEnvironment",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.E, ModifierKeys.Control) }
        );

        // Thêm lệnh mới cho việc chèn hình ảnh (figure)
        public static readonly RoutedUICommand InsertFigure = new RoutedUICommand(
            "Insert Figure",
            "InsertFigure",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift) }
        );

        public static readonly RoutedUICommand GoToLine = new RoutedUICommand(
            "GoToLine",
            "GoToLine",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.G, ModifierKeys.Control) }
        );
    }
}