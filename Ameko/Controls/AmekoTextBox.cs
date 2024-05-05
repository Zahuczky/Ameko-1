using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Controls
{
    /// <summary>
    /// This is an insanely stupid workaround to TextBox.OnLostFocus() always
    /// calling ClearSelection(), which breaks the toggle buttons.
    /// This wrapper saves the selection end before clearing the selection
    /// so the selection end can be used for the toggle buttons.
    /// If/When Avalonia#11693 (https://github.com/AvaloniaUI/Avalonia/issues/11693) is resolved,
    /// this wrapper should be nuked from existence, and the ClearSelectionOnLostFocus flag used instead. 
    /// </summary>
    public class AmekoTextBox : TextBox
    {
        public int _focusLostSelectionEnd;

        public int FocusLostSelectionEnd
        {
            get => _focusLostSelectionEnd;
        }

        protected override Type StyleKeyOverride { get { return typeof(TextBox); } }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            _focusLostSelectionEnd = SelectionEnd;
            base.OnLostFocus(e);
        }
    }
}
