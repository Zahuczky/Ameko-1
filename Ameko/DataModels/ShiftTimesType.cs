using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.DataModels
{
    public enum ShiftTimesType
    {
        TIME,
        FRAMES
    }

    public enum ShiftTimesDirection
    {
        FORWARD,
        BACKWARD
    }

    public enum ShiftTimesFilter
    {
        ALL_ROWS,
        SELECTED_ROWS
    }

    public enum ShiftTimesTarget
    {
        BOTH,
        START,
        END
    }
}
