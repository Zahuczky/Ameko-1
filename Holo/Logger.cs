using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Holo
{
    /// <summary>
    /// Logger
    /// </summary>
    public class Logger : INotifyPropertyChanged
    {
        internal static readonly Logger Instance = new Logger();

        private readonly List<Log> _logs = new List<Log>();

        /// <summary>
        /// List of logs
        /// </summary>
        public List<Log> Logs => _logs;

        public void Info(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.INFO));
            OnPropertyChanged(nameof(Logs));
        }

        public void Debug(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.DEBUG));
            OnPropertyChanged(nameof(Logs));
        }

        public void Warn(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.WARN));
            OnPropertyChanged(nameof(Logs));
        }

        public void Error(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.ERROR));
            OnPropertyChanged(nameof(Logs));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Logger() { }
    }

    public class Log
    {
        private readonly LogLevel _logLevel;
        private readonly string _message;
        private readonly string _source;
        private readonly DateTime _timestamp;

        public Log(string message, string source, LogLevel logLevel)
        {
            _source = source;
            _message = message;
            _logLevel = logLevel;
            _timestamp = DateTime.Now;
        }

        public string Message => _message;
        public string Source => _source;
        public LogLevel LogLevel => _logLevel;
        public string LogLevelString => _logLevel.ToString();
        public DateTime Timestamp => _timestamp;
        public string TimeString => _timestamp.ToString("yyyy-MM-ddTHH:mm:ss");

        public override string ToString()
        {
            return $"{_timestamp:yyyy-MM-ddTHH:mm:ss} [{_logLevel}] {(_source != string.Empty ? $"({_source}) " : "")}→ {_message}";
        }
    }

    public enum LogLevel
    {
        INFO,
        DEBUG,
        WARN,
        ERROR
    }
}
