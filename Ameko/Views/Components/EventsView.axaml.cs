using Ameko.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Holo;

namespace Ameko.Views.Components
{
    public partial class EventsView : UserControl
    {
        public static readonly StyledProperty<FileWrapper> FileWrapperProperty = AvaloniaProperty.Register<EventsView, FileWrapper>(nameof(FileWrapper));
        public FileWrapper FileWrapper
        {
            get => GetValue(FileWrapperProperty);
            set => SetValue(FileWrapperProperty, value);
        }

        public EventsView()
        {
            InitializeComponent();
        }
    }
}
