namespace Sample.More
{
    internal partial class Person : BinaryBlocks.IBinaryBlock
    {
        public Person()
        {
        }
        /* Private */
        private bool _Name_exists;
        private const ushort _Name_ordinal = 1;
        private string _Name_value;
        private bool _DoB_exists;
        private const ushort _DoB_ordinal = 2;
        private System.Timestamp _DoB_value;
        private bool _Len_exists;
        private const ushort _Len_ordinal = 3;
        private System.TimeSpan _Len_value;
        private bool _Mother_exists;
        private const ushort _Mother_ordinal = 4;
        private Person _Mother_value;
        /* Public */
        public string Name
        {
            get
            {
                if (_Name_exists)
                    return _Name_value;
                throw new System.InvalidOperationException();
            }
            set
            {
                _Name_value = value;
                _Name_exists = true;
            }
        }
        public bool Name_exists
        {
            get { return _Name_exists; }
            set
            {
                if (value)
                    throw new System.InvalidOperationException();
                _Name_exists = false;
            }
        }
        public System.Timestamp DoB
        {
            get
            {
                if (_DoB_exists)
                    return _DoB_value;
                throw new System.InvalidOperationException();
            }
            set
            {
                _DoB_value = value;
                _DoB_exists = true;
            }
        }
        public bool DoB_exists
        {
            get { return _DoB_exists; }
            set
            {
                if (value)
                    throw new System.InvalidOperationException();
                _DoB_exists = false;
            }
        }
        [System.Obsolete("member has been deprecated")]
        public System.TimeSpan Len
        {
            get
            {
                if (_Len_exists)
                    return _Len_value;
                throw new System.InvalidOperationException();
            }
            set
            {
                _Len_value = value;
                _Len_exists = true;
            }
        }
        public bool Len_exists
        {
            get { return _Len_exists; }
            set
            {
                if (value)
                    throw new System.InvalidOperationException();
                _Len_exists = false;
            }
        }
        public Person Mother
        {
            get
            {
                if (_Mother_exists)
                    return _Mother_value;
                throw new System.InvalidOperationException();
            }
            set
            {
                _Mother_value = value;
                _Mother_exists = true;
            }
        }
        public bool Mother_exists
        {
            get { return _Mother_exists; }
            set
            {
                if (value)
                    throw new System.InvalidOperationException();
                _Mother_exists = false;
            }
        }

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
                        case _Name_ordinal:
                            {
                                this.Name = reader.ReadString();
                            } break;
                        case _DoB_ordinal:
                            {
                                this.DoB = reader.ReadTimestamp();
                            } break;
                        case _Len_ordinal:
                            {
                                this.Len = reader.ReadTimespan();
                            } break;
                        case _Mother_ordinal:
                            {
                                this.Mother = reader.ReadStruct<Person>();
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
                if (_Name_exists)
                {
                    writer.WriteString(_Name_value, _Name_ordinal, BinaryBlocks.BlockFlags.None);
                }
                if (_DoB_exists)
                {
                    writer.WriteTimestamp(_DoB_value, _DoB_ordinal, BinaryBlocks.BlockFlags.None);
                }
                if (_Len_exists)
                {
                    writer.WriteTimespan(_Len_value, _Len_ordinal, BinaryBlocks.BlockFlags.Deprecated);
                }
                if (_Mother_exists)
                {
                    writer.WriteStruct<Person>(_Mother_value, _Mother_ordinal, BinaryBlocks.BlockFlags.None);
                }
            }
        }
    }
}
