namespace Sample
{
    internal partial class Basic : BinaryBlocks.IBinaryBlock
    {
        public Basic()
        {
            this.Values = new System.Collections.Generic.List<float>();
        }
        /* Private */
        private bool _Text_exists;
        private const ushort _Text_ordinal = 1;
        private string _Text_value;
        private bool _Value_exists;
        private const ushort _Value_ordinal = 2;
        private int _Value_value;
        private const ushort _Values_ordinal = 3;
        /* Public */
        public string Text
        {
            get
            {
                if (_Text_exists)
                    return _Text_value;
                throw new System.InvalidOperationException();
            }
            set
            {
                _Text_value = value;
                _Text_exists = true;
            }
        }
        public bool Text_exists
        {
            get { return _Text_exists; }
            set
            {
                if (value)
                    throw new System.InvalidOperationException();
                _Text_exists = false;
            }
        }
        public int Value
        {
            get
            {
                if (_Value_exists)
                    return _Value_value;
                throw new System.InvalidOperationException();
            }
            set
            {
                _Value_value = value;
                _Value_exists = true;
            }
        }
        public bool Value_exists
        {
            get { return _Value_exists; }
            set
            {
                if (value)
                    throw new System.InvalidOperationException();
                _Value_exists = false;
            }
        }
        public System.Collections.Generic.List<float> Values { get; private set; }

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
                        case _Text_ordinal:
                            {
                                this.Text = reader.ReadString();
                            } break;
                        case _Value_ordinal:
                            {
                                this.Value = reader.ReadSint();
                            } break;
                        case _Values_ordinal:
                            {
                                this.Values = reader.ReadSingleList();
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
                if (_Text_exists)
                {
                    writer.WriteString(_Text_value, _Text_ordinal);
                }
                if (_Value_exists)
                {
                    writer.WriteSint(_Value_value, _Value_ordinal);
                }
                writer.WriteSingleList(this.Values, _Values_ordinal);
            }
        }
    }
}
