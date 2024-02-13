using Ameko.ViewModels;
using AssCS;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.Remote.Protocol.Input;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Ameko.Views
{
    public partial class TabView : ReactiveUserControl<TabItemViewModel>
    {
        private List<TabItemViewModel> previousVMs;

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

        private void DoScrollIntoView(InteractionContext<Event, Unit> interaction)
        {
            if (interaction == null) return;
            eventsGrid.SelectedItem = interaction.Input;
            eventsGrid.ScrollIntoView(interaction.Input, null);
            interaction.SetOutput(Unit.Default);
        }

        public TabView()
        {
            InitializeComponent();
            previousVMs = new List<TabItemViewModel>();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.GetObservable(ViewModelProperty).WhereNotNull()
                .Subscribe(vm =>
                {
                    // Skip if already subscribed
                    if (previousVMs.Contains(vm)) return;
                    previousVMs.Add(vm);

                    vm.CopySelectedEvents.RegisterHandler(DoCopySelectedEventAsync);
                    vm.CutSelectedEvents.RegisterHandler(DoCutSelectedEventAsync);
                    vm.Paste.RegisterHandler(DoPasteAsync);
                    vm.ScrollIntoViewInteraction.RegisterHandler(DoScrollIntoView);

                    startBox.AddHandler(InputElement.KeyDownEvent, TimeBox_PreKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
                    endBox.AddHandler(InputElement.KeyDownEvent, TimeBox_PreKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);

                    eventsGrid.SelectionChanged += EventsGrid_SelectionChanged;
                })
                .DisposeWith(disposables);
            });
        }

        private void EventsGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            List<Event> list = eventsGrid.SelectedItems.Cast<Event>().ToList();
            Event recent = (Event)eventsGrid.SelectedItem;
            ViewModel?.UpdateEventSelection(list, recent);
            eventsGrid.ScrollIntoView(eventsGrid.SelectedItem, null);
            editBox.Focus();
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
                } else
                {
                    ViewModel?.NextOrAddEventCommand.Execute(Unit.Default);
                    editBox.Focus();
                }
            }
        }

        /// <summary>
        /// This (pre)override is needed to capture [BACKSPACE] and the arrow keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeBox_PreKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (sender == null) return;
            var box = (TextBox)sender;
            if (box.Text == null) return;

            // TODO: cut/copy/insert handling

            // Movement Keys
            switch (e.Key)
            {
                case Avalonia.Input.Key.Back:
                case Avalonia.Input.Key.Left:
                case Avalonia.Input.Key.Down:
                    box.CaretIndex -= 1;
                    e.Handled = true;
                    return;
                case Avalonia.Input.Key.Home:
                    box.CaretIndex = 0;
                    e.Handled = true;
                    return;
                case Avalonia.Input.Key.End:
                    box.CaretIndex = box.Text.Length;
                    e.Handled = true;
                    return;
                case Avalonia.Input.Key.Right:
                case Avalonia.Input.Key.Up:
                    box.CaretIndex += 1;
                    e.Handled = true;
                    return;
                default:
                    break;
            }

            // Discard if at the end
            if (box.CaretIndex >= box.Text.Length)
            {
                e.Handled = true;
                return;
            }

            // Discard if invalid character
            var key = Convert.ToChar(e.KeySymbol ?? "a");
            if ((key < '0' || key > '9') && key != ';' && key != '.' && key != ',' && key != ':')
            {
                e.Handled = true;
                return;
            }

            // Move forward if at punctuation
            var next = box.Text?[box.CaretIndex] ?? '-';
            if (next == ':' || next == '.' || next == ',')
            {
                box.CaretIndex += 1;
            }

            // Nothing more needed for punctuation
            if (key == ';' || key == '.' || key == ',' || key == ':')
            {
                e.Handled = true;
                return;
            }

            box.Text = box.Text?.Remove(box.CaretIndex, 1)
                .Insert(box.CaretIndex, Convert.ToString(key));
            box.CaretIndex += 1;
            e.Handled = true;
            return;
        }
    }
}
