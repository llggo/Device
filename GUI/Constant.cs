using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    class Constant
    {
        static class Led
        {

        }

        class Keyboard
        {
            const int DeviceID = 2;
            enum Command : int
            {
                Next = 0x30,
                Recall = 0x31,
                Delete = 0x32,
                Finish = 0x33,
                Forward = 0x34,
                Restore = 0x36,
                LoadService = 0x102,
                LoadServiceDone = 0x103,
                LoadCounter = 0x104,
                LoadCounterDone = 0x105,
            }
        }
    }
}
