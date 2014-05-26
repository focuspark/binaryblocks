using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BinaryBlocks.CsharpGenerator;

namespace BinaryBlocks.Test.CsharpGenerator
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void ParseEnum()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                int index = 0;
                BinaryBlocks.CsharpGenerator.Block.Enum result = Program.ParseEnum(" Foo {\r\n\tBar = 1,\r\n\tBaz = 2,\r\n}", "mstest", ref index);
                Assert.IsTrue(result.Name == "Foo");
                Assert.IsTrue(index == 31);
                Assert.IsTrue(result.Members.Count == 2);
                Assert.IsTrue(result.Members[1] == "Bar");
                Assert.IsTrue(result.Members[2] == "Baz");

                try
                {
                    int index2 = 0;
                    Program.ParseEnum(" Foo {\r\n\tBar = 1\r\n\tBaz = 2\r\n}", "mstest", ref index2);
                    Assert.Fail();
                }
                catch { /* exception expected */ }
            }
        }

        [TestMethod]
        public void ParseNamespace()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                int index = 0;
                BinaryBlocks.CsharpGenerator.Block.Namespace result = Program.ParseNamespace("Foo { /* empty namespace */ }", "mstest", ref index);
                Assert.IsTrue(result.Name == "Foo");
                Assert.IsTrue(index == 29);

                try
                {
                    int index2 = 0;
                    Program.ParseNamespace("2Foo { }", "mstest", ref index2);
                    Assert.Fail();
                }
                catch { /* exception expected here */ }

                try
                {
                    int index2 = 0;
                    Program.ParseNamespace("Foo { ", "mstest", ref index2);
                    Assert.Fail();
                }
                catch { /* exception expected here */ }
            }
        }

        [TestMethod]
        public void ParseStruct()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                int index = 0;
                BinaryBlocks.CsharpGenerator.Block.Struct result = Program.ParseStruct(" Foo {\r\n\ttype string Bar = 1;\r\n\ttype sint Baz = 2;\r\n}", "mstest", ref index);
                Assert.IsTrue(result.Name == "Foo");
                Assert.IsTrue(index == 53);
                Assert.IsTrue(result.Members.Count == 2);
                Assert.IsTrue(result.Members[1].Name == "Bar");
                Assert.IsTrue(result.Members[1].Type == BlockType.String);
                Assert.IsTrue(result.Members[2].Name == "Baz");
                Assert.IsTrue(result.Members[2].Type == BlockType.Sint);

                try
                {
                    int index2 = 0;
                    Program.ParseStruct(" Foo {\r\n\ttype string Bar = 1\r\n\ttype sint Baz = 2\r\n}", "mstest", ref index2);
                    Assert.Fail();
                }
                catch { /* exception expected */ }

                try
                {
                    int index2 = 0;
                    Program.ParseStruct(" Foo {\r\n\ttype string Bar = 1;\r\n\ttype sint Baz = 2;\r\n", "mstest", ref index2);
                    Assert.Fail();
                }
                catch { /* exception expected */ }

                try
                {
                    int index2 = 0;
                    Program.ParseStruct(" 2Foo {\r\n\ttype string Bar = 1;\r\n\ttype sint Baz = 2;\r\n}", "mstest", ref index2);
                    Assert.Fail();
                }
                catch { /* exception expected */ }
            }
        }

        [TestMethod]
        public void ParseMember()
        {
            const int MaxTimeMs = 100;

            using (Timer timer = new Timer((object o) => { Assert.Fail("Timed out"); }, null, MaxTimeMs, -1))
            {
                int index = 0;
                BinaryBlocks.CsharpGenerator.Block.Member result = Program.ParseMember(" ulong Value = 4;", "mstest", ref index, false);
                Assert.IsTrue(result.Name == "Value");
                Assert.IsTrue(result.Ordinal == 4);
                Assert.IsTrue(result.Type ==  BlockType.Ulong);
                Assert.IsTrue(index == 17);

                try
                {
                    int index2 = 0;
                    Program.ParseMember(" vlong Value = 4;", "mstest", ref index2, false);
                    Assert.Fail();
                }
                catch { /* exception expected */ }

                try
                {
                    int index2 = 0;
                    Program.ParseMember(" ulong Value;", "mstest", ref index2, false);
                    Assert.Fail();
                }
                catch { /* exception expected */ }

                try
                {
                    int index2 = 0;
                    Program.ParseMember(" ulong Value = 4", "mstest", ref index2, false);
                    Assert.Fail();
                }
                catch { /* exception expected */ }
            }
        }
    }
}
