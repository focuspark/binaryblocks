/**
 * Copyright (c) 2013 Jeremy Wyman
 * Microsoft Public License (Ms-PL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 * 1. Definitions
 *    The terms "reproduce", "reproduction", "derivative works", and "distribution" have the same meaning here as under U.S. copyright law.
 *    A "contribution" is the original software, or any additions or changes to the software.
 *    A "contributor" is any person that distributes its contribution under this license.
 *    "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 * 2. Grant of Rights
 *    (A) Copyright Grant - Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 *    (B) Patent Grant - Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 * 3. Conditions and Limitations
 *    (A) No Trademark License - This license does not grant you rights to use any contributors' name, logo, or trademarks.
 *    (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 *    (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 *    (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 *    (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 *    
 * More info on: http://binaryblocks.codeplex.com
**/

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
                    case BlockType.ByteList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<byte[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<byte>";
                                child[2] = blockReader.ReadByte();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Char:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<char>";
                            node[2] = blockReader.ReadChar();
                        } break;
                    case BlockType.CharList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<char[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<char>";
                                child[2] = blockReader.ReadChar();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Double:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<double>";
                            node[2] = blockReader.ReadDouble();
                        } break;
                    case BlockType.DoubleList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<double[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<double>";
                                child[2] = blockReader.ReadDouble();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Enum:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<enum>";
                            node[2] = blockReader.ReadSint();
                        } break;
                    case BlockType.EnumList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<enum[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<enum>";
                                child[2] = blockReader.ReadSint();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Guid:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<guid>";
                            node[2] = blockReader.ReadGuid();
                        } break;
                    case BlockType.GuidList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<guid[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<guid>";
                                child[2] = blockReader.ReadGuid();
                                node.Nodes.Add(child);
                            }
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
                    case BlockType.SintList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<sint[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<sint>";
                                child[2] = blockReader.ReadSint();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Slong:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<slong>";
                            node[2] = blockReader.ReadSlong();
                        } break;
                    case BlockType.SlongList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<slong[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<slong>";
                                child[2] = blockReader.ReadSlong();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.String:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<string>";
                            node[2] = "\"" + blockReader.ReadString() + "\"";
                        } break;
                    case BlockType.StringList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<string[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<string>";
                                child[2] = "\"" + blockReader.ReadString() + "\"";
                                node.Nodes.Add(child);
                            }
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
                                CommonTools.Node structNode = new CommonTools.Node();
                                structNode[0] = block.Ordinal;
                                structNode[1] = "<struct>";
                                structNode[2] = "...";
                                node.Nodes.Add(structNode);
                                int length = blockReader.ReadSint();
                                this.FillNodeCollection(structNode.Nodes, new StreamSegment(stream, length));
                            }
                        } break;
                    case BlockType.Timespan:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<timespan>";
                            node[2] = blockReader.ReadTimespan();
                        } break;
                    case BlockType.TimespanList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<timespan[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<timespan>";
                                child[2] = blockReader.ReadTimespan();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Timestamp:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<timestamp>";
                            node[2] = blockReader.ReadTimestamp();
                        } break;
                    case BlockType.TimestampList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<timestamp[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<timestamp>";
                                child[2] = blockReader.ReadTimestamp();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Uint:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<uint>";
                            node[2] = blockReader.ReadUint();
                        } break;
                    case BlockType.UintList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<uint[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<uint>";
                                child[2] = blockReader.ReadUint();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Ulong:
                        {
                            node[0] = block.Ordinal;
                            node[1] = "<ulong>";
                            node[2] = blockReader.ReadUlong();
                        } break;
                    case BlockType.UlongList:
                        {
                            int count = blockReader.ReadSint();
                            node[0] = block.Ordinal;
                            node[1] = "<ulong[]>";
                            node[2] = "count = " + count;
                            for (int i = 0; i < count; i++)
                            {
                                CommonTools.Node child = new CommonTools.Node();
                                child[0] = i;
                                child[1] = "<ulong>";
                                child[2] = blockReader.ReadUlong();
                                node.Nodes.Add(child);
                            }
                        } break;
                    case BlockType.Unknown:
                    default:
                        throw new Exception();
                }
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

        private void DecodeBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form window = new TextForm((Stream stream) =>
            {
                _dataTreeview.Nodes.Clear();
                this.FillNodeCollection(_dataTreeview.Nodes, stream);
            });
            window.Show(this);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().Show(this);
        }

        private void CodeplexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://binaryblocks.codeplex.com/");
        }
    }
}
