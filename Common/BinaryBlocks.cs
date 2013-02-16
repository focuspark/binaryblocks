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

namespace Isris.BinaryBlocks
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
        Datetime = 0x0A,
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
        public static System.DateTime BaseDateTime = new System.DateTime(1, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        [System.Runtime.InteropServices.FieldOffset(0)]
        public BlockType Type;
        [System.Runtime.InteropServices.FieldOffset(1)]
        public byte Ordinal;
        [System.Runtime.InteropServices.FieldOffset(2)]
        public ushort Reserved;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public uint Value;
    }

    internal unsafe class BinaryBlockReader : System.IDisposable
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

        public System.DateTime ReadDatetime()
        {
            long offset = this.ReadSlong();
            return BinaryBlock.BaseDateTime.AddMilliseconds(offset);
        }

        public System.Collections.Generic.List<System.DateTime> ReadDatetimeList()
        {
            System.Collections.Generic.List<System.DateTime> values = new System.Collections.Generic.List<System.DateTime>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                long offset = _reader.ReadInt64();
                values.Add(BinaryBlock.BaseDateTime.AddMilliseconds(offset));
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
            byte[] bytes = _reader.ReadBytes(sizeof(System.Guid));
            return new System.Guid(bytes);
        }

        public System.Collections.Generic.List<System.Guid> ReadGuidList()
        {
            System.Collections.Generic.List<System.Guid> values = new System.Collections.Generic.List<System.Guid>();
            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = _reader.ReadBytes(sizeof(System.Guid));
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
                    case BlockType.Datetime:
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

    internal unsafe class BinaryBlockWriter : System.IDisposable
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
        public void WriteBlob(byte[] value, byte ordinal)
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

        public void WriteBlobList(System.Collections.Generic.List<byte[]> values, byte ordinal)
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

        public void WriteByte(byte value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Byte }).Value);
            _writer.Write(value);
        }

        public void WriteByteList(System.Collections.Generic.List<byte> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Byte }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteChar(char value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Char }).Value);
            _writer.Write(value);
        }

        public void WriteCharList(System.Collections.Generic.List<char> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Char }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteDatetime(System.DateTime value, byte ordinal)
        {
            long offset = (long)((value - BinaryBlock.BaseDateTime).TotalMilliseconds);
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Datetime }).Value);
            _writer.Write(offset);
        }

        public void WriteDatetimeList(System.Collections.Generic.List<System.DateTime> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Double }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                long value = (long)((values[i] - BinaryBlock.BaseDateTime).TotalMilliseconds);
                _writer.Write(value);
            }
        }

        public void WriteDouble(double value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Double }).Value);
            _writer.Write(value);
        }

        public void WriteDoubleList(System.Collections.Generic.List<double> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Double }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteGuid(System.Guid value, byte ordinal)
        {
            byte[] bytes = new byte[sizeof(System.Guid)];
            fixed (byte* b = bytes) { *((System.Guid*)b) = value; }
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Guid }).Value);
            _writer.Write(bytes);
        }

        public void WriteGuidList(System.Collections.Generic.List<System.Guid> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Guid }).Value);
            _writer.Write(values.Count);
            byte[] bytes = new byte[sizeof(System.Guid)];
            for (int i = 0; i < values.Count; i++)
            {
                fixed (byte* b = bytes) { *((System.Guid*)b) = values[i]; }
                _writer.Write(bytes);
            }
        }

        public void WriteSingle(float value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Single }).Value);
            _writer.Write(value);
        }

        public void WriteSingleList(System.Collections.Generic.List<float> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Single }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteSint(int value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Sint }).Value);
            _writer.Write(value);
        }

        public void WriteSintList(System.Collections.Generic.List<int> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Sint }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteSlong(long value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Slong }).Value);
            _writer.Write(value);
        }

        public void WriteSlongList(System.Collections.Generic.List<long> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Slong }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteString(System.String value, byte ordinal)
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

        public void WriteStringList(System.Collections.Generic.List<System.String> values, byte ordinal)
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

        public void WriteStruct<T>(T value, byte ordinal) where T : IBinaryBlock, new()
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

        public void WriteStructList<T>(System.Collections.Generic.List<T> values, byte ordinal)
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

        public void WriteTimespan(System.TimeSpan value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Timespan }).Value);
            _writer.Write(value.Ticks);
        }

        public void WriteTimespanList(System.Collections.Generic.List<System.TimeSpan> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Timespan }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i].Ticks);
            }
        }

        public void WriteUint(uint value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Uint }).Value);
            _writer.Write(value);
        }

        public void WriteUintList(System.Collections.Generic.List<uint> values, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.List | BlockType.Uint }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteUlong(ulong value, byte ordinal)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Ulong }).Value);
            _writer.Write(value);
        }

        public void WriteUlongList(System.Collections.Generic.List<ulong> values, byte ordinal)
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

    internal abstract unsafe class IBinaryBlock
    {
        public abstract unsafe void Serialize(System.IO.Stream input);
        public abstract unsafe void Deserialize(System.IO.Stream output);
    }

    internal class InvalidBlockTypeException : System.Exception
    {
        public InvalidBlockTypeException() : base() { }
        public InvalidBlockTypeException(string message) : base(message) { }
        public InvalidBlockTypeException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
