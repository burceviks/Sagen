﻿using System;
using System.Runtime.CompilerServices;

namespace HAARP
{
    internal static class Extensions
    {
        public static ulong RotL(this ulong data, int times)
        {
            return (data << (times % 64)) | (data >> (64 - (times % 64)));
        }

        public static ulong RotR(this ulong data, int times)
        {
            return (data >> (times % 64)) | (data << (64 - (times % 64)));
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tilt(this float sample, float frequency, float spectralTilt, float nyquist = 22050.0f)
        {
            return sample * (float)Math.Pow(2, (frequency / nyquist * 2f - 1f) * spectralTilt);
        }
    }
}