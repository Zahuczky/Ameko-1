using Ameko.DataModels;
using AssCS.IO;
using Avalonia.Threading;
using Holo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public class AutosaveService
    {
        private DispatcherTimer timer;
        private string autosaveDir;
        private bool isRunning = false;

        private void Tick(object? sender, EventArgs e)
        {
            foreach (var file in HoloContext.Instance.Workspace.Files)
            {
                if (file.UpToDate) continue;

                string filename;
                if (file.FilePath != null)
                    filename = Path.Combine(
                        autosaveDir,
                        $"{Path.GetFileNameWithoutExtension(file.FilePath.LocalPath)}#{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.ass"
                    );
                else
                    filename = Path.Combine(
                        autosaveDir,
                        $"{file.Title}#{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.ass"
                    );
                var writer = new AssWriter(file.File, filename, AmekoInfo.Instance);
                writer.Write(false);
            }
        }

        public void Start()
        {
            if (isRunning) return;
            timer.Start();
            isRunning = true;
        }

        public void Stop()
        {
            if (!isRunning) return;
            timer.Stop();
            isRunning = false;
        }

        public AutosaveService()
        {
            autosaveDir = Path.Combine(HoloContext.Directories.AmekoDataHome, "autosave");

            if (!Directory.Exists(autosaveDir))
            {
                Directory.CreateDirectory(autosaveDir);
            }

            timer = new DispatcherTimer();
            timer.Tick += Tick;
            timer.Interval = new TimeSpan(0, 0, HoloContext.Instance.ConfigurationManager.AutosaveInterval);
            if (HoloContext.Instance.ConfigurationManager.Autosave) Start();

            HoloContext.Instance.ConfigurationManager.PropertyChanged += ConfigurationManager_PropertyChanged;
        }

        private void ConfigurationManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var cm = sender as ConfigurationManager;
            switch (e.PropertyName)
            {
                case nameof(ConfigurationManager.Autosave):
                    if (cm != null && cm.Autosave) Start();
                    else Stop();
                    break;
                case nameof(ConfigurationManager.AutosaveInterval):
                    if (cm != null)
                        timer.Interval = new TimeSpan(0, 0, HoloContext.Instance.ConfigurationManager.AutosaveInterval);
                    break;
            }
        }
    }
}
