using System;
using System.Collections.Generic;

namespace Isris.BinaryBlocks.CsharpGenerator.Block
{
    [System.Diagnostics.DebuggerDisplay("{Type} {Name}")]
    internal class Member : Block.Base
    {
        public Member(string name, string type, bool isList, string source, int index)
            : base(name, source, index)
        {
            this.TypeName = type;

            switch (this.TypeName.ToLower())
            {
                case "blob": this.Type = BlockType.Blob; break;
                case "byte": this.Type = BlockType.Byte; break;
                case "char": this.Type = BlockType.Char; break;
                case "datetime": this.Type = BlockType.Datetime; break;
                case "double": this.Type = BlockType.Double; break;
                case "guid": this.Type = BlockType.Guid; break;
                case "single": this.Type = BlockType.Single; break;
                case "sint": this.Type = BlockType.Sint; break;
                case "slong": this.Type = BlockType.Slong; break;
                case "string": this.Type = BlockType.String; break;
                case "timespan": this.Type = BlockType.Timespan; break;
                case "uint": this.Type = BlockType.Uint; break;
                case "ulong": this.Type = BlockType.Ulong; break;
                default: this.Type = BlockType.Unknown; break;
            }

            if (isList)
            {
                this.Type |= BlockType.List;
            }
        }

        public bool IsList
        {
            get { return ((this.Type & BlockType.List) == BlockType.List); }
        }
        public BlockType Type { get; set; }
        public string TypeName { get; set; }
        public byte Ordinal { get; set; }
    }
}
