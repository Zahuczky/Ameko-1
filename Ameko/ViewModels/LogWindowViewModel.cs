using Ameko.DataModels;
using Ameko.Services;
using Avalonia.Threading;
using DynamicData;
using Holo;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class LogWindowViewModel : ViewModelBase
    {
        private List<Log> _logs;

        public List<Log> Logs
        {
            get => _logs;
            private set => this.RaiseAndSetIfChanged(ref _logs, value);
        }

        public LogWindowViewModel()
        {
            _logs = new List<Log>(HoloContext.Instance.Logger.Logs);
            _logs.Reverse();

            HoloContext.Instance.Logger.PropertyChanged += Logger_LogAdded;
        }

        private void Logger_LogAdded(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Logs = new List<Log>(HoloContext.Instance.Logger.Logs);
            Logs.Reverse();
        }
    }
}
