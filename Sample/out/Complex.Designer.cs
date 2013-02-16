namespace Sample
{
    internal partial class Complex : BinaryBlocks.IBinaryBlock
    {
        public Complex()
        {
            this.Children = new System.Collections.Generic.List<Complex>();
        }
        /* Private */
        private const byte _Children_ordinal = 4;
        /* Public */
        public System.Collections.Generic.List<Complex> Children { get; private set; }

        public unsafe override void Deserialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException();
            if (!stream.CanRead || !stream.CanSeek)
                throw new System.InvalidOperationException();

            BinaryBlocks.BinaryBlockReader reader = new BinaryBlocks.BinaryBlockReader(stream);

            while (reader.Position < reader.Length)
            {
                BinaryBlocks.BinaryBlock block = reader.ReadBinaryBlock();

                switch (block.Ordinal)
                {
                    case _Children_ordinal:
                        {
                            this.Children = reader.ReadStructList<Complex>();
                        } break;
                    default:
                        reader.SkipBlock(block);
                        break;
                }
            }
        }

        public unsafe override void Serialize(System.IO.Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException();
            if (!stream.CanWrite)
                throw new System.InvalidOperationException();

            BinaryBlocks.BinaryBlockWriter writer = new BinaryBlocks.BinaryBlockWriter(stream);

            writer.WriteStructList<Complex>(this.Children, _Children_ordinal);
        }
    }
}
