﻿/**
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
        /* list of types */
        ByteList = Byte | List,
        CharList = Char | List,
        SintList = Sint | List,
        UintList = Uint | List,
        SlongList = Slong | List,
        UlongList = Ulong | List,
        SingleList = Single | List,
        DoubleList = Double | List,
        StringList = String | List,
        TimestampList = Timestamp | List,
        TimespanList = Timespan | List,
        BlobList = Blob | List,
        GuidList = Guid | List,
        EnumList = Enum | List,
        StructList = Struct | List,
    }

    [System.Flags]
    internal enum BlockFlags : byte
    {
        None = 0,
        Deprecated = 1 << 0,
        UnusedFlag1 = 1 << 1,
        UnusedFlag2 = 1 << 2,
        UnusedFlag3 = 1 << 3,
        UnusedFlag4 = 1 << 4,
        UnusedFlag5 = 1 << 5,
        UnusedFlag6 = 1 << 6,
        UnusedFlag7 = 1 << 7,
    }

#if WINDOWS_PHONE || NETCF
    internal struct BinaryBlock
    {
        public const int NullExists = -1;
        public static readonly BinaryBlock Empty = new BinaryBlock() { Value = 0 };

        public BlockType Type;
        public ushort Ordinal;
        public BlockFlags Flags;
        public uint Value
        {
            get { return (((uint)Ordinal) << 8) + (uint)Type; }
            set
            {
                Ordinal = (ushort)(value >> 8);
                Type = (BlockType)(value & 0x000000FF);
            }
        }
    }
#else
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
    internal struct BinaryBlock
    {
        public const int NullExists = -1;
        public static readonly BinaryBlock Empty = new BinaryBlock() { Value = 0 };

        [System.Runtime.InteropServices.FieldOffset(0)]
        public BlockType Type;
        [System.Runtime.InteropServices.FieldOffset(1)]
        public ushort Ordinal;
        [System.Runtime.InteropServices.FieldOffset(3)]
        public BlockFlags Flags;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public uint Value;
    }
#endif

    internal class BinaryBlockReader : System.IDisposable
    {
        #region Constants
        const int BufferSize = 64 * 1024; // 64kb
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
                return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
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
                    values.Add(System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length));
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
                value.Deserialize(new StreamSegment(_stream, length));
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
                    value.Deserialize(new StreamSegment(_stream, length));
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
                switch (block.Type & ~BlockType.List)
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
            _stream.Flush();
        }
        #endregion
    }

    internal class BinaryBlockWriter : System.IDisposable
    {
        #region Constants
        const int BufferSize = 64 * 1024; // 64kb
        #endregion
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
        public void WriteBlob(byte[] value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Blob, Flags = flags }).Value);
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

        public void WriteBlobList(System.Collections.Generic.List<byte[]> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.BlobList, Flags = flags }).Value);
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

        public void WriteByte(byte value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Byte, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteByteList(System.Collections.Generic.List<byte> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.ByteList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteChar(char value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Char, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteCharList(System.Collections.Generic.List<char> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.CharList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteDouble(double value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Double, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteDoubleList(System.Collections.Generic.List<double> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.DoubleList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteGuid(System.Guid value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            byte[] bytes = value.ToByteArray();
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Guid, Flags = flags }).Value);
            _writer.Write(bytes);
        }

        public void WriteGuidList(System.Collections.Generic.List<System.Guid> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.GuidList, Flags = flags }).Value);
            _writer.Write(values.Count);
            byte[] bytes = null;
            for (int i = 0; i < values.Count; i++)
            {
                bytes = values[i].ToByteArray();
                _writer.Write(bytes);
            }
        }

        public void WriteSingle(float value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Single, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteSingleList(System.Collections.Generic.List<float> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.SingleList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteSint(int value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Sint, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteSintList(System.Collections.Generic.List<int> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.SintList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteSlong(long value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Slong, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteSlongList(System.Collections.Generic.List<long> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.SlongList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteString(System.String value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.String, Flags = flags }).Value);
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

        public void WriteStringList(System.Collections.Generic.List<System.String> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.StringList, Flags = flags }).Value);
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

        public void WriteStruct<T>(T value, ushort ordinal, BlockFlags flags = BlockFlags.None) where T : IBinaryBlock, new()
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Struct, Flags = flags }).Value);
            if (value == null)
            {
                _writer.Write(BinaryBlock.NullExists);
            }
            else
            {
                // write in-place if the stream supports seek
                if (_stream.CanSeek)
                {
                    // create a new stream wrapper
                    StreamSegment stream = new StreamSegment(_stream, 0);
                    // write a placeholder for the length
                    _writer.Write((int)0);
                    // serialize the struct into the stream
                    (value as IBinaryBlock).Serialize(stream);
                    // move the write cursor back to the placeholder
                    int length = (int)stream.Position;
                    _stream.Position -= (length);
                    // write the size of the struct
                    _writer.Write(length);
                    // move the write cursor back to the correct position
                    _stream.Position += length;
                }
                else
                {
                    byte[] buffer = new byte[BufferSize];
                    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                    {
                        // serialize the struct into the stream
                        (value as IBinaryBlock).Serialize(memory);
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

        public void WriteStructList<T>(System.Collections.Generic.List<T> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.StructList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    _writer.Write(BinaryBlock.NullExists);
                }
                else
                {
                    // write in-place if the stream supports seek
                    if (_stream.CanSeek)
                    {
                        // write a placeholder for the length
                        _writer.Write((int)0);
                        // create a new stream wrapper
                        StreamSegment stream = new StreamSegment(_stream, 0);
                        // serialize the struct into the stream
                        (values[i] as IBinaryBlock).Serialize(stream);
                        // move the write cursor back to the placeholder
                        int length = (int)stream.Position;
                        _stream.Position -= (length + sizeof(int));
                        // write the size of the struct
                        _writer.Write(length);
                        // move the write cursor back to the correct position
                        _stream.Position += length;
                    }
                    else
                    {
                        byte[] buffer = new byte[BufferSize];
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
        }

        public void WriteTimespan(System.TimeSpan value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Timespan, Flags = flags }).Value);
            _writer.Write(value.Ticks);
        }

        public void WriteTimespanList(System.Collections.Generic.List<System.TimeSpan> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.TimespanList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i].Ticks);
            }
        }

        public void WriteTimestamp(System.Timestamp value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Timestamp, Flags = flags }).Value);
            _writer.Write(value.TotalMilliseconds);
        }

        public void WriteTimestampList(System.Collections.Generic.List<System.Timestamp> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.TimestampList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i].TotalMilliseconds);
            }
        }

        public void WriteUint(uint value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Uint, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteUintList(System.Collections.Generic.List<uint> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.UintList, Flags = flags }).Value);
            _writer.Write(values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                _writer.Write(values[i]);
            }
        }

        public void WriteUlong(ulong value, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.Ulong, Flags = flags }).Value);
            _writer.Write(value);
        }

        public void WriteUlongList(System.Collections.Generic.List<ulong> values, ushort ordinal, BlockFlags flags = BlockFlags.None)
        {
            _writer.Write((new BinaryBlock() { Ordinal = ordinal, Type = BlockType.UlongList, Flags = flags }).Value);
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
            _stream.Flush();
        }
        #endregion
    }

    internal class StreamSegment : System.IO.Stream
    {
        #region Constructors
        public StreamSegment(System.IO.Stream stream, int length)
        {
            _base = stream;
            _length = length;
            _start = stream.Position;
        }
        #endregion
        #region Members
        public override bool CanRead { get { return _base.CanRead; } }
        public override bool CanSeek { get { return _base.CanSeek; } }
        public override bool CanTimeout { get { return _base.CanTimeout; } }
        public override bool CanWrite { get { return _base.CanWrite; } }
        public override long Length { get { return System.Math.Max(this.Position, _length); } }
        public override long Position
        {
            get { return _base.Position - _start; }
            set { _base.Position = value - _start; }
        }

        private System.IO.Stream _base;
        private int _length;
        private long _start;
        #endregion
        #region Methods
        public override void Flush()
        {
            if (_base.CanWrite && _base.CanSeek)
            {
                _base.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.Position + offset > _length)
                return 0;
            if (this.Position + offset + count > _length)
            {
                count = (int)this.Position + offset + count - _length;
            }

            int read = _base.Read(buffer, offset, count);
            return read;
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            long root = 0;

            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    root = _start;
                    break;
                case System.IO.SeekOrigin.Current:
                    root = this.Position;
                    break;
                case System.IO.SeekOrigin.End:
                    root = _length;
                    break;
            }

            long position = root + offset - _start;

            _base.Seek(root + offset, origin);

            return position;
        }

        public override void SetLength(long value)
        {
            throw new System.NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _base.Write(buffer, offset, count);
        }
        #endregion
    }

    internal class EnumerableStructStream<T> : System.IDisposable, System.Collections.Generic.IEnumerable<T>
        where T : IBinaryBlock, new()
    {
        #region Constructors
        public EnumerableStructStream(System.IO.Stream baseStream, bool blocking = true)
        {
            if (baseStream == null)
                throw new System.ArgumentNullException("baseStream");
            if (!baseStream.CanRead)
                throw new System.IO.InvalidDataException("baseStream must be readable");
            if (!baseStream.CanWrite)
                throw new System.IO.InvalidDataException("baseStream must be writable");

            _baseStream = baseStream;
            _writer = new BinaryBlockWriter(baseStream);
            _blocking = blocking;
        }
        #endregion
        #region Members
        private System.IO.Stream _baseStream;
        private BinaryBlockWriter _writer;
        private bool _blocking;

        public bool IsBlocked { get; private set; }
        #endregion
        #region Methods
        public void Add(T value)
        {
            lock (_baseStream)
            {
                // record the position of the base-stream
                long position = _baseStream.Position;
                // seek to the end of the base-stream
                _baseStream.Seek(0, System.IO.SeekOrigin.End);
                // write the struct to the stream
                _writer.WriteStruct<T>(value, 0, BlockFlags.None);
                // seek back to the original position
                _baseStream.Seek(position, System.IO.SeekOrigin.Begin);
                // notify all threads that the stream has been updated
                System.Threading.Monitor.PulseAll(_baseStream);
            }
        }
        #endregion
        #region System.Collections.Generic.IEnumerator<T>
        /// <summary>
        /// Yields a series of values of type T, if the end of stream is reached the enumerator blocks waiting for more content.
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            long position = _baseStream.Position;
            using (BinaryBlockReader reader = new BinaryBlockReader(_baseStream))
            {
                lock (_baseStream)
                {
                    // so long as their is nothing to read (position == length)...
                    while (_baseStream.Position == _baseStream.Length)
                    {
                        // if the enumerator is blocking, wait
                        if (_blocking)
                        {
                            this.IsBlocked = true;
                            System.Threading.Monitor.Wait(_baseStream);
                        }
                        else
                        {
                            yield break;
                        }
                    }
                    this.IsBlocked = false;
                    // seek to last known good read position
                    _baseStream.Seek(position, System.IO.SeekOrigin.Begin);
                    // read the binary block and verify it
                    BinaryBlock block = reader.ReadBinaryBlock();
                    if (block.Ordinal != 0 || block.Type != BlockType.Struct)
                        throw new System.IO.InvalidDataException();
                    // read the struct from the stream
                    T value = reader.ReadStruct<T>();
                    // record a new last known good position
                    position = _baseStream.Position;
                    // yield the value
                    yield return value;
                }
            }
            yield break;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var value in this)
            {
                yield return value;
            }
            yield break;
        }
        #endregion
        #region System.IDisposable
        void System.IDisposable.Dispose()
        {
            _baseStream.Flush();
        }
        #endregion
    }

    internal interface IBinaryBlock
    {
        void Serialize(System.IO.Stream input);
        void Deserialize(System.IO.Stream output);
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
    internal struct Timestamp : System.IComparable, System.IComparable<Timestamp>, System.IEquatable<Timestamp>, System.IEquatable<DateTime>
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
        public const int MonthAffectByLeapYear = 1;
        public const int TicksPerMillisecond = 10000;
        public const long MillisecondsPerSecond = 1000;
        public const long MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;
        public const long MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;
        public const long MillisecondsPerDay = MillisecondsPerHour * HoursPerDay;
        public const long MillisecondsPerYear = MillisecondsPerDay * DaysPerYear;
        public static readonly int[] DaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public static int MonthsPerYear { get { return Timestamp.DaysPerMonth.Length; } }

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
            BCE = -1,
            /// <summary>
            /// Common Era. All dates after and including 1 Jan 0001
            /// </summary>
            CE = 0
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
                throw new System.ArgumentOutOfRangeException("milliseconds", System.String.Format("The milliseconds parameter is restricted to [{0}, {1}]", Timestamp.MinValue.TotalMilliseconds, Timestamp.MaxValue.TotalMilliseconds));
            #endregion
            // record the actuall milliseconds passed in as the base value
            this.TotalMilliseconds = milliseconds;
            // get the absolute number of milliseconds because the math works this way
            milliseconds = System.Math.Abs(milliseconds);
            // local variables
            int year = 0;
            int month = 0;
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
            const int DaysPer100Years = 36524;
            while (day > DaysPer100Years)
            {
                year += 100;
                day -= DaysPer100Years;
            }
            while (day >= DaysPerYear)
            {
                year++;
                day -= DaysPerYear;

                if (IsLeapYear(year + 1))
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
                if (i == MonthAffectByLeapYear && IsLeapYear(year + 1))
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
            System.Diagnostics.Debug.Assert(day >= 0 && day < DaysPerMonth[month] + (IsLeapYear(year) ? 1 : 0), "Invalid value for System.Timestamp.Day");
            System.Diagnostics.Debug.Assert(hour >= 0 && hour < 24, "Invalid value for System.Timestamp.Hour");
            System.Diagnostics.Debug.Assert(minute >= 0 && month < 60, "Invalid value for System.Timestamp.Minute");
            System.Diagnostics.Debug.Assert(second >= 0 && second < 60, "Invalid value for System.Timestamp.Second");
            System.Diagnostics.Debug.Assert(millisecond >= 0 && millisecond < 1000, "Invalid value for System.Timestamp.Millisecond");
            // assign all public values
            _year = year;
            _month = month;
            _day = day;
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
            int absyear = System.Math.Abs(year) - 1;
            month -= 1;
            day -= 1;
            long milliseconds = absyear * MillisecondsPerYear;
            milliseconds += (absyear / YearsPerLeapYear) * MillisecondsPerDay;
            milliseconds -= (absyear / YearsPer1CLeapYear) * MillisecondsPerDay;
            milliseconds += (absyear / YearsPer4CLeapYear) * MillisecondsPerDay;
            for (int i = 0; i < month - 1; i++)
            {
                milliseconds += DaysPerMonth[i] * MillisecondsPerDay;
                // if we're in March, add an extra day if it is a leap year
                if (i == MonthAffectByLeapYear && IsLeapYear(year))
                {
                    milliseconds += MillisecondsPerDay;
                }
            }
            milliseconds += day * MillisecondsPerDay;
            milliseconds += hour * MillisecondsPerHour;
            milliseconds += minute * MillisecondsPerMinute;
            milliseconds += second * MillisecondsPerSecond;

            _year = absyear;
            _month = month;
            _day = day;
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
            : this(datetime.Ticks / TicksPerMillisecond * (era == TimestampEra.CE ? 1 : -1))
        { }

        public Timestamp(System.Timestamp timestamp)
            : this(timestamp.Ticks / TicksPerMillisecond)
        { }
        #endregion
        #region Members
        /// <summary>
        /// Gets a System.DateTime with the same date as this instance, and the time value set to 12:00:00 midnight (00:00:00).
        /// </summary>
        public DateTime Date { get { return new System.DateTime(this.Year, this.Month, this.Day, 0, 0, 0, DateTimeKind.Utc); } }
        /// <summary>
        /// Gets the day of the month represented by this instance, expressed as value between 1 and the length of the month.
        /// </summary>
        public int Day { get { return _day + 1; } }
        public readonly int _day;
        /// <summary>
        /// Gets the hour component of the date represented by this instance, expressed as a value between 0 and 23.
        /// </summary>
        public readonly int Hour;
        /// <summary>
        /// Gets the millisecond component of the date represented by this instance, expressed as a value between 0 and 999.
        /// </summary>
        public readonly int Millisecond;
        /// <summary>
        /// Gets the minute component of the date represented by this instance, expressed as a value between 0 and 59.
        /// </summary>
        public readonly int Minute;
        /// <summary>
        /// Gets the month component of the date represented by this instance, expressed as a value between 1 and 12.
        /// </summary>
        public int Month { get { return _month + 1; } }
        private readonly int _month;
        /// <summary>
        /// Gets the seconds component of the date represented by this instance, expressed as a value between 0 and 59.
        /// </summary>
        public readonly int Second;
        /// <summary>
        /// Gets the year component of the date represented by this instance, expressed as a value between -250,000 and 250,000.
        /// </summary>
        public int Year { get { return _year + 1; } }
        private readonly int _year;
        /// <summary>
        /// Gets the era component of the date represented by this instance.
        /// </summary>
        public readonly TimestampEra Era;
        /// <summary>
        /// Gets the number of ticks that represent the date and time of this instance either before or after 0-1-1 00:00:00Z.
        /// </summary>
        public long Ticks { get { return this.TotalMilliseconds * TicksPerMillisecond; } }
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
            return this.AddMilliseconds((long)value.TotalMilliseconds);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">A number of days. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of days represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddDays(int value)
        {
            return this.AddMilliseconds(((long)value) * MillisecondsPerDay);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="value">A number of hours. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of hours represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddHours(int value)
        {
            return this.AddMilliseconds(((long)value) * MillisecondsPerHour);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of milliseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of milliseconds. The value parameter can be negative or positive. Note that this value is rounded to the nearest integer.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of milliseconds represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddMilliseconds(int value)
        {
            return this.AddMilliseconds((long)value);
        }

        private System.Timestamp AddMilliseconds(long value)
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
            return this.AddMilliseconds(((long)value) * MillisecondsPerMinute);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="months">A number of months. The months parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and months.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddMonths(int months)
        {
            long milliseconds = this.TotalMilliseconds;
            for (int i = 0; i < months; i++)
            {
                int month = (_month + i) % MonthsPerYear;
                milliseconds += DaysPerMonth[month] * MillisecondsPerDay;
                int year = (this.Year + ((_month + i) / MonthsPerYear));
                if (month == MonthAffectByLeapYear && IsLeapYear(year))
                {
                    milliseconds += MillisecondsPerDay;
                }
            }
            return new System.Timestamp(milliseconds);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of seconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of seconds. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of seconds represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddSeconds(int value)
        {
            return this.AddMilliseconds(((long)value) * MillisecondsPerSecond);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of ticks to the value of this instance.
        /// </summary>
        /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be positive or negative.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the time represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddTicks(long value)
        {
            long milliseconds = value / TicksPerMillisecond;
            // round up the value to make Timestamp compliant with DateTime
            if (value - milliseconds >= TicksPerMillisecond / 2)
                milliseconds += 1;
            return this.AddMilliseconds(milliseconds);
        }
        /// <summary>
        /// Returns a new System.Timestamp that adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>A System.Timestamp whose value is the sum of the date and time represented by this instance and the number of years represented by value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Value or the resulting System.Timestamp is less than System.Timestamp.MinValue or greater than System.Timestamp.MaxValue.</exception>
        public System.Timestamp AddYears(int value)
        {
            long milliseconds = this.TotalMilliseconds;
            for (int i = 0; i < value; i++)
            {
                int year = this.Year + i;
                milliseconds += MillisecondsPerYear;
                if (IsLeapYear(year))
                {
                    milliseconds += MillisecondsPerDay;
                }
            }
            return new System.Timestamp(milliseconds);
        }

        public bool IsLeapYear(int year)
        {
            return year % YearsPerLeapYear == 0 && (year % YearsPer1CLeapYear != 0 || year % YearsPer4CLeapYear == 0);
        }
        /// <summary>
        /// Converts System.Timestamp to System.DateTime.
        /// </summary>
        /// <returns>A System.DateTime which represents the same date and time as this System.Timespace.</returns>
        [Obsolete("Use explicit cast (DateTime) instead")]
        public System.DateTime ToDateTime()
        {
            return (DateTime)this;
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
            return string.Format("{0:##########00}-{1:00}-{2:00} {3} {4:00}:{5:00}:{6:00}Z", this.Year, this.Month, this.Day, Era, this.Hour, this.Minute, this.Second);
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
        /// <param name="that">A Timestamp value to compare with this value.</param>
        /// <returns>true if the current value is equal to the value parameter; otherwise, false.</returns>
        bool System.IEquatable<System.Timestamp>.Equals(System.Timestamp that)
        {
            // Timestamp is a struct, no need to check null first
            return this == that;
        }
        #endregion
        #region System.IEquatable<DateTime>
        /// <summary>
        /// Indicates whether the current value is equal to another value of a DateTime.
        /// </summary>
        /// <param name="that">A DateTime value to compare with this value.</param>
        /// <returns>true if the current value is equal to the value parameter; otherwise, false.</returns>
        bool System.IEquatable<System.DateTime>.Equals(System.DateTime that)
        {
            // DateTime is a struct, no need to check null first
            return this == that;
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
            long ticks = (a.TotalMilliseconds - b.TotalMilliseconds) * TicksPerMillisecond;
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
            return new System.Timestamp(d.TotalMilliseconds - (t.Ticks / TicksPerMillisecond));
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
            return new System.Timestamp(d.TotalMilliseconds + (t.Ticks / TicksPerMillisecond));
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
        /// Determines whether an instance of System.Timestamp is equivolent to an instance of System.DateTime (millisecond granularity)
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a and b represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(System.Timestamp a, System.DateTime b)
        {
            long milliseconds = b.Ticks / TicksPerMillisecond;
            return a.TotalMilliseconds == milliseconds;
        }
        /// <summary>
        /// Determines whether an instance of System.Timestamp is equivolent to an instance of System.DateTime (millisecond granularity)
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a and b represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(System.DateTime a, System.Timestamp b)
        {
            long milliseconds = a.Ticks / TicksPerMillisecond;
            // round up the value to make Timestamp compliant with DateTime
            if (b.TotalMilliseconds - milliseconds >= TicksPerMillisecond / 2)
                milliseconds += 1;
            return b.TotalMilliseconds == milliseconds;
        }
        /// <summary>
        /// Determines whether an instance of System.Timestamp is equivolent to an instance of System.DateTime (millisecond granularity)
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a and b do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(System.Timestamp a, System.DateTime b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Determines whether an instance of System.Timestamp is equivolent to an instance of System.DateTime (millisecond granularity)
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>true if a and b do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(System.DateTime a, System.Timestamp b)
        {
            return !(a == b);
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

        public static explicit operator System.DateTime(System.Timestamp a)
        {
            return new System.DateTime(a.TotalMilliseconds * TicksPerMillisecond, System.DateTimeKind.Utc);
        }

        public static explicit operator System.Timestamp(System.DateTime a)
        {
            return new System.Timestamp(a.ToUniversalTime());
        }
        #endregion
    }
}
