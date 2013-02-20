using System;
using System.Collections.Generic;

namespace BinaryBlocks.CsharpGenerator.Block
{
    [System.Diagnostics.DebuggerDisplay("enum {Name}")]
    internal class Enum : Block.Base
    {
        public Enum(string name, string source, int index)
            :base(name, source, index)
        {
            this.Members = new Dictionary<uint, string>();
        }

        public Dictionary<uint, string> Members { get; private set; }
    }
}
