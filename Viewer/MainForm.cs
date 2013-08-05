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
                            node[0] = block.Ordinal;
                            node[1] = "<blob[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                int length = binaryReader.ReadInt32();
                                child[0] = i;
                                child[1] = "<blob>";
                                child[2] = "length = " + length;
                                binaryReader.BaseStream.Position += length;
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Byte:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<byte>";
                            node[2] = blockReader.ReadByte();
                        } break;
                    case BlockType.Char:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<char>";
                            node[2] = blockReader.ReadChar();
                        } break;
                    case BlockType.Double:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<double>";
                            node[2] = blockReader.ReadDouble();
                        } break;
                    case BlockType.Enum:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<enum>";
                            node[2] = blockReader.ReadSint();
                        } break;
                    case BlockType.Guid:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<guid>";
                            node[2] = blockReader.ReadGuid();
                        } break;
                    case BlockType.Single:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<single>";
                            node[2] = blockReader.ReadSingle();
                        } break;
                    case BlockType.SingleList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<single[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = block.Ordinal;
                                child[1] = "<single>";
                                child[2] = blockReader.ReadSingle();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Sint:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<sint>";
                            node[2] = blockReader.ReadSint();
                        } break;
                    case BlockType.Slong:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<slong>";
                            node[2] = blockReader.ReadSlong();
                        } break;
                    case BlockType.String:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<string>";
                            node[2] = blockReader.ReadString();
                        } break;
                    case BlockType.Struct:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<struct>";
                            node[2] = "...";
                            int length = blockReader.ReadSint();
                            FillNodeCollection(node.Nodes, new StreamSegment(stream, length));
                        } break;
                    case BlockType.StructList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<struct[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                int length = blockReader.ReadSint();
                                FillNodeCollection(node.Nodes, new StreamSegment(stream, length));
                            }
                        } break;
                    case BlockType.Timespan:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<timespan>";
                            node[2] = blockReader.ReadTimespan();
                        } break;
                    case BlockType.Timestamp:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<timestamp>";
                            node[2] = blockReader.ReadTimestamp();
                        } break;
                    case BlockType.Uint:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<uint>";
                            node[2] = blockReader.ReadUint();
                        } break;
                    case BlockType.Ulong:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<ulong>";
                            node[2] = blockReader.ReadUlong();
                        } break;
                    case BlockType.Unknown:
                    default:
                        throw new Exception();
                }
            }
        }

        private void FileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.toolStripStatusLabel1.Text = "loading file...";

                string path = _openFileDialog.FileName;
                FileInfo info = new FileInfo(path);

                this.toolStripStatusLabel2.Text = "File: " + info.Name;
                this.toolStripStatusLabel3.Text = "Size: " + GetFileSize(info.Length);

                _dataTreeview.Nodes.Clear();
                using (FileStream stream = File.Open(path, FileMode.Open))
                {
                    this.FillNodeCollection(_dataTreeview.Nodes, stream);
                }

                this.toolStripStatusLabel1.Text = "ready";
            }
        }

        private string GetFileSize(long bytes)
        {
            if (bytes < 1024)
                return String.Format("{0} B", bytes);
            else if (bytes < 1024 * 1024)
                return String.Format("{0} KB", bytes / 1024);
            else if (bytes < 1024 * 1024 * 1024)
                return String.Format("{0} MB", bytes / (1024 * 1024));
            else 
                return String.Format("{0} GB", bytes / (1024 * 1024 * 1024));
        }
    }
}
