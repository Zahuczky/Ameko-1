using AssCS;
using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class SearchWindowViewModel : ViewModelBase
    {
        private string title = "Search";
        public string WindowTitle
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }
        public string Query { get; set; } = string.Empty;
        public ICommand FindNextCommand { get; }

        private MainViewModel mainVM;
        private string? previousQuery;
        private Event[]? queryResults;
        private int queryIndex;

        private void GenerateQueryResults()
        {
            queryIndex = 0;
            var currentFile = HoloContext.Instance.Workspace.WorkingFile;
            WindowTitle = $"Search: {currentFile.Title}";
            queryResults = currentFile.File.EventManager.Ordered.Where(e => e.Text.Contains(Query, StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }

        public SearchWindowViewModel(MainViewModel mainVM) 
        {
            this.mainVM = mainVM;

            FindNextCommand = ReactiveCommand.Create(async () =>
            {
                // if the query changed, generate a new set of results
                if (!Query.Equals(previousQuery))
                {
                    GenerateQueryResults();
                    previousQuery = Query;
                }
                if (queryResults == null || queryResults.Length == 0) return;

                // Loop back if needed
                if (queryIndex >= queryResults.Length) queryIndex = 0;

                var tabVM = mainVM.Tabs[mainVM.SelectedTabIndex];

                var interaction = tabVM.ScrollIntoViewInteraction;
                await interaction.Handle(queryResults[queryIndex++]);
            });
        }
    }
}
