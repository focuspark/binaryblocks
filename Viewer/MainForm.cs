using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinaryBlocks.Viewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            using (FileStream stream = File.Open("complex.b3", FileMode.Open))
            {
                FillNodeCollection(dataTreeview.Nodes, stream);
            }
        }

        private void FillNodeCollection(CommonTools.NodeCollection collection, Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException();
            if (!stream.CanRead || !stream.CanSeek)
                throw new System.InvalidOperationException();

            BinaryBlocks.BinaryBlockReader blockReader = new BinaryBlocks.BinaryBlockReader(stream);
            BinaryReader binaryReader = new BinaryReader(stream);

            while (blockReader.Position < blockReader.Length)
            {
                BinaryBlocks.BinaryBlock block = blockReader.ReadBinaryBlock();
                CommonTools.Node node = new CommonTools.Node();
                collection.Add(node);

                switch (block.Type)
                {
                    case BlockType.Blob:
                        {
                            int length = binaryReader.ReadInt32();
                            node[0] = block.Ordinal;
                            node[1] = "<blob>";
                            node[2] = "length = " + length;
                            binaryReader.BaseStream.Position += length;
                        } break;
                    case BlockType.BlobList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = String.Format("#{0}#<blob[]> = {1}", block.Ordinal, count);
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                int length = binaryReader.ReadInt32();
                                child[0] = String.Format("#{0}#<blob> = [{1}]", block.Ordinal, length);
                                binaryReader.BaseStream.Position += length;
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Byte:
                        {
                            node[0] = String.Format("#{0}#<byte> = {1}", block.Ordinal, blockReader.ReadByte());
                        } break;
                    case BlockType.Char:
                        {
                            node[0] = String.Format("#{0}#<char> = {1}", block.Ordinal, blockReader.ReadChar());
                        } break;
                    case BlockType.Double:
                        {
                            node[0] = String.Format("#{0}#<double> = {1}", block.Ordinal, blockReader.ReadDouble());
                        } break;
                    case BlockType.Enum:
                        {
                            node[0] = String.Format("#{0}#<enum> = {1}", block.Ordinal, blockReader.ReadUint());
                        } break;
                    case BlockType.Guid:
                        {
                            node[0] = String.Format("#{0}#<guid> = {1:N}", block.Ordinal, blockReader.ReadGuid());
                        } break;
                    case BlockType.Single:
                        {
                            node[0] = String.Format("#{0}#<single> = {1}", block.Ordinal, blockReader.ReadSingle());
                        } break;
                    case BlockType.SingleList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = String.Format("#{0}#<single[]> = {1}", block.Ordinal, count);
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = String.Format("#{0}#<single> = {1}", block.Ordinal, blockReader.ReadSingle());
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Sint:
                        {
                            node[0] = String.Format("#{0}#<sint> = {1}", block.Ordinal, blockReader.ReadSint());
                        } break;
                    case BlockType.Slong:
                        {
                            node[0] = String.Format("#{0}#<slong> = {1}", block.Ordinal, blockReader.ReadSlong());
                        } break;
                    case BlockType.String:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<string>";
                            node[2] = blockReader.ReadString();
                        } break;
                    case BlockType.Struct:
                        {
                            node[0] = String.Format("#{0}#<struct>", block.Ordinal);
                            int length = blockReader.ReadSint();
                            FillNodeCollection(node.Nodes, new StructStream(stream, length));
                        } break;
                    case BlockType.StructList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = String.Format("#{0}#<struct[]> = {1}", block.Ordinal, count);
                            for (int i = 0; i < count; i++)
                            {
                                int length = blockReader.ReadSint();
                                FillNodeCollection(node.Nodes, new StructStream(stream, length));
                            }
                        } break;
                    case BlockType.Timespan:
                        {
                            node[0] = String.Format("#{0}#<timespan> = {1}", block.Ordinal, blockReader.ReadTimespan());
                        } break;
                    case BlockType.Timestamp:
                        {
                            node[0] = String.Format("#{0}#<timestamp> = {1}", block.Ordinal, blockReader.ReadTimestamp());
                        } break;
                    case BlockType.Uint:
                        {
                            node[0] = String.Format("#{0}#<uint> = {1}", block.Ordinal, blockReader.ReadUint());
                        } break;
                    case BlockType.Ulong:
                        {
                            node[0] = String.Format("#{0}#<ulong> = {1}", block.Ordinal, blockReader.ReadUlong());
                        } break;
                    case BlockType.Unknown:
                    default:
                        throw new Exception();
                }
            }
        }

        private void FileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
