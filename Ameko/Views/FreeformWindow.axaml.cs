using Ameko.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaEdit.Document;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using TextMateSharp.Grammars;

namespace Ameko.Views
{
    public partial class FreeformWindow : ReactiveWindow<FreeformWindowViewModel>
    {
        private readonly RegistryOptions _registryOptions;
        private readonly TextMate.Installation _textMateInstallation;

        public FreeformWindow()
        {
            InitializeComponent();

            _registryOptions = new RegistryOptions(ThemeName.Monokai);
            _textMateInstallation = Editor.InstallTextMate(_registryOptions);
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByExtension(".cs"));

            Editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(Editor.Options);
        }
    }
}
