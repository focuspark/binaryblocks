using System;
using System.Collections.Generic;

namespace BinaryBlocks.CsharpGenerator.Block
{
    internal class Definition
    {
        public Definition()
        {
            this.Nodes = new Dictionary<string, Block.Base>();
            this.Roots = new List<Block.Base>();
            this.BaseNamespace = null;
        }

        public Dictionary<string, Block.Base> Nodes { get; set; }
        public List<Block.Base> Roots { get; set; }
        public string BaseNamespace { get; set; }
    }
}
