﻿namespace VGAudio.Utilities
{
    public static class Bit
    {
        public static uint BitReverse32(uint value)
        {
            value = ((value & 0xaaaaaaaa) >> 1) | ((value & 0x55555555) << 1);
            value = ((value & 0xcccccccc) >> 2) | ((value & 0x33333333) << 2);
            value = ((value & 0xf0f0f0f0) >> 4) | ((value & 0x0f0f0f0f) << 4);
            value = ((value & 0xff00ff00) >> 8) | ((value & 0x00ff00ff) << 8);
            return (value >> 16) | (value << 16);
        }
        public static int BitReverse32(int value) => (int)BitReverse32((uint)value);
        public static uint BitReverse32(uint value, int bitCount) => BitReverse32(value) >> (32 - bitCount);
        public static int BitReverse32(int value, int bitCount) => (int) BitReverse32((uint) value, bitCount);

        public static byte BitReverse8(byte value)
        {
            return (byte)(((value * 0x80200802ul) & 0x0884422110ul) * 0x0101010101ul >> 32);
        }
    }
}
