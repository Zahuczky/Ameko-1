using Ameko.DataModels;
using AssCS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ameko.ViewModels
{
    public class ShiftTimesWindowViewModel : ViewModelBase
    {
        public Time ShiftTime { get; set; }

        public ShiftTimesType ShiftTimesType { get; set; }
        public ShiftTimesDirection ShiftTimesDirection { get; set; }
        public ShiftTimesFilter ShiftTimesFilter { get; set; }
        public ShiftTimesTarget ShiftTimesTarget { get; set; }

        public ICommand ShiftTimesCommand { get; }

        public ShiftTimesWindowViewModel()
        {
            ShiftTime = new Time();
            ShiftTimesType = ShiftTimesType.TIME;
            ShiftTimesDirection = ShiftTimesDirection.FORWARD;
            ShiftTimesFilter = ShiftTimesFilter.SELECTED_ROWS;
            ShiftTimesTarget = ShiftTimesTarget.BOTH;

            ShiftTimesCommand = ReactiveCommand.Create(() =>
            {

            });
        }
    }
}
