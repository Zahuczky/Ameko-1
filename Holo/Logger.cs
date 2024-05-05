using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private readonly string logsRoot;

        /// <summary>
        /// List of logs
        /// </summary>
        public List<Log> Logs => _logs;

        public void Info(string message, string source = "")
        {
            Log log = new Log(message, source, LogLevel.INFO);
            _logs.Add(log);
            WriteLogToFile(log);
            OnPropertyChanged(nameof(Logs));
        }

        public void Debug(string message, string source = "")
        {
            Log log = new Log(message, source, LogLevel.DEBUG);
            _logs.Add(log);
            WriteLogToFile(log);
            OnPropertyChanged(nameof(Logs));
        }

        public void Warn(string message, string source = "")
        {
            Log log = new Log(message, source, LogLevel.WARN);
            _logs.Add(log);
            WriteLogToFile(log);
            OnPropertyChanged(nameof(Logs));
        }

        public void Error(string message, string source = "")
        {
            Log log = new Log(message, source, LogLevel.ERROR);
            _logs.Add(log);
            WriteLogToFile(log);
            OnPropertyChanged(nameof(Logs));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void WriteLogToFile(Log log)
        {
            var filename = Path.Combine(logsRoot, $"log_{log.Timestamp:yyyy-MM-dd}.txt");
            using (StreamWriter writer = File.AppendText(filename))
            {
                writer.WriteLine(log);
            }
        }

        private Logger()
        {
            logsRoot = Path.Combine(HoloContext.Directories.HoloStateHome, "logs");
            if (!Directory.Exists(logsRoot))
                Directory.CreateDirectory(logsRoot);
        }
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
