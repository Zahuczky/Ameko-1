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
        }

        public void Debug(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.DEBUG));
        }

        public void Warn(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.WARN));
        }

        public void Error(string message, string source = "")
        {
            _logs.Add(new Log(message, source, LogLevel.ERROR));
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
        public readonly LogLevel LogLevel;
        public readonly string Message;
        public readonly string Source;
        public readonly DateTime Timestamp;

        public Log(string message, string source, LogLevel logLevel)
        {
            Source = source;
            Message = message;
            LogLevel = logLevel;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-ddTHH:mm:ss} [{LogLevel}] {(Source != string.Empty ? $"({Source}) " : "")}→ {Message}";
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
