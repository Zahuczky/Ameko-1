using Ameko.DataModels;
using Ameko.Services;
using Ameko.ViewModels;
using AssCS;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.Remote.Protocol.Input;
using DynamicData;
using HarfBuzzSharp;
using Holo;
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

        private async Task DoPasteAsync(InteractionContext<TabItemViewModel, string[]?> interaction)
        {
            var window = TopLevel.GetTopLevel(this);
            if (window == null)
            {
                interaction.SetOutput([]);
                return;
            }
            var result = await window.Clipboard!.GetTextAsync();
            if (result != null)
            {
                interaction.SetOutput(result.Split("\n"));
                return;
            }
            else
                interaction.SetOutput([]);
        }

        private async Task DoShowPasteOverDialogAsync(InteractionContext<PasteOverWindowViewModel, PasteOverField> interaction)
        {
            var window = TopLevel.GetTopLevel(this);
            if (window == null)
            {
                interaction.SetOutput(PasteOverField.None);
                return;
            }
            var dialog = new PasteOverWindow();
            dialog.DataContext = interaction.Input;
            var result = await dialog.ShowDialog<PasteOverField>((Window)window);
            // thonk
            interaction.SetOutput(interaction.Input.Fields);
        }

        private void DoScrollIntoView(InteractionContext<Event, Unit> interaction)
        {
            if (interaction == null) return;
            interaction.SetOutput(Unit.Default);
            eventsGrid.SelectedItem = interaction.Input;
            eventsGrid.ScrollIntoView(interaction.Input, null);
        }

        private void SetKeybinds()
        {
            if (ViewModel == null) return;
            // GRID
            eventsGrid.KeyBindings.Clear();
            KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, "ameko.event.duplicate", ViewModel.DuplicateSelectedEventsCommand);
            KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, "ameko.event.copy", ViewModel.CopySelectedEventsCommand);
            KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, "ameko.event.cut", ViewModel.CutSelectedEventsCommand);
            KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, "ameko.event.paste", ViewModel.PasteCommand);
            KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, "ameko.event.pasteover", ViewModel.PasteOverCommand);
            KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, "ameko.event.delete", ViewModel.DeleteSelectedCommand);
            foreach (var pair in HoloContext.Instance.ConfigurationManager.KeybindsRegistry.GridBinds)
            {
                if (pair.Key.StartsWith("ameko")) continue; // Skip builtins
                KeybindService.TrySetKeybind(eventsGrid, KeybindContext.GRID, pair.Key, ViewModel.ActivateScriptCommand, pair.Key);
            }

            //EDIT
            editorPanel.KeyBindings.Clear();
            foreach (var pair in HoloContext.Instance.ConfigurationManager.KeybindsRegistry.EditBinds)
            {
                if (pair.Key.StartsWith("ameko")) continue; // Skip builtins
                KeybindService.TrySetKeybind(editorPanel, KeybindContext.EDIT, pair.Key, ViewModel.ActivateScriptCommand, pair.Key);
            }
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
                    // Every time
                    DiscordRPCService.Instance.Set($"{vm.Title}.ass", HoloContext.Instance.Workspace.Name);
                    SetKeybinds();

                    // Skip the rest if already subscribed
                    if (previousVMs.Contains(vm)) return;
                    previousVMs.Add(vm);

                    vm.CopySelectedEvents.RegisterHandler(DoCopySelectedEventAsync);
                    vm.CutSelectedEvents.RegisterHandler(DoCutSelectedEventAsync);
                    vm.Paste.RegisterHandler(DoPasteAsync);
                    vm.ScrollIntoViewInteraction.RegisterHandler(DoScrollIntoView);
                    vm.ShowPasteOverFieldDialog.RegisterHandler(DoShowPasteOverDialogAsync);

                    startBox.AddHandler(InputElement.KeyDownEvent, Helpers.TimeBox_PreKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
                    endBox.AddHandler(InputElement.KeyDownEvent, Helpers.TimeBox_PreKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);

                    eventsGrid.SelectionChanged += EventsGrid_SelectionChanged;

                    HoloContext.Instance.ConfigurationManager.PropertyChanged += ConfigurationManager_PropertyChanged;
                })
                .DisposeWith(disposables);
            });
        }

        private void ConfigurationManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HoloContext.Instance.ConfigurationManager.KeybindsRegistry):
                    SetKeybinds();
                    break;
            }
        }

        private void EventsGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            List<Event> list = eventsGrid.SelectedItems.Cast<Event>().ToList();
            Event recent = (Event)eventsGrid.SelectedItem;
            ViewModel?.UpdateEventSelection(list, recent);
            eventsGrid.ScrollIntoView(eventsGrid.SelectedItem, null);
            editBox.Focus();
        }

        /// <summary>
        /// Check the workspace and the configuration to determine whether to use soft or hard linebreaks
        /// </summary>
        private static bool UseSoftLinebreaks
        {
            get
            {
                if (HoloContext.Instance.Workspace.UseSoftLinebreaks != null)
                    return HoloContext.Instance.Workspace.UseSoftLinebreaks ?? false;
                return HoloContext.Instance.ConfigurationManager?.UseSoftLinebreaks ?? false;
            }
        }

        private void TextBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                if (e.KeyModifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift))
                {
                    int idx = editBox.CaretIndex;
                    editBox.Text = editBox.Text?.Insert(idx, UseSoftLinebreaks ? "\\n" : "\\N");
                    editBox.CaretIndex += 2;
                } else
                {
                    ViewModel?.NextOrAddEventCommand.Execute(Unit.Default);
                    editBox.Focus();
                }
            }
        }
    }
}
