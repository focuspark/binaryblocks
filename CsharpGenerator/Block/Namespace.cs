using System;
using System.Collections.Generic;

namespace Isris.BinaryBlocks.CsharpGenerator.Block
{
    [System.Diagnostics.DebuggerDisplay("namespace {Name}")]
    internal class Namespace : Block.Base
    {
        public Namespace(string name, string source, int index)
            : base(name, source, index)
        {
            this.Children = new List<Block.Base>();
        }

        public List<Block.Base> Children { get; private set; }
    }
}
