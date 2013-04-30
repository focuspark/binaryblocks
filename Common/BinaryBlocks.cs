/**
 * Copyright (c) 2013 Jeremy Wyman
 * Microsoft Public License (Ms-PL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 * 1. Definitions
 *    The terms "reproduce", "reproduction", "derivative works", and "distribution" have the same meaning here as under U.S. copyright law.
 *    A "contribution" is the original software, or any additions or changes to the software.
 *    A "contributor" is any person that distributes its contribution under this license.
 *    "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 * 2. Grant of Rights
 *    (A) Copyright Grant - Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 *    (B) Patent Grant - Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 * 3. Conditions and Limitations
 *    (A) No Trademark License - This license does not grant you rights to use any contributors' name, logo, or trademarks.
 *    (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 *    (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 *    (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 *    (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 *    
 * More info on: http://binaryblocks.codeplex.com
**/

namespace BinaryBlocks
{
    internal enum BlockType : byte
    {
        Unknown = 0x00,
        Byte = 0x01,
        Char = 0x02,
        Sint = 0x03,
        Uint = 0x04,
        Slong = 0x05,
        Ulong = 0x06,
        Single = 0x07,
        Double = 0x08,
        String = 0x09,
        Timestamp = 0x0A,
        Timespan = 0x0B,
        Blob = 0x0C,
        Guid = 0x0D,
        Enum = 0x0E,
        Struct = 0x0F,
        List = 0x80,
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
    internal struct BinaryBlock
    {
        public const int NullExists = -1;
        public static BinaryBlock Empty = new BinaryBlock() { };

        [System.Runtime.InteropServices.FieldOffset(0)]
        public BlockType Type;
        [System.Runtime.InteropServices.FieldOffset(1)]
        public ushort Ordinal;
        [System.Runtime.InteropServices.FieldOffset(2)]
        public byte Reserved;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public uint Value;
    }

    internal class BinaryBlockReader : System.IDisposable
    {
        #region Constants
        const int MaxReadSize = 64 * 1024; // 64kb
        #endregion
        #region Constructors
        public BinaryBlockReader(System.IO.Stream input)
        {
            _stream = input;
            _reader = new System.IO.BinaryReader(input);
        }
        #endregion
        #region Members
        public long Length { get { return _stream.Length; } }
        public long Position { get { return _stream.Position; } }

        private System.IO.BinaryReader _reader;
        private System.IO.Stream _stream;
        #endregion
        #region Methods
        public BinaryBlock ReadBinaryBlock()
        {
            return new BinaryBlock() { Value = _reader.ReadUInt32() };
        }

        public byte[] ReadBlob()
        {
            int length = _reader.ReadInt32();
            if (length == BinaryBlock.NullExists)
            {
                return null;
            }
            else
            {
                return _reader.ReadBytes(length);
            }
        }

        public System.Collections.Generic.List<byte[]> ReadBlobList()
        {
            System.Collections.Generic.List<byte[]> values = new System.Collections.Generic.List<byte[]>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int length = _reader.ReadInt32();
                if (length == BinaryBlock.NullExists)
                {
                    values.Add(null);
                }
                else
                {
                    values.Add(_reader.ReadBytes(length));
                }
            }
            return values;
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public System.Collections.Generic.List<byte> ReadByteList()
        {
            System.Collections.Generic.List<byte> values = new System.Collections.Generic.List<byte>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadByte());
            }
            return values;
        }

        public char ReadChar()
        {
            return _reader.ReadChar();
        }

        public System.Collections.Generic.List<char> ReadCharList()
        {
            System.Collections.Generic.List<char> values = new System.Collections.Generic.List<char>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadChar());
            }
            return values;
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public System.Collections.Generic.List<double> ReadDoubleList()
        {
            System.Collections.Generic.List<double> values = new System.Collections.Generic.List<double>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadDouble());
            }
            return values;
        }

        public System.Guid ReadGuid()
        {
            byte[] bytes = _reader.ReadBytes(16);
            return new System.Guid(bytes);
        }

        public System.Collections.Generic.List<System.Guid> ReadGuidList()
        {
            System.Collections.Generic.List<System.Guid> values = new System.Collections.Generic.List<System.Guid>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = _reader.ReadBytes(16);
                values.Add(new System.Guid(bytes));
            }
            return values;
        }

        public float ReadSingle()
        {
            return _reader.ReadSingle();
        }

        public System.Collections.Generic.List<float> ReadSingleList()
        {
            System.Collections.Generic.List<float> values = new System.Collections.Generic.List<float>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadSingle());
            }
            return values;
        }

        public int ReadSint()
        {
            return _reader.ReadInt32();
        }

        public System.Collections.Generic.List<int> ReadSintList()
        {
            System.Collections.Generic.List<int> values = new System.Collections.Generic.List<int>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadInt32());
            }
            return values;
        }

        public long ReadSlong()
        {
            return _reader.ReadInt64();
        }

        public System.Collections.Generic.List<long> ReadSlongList()
        {
            System.Collections.Generic.List<long> values = new System.Collections.Generic.List<long>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadInt64());
            }
            return values;
        }

        public System.String ReadString()
        {
            int length = _reader.ReadInt32();
            if (length == BinaryBlock.NullExists)
            {
                return null;
            }
            else
            {
                byte[] bytes = _reader.ReadBytes(length);
                return System.Text.Encoding.UTF7.GetString(bytes);
            }
        }

        public System.Collections.Generic.List<System.String> ReadStringList()
        {
            System.Collections.Generic.List<System.String> values = new System.Collections.Generic.List<string>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int length = _reader.ReadInt32();
                if (length == BinaryBlock.NullExists)
                {
                    values.Add(null);
                }
                else
                {
                    byte[] bytes = _reader.ReadBytes(length);
                    values.Add(System.Text.Encoding.UTF8.GetString(bytes));
                }
            }
            return values;
        }

        public T ReadStruct<T>() where T : IBinaryBlock, new()
        {
            T value = new T();
            int length = _reader.ReadInt32();
            if (length == BinaryBlock.NullExists)
            {
                return default(T);
            }
            else
            {

                byte[] buffer = new byte[MaxReadSize];
                using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                {
                    int read = 0;
                    while ((read = _stream.Read(buffer, 0, MaxReadSize < length ? MaxReadSize : length)) > 0)
                    {
                        memory.Write(buffer, 0, read);
                        length -= read;
                    }
                    memory.Seek(0, System.IO.SeekOrigin.Begin);
                    value.Deserialize(memory);
                }
                return value;
            }
        }

        public System.Collections.Generic.List<T> ReadStructList<T>() where T : IBinaryBlock, new()
        {
            System.Collections.Generic.List<T> values = new System.Collections.Generic.List<T>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                T value = new T();
                int length = _reader.ReadInt32();
                if (length == BinaryBlock.NullExists)
                {
                    values.Add(default(T));
                }
                else
                {
                    byte[] buffer = new byte[MaxReadSize];
                    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                    {
                        int read = 0;
                        while ((read = _stream.Read(buffer, 0, MaxReadSize < length ? MaxReadSize : length)) > 0)
                        {
                            memory.Write(buffer, 0, read);
                            length -= read;
                        }
                        memory.Seek(0, System.IO.SeekOrigin.Begin);
                        value.Deserialize(memory);
                    }
                    values.Add(value);
                }
            }
            return values;
        }

        public System.TimeSpan ReadTimespan()
        {
            long offset = _reader.ReadInt64();
            return new System.TimeSpan(offset);
        }

        public System.Collections.Generic.List<System.TimeSpan> ReadTimespanList()
        {
            System.Collections.Generic.List<System.TimeSpan> values = new System.Collections.Generic.List<System.TimeSpan>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                long ticks = _reader.ReadInt64();
                values.Add(new System.TimeSpan(ticks));
            }
            return values;
        }

        public System.Timestamp ReadTimestamp()
        {
            long microseconds = _reader.ReadInt64();
            return new System.Timestamp(microseconds);
        }

        public System.Collections.Generic.List<System.Timestamp> ReadTimestampList()
        {
            System.Collections.Generic.List<System.Timestamp> values = new System.Collections.Generic.List<System.Timestamp>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                long microseconds = this.ReadSlong();
                values.Add(new System.Timestamp(microseconds));
            }
            return values;
        }

        public uint ReadUint()
        {
            return _reader.ReadUInt32();
        }

        public System.Collections.Generic.List<uint> ReadUintList()
        {
            System.Collections.Generic.List<uint> values = new System.Collections.Generic.List<uint>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadUInt32());
            }
            return values;
        }

        public ulong ReadUlong()
        {
            return _reader.ReadUInt64();
        }

        public System.Collections.Generic.List<ulong> ReadUlongList()
        {
            System.Collections.Generic.List<ulong> values = new System.Collections.Generic.List<ulong>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                values.Add(_reader.ReadUInt64());
            }
            return values;
        }

        public void SkipBlock(BinaryBlock block)
        {
            int count = ((block.Type & BlockType.List) == BlockType.List)
                        ? _reader.ReadInt32()
                        : 1;

            for (int i = 0; i < count; i++)
            {
                switch (block.Type ^ BlockType.List)
                {
                    case BlockType.Byte:
                        _stream.Seek(1, System.IO.SeekOrigin.Current);
                        break;
                    case BlockType.Char:
                        _stream.Seek(2, System.IO.SeekOrigin.Current);
                        break;
                    case BlockType.Single:
                    case BlockType.Sint:
                    case BlockType.Uint:
                        _stream.Seek(4, System.IO.SeekOrigin.Current);
                        break;
                    case BlockType.Timestamp:
                    case BlockType.Double:
                    case BlockType.Slong:
                    case BlockType.Timespan:
                    case BlockType.Ulong:
                        _stream.Seek(8, System.IO.SeekOrigin.Current);
                        break;
                    case BlockType.Guid:
                        _stream.Seek(16, System.IO.SeekOrigin.Current);
                        break;
                    case BlockType.Blob:
                    case BlockType.String:
                    case BlockType.Struct:
                        {
                            int length = _reader.ReadInt32();
                            _stream.Seek(length, System.IO.SeekOrigin.Current);
                        } break;
                    default:
                        throw new InvalidBlockTypeException();
                }
            }
        }
        #endregion
        #region System.IDisposable
        void System.IDisposable.Dispose()
        {
            _reader.Dispose();
            _stream.Dispose();
        }
        #endregion
    }

    internal class BinaryBlockWriter : System.IDisposable
    {
        #region Constructors
        public BinaryBlockWriter(System.IO.Stream output)
        {
            _stream = output;
            _writer = new System.IO.BinaryWriter(output);
        }
        #endregion
        #region Members
        public long Length { get { return _stream.Length; } }
        public long Position { get { return _stream.Position; } }

        private System.IO.BinaryWriter _writer;
        private System.IO.Stream _stream;
        #endregion
        #region Methods
        public void WriteBlob(byte[] value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Blob }).Value);
            if (value == null)
            {
                _writer.Write(BinaryBlock.NullExists);
            }
            else
            {
                _writer.Write(value.Length);
                _writer.Write(value);
            }
        }

        public void WriteBlobList(System.Collections.Generic.List<byte[]> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Byte }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    _writer.Write(BinaryBlock.NullExists);
                }
                else
                {
                    _writer.Write(values[i].Length);
                    _writer.Write(values[i]);
                }
            }
        }

        public void WriteByte(byte value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Byte }).Value);
            _writer.Write(value);
        }

        public void WriteByteList(System.Collections.Generic.List<byte> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Byte }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteChar(char value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Char }).Value);
            _writer.Write(value);
        }

        public void WriteCharList(System.Collections.Generic.List<char> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Char }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteDouble(double value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Double }).Value);
            _writer.Write(value);
        }

        public void WriteDoubleList(System.Collections.Generic.List<double> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Double }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteGuid(System.Guid value, ushort ordinal)
        {
            byte[] bytes = value.ToByteArray();
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Guid }).Value);
            _writer.Write(bytes);
        }

        public void WriteGuidList(System.Collections.Generic.List<System.Guid> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Guid }).Value);
            _writer.Write(values.Count);
            byte[] bytes = null;
            for (int i = 0; i < values.Count; i++)
            {
                bytes = values[i].ToByteArray();
                _writer.Write(bytes);
            }
        }

        public void WriteSingle(float value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Single }).Value);
            _writer.Write(value);
        }

        public void WriteSingleList(System.Collections.Generic.List<float> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Single }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteSint(int value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Sint }).Value);
            _writer.Write(value);
        }

        public void WriteSintList(System.Collections.Generic.List<int> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Sint }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteSlong(long value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Slong }).Value);
            _writer.Write(value);
        }

        public void WriteSlongList(System.Collections.Generic.List<long> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Slong }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteString(System.String value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.String }).Value);
            if (value == null)
            {
                _writer.Write(BinaryBlock.NullExists);
            }
            else
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
                _writer.Write(buffer.Length);
                _writer.Write(buffer);
            }
        }

        public void WriteStringList(System.Collections.Generic.List<System.String> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.String }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    _writer.Write(BinaryBlock.NullExists);
                }
                else
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(values[i]);
                    _writer.Write(bytes.Length);
                    _writer.Write(bytes);
                }
            }
        }

        public void WriteStruct<T>(T value, ushort ordinal) where T : IBinaryBlock, new()
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Struct }).Value);
            if (value == null)
            {
                _writer.Write(BinaryBlock.NullExists);
            }
            else
            {
                using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                {
                    // serialize the struct into the stream
                    (value as IBinaryBlock).Serialize(memory);
                    // write the size of the struct
                    int length = (int)memory.Position;
                    _writer.Write(length);
                    // reset the stream and write content
                    memory.Seek(0, System.IO.SeekOrigin.Begin);
                    byte[] buffer = new byte[64 * 1024];
                    int read = 0;
                    while ((read = memory.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        _writer.Write(buffer, 0, read);
                    }
                }
            }
        }

        public void WriteStructList<T>(System.Collections.Generic.List<T> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Struct }).Value);
            _writer.Write(values.Count);
            byte[] buffer = new byte[64 * 1024];
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    _writer.Write(BinaryBlock.NullExists);
                }
                else
                {
                    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                    {
                        // serialize the struct into the stream
                        (values[i] as IBinaryBlock).Serialize(memory);
                        // write the size of the struct
                        int length = (int)memory.Position;
                        _writer.Write(length);
                        // reset the stream and write content
                        memory.Seek(0, System.IO.SeekOrigin.Begin);
                        int read = 0;
                        while ((read = memory.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            _writer.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }

        public void WriteTimespan(System.TimeSpan value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Timespan }).Value);
            _writer.Write(value.Ticks);
        }

        public void WriteTimespanList(System.Collections.Generic.List<System.TimeSpan> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Timespan }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i].Ticks);
            }
        }

        public void WriteTimestamp(System.Timestamp value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Timestamp }).Value);
            _writer.Write(value.TotalMilliseconds);
        }

        public void WriteTimestampList(System.Collections.Generic.List<System.Timestamp> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Double }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i].TotalMilliseconds);
            }
        }

        public void WriteUint(uint value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Uint }).Value);
            _writer.Write(value);
        }

        public void WriteUintList(System.Collections.Generic.List<uint> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Uint }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteUlong(ulong value, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Ulong }).Value);
            _writer.Write(value);
        }

        public void WriteUlongList(System.Collections.Generic.List<ulong> values, ushort ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Ulong }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }
        #endregion
        #region System.IDisposable
        void System.IDisposable.Dispose()
        {
            _writer.Dispose();
            _stream.Dispose();
        }
        #endregion
    }

    internal abstract class IBinaryBlock
    {
        public abstract void Serialize(System.IO.Stream input);
        public abstract void Deserialize(System.IO.Stream output);
    }

    internal class InvalidBlockTypeException : System.Exception
    {
        public InvalidBlockTypeException() : base() { }
        public InvalidBlockTypeException(string message) : base(message) { }
        public InvalidBlockTypeException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}

namespace System
{
    /// <summary>
    /// Structure which represents the date before or after 00-01-01 00:00:00.0000Z with microsecond granuarity. Effective range is roughly 600 million years ranging from 250,000 BCE to 250,000 CE.
    /// </summary>
    /// <remarks>All values of System.Timestamp are in UTC and stored to the nearest microsecond (10^-6 seconds)</remarks>
    internal struct Timestamp : System.IComparable, System.IComparable<Timestamp>, System.IEquatable<Timestamp>
    {
        #region Constants
        private const int MinimumYear = -250000000;
        private const int MaximumYear = 249999999;
        public const int SecondsPerMinute = 60;
        public const int MinutesPerHour = 60;
        public const int HoursPerDay = 24;
        public const int DaysPerYear = 365;
        public const int YearsPerLeapYear = 4;
        public const int YearsPer1CLeapYear = 100;
        public const int YearsPer4CLeapYear = 400;
        public const int YearsPer4MLeapYear = 4000;
        public const int TicksPerMicrosecond = 10000;
        public const long MillisecondsPerSecond = 1000;
        public const long MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;
        public const long MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;
        public const long MillisecondsPerDay = MillisecondsPerHour * HoursPerDay;
        public const long MillisecondsPerYear = MillisecondsPerDay * DaysPerYear;
        public static readonly int[] DaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        /// <summary>
        /// Represents the smallest possible value of System.Timestamp. This field is read-only.
        /// </summary>
        public static readonly Timestamp MinValue = new Timestamp(MinimumYear, 1, 1, 0, 0, 0, 0);
        /// <summary>
        /// Represents the largest possible value of System.Timestamp. This field is read-only.
        /// </summary>
        public static readonly Timestamp MaxValue = new Timestamp(MaximumYear, 12, 31, 23, 59, 59, 999);
        #endregion
        #region Enumerations
        public enum TimestampEra
        {
            /// <summary>
            /// Before the Common Era. All dates previous to 1 Jan 0001.
            /// </summary>
            BCE,
            /// <summary>
            /// Common Era. All dates after and including 1 Jan 0001
            /// </summary>
            CE
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a Timestamp structure from the raw number of microseconds
        /// </summary>
        /// <param name="milliseconds">The number of microseconds before or after 0-01-01 CE 00:00:00.0000Z</param>
        public Timestamp(long milliseconds)
            : this()
        {
            #region Parameter Validation
            if (milliseconds < Timestamp.MinValue.TotalMilliseconds || milliseconds > Timestamp.MaxValue.TotalMilliseconds)
                throw new System.ArgumentOutOfRangeException("microseconds", System.String.Format("The microseconds parameter is restricted to [{0}, {1}]", Timestamp.MinValue.TotalMilliseconds, Timestamp.MaxValue.TotalMilliseconds));
            #endregion
            // record the actuall milliseconds passed in as the base value
            this.TotalMilliseconds = milliseconds;
            // get the absolute number of milliseconds because the math works this way
            milliseconds = System.Math.Abs(milliseconds);
            // local variables
            int year = 0;
            int month = -1;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int millisecond = 0;
            // compute the year
            day = (int)(milliseconds / MillisecondsPerDay);
            milliseconds -= day * MillisecondsPerDay;
            // calculate the year based on the number of days, first by blocks of 400 the by per year
            /**
             * investigated into doing this via a simple math equation with years first, but was unable to because
             * deriving years first gave the wrong number of leap years too frequently
            **/
            const int DaysPer400Years = 146097;
            while (day >= DaysPer400Years)
            {
                year += 400;
                day -= DaysPer400Years;
            }
            while (day >= DaysPerYear)
            {
                year++;
                day -= DaysPerYear;

                if (year % YearsPerLeapYear == 0 && (year % YearsPer1CLeapYear != 0 || year % YearsPer4CLeapYear == 0))
                    day -= 1;
            }
            // compute the hours
            hour = (int)(milliseconds / MillisecondsPerHour);
            milliseconds -= hour * MillisecondsPerHour;
            // compute the minutes
            minute = (int)(milliseconds / MillisecondsPerMinute);
            milliseconds -= minute * MillisecondsPerMinute;
            // compute the seconds
            second = (int)(milliseconds / MillisecondsPerSecond);
            milliseconds -= second * MillisecondsPerSecond;
            // collect the left overs
            millisecond = (int)milliseconds;
            // discover the month by subtracting the days per month of each month starting with January
            for (int i = 0; i < DaysPerMonth.Length; i++)
            {
                int daysInTheMonth = DaysPerMonth[i];
                // account for leap years -- annoying 365.2425 day year
                if (i == 2 && year % YearsPerLeapYear == 0 && (year % YearsPer1CLeapYear != 0 || year % YearsPer4CLeapYear == 0))
                {
                    daysInTheMonth += 1;
                }
                // once we find a month with more days that we have left, we've found our month
                if (day < daysInTheMonth)
                {
                    month = i;
                    break;
                }
                day -= daysInTheMonth;
            }
            // some debug only asserts to make sure we're on the right path
            System.Diagnostics.Debug.Assert(month >= 0 && month <= 11, "Invalid value for System.Timestamp.Month");
            System.Diagnostics.Debug.Assert(day >= 0 && day < DaysPerMonth[month], "Invalid value for System.Timestamp.Day");
            System.Diagnostics.Debug.Assert(hour >= 0 && hour < 24, "Invalid value for System.Timestamp.Hour");
            System.Diagnostics.Debug.Assert(minute >= 0 && month < 60, "Invalid value for System.Timestamp.Minute");
            System.Diagnostics.Debug.Assert(second >= 0 && second < 60, "Invalid value for System.Timestamp.Second");
            System.Diagnostics.Debug.Assert(millisecond >= 0 && millisecond < 1000, "Invalid value for System.Timestamp.Millisecond");
            // assign all public values
            this.Year = year;
            this.Month = month + 1;
            this.Day = day + 1;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Era = this.TotalMilliseconds < 0 ? TimestampEra.BCE : TimestampEra.CE;
        }
        /// <summary>
        /// Creates a Timestamp structure
        /// </summary>
        /// <param name="year">The year component of the date represented by this instance, expressed as a value between -250,000 and 250,000.</param>
        /// <param name="month">The month component of the date represented by this instance, expressed as a value between 1 and 12.</param>
        /// <param name="day">The day of the month represented by this instance, expressed as value between 1 and the length of the month.</param>
        /// <param name="hour">The hour component of the date represented by this instance, expressed as a value between 0 and 23.</param>
        /// <param name="minute">The minute component of the date represented by this instance, expressed as a value between 0 and 59.</param>
        /// <param name="second">The seconds component of the date represented by this instance, expressed as a value between 0 and 59.</param>
        /// <param name="millisecond">The millisecond component of the date represented by this instance, expressed as a value between 0 and 999.</param>
        public Timestamp(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
            : this()
        {
            #region Parameter Validation
            if (year < MinimumYear || year > MaximumYear)
                throw new System.ArgumentOutOfRangeException("year", "The year parameter is restricted to [" + MinimumYear + ", " + MaximumYear + "]");
            if (month < 1 || month > 12)
                throw new System.ArgumentOutOfRangeException("month", "The month parameter is restricted to [1, 12]");
            if (day < 1 || day > DaysPerMonth[month - 1])
                throw new System.ArgumentOutOfRangeException("day", System.String.Format("The day parameter is restricted to [1, {0}]", DaysPerMonth[month]));
            if (hour < 0 || hour > 23)
                throw new System.ArgumentOutOfRangeException("hour", "The hour parameter is restricted to [0, 23)");
            if (minute < 0 || minute > 59)
                throw new System.ArgumentOutOfRangeException("minute", "The minute parameter is restricted to [0, 60)");
            if (second < 0 || second > 59)
                throw new System.ArgumentOutOfRangeException("second", "The second parameter is restricted to [0, 60)");
            if (millisecond < 0 || millisecond > 999)
                throw new System.ArgumentOutOfRangeException("millisecond", "The millisecond parameter is restricted to [0, 1000)");
            #endregion
            int absyear = System.Math.Abs(year);
            long milliseconds = absyear * MillisecondsPerYear;
            milliseconds += (absyear / YearsPerLeapYear) * MillisecondsPerDay;
            milliseconds -= (absyear / YearsPer1CLeapYear) * MillisecondsPerDay;
            milliseconds += (absyear / YearsPer4CLeapYear) * MillisecondsPerDay;
            for (int i = 0; i < month - 1; i++)
            {
                milliseconds += DaysPerMonth[i] * MillisecondsPerDay;
                // if we're in March, add an extra day if it is a leap year
                if (i == 2 && year % YearsPerLeapYear == 0 && (year % YearsPer1CLeapYear != 0 || year % YearsPer4CLeapYear == 0))
                {
                    milliseconds += MillisecondsPerDay;
                }
            }
            milliseconds += (day - 1) * MillisecondsPerDay;
            milliseconds += hour * MillisecondsPerHour;
            milliseconds += minute * MillisecondsPerMinute;
            milliseconds += second * MillisecondsPerSecond;

            this.Year = absyear;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Era = year < 0 ? TimestampEra.BCE : TimestampEra.CE;
            this.TotalMilliseconds = year < 0 ? -milliseconds : milliseconds;
        }
        /// <summary>
        /// Creates a System.Timestamp from a System.DateTime
        /// </summary>
        /// <param name="datetime">The System.DateTime to be converted</param>
        public Timestamp(System.DateTime datetime)
            : this(datetime, TimestampEra.CE)
        { }
        /// <summary>
        /// Creates a Timestamp from a System.DateTime with date before 00-01-01 possible
        /// </summary>
        /// <param name="datetime">The System.DateTime to be converted</param>
        /// <param name="era">Sets the era as CE or BCE</param>
        public Timestamp(System.DateTime datetime, TimestampEra era)
            : this(datetime.Ticks / TicksPerMicrosecond * (era == TimestampEra.CE ? 1 : -1))
        { }

        public Timestamp(System.Timestamp timestamp)
            : this()
        {
            this.Day = timestamp.Day;
            this.Era = timestamp.Era;
            this.Hour = timestamp.Hour;
            this.Millisecond = timestamp.Millisecond;
            this.Minute = timestamp.Minute;
            this.Month = timestamp.Month;
            this.Second = timestamp.Second;
            this.Year = timestamp.Year;
            this.TotalMilliseconds = timestamp.TotalMilliseconds;
        }
        #endregion
        #region Members
        /// <summary>
        /// Gets a System.DateTime with the same date as this instance, and the time value set to 12:00:00 midnight (00:00:00).
        /// </summary>
        public DateTime Date { get { return new System.DateTime(this.Year, this.Month, this.Day, 0, 0, 0, DateTimeKind.Utc); } }
        /// <summary>
        /// Gets the day of the month represented by this instance, expressed as value between 1 and the length of the month.
        /// </summary>
        public int Day { get; private set; }
        /// <summary>
        /// Gets the hour component of the date represented by this instance, expressed as a value between 0 and 23.
        /// </summary>
        public int Hour { get; private set; }
        /// <summary>
        /// Gets the millisecond component of the date represented by this instance, expressed as a value between 0 and 999.
        /// </summary>
        public int Millisecond { get; private set; }
        /// <summary>
        /// Gets the minute component of the date represented by this instance, expressed as a value between 0 and 59.
        /// </summary>
        public int Minute { get; private set; }
        /// <summary>
        /// Gets the month component of the date represented by this instance, expressed as a value between 1 and 12.
        /// </summary>
        public int Month { get; private set; }
        /// <summary>
        /// Gets the seconds component of the date represented by this instance, expressed as a value between 0 and 59.
        /// </summary>
        public int Second { get; private set; }
        /// <summary>
        /// Gets the year component of the date represented by this instance, expressed as a value between -250,000 and 250,000.
        /// </summary>
        public int Year { get; private set; }
        /// <summary>
        /// Gets the era component of the date represented by this instance.
        /// </summary>
        public TimestampEra Era { get; private set; }
        /// <summary>
        /// Gets the number of ticks that represent the date and time of this instance either before or after 0-1-1 00:00:00Z.
        /// </summary>
        public long Ticks { get { return this.TotalMilliseconds * TicksPerMicrosecond; } }
        /// <summary>
        /// Get the number of milliseconds before or after 00-01-01 00:00:00.000Z
        /// </summary>
        public long TotalMilliseconds { get; private set; }
        #endregion
        #region Methods
        /// <summary>
        /// Returns a new System.Timestamp that adds the value of the specified System.TimeSpan to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the time interval represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp Add(System.TimeSpan value)
        {
            return new System.Timestamp(this.TotalMilliseconds + value.Ticks / TicksPerMicrosecond);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">A number of days. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of days represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddDays(int value)
        {
            return new System.Timestamp(this.TotalMilliseconds + (value * MillisecondsPerDay));
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="value">A number of hours. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of hours represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddHours(int value)
        {
            return new System.Timestamp(this.TotalMilliseconds + (value * MillisecondsPerHour));
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of milliseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of milliseconds. The value parameter can be negative or positive. Note that this value is rounded to the nearest integer.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of milliseconds represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddMilliseconds(int value)
        {
            return new System.Timestamp(this.TotalMilliseconds + value);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole minutes. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of minutes represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddMinutes(int value)
        {
            return new System.Timestamp(this.TotalMilliseconds + (value * MillisecondsPerMinute));
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="months">A number of months. The months parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and months.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddMonths(int months)
        {
            long microseconds = this.TotalMilliseconds;
            for (int i = 0; i < months; i++)
            {
                int month = (this.Month + i) % DaysPerMonth.Length;
                microseconds += DaysPerMonth[month] * MillisecondsPerDay;
                if (month == 1 &&
                    (this.Year + (this.Month + i) / 12) % YearsPerLeapYear == 0 &&
                    (this.Year + (this.Month + i) / 12) % YearsPer1CLeapYear != 100 &&
                    (this.Year + (this.Month + i) / 12) % YearsPer4CLeapYear == 0)
                {
                    microseconds += MillisecondsPerDay;
                }
            }
            return new System.Timestamp(microseconds);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of seconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of seconds. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of seconds represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddSeconds(int value)
        {
            return new System.Timestamp(this.TotalMilliseconds + (value * MillisecondsPerSecond));
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of ticks to the value of this instance.
        /// </summary>
        /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be positive or negative.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the time represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddTicks(long value)
        {
            return new System.Timestamp(this.TotalMilliseconds + value / TicksPerMicrosecond);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of years represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddYears(int value)
        {
            return new Timestamp(this.TotalMilliseconds + value * MillisecondsPerYear);
        }
        /// <summary>
        /// Converts System.Timestamp to System.DateTime.
        /// </summary>
        /// <returns>A System.DateTime which represents the same date and time as this System.Timespace.</returns>
        public System.DateTime ToDateTime()
        {
            return new System.DateTime(this.TotalMilliseconds * TicksPerMicrosecond, System.DateTimeKind.Utc);
        }
        #region System.Object
        /// <summary>
        /// Determines whether the specified System.Timestamp is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The value to compare with the current value.</param>
        /// <returns> true if the specified value is equal to the value object; otherwise, false.</returns>
        /// <remarks>Overrides System.Object.Equals</remarks>
        /// <exception cref="System.ArgumentException">obj is not the same type as this instance.</exception>
        public override bool Equals(object obj)
        {
            #region Parameter Validation
            if (obj == null)
                throw new System.ArgumentNullException("obj");
            if (!(obj is Timestamp))
                throw new System.ArgumentException("The type of obj is not System.Timestamp", "obj");
            #endregion
            return this.TotalMilliseconds.Equals(((Timestamp)obj).TotalMilliseconds);
        }
        /// <summary>
        /// Serves as a hash function for a System.Timestamp.
        /// </summary>
        /// <returns>A hash code for the current System.Timestamp.</returns>
        /// <remarks>Overrides System.Object.GetHashCode</remarks>
        public override int GetHashCode()
        {
            return this.TotalMilliseconds.GetHashCode();
        }
        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        /// <returns>A UTC formatted string that represents the current value.</returns>
        public override string ToString()
        {
            return string.Format("{0:###,###,###,#00}-{1:00}-{2:00} {3} {4:00}:{5:00}:{6:00}Z", Year, Month, Day, Era, Hour, Minute, Second);
        }
        #endregion
        #region System.IComparable
        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.The return value has these meanings: Value Meaning Less than zero This instance precedes obj in the sort order. Zero This instance occurs in the same position in the sort order as obj. Greater than zero This instance follows obj in the sort order.</returns>
        /// <exception cref="System.ArgumentException">obj is not the same type as this instance.</exception>
        int System.IComparable.CompareTo(object obj)
        {
            #region Parameter Validation
            if (obj == null)
                throw new System.ArgumentNullException("obj");
            if (!(obj is Timestamp))
                throw new System.ArgumentException("The type of obj is not System.Timestamp", "obj");
            #endregion
            return this.TotalMilliseconds.CompareTo(((Timestamp)obj).TotalMilliseconds);
        }
        #endregion
        #region System.IComparable<Timestamp>
        /// <summary>
        /// Compares the current value with another value of the same type.
        /// </summary>
        /// <param name="that">A value to compare with this value.</param>
        /// <returns>A value that indicates the relative order of the values being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter. Zero This object is equal to other. Greater than zero This object is greater than other.</returns>
        [System.Security.SecurityCritical]
        int System.IComparable<System.Timestamp>.CompareTo(System.Timestamp that)
        {
            // Timestamp is a struct, no need to check null first
            return this.TotalMilliseconds.CompareTo(that.TotalMilliseconds);
        }
        #endregion
        #region System.IEquatable<Timestamp>
        /// <summary>
        /// Indicates whether the current value is equal to another value of the same type.
        /// </summary>
        /// <param name="that">A value to compare with this value.</param>
        /// <returns>true if the current value is equal to the value parameter; otherwise, false.</returns>
        bool System.IEquatable<System.Timestamp>.Equals(System.Timestamp that)
        {
            // Timestamp is a struct, no need to check null first
            return this.TotalMilliseconds == that.TotalMilliseconds;
        }
        #endregion
        #endregion
        #region Operators
        /// <summary>
        /// Subtracts a specified date and time from another specified date and time and returns a time interval.
        /// </summary>
        /// <param name="a">The date and time value to subtract from (the minuend).</param>
        /// <param name="b">The date and time value to subtract (the subtrahend).</param>
        /// <returns>The time interval between a and b; that is, a minus b.</returns>
        public static System.TimeSpan operator -(System.Timestamp a, System.Timestamp b)
        {
            long ticks = (a.TotalMilliseconds - b.TotalMilliseconds) * 10;
            return new System.TimeSpan(ticks);
        }
        /// <summary>
        /// Subtracts a specified time interval from a specified date and time and returns a new date and time.
        /// </summary>
        /// <param name="d">The date and time value to subtract from.</param>
        /// <param name="t">The time interval to subtract.</param>
        /// <returns>A System.Timestamp whose value is the value of d minus the value of t.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public static System.Timestamp operator -(System.Timestamp d, System.TimeSpan t)
        {
            return new System.Timestamp(d.TotalMilliseconds - (t.Ticks / System.Timestamp.TicksPerMicrosecond));
        }
        /// <summary>
        /// Determines whether two specified instances of System.Timestamp are not equal.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a and b do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(System.Timestamp a, System.Timestamp b)
        {
            {
                return a.TotalMilliseconds != b.TotalMilliseconds;
            }
        }
        /// <summary>
        /// Adds a specified time interval to a specified date and time, yielding a new date and time.
        /// </summary>
        /// <param name="d">The date and time value to add.</param>
        /// <param name="t">The time interval to add.</param>
        /// <returns>A System.Timestamp that is the sum of the values of d and t.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public static System.Timestamp operator +(System.Timestamp d, System.TimeSpan t)
        {
            return new System.Timestamp(d.TotalMilliseconds + (t.Ticks / System.Timestamp.TicksPerMicrosecond));
        }
        /// <summary>
        /// Determines whether one specified System.Timestamp is less than another specified System.Timestamp.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a is less than b; otherwise, false.</returns>
        public static bool operator <(System.Timestamp a, System.Timestamp b)
        {
            return a.TotalMilliseconds < b.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether one specified System.Timestamp is less than or equal to another specified System.Timestamp.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a is less than or equal to b; otherwise, false.</returns>
        public static bool operator <=(System.Timestamp a, System.Timestamp b)
        {
            return a.TotalMilliseconds <= b.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether two specified instances of System.Timestamp are equal.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a and b represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(System.Timestamp a, System.Timestamp b)
        {
            return a.TotalMilliseconds == b.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether one specified System.Timestamp is greater than another specified System.Timestamp.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns> true if a is greater than b; otherwise, false.</returns>
        public static bool operator >(System.Timestamp a, System.Timestamp b)
        {
            return a.TotalMilliseconds > b.TotalMilliseconds;
        }
        /// <summary>
        /// Determines whether one specified System.Timestamp is greater than or equal to another specified System.Timestamp
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a is greater than or equal to b; otherwise, false.</returns>
        public static bool operator >=(System.Timestamp a, System.Timestamp b)
        {
            return a.TotalMilliseconds >= b.TotalMilliseconds;
        }
        #endregion
    }
}
