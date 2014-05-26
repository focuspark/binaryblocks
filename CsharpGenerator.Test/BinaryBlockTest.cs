using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BinaryBlocks.CsharpGenerator;

namespace BinaryBlocks.Test.CsharpGenerator
{
    [TestClass]
    public class BinaryBlockTest
    {
        private static readonly Random rand = new Random();

        [TestMethod]
        public void BinaryBlock()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                BinaryBlock source = new BinaryBlock() { Value = (uint)rand.Next() };

                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(source.Value);

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock result = reader.ReadBinaryBlock();
                        Assert.IsTrue(result.Value == source.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void Blob()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                byte[] source = System.Text.Encoding.UTF8.GetBytes("this is some sample text used to generate bytes to test blobs");

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteBlob(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Blob);

                        byte[] result = reader.ReadBlob();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Length == source.Length);
                        for (int i = 0; i < result.Length && i < source.Length; i++)
                        {
                            Assert.IsTrue(result[i] == source[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void BlobList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<byte[]> source = new List<byte[]>
                {
                    System.Text.Encoding.UTF8.GetBytes("this is some sample text used to generate bytes to test blobs"),
                    System.Text.Encoding.UTF8.GetBytes("this is another string used to generate bytes")
                };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteBlobList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.BlobList);

                        List<byte[]> result = reader.ReadBlobList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(source.Count == result.Count);

                        for (int i = 0; i < result.Count; i++)
                        {
                            Assert.IsFalse(source[i] == result[i]);
                            Assert.IsTrue(source[i].Length == result[i].Length);
                            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(source[i], 0, source[i].Length),
                                            System.Text.Encoding.UTF8.GetString(result[i], 0, result[i].Length));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Byte()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                byte source = 127;

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteByte(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Byte);

                        byte result = reader.ReadByte();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void ByteList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<byte> source = new List<byte> { 1, 2, 3, 4 };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteByteList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.ByteList);

                        List<byte> result = reader.ReadByteList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Char()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                char source = 'x';

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteChar(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Char);

                        char result = reader.ReadChar();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void CharList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<char> source = new List<char> { 't', 'e', 's', 't' };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteCharList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.CharList);

                        List<char> result = reader.ReadCharList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void DoubleTest()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                double source = rand.NextDouble();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteDouble(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Double);

                        double result = reader.ReadDouble();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void DoubleList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<double> source = new List<double> { rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), rand.NextDouble() };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteDoubleList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.DoubleList);

                        List<double> result = reader.ReadDoubleList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Guid()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Guid source = System.Guid.NewGuid();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteGuid(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Guid);

                        Guid result = reader.ReadGuid();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void GuidList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<Guid> source = new List<Guid> { System.Guid.NewGuid(), System.Guid.NewGuid(), System.Guid.NewGuid(), System.Guid.NewGuid(), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteGuidList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.GuidList);

                        List<Guid> result = reader.ReadGuidList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Single()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                float source = (float)rand.NextDouble();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteSingle(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Single);

                        float result = reader.ReadSingle();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void SingleList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<float> source = new List<float> { (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble() };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteSingleList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.SingleList);

                        List<float> result = reader.ReadSingleList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Sint()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                int source = rand.Next();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteSint(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Sint);

                        int result = reader.ReadSint();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void SintList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<int> source = new List<int> { rand.Next(), rand.Next(), rand.Next(), rand.Next(), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteSintList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.SintList);

                        List<int> result = reader.ReadSintList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Slong()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                long source = rand.Next();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteSlong(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Slong);

                        long result = reader.ReadSlong();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void SlongList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<long> source = new List<long> { rand.Next(), rand.Next(), rand.Next(), rand.Next(), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteSlongList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.SlongList);

                        List<long> result = reader.ReadSlongList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void String()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                string source = "this is a test string";

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteString(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.String);

                        string result = reader.ReadString();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void StringList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<string> source = new List<string> { "tests 1 string", "test two string", "third test string", "fourth string for test" };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteStringList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.StringList);

                        List<string> result = reader.ReadStringList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        public class TestStruct : IBinaryBlock
        {
            private const int buffer_length = 16;

            public TestStruct()
            {
                Value = rand.Next();
                Values = new byte[buffer_length];
                rand.NextBytes(Values);
            }

            public int Value { get; private set; }
            public byte[] Values { get; private set; }

            public void Serialize(System.IO.Stream input)
            {
                BinaryWriter writer = new BinaryWriter(input);
                writer.Write(Value);
                writer.Write(Values);
            }
            public void Deserialize(System.IO.Stream output)
            {
                BinaryReader reader = new BinaryReader(output);
                Value = reader.ReadInt32();
                Values = reader.ReadBytes(buffer_length);
            }
        }

        [TestMethod]
        public void Struct()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                TestStruct source = new TestStruct();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteStruct<TestStruct>(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1, "block.Ordinal != 1");
                        Assert.IsTrue(block.Type == BlockType.Struct, "block.Type != Struct");

                        TestStruct result = reader.ReadStruct<TestStruct>();
                        Assert.IsFalse(source == result, "source != result");
                        Assert.IsTrue(source.Value == result.Value, "source.Value != result.Value");
                        Assert.IsTrue(source.Values.Length == result.Values.Length, "source.Values.Length != result.Values.Length");
                        for (int i = 0; i < source.Values.Length && i < result.Values.Length; i++)
                        {
                            Assert.IsTrue(source.Values[i] == result.Values[i], System.String.Format("source.Values[{0}] != result.Values[{0}]", i));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void StructList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<TestStruct> source = new List<TestStruct> { new TestStruct(), new TestStruct(), new TestStruct(), new TestStruct(), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteStructList<TestStruct>(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.StructList);

                        List<TestStruct> result = reader.ReadStructList<TestStruct>();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsFalse(source[i] == result[i]);
                            Assert.IsTrue(source[i].Value == result[i].Value);
                            Assert.IsTrue(source[i].Values.Length == result[i].Values.Length);
                            for (int j = 0; j < source[i].Values.Length && j < result[i].Values.Length; j++)
                            {
                                Assert.IsTrue(source[i].Values[i] == result[i].Values[i]);
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Timespan()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                TimeSpan source = TimeSpan.FromTicks(rand.Next()); ;

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteTimespan(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Timespan);

                        TimeSpan result = reader.ReadTimespan();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void TimespanList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<TimeSpan> source = new List<TimeSpan> { TimeSpan.FromTicks(rand.Next()), TimeSpan.FromTicks(rand.Next()), TimeSpan.FromTicks(rand.Next()), TimeSpan.FromTicks(rand.Next()), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteTimespanList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.TimespanList);

                        List<TimeSpan> result = reader.ReadTimespanList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Timestamp()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                Timestamp source = new Timestamp((long)rand.Next());

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteTimestamp(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Timestamp);

                        Timestamp result = reader.ReadTimestamp();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void TimestampList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<Timestamp> source = new List<Timestamp> { new Timestamp((long)rand.Next()), new Timestamp((long)rand.Next()), new Timestamp((long)rand.Next()), new Timestamp((long)rand.Next()), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteTimestampList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.TimestampList);

                        List<Timestamp> result = reader.ReadTimestampList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Uint()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                uint source = (uint)rand.Next();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteUint(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Uint);

                        uint result = reader.ReadUint();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void UintList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<uint> source = new List<uint> { (uint)rand.Next(), (uint)rand.Next(), (uint)rand.Next(), (uint)rand.Next(), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteUintList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.UintList);

                        List<uint> result = reader.ReadUintList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Ulong()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                ulong source = (ulong)rand.Next();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteUlong(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Ulong);

                        ulong result = reader.ReadUlong();
                        Assert.IsTrue(source == result);
                    }
                }
            }
        }

        [TestMethod]
        public void UlongList()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                List<ulong> source = new List<ulong> { (ulong)rand.Next(), (ulong)rand.Next(), (ulong)rand.Next(), (ulong)rand.Next(), };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteUlongList(source, 1);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.UlongList);

                        List<ulong> result = reader.ReadUlongList();
                        Assert.IsFalse(source == result);
                        Assert.IsTrue(result.Count == source.Count);
                        for (int i = 0; i < result.Count && i < source.Count; i++)
                        {
                            Assert.IsTrue(source[i] == result[i]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SkipBlock()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                string source = "this is a test string";

                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryBlockWriter writer = new BinaryBlockWriter(stream))
                    {
                        writer.WriteGuid(System.Guid.NewGuid(), 1);
                        writer.WriteStruct<TestStruct>(new TestStruct(), 2);
                        writer.WriteString(source, 3);
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    using (BinaryBlockReader reader = new BinaryBlockReader(stream))
                    {
                        BinaryBlock block = reader.ReadBinaryBlock();
                        Assert.IsTrue(block.Ordinal == 1);
                        Assert.IsTrue(block.Type == BlockType.Guid);

                        reader.SkipBlock(block);

                        BinaryBlock block2 = reader.ReadBinaryBlock();
                        Assert.IsTrue(block2.Ordinal == 2);
                        Assert.IsTrue(block2.Type == BlockType.Struct);

                        reader.SkipBlock(block2);

                        BinaryBlock block3 = reader.ReadBinaryBlock();
                        Assert.IsTrue(block3.Ordinal == 3);
                        Assert.IsTrue(block3.Type == BlockType.String);

                        string result = reader.ReadString();
                        Assert.IsTrue(result == source);
                    }
                }
            }
        }

        [TestMethod]
        public void EnumerableStructStream()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                TestStruct[] values = new TestStruct[20];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = new TestStruct();
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    EnumerableStructStream<TestStruct> reader = new EnumerableStructStream<TestStruct>(stream);
                    EnumerableStructStream<TestStruct> writer = new EnumerableStructStream<TestStruct>(stream);

                    for (int i = 0; i < values.Length; i++)
                    {
                        writer.Add(values[i]);
                    }

                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    int j = 0;
                    foreach (TestStruct t in reader)
                    {
                        Assert.IsNotNull(t);
                        Assert.IsTrue(t.Value == values[j].Value);
                        Assert.IsTrue(t.Values.Length == values[j].Values.Length);
                        for (int k = 0; k < t.Values.Length; k++)
                        {
                            Assert.IsTrue(t.Values[k] == values[j].Values[k]);
                        }
                        j++;
                    }
                }
            }
        }
    }
}
