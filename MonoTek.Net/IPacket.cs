using System;
using System.Collections.Generic;

namespace MonoTek.Net
{
    public interface IPacket : IDisposable
    {
        IPacketReader Reader { get; }
        IPacketWriter Writer { get; }
        int Size { get; }

        void Reset(bool shouldReset);
    }
    
    public class Packet : IPacket
    {
        protected bool _disposed;
        protected IPacketReader _reader;
        protected IPacketWriter _writer;

        public IPacketReader Reader => _reader;
        public IPacketWriter Writer => _writer;
        public List<byte> Buffer = new List<byte>();
        public int Size => Buffer.Count;

        public Packet(int id) : this() { _writer.Write(id); }
        public Packet(byte[] buffer) : this() { Buffer.AddRange(buffer); }
        public Packet()
        {
            _disposed = false;
            Buffer = new List<byte>();
            _reader = PacketReader.Begin(ref Buffer);
            _writer = PacketWriter.Begin(ref Buffer);
        }

        public void Reset(bool shouldReset = true)
        {
            _reader.Reset(shouldReset);
            _writer.Reset(shouldReset);
            if (shouldReset) _disposed = false;
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
            Buffer = null;
            _reader.Dispose();
            _reader = null;
            _writer = null;
        }
    }
}
