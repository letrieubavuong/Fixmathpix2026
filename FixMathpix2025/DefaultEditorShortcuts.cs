using System.Collections.Generic;

namespace FixMathpix2025
{
    public static class DefaultEditorShortcuts
    {
        public static Dictionary<string, string> GetDefaultShortcuts()
        {
            return new Dictionary<string, string>
            {
                { "ApplicationCommands.Open", "Ctrl+O" },
                { "ApplicationCommands.Save", "Ctrl+S" },
                { "ApplicationCommands.Find", "Ctrl+F" },
                { "ApplicationCommands.Replace", "Ctrl+H" },
                { "local:CustomCommands.Bold", "Ctrl+B" },
                { "local:CustomCommands.Italic", "Ctrl+I" },
                { "local:CustomCommands.Underline", "Ctrl+U" },
                { "avalonedit:AvalonEditCommands.ConvertToUppercase", "Ctrl+Shift+U" },
                { "local:CustomCommands.ToggleComment", "Ctrl+/" },
                { "local:CustomCommands.MathMode", "Ctrl+M" },
                { "local:CustomCommands.InsertLoigiai", "Ctrl+L" },
                { "local:CustomCommands.SpellCheck", "Ctrl+F7" },
                { "local:CustomCommands.CleanupText", "Ctrl+Alt+L" }
            };
        }
    }
}
