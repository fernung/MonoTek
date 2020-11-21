using MonoTek.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoTek.Net
{
    public interface IPacketWriter
    {
        IPacketWriter Write(Byte1 value);
        IPacketWriter Write(Byte2 value);
        IPacketWriter Write(Byte4 value);
        IPacketWriter Write(Byte8 value);
        IPacketWriter Write(IByte value);
        IPacketWriter Write(ByteString value);
        IPacketWriter Write(byte value);
        IPacketWriter Write(bool value);
        IPacketWriter Write(char value);
        IPacketWriter Write(short value);
        IPacketWriter Write(ushort value);
        IPacketWriter Write(float value);
        IPacketWriter Write(int value);
        IPacketWriter Write(uint value);
        IPacketWriter Write(double value);
        IPacketWriter Write(long value);
        IPacketWriter Write(ulong value);
        IPacketWriter Write(byte[] value);
        byte[] End(bool writeSize);
        void Reset(bool shouldReset);
    }
    public class PacketWriter : IPacketWriter
    {
        protected List<byte> _buffer;

        public PacketWriter() { _buffer = new List<byte>(); }
        public PacketWriter(ref Packet packet) : this(ref packet.Buffer) { }
        public PacketWriter(ref List<byte> buffer)
        {
            _buffer = buffer;
        }

        public void Reset(bool shouldReset = true)
        {
            if (!shouldReset) return;
            else
            {
                _buffer.Clear();
            }
        }

        public IPacketWriter Write(Byte1 value) => Write(value.Bytes);
        public IPacketWriter Write(Byte2 value) => Write(value.Bytes);
        public IPacketWriter Write(Byte4 value) => Write(value.Bytes);
        public IPacketWriter Write(Byte8 value) => Write(value.Bytes);
        public IPacketWriter Write(IByte value) => Write(value.Bytes);
        public IPacketWriter Write(ByteString value) => Write((Byte4)value.String.Length).Write(Encoding.ASCII.GetBytes(value.String));
        public IPacketWriter Write(byte value) => Write((Byte1)value);
        public IPacketWriter Write(bool value) => Write((Byte1)value);
        public IPacketWriter Write(char value) => Write((Byte2)value);
        public IPacketWriter Write(short value) => Write((Byte2)value);
        public IPacketWriter Write(ushort value) => Write((Byte2)value);
        public IPacketWriter Write(float value) => Write((Byte4)value);
        public IPacketWriter Write(int value) => Write((Byte4)value);
        public IPacketWriter Write(uint value) => Write((Byte4)value);
        public IPacketWriter Write(double value) => Write((Byte8)value);
        public IPacketWriter Write(long value) => Write((Byte8)value);
        public IPacketWriter Write(ulong value) => Write((Byte8)value);
        public IPacketWriter Write(byte[] value) { _buffer.AddRange(value); return this; }

        public static IPacketWriter Begin() => new PacketWriter();
        public static IPacketWriter Begin(ref Packet packet) => new PacketWriter(ref packet);
        public static IPacketWriter Begin(ref List<byte> buffer) => new PacketWriter(ref buffer);
        public byte[] End(bool writeSize = true)
        {
            if (writeSize) _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
            return _buffer.ToArray();
        }
    }
}
