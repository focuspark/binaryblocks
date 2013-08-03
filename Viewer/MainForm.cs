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
        }

        private TreeNode ReadNode(Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException();
            if (!stream.CanRead || !stream.CanSeek)
                throw new System.InvalidOperationException();

            TreeNode node = new TreeNode();

            BinaryBlocks.BinaryBlockReader blockReader = new BinaryBlocks.BinaryBlockReader(stream);
            BinaryReader binaryReader = new BinaryReader(stream);

            while (blockReader.Position < blockReader.Length)
            {
                BinaryBlocks.BinaryBlock block = blockReader.ReadBinaryBlock();

                switch (block.Type ^ BlockType.List)
                {
                    case BlockType.Blob:
                        {
                            int size = binaryReader.ReadInt32();
                            node.Text = String.Format("[{0}]<blob> = [{1}]", block.Ordinal, size);
                            binaryReader.BaseStream.Position += size;
                        }break;
                    case BlockType.Byte:
                        {
                            node.Text = String.Format("[{0}]<byte> = {1}", block.Ordinal, blockReader.ReadByte());
                        }break;
                    case BlockType.Char:
                        {
                            node.Text = String.Format("[{0}]<char> = {1}", block.Ordinal, blockReader.ReadChar());
                        } break;
                    case BlockType.Double:
                        {
                            node.Text = String.Format("[{0}]<double> = {1}", block.Ordinal, blockReader.ReadDouble());
                        }break;
                    case BlockType.Enum:
                        {
                            node.Text = String.Format("[{0}]<enum> = {1}", block.Ordinal, blockReader.ReadUint());
                        }break;
                    case BlockType.Guid:
                        {
                            node.Text = String.Format("[{0}]<guid> = {1:N}", block.Ordinal, blockReader.ReadGuid());
                        }break;
                    case BlockType.Single:
                        {
                            node.Text = String.Format("[{0}]<single> = {1}", block.Ordinal, blockReader.ReadSingle());
                        }break;
                    case BlockType.Sint:
                        {
                            node.Text = String.Format("[{0}]<sint> = {1}", block.Ordinal, blockReader.ReadSint());
                        }break;
                    case BlockType.Slong:
                        {
                            node.Text = String.Format("[{0}]<slong> = {1}", block.Ordinal, blockReader.ReadSlong());
                        }break;
                    case BlockType.String:
                        {
                            node.Text = String.Format("[{0}]<string> = \"{1}\"", block.Ordinal, blockReader.ReadString());
                        }break;
                    case BlockType.Struct:
                        {
                            node.Text = String.Format("[{0}]<struct>", block.Ordinal);
                            node.Nodes.Add(ReadNode(stream));
                        } break;
                    case BlockType.Timespan:
                        {
                            node.Text = String.Format("[{0}]<timespan> = {1}", block.Ordinal, blockReader.ReadTimespan());
                        }break;
                    case BlockType.Timestamp:
                        {
                            node.Text = String.Format("[{0}]<timestamp> = {1}", block.Ordinal, blockReader.ReadTimestamp());
                        }break;
                    case BlockType.Uint:
                        {
                            node.Text = String.Format("[{0}]<uint> = {1}", block.Ordinal, blockReader.ReadUint());
                        }break;
                    case BlockType.Ulong:
                        {
                            node.Text = String.Format("[{0}]<ulong> = {1}", block.Ordinal, blockReader.ReadUlong());
                        }break;
                    case BlockType.Unknown:
                    default:
                        throw new Exception();
                }
            }

            return node;
        }

        private void FileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
