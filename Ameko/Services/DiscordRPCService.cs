using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public class DiscordRPCService
    {
        private static readonly Lazy<DiscordRPCService> _instance = new Lazy<DiscordRPCService>(() => new DiscordRPCService());
        public static DiscordRPCService Instance => _instance.Value;

        private readonly DiscordRpcClient rpcClient;
        private readonly Timestamps timestamps;

        public void Set(string filename, string workspacename)
        {
            rpcClient.SetPresence(new RichPresence()
            {
                Details = $"Editing {filename}",
                State = $"in {workspacename}",
                Timestamps = timestamps,
                Assets = new Assets()
                {
                    LargeImageKey = "ameko-logo-512",
                    LargeImageText = "Ameko"
                }
            });
        }

        public void Unload()
        {
            rpcClient.Dispose();
        }

        private DiscordRPCService()
        {
            rpcClient = new DiscordRpcClient("1209896771719921704");
            rpcClient.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            rpcClient.Initialize();
            timestamps = Timestamps.Now;
        }
    }
}
