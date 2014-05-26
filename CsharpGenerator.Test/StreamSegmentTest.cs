using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BinaryBlocks.CsharpGenerator;

namespace BinaryBlocks.Test.CsharpGenerator
{
    [TestClass]
    public class StreamSegmentTest
    {
        private const int buffer_length = 1024;
        private const int segment_start = 512;
        private const int segment_size = 32;
        private static readonly Random rand = new Random();

        [TestMethod]
        public void Read()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    byte[] buffer = new byte[buffer_length];
                    rand.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Seek(segment_start, SeekOrigin.Begin);

                    StreamSegment segment = new StreamSegment(stream, segment_size);
                    byte[] bytes = new byte[segment_size];
                    int read = segment.Read(bytes, 0, segment_size);
                    Assert.IsTrue(read == segment_size);
                    for (int i = 0; i < read; i++)
                    {
                        Assert.IsTrue(buffer[segment_start + i] == bytes[i]);
                    }
                }
            }
        }

        [TestMethod]
        public void Seek()
        {
            const int MaxTimeMs = 50000;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    byte[] buffer = new byte[buffer_length];
                    rand.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Seek(segment_start, SeekOrigin.Begin);

                    StreamSegment segment = new StreamSegment(stream, segment_size);
                    segment.Seek(16, SeekOrigin.Begin);
                    Assert.IsTrue(stream.Position == segment_start + 16);
                }
            }
        }

        [TestMethod]
        public void SetLength()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    byte[] buffer = new byte[buffer_length];
                    rand.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Seek(segment_start, SeekOrigin.Begin);

                    StreamSegment segment = new StreamSegment(stream, segment_size);
                    try
                    {
                        segment.SetLength(1024);
                        Assert.Fail();
                    }
                    catch { /* this exception is a good thing */ }
                }
            }
        }

        [TestMethod]
        public void Write()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    byte[] buffer = new byte[buffer_length];
                    rand.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Seek(segment_start, SeekOrigin.Begin);

                    StreamSegment segment = new StreamSegment(stream, segment_size);
                    byte[] bytes = new byte[16];
                    rand.NextBytes(bytes);
                    segment.Write(bytes, 0, bytes.Length);

                    Assert.IsTrue(segment.Length == segment_size);

                    byte[] buffer2 = stream.ToArray();
                    for (int i = 0; i < segment_start && i < buffer.Length && i < buffer2.Length; i++)
                    {
                        Assert.IsTrue(buffer[i] == buffer2[i]);
                    }

                    for (int i = 0; i < bytes.Length && i + segment_start < buffer2.Length; i++)
                    {
                        Assert.IsTrue(buffer2[i + segment_start] == bytes[i]);
                    }

                    for (int i = segment_start + buffer2.Length; i < segment.Length && i < buffer.Length && i < buffer2.Length; i++)
                    {
                        Assert.IsTrue(buffer[i] == buffer2[i]);
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    byte[] buffer = new byte[buffer_length];
                    rand.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Seek(segment_start, SeekOrigin.Begin);

                    StreamSegment segment = new StreamSegment(stream, segment_size);
                    byte[] bytes = new byte[buffer_length];
                    rand.NextBytes(bytes);
                    segment.Write(bytes, 0, bytes.Length);

                    Assert.IsTrue(stream.Length == buffer_length + buffer_length - segment_start);
                    Assert.IsTrue(stream.Position == stream.Length);
                    Assert.IsTrue(segment.Length == bytes.Length);

                    byte[] buffer2 = stream.ToArray();
                    for (int i = 0; i < segment_start && i < buffer.Length && i < buffer2.Length; i++)
                    {
                        Assert.IsTrue(buffer[i] == buffer2[i]);
                    }

                    for (int i = 0; i < bytes.Length && i + segment_start < buffer2.Length; i++)
                    {
                        Assert.IsTrue(buffer2[i + segment_start] == bytes[i]);
                    }

                    for (int i = segment_start + buffer2.Length; i < segment.Length && i < buffer.Length && i < buffer2.Length; i++)
                    {
                        Assert.IsTrue(buffer[i] == buffer2[i]);
                    }
                }
            }
        }
    }
}
