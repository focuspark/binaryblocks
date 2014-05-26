using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BinaryBlocks.CsharpGenerator;

namespace BinaryBlocks.Test.CsharpGenerator
{
    [TestClass]
    public class CodeWriterTest
    {
        [TestMethod]
        public void BeginBlock()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer = new CodeWriter();
                writer.BeginBlock();
                writer.BeginBlock();
                writer.BeginBlock();
                string result = writer.ToString();

                Assert.AreEqual(result, "{\r\n    {\r\n        {\r\n");
            }
        }

        [TestMethod]
        public void EndBlock()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer = new CodeWriter();
                writer.BeginBlock();
                writer.BeginBlock();
                writer.BeginBlock();
                writer.EndBlock();
                writer.EndBlock();
                writer.EndBlock();
                string result = writer.ToString();

                Assert.AreEqual(result, "{\r\n    {\r\n        {\r\n        }\r\n    }\r\n}\r\n");
            }
        }

        [TestMethod]
        public void IndentLess()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer = new CodeWriter(5);
                writer.Write("foo");
                writer.IndentLess();
                writer.Write("bar");
                string result = writer.ToString();

                Assert.AreEqual(result, "                    foo\r\n                bar\r\n");
            }
        }

        [TestMethod]
        public void IndentMore()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer = new CodeWriter(4);
                writer.Write("foo");
                writer.IndentMore();
                writer.Write("bar");
                string result = writer.ToString();

                Assert.AreEqual(result, "                foo\r\n                    bar\r\n");
            }
        }

        [TestMethod]
        public void Merge()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer1 = new CodeWriter();
                writer1.Write("class Foo");
                writer1.BeginBlock();
                writer1.Write("// body");
                writer1.EndBlock();

                CodeWriter writer2 = new CodeWriter();
                writer2.Write("class Bar");
                writer2.BeginBlock();
                writer2.Write("// body");
                writer2.EndBlock();

                writer1.Merge(writer2);
                string result = writer1.ToString();

                Assert.AreEqual(result, "class Foo\r\n{\r\n    // body\r\n}\r\nclass Bar\r\n{\r\n    // body\r\n}\r\n");
            }
        }

        [TestMethod]
        public void Write()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer = new CodeWriter();
                writer.Write("foo bar bat baz");
                string result = writer.ToString();

                Assert.AreEqual(result, "foo bar bat baz\r\n");

                writer = new CodeWriter();
                writer.Write("{0} {1} {2} {3}", "foo", "bar", "bat", "baz");
                result = writer.ToString();

                Assert.AreEqual(result, "foo bar bat baz\r\n");
            }
        }

        [TestMethod]
        public void WriteIndented()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                CodeWriter writer = new CodeWriter(1);
                writer.Write("foo");
                writer.WriteIndented("bar");
                writer.Write("bat");
                string result = writer.ToString();

                Assert.AreEqual(result, "    foo\r\n        bar\r\n    bat\r\n");

                writer = new CodeWriter(1);
                writer.Write("foo");
                writer.WriteIndented("bar{0}", "_bat");
                writer.Write("baz");
                result = writer.ToString();

                Assert.AreEqual(result, "    foo\r\n        bar_bat\r\n    baz\r\n");
            }
        }
    }
}
