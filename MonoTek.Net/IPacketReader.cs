using MonoTek.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoTek.Net
{
    public interface IPacketReader : IDisposable
    {
        int Length { get; }
        int Unread { get; }
        bool Pending { get; }

        IPacketReader Read(out Byte1 data);
        IPacketReader Read(out Byte2 data);
        IPacketReader Read(out Byte4 data);
        IPacketReader Read(out Byte8 data);
        IPacketReader Read(out ByteString data);
        IPacketReader Read(out Byte1[] data, int amount);
        IPacketReader Read(out byte data);
        IPacketReader Read(out bool data);
        IPacketReader Read(out char data);
        IPacketReader Read(out short data);
        IPacketReader Read(out ushort data);
        IPacketReader Read(out float data);
        IPacketReader Read(out int data);
        IPacketReader Read(out uint data);
        IPacketReader Read(out double data);
        IPacketReader Read(out long data);
        IPacketReader Read(out ulong data);
        IPacketReader Read(out string data);
        IPacketReader Read(out byte[] data, int amount);
        IPacketReader ReadHeader(out int data);
        void End();
        void Reset(bool shouldReset);
    }
    public class PacketReader : IPacketReader
    {
        protected bool _disposed;
        protected List<byte> _buffer;
        protected int _index;

        public int Length => _buffer.Count;
        public int Unread => _buffer.Count - _index;
        public bool Pending => Unread != 0;

        public PacketReader() { _buffer = new List<byte>(); _index = 0; _disposed = false; }
        public PacketReader(ref Packet packet) : this(ref packet.Buffer) { }
        public PacketReader(ref List<byte> buffer)
        {
            _disposed = false;
            _buffer = buffer;
            _index = 0;
        }

        public void Reset(bool shouldReset = true)
        {
            if (!shouldReset) _index -= Byte4.DefaultSize;
            else
            {
                _disposed = false;
                _buffer.Clear();
                _index = 0;
            }
        }

        public IPacketReader Read(out Byte1 data)
        {
            if (!Pending) Error("No more pending Byte1s to read from buffer.");
            data = _buffer[_index++];
            return this;
        }
        public IPacketReader Read(out Byte1[] data, int amount)
        {
            if (!Pending) Error("No more pending Byte1s to read from buffer.");
            else if (amount > Unread) amount = Unread;
            var result = _buffer.GetRange(_index, amount);
            _index += amount;
            data = result.Select(x => new Byte1 { Byte0 = x }).ToArray();
            return this;
        }
        public IPacketReader Read(out Byte2 data)
        {
            if (!Pending) Error("No more pending Byte2s to read from buffer.");
            data = BitConverter.ToInt16(_buffer.ToArray(), _index);
            _index += Byte2.DefaultSize;
            return this;
        }
        public IPacketReader Read(out Byte4 data)
        {
            if (!Pending) Error("No more pending Byte4s to read from buffer.");
            data = BitConverter.ToInt32(_buffer.ToArray(), _index);
            _index += Byte4.DefaultSize;
            return this;
        }
        public IPacketReader Read(out Byte8 data)
        {
            if (!Pending) Error("No more pending Byte8s to read from buffer.");
            data = BitConverter.ToInt64(_buffer.ToArray(), _index);
            _index += Byte8.DefaultSize;
            return this;
        }
        public IPacketReader Read(out ByteString data)
        {
            if (!Pending) Error("No more pending ByteStrings to read from buffer.");
            try
            {
                Read(out Byte4 size);
                data = Encoding.ASCII.GetString(_buffer.ToArray(), _index, size.Int0);
                if (data.String.Length > 0) _index += size.Int0;
                return this;
            }
            catch { Error("No pending ByteStrings to read from buffer."); }
            data = "";
            return this;
        }
        public IPacketReader Read(out byte data) { data = ReadByte1().Byte0; return this; }
        public IPacketReader Read(out bool data) { data = ReadByte1().Bool0; return this; }
        public IPacketReader Read(out char data) { data = ReadByte2().Char0; return this; }
        public IPacketReader Read(out short data) { data = ReadByte2().Short0; return this; }
        public IPacketReader Read(out ushort data) { data = ReadByte2().UShort0; return this; }
        public IPacketReader Read(out float data) { data = ReadByte4().Float0; return this; }
        public IPacketReader Read(out int data) { data = ReadByte4().Int0; return this; }
        public IPacketReader Read(out uint data) { data = ReadByte4().UInt0; return this; }
        public IPacketReader Read(out double data) { data = ReadByte8().Double0; return this; }
        public IPacketReader Read(out long data) { data = ReadByte8().Long0; return this; }
        public IPacketReader Read(out ulong data) { data = ReadByte8().ULong0; return this; }
        public IPacketReader Read(out string data) { data = ReadByteString().String; return this; }
        public IPacketReader Read(out byte[] data, int amount)
        {
            if (!Pending) Error("No more pending bytes to read from buffer.");
            else if (amount > Length) amount = Length;
            var result = _buffer.GetRange(_index, amount);
            _index += amount;
            data = result.ToArray();
            return this;
        }
        public IPacketReader ReadHeader(out int data)
        {
            if (!Pending) Error("No header to read from buffer.");
            data = BitConverter.ToInt32(_buffer.ToArray(), _index);
            return this;
        }

        protected Byte1 ReadByte1()
        {
            Read(out Byte1 result);
            return result;
        }
        protected Byte2 ReadByte2()
        {
            Read(out Byte2 result);
            return result;
        }
        protected Byte4 ReadByte4()
        {
            Read(out Byte4 result);
            return result;
        }
        protected Byte8 ReadByte8()
        {
            Read(out Byte8 result);
            return result;
        }
        protected ByteString ReadByteString()
        {
            Read(out ByteString result);
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (!disposing) return;
            _buffer = null;
            _index = 0;
        }

        public static PacketReader Begin(ref Packet packet) => new PacketReader(ref packet);
        public static PacketReader Begin(ref List<byte> buffer) => new PacketReader(ref buffer);
        public void End() => Dispose();

        public static implicit operator PacketReader(Packet packet) => new PacketReader(ref packet);
        public static implicit operator PacketReader(List<byte> buffer) => new PacketReader(ref buffer);
        protected void Error(string message) => throw new Exception($"{message}");
    }
}
