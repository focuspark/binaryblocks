namespace Sample
{
    internal partial class Complex : BinaryBlocks.IBinaryBlock
    {
        public Complex()
        {
            this.Leaves = new System.Collections.Generic.List<Basic>();
            this.Branches = new System.Collections.Generic.List<Complex>();
        }
        /* Private */
        private const ushort _Leaves_ordinal = 1;
        private const ushort _Branches_ordinal = 4;
        /* Public */
        public System.Collections.Generic.List<Basic> Leaves { get; private set; }
        public System.Collections.Generic.List<Complex> Branches { get; private set; }

        public void Deserialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException();
            if (!stream.CanRead || !stream.CanSeek)
                throw new System.InvalidOperationException();

            using (BinaryBlocks.BinaryBlockReader reader = new BinaryBlocks.BinaryBlockReader(stream))
            {
                while (reader.Position < reader.Length)
                {
                    BinaryBlocks.BinaryBlock block = reader.ReadBinaryBlock();

                    switch (block.Ordinal)
                    {
                        case _Leaves_ordinal:
                            {
                                this.Leaves = reader.ReadStructList<Basic>();
                            } break;
                        case _Branches_ordinal:
                            {
                                this.Branches = reader.ReadStructList<Complex>();
                            } break;
                        default:
                            reader.SkipBlock(block);
                            break;
                    }
                }
            }
        }

        public void Serialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException();
            if (!stream.CanWrite)
                throw new System.InvalidOperationException();

            using (BinaryBlocks.BinaryBlockWriter writer = new BinaryBlocks.BinaryBlockWriter(stream))
            {
                writer.WriteStructList<Basic>(this.Leaves, _Leaves_ordinal, BinaryBlocks.BlockFlags.None);
                writer.WriteStructList<Complex>(this.Branches, _Branches_ordinal, BinaryBlocks.BlockFlags.None);
            }
        }
    }
}
