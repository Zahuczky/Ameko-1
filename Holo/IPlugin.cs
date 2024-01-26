using System;
using System.Collections.Generic;
using System.Text;

namespace Holo
{
    public interface IPlugin
    {
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public string Author { get; }
        public string AuthorUrl { get; }
        public void Init();
        public void Unload();
        public IDictionary<string, dynamic> GetProperties();
    }

    public interface IAudioSourcePlugin : IPlugin
    {
        
        public AudioFrame GetFrame(long frameNumber, int stream);
    }

    public interface IVideoSourcePlugin : IPlugin
    {
        public VideoFrame GetFrame(double offset, double duration, int stream);
    }

    public interface ISubtitlePlugin : IPlugin
    {
        public int Execute(string[] args);
    }
}
