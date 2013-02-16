using System;
using System.Collections.Generic;

namespace Isris.BinaryBlocks.CsharpGenerator.Block
{
    [System.Diagnostics.DebuggerDisplay("struct {Name}")]
    internal class Struct : Block.Namespace
    {
        public Struct(string name, string source, int index)
            : base(name, source, index)
        {
            this.Members = new Dictionary<byte, Block.Member>();
        }

        public Dictionary<byte, Block.Member> Members { get; private set; }
    }
}
