using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Diagnostics;

namespace Device
{
    public class Keyboard
    {
        ModBus _modBus = new ModBus();

        const int DeviceID = 2;
        public enum SendCommand : int
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

        public enum RecivedCommand: int
        {
            SetService = 0x102,
            SetCounter = 0x104,
        }

        public byte[] SetService(int address, List<string> services)
        {
            int _serviceCount = services.Count();

            byte[] _data = new byte[] { (byte)_serviceCount };

            byte[] _temp;

            foreach(var s in services)
            {
                _temp = new byte[_data.Length + s.Length*4 + 1];

                Array.Copy(_data, _temp, _data.Length);



                var _d = Utility.ConvertStringToByte(s, true);


                Buffer.BlockCopy(_d, 0, _temp, _data.Length, _d.Length);

                _data = _temp;
            }

            return _modBus.Build(DeviceID, address, (int)RecivedCommand.SetService, _data);

        }

        public byte[] SetCounter(int address, string specialName, List<string> counters)
        {
            int _serviceCount = counters.Count();

            byte[] _data = new byte[] { (byte)_serviceCount };

            byte[] _temp;

            foreach (var s in counters)
            {
                _temp = new byte[_data.Length + s.Length * 4 + 1];

                Array.Copy(_data, _temp, _data.Length);

                var _d = Utility.ConvertStringToByte(s, true);
                
                Buffer.BlockCopy(_d, 0, _temp, _data.Length, _d.Length);

                _data = _temp;
            }

            _temp = new byte[_data.Length + specialName.Length * 4 + 1];

            var _dSpecial = Utility.ConvertStringToByte(specialName, true);

            Buffer.BlockCopy(_data, 0, _temp, _dSpecial.Length, _data.Length);
            Buffer.BlockCopy(_dSpecial, 0, _temp, 0, _dSpecial.Length);

            return _modBus.Build(DeviceID, address, (int)RecivedCommand.SetCounter, _temp);

        }
    }
}
