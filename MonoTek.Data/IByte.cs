using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoTek.Data
{
    #region IByte
    public interface IByte
    {
        byte Size { get; }
        byte SizeHalf { get; }
        byte[] Bytes { get; }
    }
    #endregion
    #region Byte1
    [StructLayout(LayoutKind.Explicit)]
    public struct Byte1 : IByte
    {
        public const byte DefaultSize = 1;
        public const byte DefaultSizeHalf = 0;

        [FieldOffset(0)]
        private byte _byte;
        [FieldOffset(0)]
        private bool _bool;

        public byte Size => DefaultSize;
        public byte SizeHalf => DefaultSizeHalf;
        public byte Byte0 { get => _byte; set => _byte = value; }
        public bool Bool0 { get => _bool; set => _bool = value; }
        public byte[] Bytes => new byte[DefaultSize] { _byte };

        public static implicit operator Byte1(bool data) => new Byte1 { _bool = data };
        public static implicit operator Byte1(byte data) => new Byte1 { _byte = data };
        public static implicit operator Byte1(byte[] data) => new Byte1 { _byte = data.Resize(DefaultSize).First() };
        public static implicit operator Byte1(Byte2 data) => new Byte1 { _byte = data.Byte0 };
        public static implicit operator Byte1(Byte4 data) => new Byte1 { _byte = data.Byte0 };
        public static implicit operator Byte1(Byte8 data) => new Byte1 { _byte = data.Byte0 };

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[Byte{DefaultSize}]->[{Bytes.ToHexString()}][{Bytes.ToBinaryString()}]");
            sb.AppendLine($"Byte: {Byte0} [{Byte0.ToHexString()}][{Byte0.ToBinaryString()}]");
            sb.AppendLine($"Bool: {Bool0} [{Bool0.ToHexString()}][{Bool0.ToBinaryString()}]");
            return sb.ToString();
        }
    }
    #endregion
    #region Byte2
    [StructLayout(LayoutKind.Explicit)]
    public struct Byte2 : IByte
    {
        public const byte DefaultSize = 2;
        public const byte DefaultSizeHalf = 1;

        [FieldOffset(0)]
        private Byte1 _low;
        [FieldOffset(1)]
        private Byte1 _high;
        [FieldOffset(0)]
        private short _short;
        [FieldOffset(0)]
        private ushort _ushort;
        [FieldOffset(0)]
        private char _char;

        public byte Size => DefaultSize;
        public byte SizeHalf => DefaultSizeHalf;
        public byte Byte0 { get => _low.Byte0; set => _low.Byte0 = value; }
        public byte Byte1 { get => _high.Byte0; set => _high.Byte0 = value; }
        public bool Bool0 { get => _low.Bool0; set => _low.Bool0 = value; }
        public bool Bool1 { get => _high.Bool0; set => _high.Bool0 = value; }
        public short Short0 { get => _short; set => _short = value; }
        public ushort UShort0 { get => _ushort; set => _ushort = value; }
        public char Char0 { get => _char; set => _char = value; }
        public byte[] Bytes => new byte[DefaultSize] { _low.Byte0, _high.Byte0 };

        public static implicit operator Byte2(byte data) => new Byte2 { _low = data, _high = 0 };
        public static implicit operator Byte2(bool data) => new Byte2 { _low = data, _high = 0 };
        public static implicit operator Byte2(short data) => new Byte2 { _short = data };
        public static implicit operator Byte2(ushort data) => new Byte2 { _ushort = data };
        public static implicit operator Byte2(char data) => new Byte2 { _char = data };
        public static implicit operator Byte2(byte[] data)
        {
            var bytes = data.Resize(DefaultSize).ToArray();
            return new Byte2 { _low = bytes[0], _high = bytes[DefaultSizeHalf] };
        }
        public static implicit operator Byte2(Byte1 data) => new Byte2 { _low = data, _high = 0 };
        public static implicit operator Byte2(Byte4 data) => new Byte2 { _low = data.Byte0, _high = data.Byte1 };
        public static implicit operator Byte2(Byte8 data) => new Byte2 { _low = data.Byte0, _high = data.Byte1 };

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[Byte{DefaultSize}]->[{Bytes.ToHexString()}][{Bytes.ToBinaryString()}]");
            sb.AppendLine($"Byte0: {Byte0} [{Byte0.ToHexString()}][{Byte0.ToBinaryString()}]");
            sb.AppendLine($"Byte1: {Byte1} [{Byte1.ToHexString()}][{Byte1.ToBinaryString()}]");
            sb.AppendLine($"Bool0: {Bool0} [{Bool0.ToHexString()}][{Bool0.ToBinaryString()}]");
            sb.AppendLine($"Bool1: {Bool1} [{Bool1.ToHexString()}][{Bool1.ToBinaryString()}]");
            sb.AppendLine($"Short: {Short0} [{Short0.ToHexString()}][{Short0.ToBinaryString()}]");
            sb.AppendLine($"UShort: {UShort0} [{UShort0.ToHexString()}][{UShort0.ToBinaryString()}]");
            sb.AppendLine($"Char: {Char0} [{Char0.ToHexString()}][{Char0.ToBinaryString()}]");
            return sb.ToString();
        }
    }
    #endregion
    #region Byte4
    [StructLayout(LayoutKind.Explicit)]
    public struct Byte4 : IByte
    {
        public const byte DefaultSize = 4;
        public const byte DefaultSizeHalf = 2;

        [FieldOffset(0)]
        private Byte2 _low;
        [FieldOffset(2)]
        private Byte2 _high;
        [FieldOffset(0)]
        private int _int;
        [FieldOffset(0)]
        private uint _uint;
        [FieldOffset(0)]
        private float _float;

        public byte Size => DefaultSize;
        public byte SizeHalf => DefaultSizeHalf;
        public byte Byte0 { get => _low.Byte0; set => _low.Byte0 = value; }
        public byte Byte1 { get => _low.Byte1; set => _low.Byte1 = value; }
        public byte Byte2 { get => _high.Byte0; set => _high.Byte0 = value; }
        public byte Byte3 { get => _high.Byte1; set => _high.Byte1 = value; }
        public bool Bool0 { get => _low.Bool0; set => _low.Bool0 = value; }
        public bool Bool1 { get => _low.Bool1; set => _low.Bool1 = value; }
        public bool Bool2 { get => _high.Bool0; set => _high.Bool0 = value; }
        public bool Bool3 { get => _high.Bool1; set => _high.Bool1 = value; }
        public short Short0 { get => _low.Short0; set => _low.Short0 = value; }
        public short Short1 { get => _high.Short0; set => _high.Short0 = value; }
        public ushort UShort0 { get => _low.UShort0; set => _low.UShort0 = value; }
        public ushort UShort1 { get => _high.UShort0; set => _high.UShort0 = value; }
        public char Char0 { get => _low.Char0; set => _low.Char0 = value; }
        public char Char1 { get => _high.Char0; set => _high.Char0 = value; }
        public int Int0 { get => _int; set => _int = value; }
        public uint UInt0 { get => _uint; set => _uint = value; }
        public float Float0 { get => _float; set => _float = value; }
        public byte[] Bytes => new byte[DefaultSize] { _low.Byte0, _low.Byte1, _high.Byte0, _high.Byte1 };

        public static implicit operator Byte4(byte data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(bool data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(short data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(ushort data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(char data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(int data) => new Byte4 { _int = data };
        public static implicit operator Byte4(uint data) => new Byte4 { _uint = data };
        public static implicit operator Byte4(float data) => new Byte4 { _float = data };
        public static implicit operator Byte4(byte[] data)
        {
            var bytes = data.Resize(DefaultSize).ToArray();
            return new Byte4 { _low = bytes[0..DefaultSizeHalf], _high = bytes[DefaultSizeHalf..DefaultSize] };
        }
        public static implicit operator Byte4(Byte1 data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(Byte2 data) => new Byte4 { _low = data, _high = 0 };
        public static implicit operator Byte4(Byte8 data) => new Byte4 { _low = data.Short0, _high = data.Short1 };

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[Byte{DefaultSize}]->[{Bytes.ToHexString()}][{Bytes.ToBinaryString()}]");
            sb.AppendLine($"Byte0: {Byte0} [{Byte0.ToHexString()}][{Byte0.ToBinaryString()}]");
            sb.AppendLine($"Byte1: {Byte1} [{Byte1.ToHexString()}][{Byte1.ToBinaryString()}]");
            sb.AppendLine($"Byte2: {Byte2} [{Byte2.ToHexString()}][{Byte2.ToBinaryString()}]");
            sb.AppendLine($"Byte3: {Byte3} [{Byte3.ToHexString()}][{Byte3.ToBinaryString()}]");
            sb.AppendLine($"Bool0: {Bool0} [{Bool0.ToHexString()}][{Bool0.ToBinaryString()}]");
            sb.AppendLine($"Bool1: {Bool1} [{Bool1.ToHexString()}][{Bool1.ToBinaryString()}]");
            sb.AppendLine($"Bool2: {Bool2} [{Bool2.ToHexString()}][{Bool2.ToBinaryString()}]");
            sb.AppendLine($"Bool3: {Bool3} [{Bool3.ToHexString()}][{Bool3.ToBinaryString()}]");
            sb.AppendLine($"Short0: {Short0} [{Short0.ToHexString()}][{Short0.ToBinaryString()}]");
            sb.AppendLine($"Short1: {Short1} [{Short1.ToHexString()}][{Short1.ToBinaryString()}]");
            sb.AppendLine($"UShort0: {UShort0} [{UShort0.ToHexString()}][{UShort0.ToBinaryString()}]");
            sb.AppendLine($"UShort1: {UShort1} [{UShort1.ToHexString()}][{UShort1.ToBinaryString()}]");
            sb.AppendLine($"Char0: {Char0} [{Char0.ToHexString()}][{Char0.ToBinaryString()}]");
            sb.AppendLine($"Char1: {Char1} [{Char1.ToHexString()}][{Char1.ToBinaryString()}]");
            sb.AppendLine($"Int: {Int0} [{Int0.ToHexString()}][{Int0.ToBinaryString()}]");
            sb.AppendLine($"UInt: {UInt0} [{UInt0.ToHexString()}][{UInt0.ToBinaryString()}]");
            sb.AppendLine($"Float: {Float0} [{Float0.ToHexString()}][{Float0.ToBinaryString()}]");
            return sb.ToString();
        }
    }
    #endregion
    #region Byte8
    [StructLayout(LayoutKind.Explicit)]
    public struct Byte8 : IByte
    {
        public const byte DefaultSize = 8;
        public const byte DefaultSizeHalf = 4;

        [FieldOffset(0)]
        private Byte4 _low;
        [FieldOffset(4)]
        private Byte4 _high;
        [FieldOffset(0)]
        private long _long;
        [FieldOffset(0)]
        private ulong _ulong;
        [FieldOffset(0)]
        private double _double;

        public byte Size => DefaultSize;
        public byte SizeHalf => DefaultSizeHalf;
        public byte Byte0 { get => _low.Byte0; set => _low.Byte0 = value; }
        public byte Byte1 { get => _low.Byte1; set => _low.Byte1 = value; }
        public byte Byte2 { get => _low.Byte2; set => _low.Byte2 = value; }
        public byte Byte3 { get => _low.Byte3; set => _low.Byte3 = value; }
        public byte Byte4 { get => _high.Byte0; set => _high.Byte0 = value; }
        public byte Byte5 { get => _high.Byte1; set => _high.Byte1 = value; }
        public byte Byte6 { get => _high.Byte2; set => _high.Byte2 = value; }
        public byte Byte7 { get => _high.Byte3; set => _high.Byte3 = value; }
        public bool Bool0 { get => _low.Bool0; set => _low.Bool0 = value; }
        public bool Bool1 { get => _low.Bool1; set => _low.Bool1 = value; }
        public bool Bool2 { get => _low.Bool2; set => _low.Bool2 = value; }
        public bool Bool3 { get => _low.Bool3; set => _low.Bool3 = value; }
        public bool Bool4 { get => _high.Bool0; set => _high.Bool0 = value; }
        public bool Bool5 { get => _high.Bool1; set => _high.Bool1 = value; }
        public bool Bool6 { get => _high.Bool2; set => _high.Bool2 = value; }
        public bool Bool7 { get => _high.Bool3; set => _high.Bool3 = value; }
        public short Short0 { get => _low.Short0; set => _low.Short0 = value; }
        public short Short1 { get => _low.Short1; set => _low.Short1 = value; }
        public short Short2 { get => _high.Short0; set => _high.Short0 = value; }
        public short Short3 { get => _high.Short1; set => _high.Short1 = value; }
        public ushort UShort0 { get => _low.UShort0; set => _low.UShort0 = value; }
        public ushort UShort1 { get => _low.UShort1; set => _low.UShort1 = value; }
        public ushort UShort2 { get => _high.UShort0; set => _high.UShort0 = value; }
        public ushort UShort3 { get => _high.UShort1; set => _high.UShort1 = value; }
        public char Char0 { get => _low.Char0; set => _low.Char0 = value; }
        public char Char1 { get => _low.Char1; set => _low.Char1 = value; }
        public char Char2 { get => _high.Char0; set => _high.Char0 = value; }
        public char Char3 { get => _high.Char1; set => _high.Char1 = value; }
        public int Int0 { get => _low.Int0; set => _low.Int0 = value; }
        public int Int1 { get => _high.Int0; set => _high.Int0 = value; }
        public uint UInt0 { get => _low.UInt0; set => _low.UInt0 = value; }
        public uint UInt1 { get => _high.UInt0; set => _high.UInt0 = value; }
        public float Float0 { get => _low.Float0; set => _low.Float0 = value; }
        public float Float1 { get => _high.Float0; set => _high.Float0 = value; }
        public long Long0 { get => _long; set => _long = value; }
        public ulong ULong0 { get => _ulong; set => _ulong = value; }
        public double Double0 { get => _double; set => _double = value; }
        public byte[] Bytes => new byte[DefaultSize]
        {
            _low.Byte0, _low.Byte1, _low.Byte2, _low.Byte3,
            _high.Byte0, _high.Byte1, _high.Byte2, _high.Byte3
        };

        public static implicit operator Byte8(byte data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(bool data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(short data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(ushort data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(char data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(int data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(uint data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(float data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(long data) => new Byte8 { _long = data };
        public static implicit operator Byte8(ulong data) => new Byte8 { _ulong = data };
        public static implicit operator Byte8(double data) => new Byte8 { _double = data };
        public static implicit operator Byte8(byte[] data)
        {
            var bytes = data.Resize(DefaultSize).ToArray();
            return new Byte8 { _low = bytes[0..DefaultSizeHalf], _high = bytes[DefaultSizeHalf..DefaultSize] };
        }
        public static implicit operator Byte8(Byte1 data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(Byte2 data) => new Byte8 { _low = data, _high = 0 };
        public static implicit operator Byte8(Byte4 data) => new Byte8 { _low = data, _high = 0 };

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[Byte{DefaultSize}]->[{Bytes.ToHexString()}][{Bytes.ToBinaryString()}]");
            sb.AppendLine($"Byte0: {Byte0} [{Byte0.ToHexString()}][{Byte0.ToBinaryString()}]");
            sb.AppendLine($"Byte1: {Byte1} [{Byte1.ToHexString()}][{Byte1.ToBinaryString()}]");
            sb.AppendLine($"Byte2: {Byte2} [{Byte2.ToHexString()}][{Byte2.ToBinaryString()}]");
            sb.AppendLine($"Byte3: {Byte3} [{Byte3.ToHexString()}][{Byte3.ToBinaryString()}]");
            sb.AppendLine($"Byte4: {Byte4} [{Byte4.ToHexString()}][{Byte4.ToBinaryString()}]");
            sb.AppendLine($"Byte5: {Byte5} [{Byte5.ToHexString()}][{Byte5.ToBinaryString()}]");
            sb.AppendLine($"Byte6: {Byte6} [{Byte6.ToHexString()}][{Byte6.ToBinaryString()}]");
            sb.AppendLine($"Byte7: {Byte7} [{Byte7.ToHexString()}][{Byte7.ToBinaryString()}]");
            sb.AppendLine($"Bool0: {Bool0} [{Bool0.ToHexString()}][{Bool0.ToBinaryString()}]");
            sb.AppendLine($"Bool1: {Bool1} [{Bool1.ToHexString()}][{Bool1.ToBinaryString()}]");
            sb.AppendLine($"Bool2: {Bool2} [{Bool2.ToHexString()}][{Bool2.ToBinaryString()}]");
            sb.AppendLine($"Bool3: {Bool3} [{Bool3.ToHexString()}][{Bool3.ToBinaryString()}]");
            sb.AppendLine($"Bool4: {Bool4} [{Bool4.ToHexString()}][{Bool4.ToBinaryString()}]");
            sb.AppendLine($"Bool5: {Bool5} [{Bool5.ToHexString()}][{Bool5.ToBinaryString()}]");
            sb.AppendLine($"Bool6: {Bool6} [{Bool6.ToHexString()}][{Bool6.ToBinaryString()}]");
            sb.AppendLine($"Bool7: {Bool7} [{Bool7.ToHexString()}][{Bool7.ToBinaryString()}]");
            sb.AppendLine($"Short0: {Short0} [{Short0.ToHexString()}][{Short0.ToBinaryString()}]");
            sb.AppendLine($"Short1: {Short1} [{Short1.ToHexString()}][{Short1.ToBinaryString()}]");
            sb.AppendLine($"Short2: {Short2} [{Short2.ToHexString()}][{Short2.ToBinaryString()}]");
            sb.AppendLine($"Short3: {Short3} [{Short3.ToHexString()}][{Short3.ToBinaryString()}]");
            sb.AppendLine($"UShort0: {UShort0} [{UShort0.ToHexString()}][{UShort0.ToBinaryString()}]");
            sb.AppendLine($"UShort1: {UShort1} [{UShort1.ToHexString()}][{UShort1.ToBinaryString()}]");
            sb.AppendLine($"UShort2: {UShort2} [{UShort2.ToHexString()}][{UShort2.ToBinaryString()}]");
            sb.AppendLine($"UShort3: {UShort3} [{UShort3.ToHexString()}][{UShort3.ToBinaryString()}]");
            sb.AppendLine($"Char0: {Char0} [{Char0.ToHexString()}][{Char0.ToBinaryString()}]");
            sb.AppendLine($"Char1: {Char1} [{Char1.ToHexString()}][{Char1.ToBinaryString()}]");
            sb.AppendLine($"Char2: {Char2} [{Char2.ToHexString()}][{Char2.ToBinaryString()}]");
            sb.AppendLine($"Char3: {Char3} [{Char3.ToHexString()}][{Char3.ToBinaryString()}]");
            sb.AppendLine($"Int0: {Int0} [{Int0.ToHexString()}][{Int0.ToBinaryString()}]");
            sb.AppendLine($"Int1: {Int1} [{Int1.ToHexString()}][{Int1.ToBinaryString()}]");
            sb.AppendLine($"UInt0: {UInt0} [{UInt0.ToHexString()}][{UInt0.ToBinaryString()}]");
            sb.AppendLine($"UInt1: {UInt1} [{UInt1.ToHexString()}][{UInt1.ToBinaryString()}]");
            sb.AppendLine($"Float0: {Float0} [{Float0.ToHexString()}][{Float0.ToBinaryString()}]");
            sb.AppendLine($"Float1: {Float1} [{Float1.ToHexString()}][{Float1.ToBinaryString()}]");
            sb.AppendLine($"Long: {Long0} [{Long0.ToHexString()}][{Long0.ToBinaryString()}]");
            sb.AppendLine($"ULong: {ULong0} [{ULong0.ToHexString()}][{ULong0.ToBinaryString()}]");
            sb.AppendLine($"Double: {Double0} [{Double0.ToHexString()}][{Double0.ToBinaryString()}]");
            return sb.ToString();
        }
    }
    #endregion
    #region ByteString
    public struct ByteString : IByte
    {
        private Byte2[] _string;

        public byte Size => (byte)Bytes.Length;
        public byte SizeHalf => (byte)(Bytes.Length >> 1);
        public byte[] Bytes => _string.SelectMany(x => x.Bytes).ToArray();
        public string String
        {
            get => string.Concat(_string.Select(x => x.Char0));
            set => _string = value.ToCharArray().Select(x => (Byte2)x).ToArray();
        }

        private ByteString(string text) :
            this(text.ToCharArray())
        { }
        private ByteString(IEnumerable<char> text)
        {
            _string = text.ToArray().Select(x => (Byte2)x).ToArray();
        }
        private ByteString(IEnumerable<Byte2> text)
        {
            _string = text.ToArray();
        }

        public override string ToString() => String;
        public static implicit operator ByteString(char data) => new ByteString(data.ToString());
        public static implicit operator ByteString(string data) => new ByteString(data);
        public static implicit operator ByteString(byte[] data)
        {
            List<Byte2> letters = new List<Byte2>();
            for (int i = 0; i < data.Length; i += 2)
                letters.Add(new Byte2 { Byte0 = data[i], Byte1 = data[i + 1] });
            return new ByteString(letters);
        }
    }
    #endregion
    #region Extensions
    public static class ByteExtensions
    {
        private const int BinaryFormat = 2;
        private const string HexFormat = "X2";
        public static string ToBinaryString(this byte data) => Convert.ToString(data, BinaryFormat).PadLeft(8, '0');
        public static string ToBinaryString(this bool data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this short data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this ushort data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this char data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this int data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this uint data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this float data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this long data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this ulong data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this double data) => BitConverter.GetBytes(data).ToBinaryString();
        public static string ToBinaryString(this IEnumerable<byte> data) => string.Concat(data.Reverse().Select(x => x.ToBinaryString()));
        public static string ToHexString(this byte data) => data.ToString(HexFormat);
        public static string ToHexString(this bool data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this short data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this ushort data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this char data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this int data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this uint data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this float data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this long data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this ulong data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this double data) => BitConverter.GetBytes(data).ToHexString();
        public static string ToHexString(this IEnumerable<byte> data) => string.Concat(data.Reverse().Select(x => x.ToHexString()));
        public static IEnumerable<T> Resize<T>(this IEnumerable<T> buffer, int size)
        {
            if (buffer.Count() < size)
            {
                var temp = new List<T>(buffer);
                for (int i = buffer.Count(); i < size; i++) temp.Add(default);
                return temp;
            }
            else if (buffer.Count() > size) return buffer.Take(size);
            return buffer;
        }
    }
    #endregion
}
