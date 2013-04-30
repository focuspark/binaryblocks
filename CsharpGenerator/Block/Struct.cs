using System;
using System.Collections.Generic;

namespace BinaryBlocks.CsharpGenerator.Block
{
    [System.Diagnostics.DebuggerDisplay("struct {Name}")]
    internal class Struct : Block.Namespace
    {
        public Struct(string name, string source, int index)
            : base(name, source, index)
        {
            this.Members = new Dictionary<int, Block.Member>();
        }

        public Dictionary<int, Block.Member> Members { get; private set; }
    }
}
