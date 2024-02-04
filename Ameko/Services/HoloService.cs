using Holo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Services
{
    public class HoloService
    {
        public static HoloContext HoloInstance { get; } = HoloContext.Instance;
    }
}
