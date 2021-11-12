using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    public interface ICipher<T>
    {
        byte[] Cipher(byte[] text, T key);
        byte[] Decipher(byte[] text, T key);
    }
}
