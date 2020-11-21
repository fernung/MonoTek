using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MonoTek.Graphics
{
    public interface IPixel
    {
        byte R { get; set; }
        byte G { get; set; }
        byte B { get; set; }
        byte A { get; set; }
        int Packed { get; set; }
        byte[] Bytes { get; set; }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel : IPixel
    {
        [FieldOffset(0)]
        private int _packed;
        public byte[] Bytes
        {
            get => new byte[4] { B, G, R, A };
            set
            {
                B = (byte)0xFF;
                G = (byte)0xFF;
                R = (byte)0xFF;
                A = (byte)0xFF;
                if (value.Length > 2)
                {
                    B = value[0];
                    G = value[1];
                    R = value[2];
                    A = (value.Length > 3) ?
                        value[3] :
                        (byte)0xFF;
                }
            }
        }
        [FieldOffset(0)]
        private byte _b;
        [FieldOffset(1)]
        private byte _g;
        [FieldOffset(2)]
        private byte _r;
        [FieldOffset(3)]
        private byte _a;

        public int Packed { get => _packed; set => _packed = value; }
        public byte B { get => _b; set => _b = value; }
        public byte G { get => _g; set => _g = value; }
        public byte R { get => _r; set => _r = value; }
        public byte A { get => _a; set => _a = value; }

        public static implicit operator Pixel(int value) => new Pixel { Packed = value };
        public static implicit operator Pixel(byte[] value) => new Pixel { Bytes = value };
        public static implicit operator Pixel(Color value) => new Pixel
        {
            R = value.R,
            G = value.G,
            B = value.B,
            A = value.A
        };
    }

    public static class PixelExtensions
    {
        public static IEnumerable<int> ToPacked(this IEnumerable<IPixel> source)
        {
            foreach (var b in source)
                yield return b.Packed;
        }
        public static IEnumerable<byte> ToBytes(this IEnumerable<IPixel> source) =>
            source.ToByteGroup().SelectMany(x => x);
        private static IEnumerable<IEnumerable<byte>> ToByteGroup(this IEnumerable<IPixel> source)
        {
            foreach (var b in source)
                yield return b.Bytes;
        }
        public static IEnumerable<Color> ToColors(this IEnumerable<IPixel> source)
        {
            foreach (var b in source)
                yield return new Color(b.R, b.G, b.B, b.A);
        }

        public static IEnumerable<IPixel> ToPixels(this IEnumerable<int> source)
        {
            foreach (var b in source)
                yield return (Pixel)b;
        }
        public static IEnumerable<IPixel> ToPixels(this IEnumerable<byte> source)
        {
            var offset = source.Count() % 4;
            if (offset != 0)
                source.Append<byte>(0, 4 - offset);
            foreach(var batch in source.Batch(4))
                yield return (Pixel)batch.ToArray();
        }
        public static IEnumerable<IPixel> ToPixels(this IEnumerable<Color> source)
        {
            foreach (var b in source)
                yield return (Pixel)b;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element, int amountToAdd)
        {
            for(int i = 0; i < amountToAdd; i++)
                source.Append(element);
            return source;
        }
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }
        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }
    }
}
