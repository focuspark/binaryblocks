using System;
using System.Collections.Generic;

namespace Isris.BinaryBlocks.CsharpGenerator.Block
{
    internal abstract class Base
    {
        public Base(string name, string source, int index)
        {
            this.Index = index;
            this.Name = name;
            this.Parent = null;
            this.Source = source;
        }

        public string FullName
        {
            get
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                Block.Base node = this;
                while (node != null)
                {
                    buffer.Insert(0, ".")
                          .Insert(0, node.Name);
                    node = node.Parent;
                }
                buffer.Remove(buffer.Length - 1, 1);
                return buffer.ToString();
            }
        }
        public int Index { get; private set; }
        public string Name { get; internal set; }
        public Block.Base Parent { get; internal set; }
        public string Source { get; private set; }
    }
}
