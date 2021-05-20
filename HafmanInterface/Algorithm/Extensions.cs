using System;
using System.Collections;

namespace HafmanInterface.Algorithm
{
    public static class Extensions
    {
        public static BitArray Append(this BitArray current, BitArray after) {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public static byte GetFirstByte(this BitArray current)
        {
            if (current.Count != 8)
            {
                throw new ArgumentException("Длина bitarray !=8");
            }

            byte[] bytes = new byte[1];
            current.CopyTo(bytes,0);
            return bytes[0];
        }
    }
}