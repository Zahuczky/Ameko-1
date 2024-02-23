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
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void SetInterval(TimeSpan interval)
        {
            timer.Interval = interval;
        }

        public AutosaveService()
        {
            autosaveDir = Path.Combine(HoloContext.AmekoDirectory, "autosave");

            if (!Directory.Exists(autosaveDir))
            {
                Directory.CreateDirectory(autosaveDir);
            }

            timer = new DispatcherTimer();
            timer.Tick += Tick;
        }
    }
}
