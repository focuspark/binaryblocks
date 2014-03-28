using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BinaryBlocks.CsharpGenerator;

namespace BinaryBlocks.Test.CsharpGenerator
{
    [TestClass]
    public class TextParserTest
    {
        [TestMethod]
        public void SkipComment()
        {
            int index1 = 0;
            TextParser.SkipComment("/* comment */ not_a_comment", ref index1);
            Assert.IsTrue(index1 == 13);

            int index2 = 0;
            TextParser.SkipComment("// comment\r\nnot_a_comment", ref index2);
            Assert.IsTrue(index2 == 12);

            int index3 = 0;
            TextParser.SkipComment(" // comment\r\nnot_a_comment", ref index3);
            Assert.IsTrue(index3 == 13);

            try
            {
                int index4 = 0;
                TextParser.SkipComment("there's no comment here", ref index4);
                Assert.Fail();
            }
            catch { /* this exception is a good thing */ }
        }

        [TestMethod]
        public void ParseString()
        {
            int index1 = 0;
            string result1 = TextParser.ParseString("\"a string\"", ref index1);
            Assert.AreEqual(result1, "a string");
            Assert.IsTrue(index1 == 9);

            int index2 = 1;
            string result2 = TextParser.ParseString(" \"a string\" ", ref index2);
            Assert.AreEqual(result1, "a string");
            Assert.IsTrue(index2 == 10);

            int index3 = 13;
            string result3 = TextParser.ParseString("not a string \"a string\"", ref index3);
            Assert.AreEqual(result1, "a string");
            Assert.IsTrue(index3 == 22);

            try
            {
                int index4 = 0;
                string result4 = TextParser.ParseString("not a string \"a string\"", ref index4);
                Assert.Fail();
            }
            catch { /* this exception is a good thing */ }
        }

        [TestMethod]
        public void PeekAny()
        {
            // this test should pass because only whitespace preceeds the first match
            int index1 = 0;
            if (!TextParser.PeekAny("  \t\r\n\t  [foo]", index1, '{', '['))
            {
                Assert.Fail();
            }

            // this test should fail because non-whitespace preceeds the first match
            int index2 = 0;
            if (TextParser.PeekAny("bar [foo]", index2, '{', '['))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SeekAny()
        {
            // this test should pass because only whitespace preceeds the first match
            int index1 = 0;
            TextParser.SeekAny("  \t\r\n\t  [foo]", ref index1, '{', '[');
            Assert.IsTrue(index1 == 8);

            // this test should pass because the index skips all non-whitespace preceeding the first match
            int index2 = 3;
            TextParser.SeekAny("bar [foo]", ref index2, '{', '[');
            Assert.IsTrue(index2 == 4);

            // this test should pass because the index skips all non-whitespace preceeding the first match
            int index3 = 3;
            TextParser.SeekAny("bar /* comment */ [foo]", ref index3, '{', '[');
            Assert.IsTrue(index3 == 18);

            // this test should fail because non-whitespace preceeds the first match
            try
            {
                int index4 = 0;
                TextParser.SeekAny("bar [foo]", ref index4, '{', '[');
            }
            catch { /* this exception is a good thing */ }
        }

        [TestMethod]
        public void SeekNext()
        {
            int index1 = 0;
            TextParser.SeekNext("\t  \r\n  next", ref index1);
            Assert.IsTrue(index1 == 7);
        }

        [TestMethod]
        public void ParseBlock()
        {
        }

        [TestMethod]
        public void ParseWord()
        {
        }
    }
}
