using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    public interface ILZWCompressor
    {
        byte[] Compress(byte[] text);
        byte[] Decompression(byte[] compressedText);
    }
}
