using Ameko.ViewModels;
using AssCS;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Ameko.Views
{
    public partial class TabView : ReactiveUserControl<TabItemViewModel>
    {
        private async Task DoCopySelectedEventAsync(InteractionContext<TabItemViewModel, string?> interaction)
        {
            var window = TopLevel.GetTopLevel(this);
            var selected = interaction.Input.Wrapper.SelectedEvents;
            if (window == null || selected == null)
            {
                interaction.SetOutput("");
                return;
            }
            var result = string.Join("\n", selected.Select(e => e.AsAss()));
            await window.Clipboard!.SetTextAsync(result);
            interaction.SetOutput(result);
        }

        private async Task DoCutSelectedEventAsync(InteractionContext<TabItemViewModel, string?> interaction)
        {
            var window = TopLevel.GetTopLevel(this);
            var selectedEvents = interaction.Input.Wrapper.SelectedEvents;
            var selectedEvent = interaction.Input.Wrapper.SelectedEvent;
            if (window == null || selectedEvents == null || selectedEvent == null)
            {
                interaction.SetOutput("");
                return;
            }
            var result = string.Join("\n", selectedEvents.Select(e => e.AsAss()));
            await window.Clipboard!.SetTextAsync(result);
            interaction.Input.Wrapper.Remove(selectedEvents, selectedEvent);
            interaction.SetOutput(result);
        }

        private async Task DoPasteAsync(InteractionContext<TabItemViewModel, string?> interaction)
        {
            // TODO: Distinguish between lines and raw text

            var window = TopLevel.GetTopLevel(this);
            var file = interaction.Input.Wrapper.File;
            var selectedId = interaction.Input.Wrapper.SelectedEvent?.Id ?? -1;
            if (window == null || selectedId == -1)
            {
                interaction.SetOutput("");
                return;
            }
            var result = await window.Clipboard!.GetTextAsync();
            if (result != null)
            {
                foreach (var linedata in result.Split("\n"))
                {
                    if (linedata.Trim().Equals(string.Empty)) continue;

                    var line = new Event(file.EventManager.NextId, linedata.Trim());
                    selectedId = file.EventManager.AddAfter(selectedId, line);
                }
            }
            interaction.SetOutput(result);
        }

        public TabView()
        {
            InitializeComponent();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.CopySelectedEvents.RegisterHandler(DoCopySelectedEventAsync);
                    ViewModel.CutSelectedEvents.RegisterHandler(DoCutSelectedEventAsync);
                    ViewModel.Paste.RegisterHandler(DoPasteAsync);

                    eventsGrid.SelectionChanged += (o, e) =>
                    {
                        List<Event> list = eventsGrid.SelectedItems.Cast<Event>().ToList();
                        Event recent = (Event)eventsGrid.SelectedItem;
                        ViewModel?.UpdateEventSelection(list, recent);
                        eventsGrid.ScrollIntoView(eventsGrid.SelectedItem, null);
                    };
                }

                Disposable.Create(() => { }).DisposeWith(disposables);
            });
        }

        private void TextBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                if (e.KeyModifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift))
                {
                    int idx = editBox.CaretIndex;
                    editBox.Text = editBox.Text?.Insert(idx, "\\N");
                    editBox.CaretIndex += 2;
                }
            }
        }
    }
}
