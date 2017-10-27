using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class Utility
    {
        public static byte[] ConvertStringToByte(string str, bool insertLength = false)
        {
            var _char = str.ToCharArray().Select(c => c.ToString()).ToArray();

            var length = str.Length * 4;

            byte[] _d;

            var index = 0;

            if (insertLength)
            {
                length += 1;
                index += 1;
                _d = new byte[length];
                _d[0] = (byte)str.Length;
            }
            else
            {
                _d = new byte[length];
            }

            for (int i = 0; i < _char.Length; i++)
            {
                var _t = Encoding.UTF8.GetBytes(_char[i]);
                Buffer.BlockCopy(_t, 0, _d, index + i * 4, _t.Length);
            }

            return _d;
        }
    }
}
