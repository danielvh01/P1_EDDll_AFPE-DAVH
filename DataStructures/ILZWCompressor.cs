using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    public interface ILZWCompressor
    {
        byte[] Compress(string text);
        string Decompression(byte[] compressedText);
    }
}
