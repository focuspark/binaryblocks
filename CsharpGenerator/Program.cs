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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryBlocks.CsharpGenerator
{
    public static partial class Program
    {
        #region Constructors
        static Program()
        {
            _definition = new Block.Definition();
            _blocks = new List<Block.Base>();
            _fileLines = new Dictionary<string, List<int>>();
        }
        #endregion
        #region Members
        private static string _baseNamespace;
        private static Block.Definition _definition;
        private static List<Block.Base> _blocks;
        private static Dictionary<string, List<int>> _fileLines;
        #endregion
        #region Methods
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Binary Blocks to CSharp Generator (Copyright (c) 2013 Jeremy Wyman)");
                Console.WriteLine("bbcsgen -files <file [file]> -output <output_dir> -namespace <root_namespace>");
                Environment.Exit(Environment.ExitCode);
            }

            _baseNamespace = String.Empty;
            string outDirectory = String.Empty;
            List<string> inputFiles = new List<string>();

            switch (args[0].ToLower())
            {
                case "?":
                case "-?":
                case "/?":
                case "-h":
                case "-help":
                case "--help":
                case "/h":
                case "/help":
                    new ScreenWriter().WriteText(Properties.Resources.Command);
                    Environment.Exit(Environment.ExitCode);
                    break;
                case "specification":
                    new ScreenWriter().WriteText(Properties.Resources.Specification);
                    Environment.Exit(Environment.ExitCode);
                    break;
                case "license":
                    new ScreenWriter().WriteText(Properties.Resources.License);
                    Environment.Exit(Environment.ExitCode);
                    break;
                case "export":
                    using (StreamWriter writer = new StreamWriter(File.OpenWrite("BinaryBlocks.cs")))
                    {
                        writer.WriteLine(Properties.Resources.BinaryBlocks);
                    }
                    Console.WriteLine("BinaryBlocks.cs created");
                    Environment.Exit(Environment.ExitCode);
                    break;
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-f":
                    case "-file":
                    case "-files":
                    case "--file":
                    case "--files":
                    case "/f":
                    case "/file":
                    case "/files":
                        while (++i < args.Length && !(args[i].StartsWith("-") || args[i].StartsWith("/")))
                        {
                            inputFiles.Add(args[i]);
                        }
                        i--;
                        break;
                    case "-o":
                    case "-out":
                    case "-output":
                    case "--out":
                    case "--output":
                    case "/o":
                    case "/out":
                    case "/output":
                        outDirectory = args[++i];
                        break;
                    case "--namespace":
                    case "-namespace":
                    case "-n":
                    case "/namespace":
                    case "/n":
                        _baseNamespace = args[++i].Trim(new char[4] { ' ', '.', '\'', '"' }) + ".";
                        break;
                    default:
                        Console.Error.WriteLine("{0} is an unknown parameter. Please use /? to access the help menu.", args[i]);
                        Environment.Exit(-1);
                        break;
                }
            }

            foreach (string file in inputFiles)
            {
                ParseFile(file);
            }

            foreach (Block.Base root in _definition.Roots)
            {
                WriteRoot(root, outDirectory);
            }
        }
        #region Parsing Methods
        internal static void ParseFile(string path)
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, path)))
            {
                Console.Error.WriteLine("The file \"{0}\" was not found", path);
                Environment.Exit(-1);
            }

            string content = null;
            _fileLines.Add(path, new List<int>());

            StringBuilder buffer = new StringBuilder();
            using (StreamReader reader = File.OpenText(path))
            {
                int length = 0;
                string line = null;
                int newlinelen = Environment.NewLine.Length;
                while ((line = reader.ReadLine()) != null)
                {
                    buffer.AppendLine(line);
                    length += line.Length + newlinelen;
                    _fileLines[path].Add(length);
                }
            }
            content = buffer.ToString();
            buffer = null;

            try
            {
                for (int index = 0; index < content.Length; index++)
                {
                    if (TextParser.SeekNext(content, ref index))
                    {
                        if (Char.IsLetter(content[index]))
                        {
                            string word = TextParser.ParseWord(content, ref index);

                            if (String.Equals("struct", word, StringComparison.OrdinalIgnoreCase))
                            {
                                Block.Struct child = ParseStruct(content, path, ref index);
                                _definition.Roots.Add(child);
                            }
                            else if (String.Equals("enum", word, StringComparison.OrdinalIgnoreCase))
                            {
                                Block.Enum child = ParseEnum(content, path, ref index);
                                _definition.Roots.Add(child);
                            }
                            else if (String.Equals("namespace", word, StringComparison.OrdinalIgnoreCase))
                            {
                                Block.Namespace child = ParseNamespace(content, path, ref index);
                                _definition.Roots.Add(child);
                            }
                            else if (String.Equals("import", word, StringComparison.OrdinalIgnoreCase))
                            {
                                TextParser.SeekAny(content, ref index, TextParser.StringDelimiter);
                                string import = TextParser.ParseString(content, ref index);
                                import = Path.Combine(Path.GetDirectoryName(path), import);
                                ParseFile(import);
                            }
                            else
                            {
                                throw new TextParser.Exception(index);
                            }
                        }
                        else if (content[index] == TextParser.CommentDelimiter)
                        {
                            TextParser.SkipComment(content, ref index);
                        }
                        else
                        {
                            throw new TextParser.Exception(index);
                        }
                    }
                }

                foreach (Block.Base block in _blocks)
                {
                    if (!_definition.Nodes.ContainsKey(block.FullName))
                        _definition.Nodes.Add(block.FullName, block);
                }
            }
            catch (TextParser.Exception exception)
            {
                int linenumber = 0;
                int position = 0;
                for (int i = 0; i < _fileLines[path].Count; i++)
                {
                    int charcount = _fileLines[path][i];
                    if (charcount > exception.Index)
                    {
                        linenumber = i;
                        if (i > 0)
                        {
                            position = _fileLines[path][i - 1];
                        }
                        position = exception.Index - position;
                        break;
                    }
                }
                Console.Error.WriteLine("Parse error in {0} line {1} character {2}", path, linenumber + 1, position + 1);
                Console.Error.WriteLine(exception.Message);
                Debug.Break();
                Environment.Exit(-1);
            }
            catch (CsGeneratorException exception)
            {
                if (!_fileLines.ContainsKey(exception.Block.Source))
                    throw exception;

                int linenumber = 0;
                int position = 0;
                for (int i = 0; i < _fileLines[exception.Block.Source].Count; i++)
                {
                    if (_fileLines[exception.Block.Source][i] > exception.Block.Index)
                    {
                        linenumber = i;
                        if (i > 0)
                        {
                            position = _fileLines[exception.Block.Source][i - 1];
                        }
                        position = exception.Block.Index - position;
                        break;
                    }
                }
                Console.Error.WriteLine("Parse error in {0} line {1} character {2} \"{3}\"", exception.Block.Source, linenumber + 1, position + 1);
                Console.Error.WriteLine(exception.Message);
                Debug.Break();
                Environment.Exit(-1);
            }
        }

        internal static Block.Enum ParseEnum(string content, string source, ref int index)
        {
            TextParser.SeekNext(content, ref index);
            int initialIndex = index;
            string name = TextParser.ParseWord(content, ref index);

            Block.Enum block = new Block.Enum(name, source, initialIndex);

            if (!TextParser.PeekAny(content, index, TextParser.BlockBeginDelimiter))
                throw new TextParser.Exception(index, String.Format("'{0}' expected", TextParser.BlockBeginDelimiter));
            TextParser.SeekAny(content, ref index, TextParser.BlockBeginDelimiter);
            index++;

            while (index < content.Length)
            {
                if (TextParser.SeekNext(content, ref index))
                {
                    if (Char.IsLetter(content[index]))
                    {
                        string word = TextParser.ParseWord(content, ref index);
                        byte ordinal = 0;
                        if (!ValidateBlockName(name))
                            throw new TextParser.Exception(index, "invalid name");
                        TextParser.SeekAny(content, ref index, '=');
                        index++;
                        TextParser.SeekNext(content, ref index);
                        if (!Byte.TryParse(TextParser.ParseWord(content, ref index), out ordinal))
                            throw new TextParser.Exception(index, "invalid ordinal/value assignment");
                        block.Members.Add(ordinal, word);
                        TextParser.SeekAny(content, ref index, ',');
                        index++;
                    }
                    else if (content[index] == TextParser.CommentDelimiter)
                    {
                        TextParser.SkipComment(content, ref index);
                    }
                    else if (content[index] == TextParser.BlockEndDelimiter)
                    {
                        index++;
                        break;
                    }
                    else
                    {
                        throw new TextParser.Exception(index);
                    }
                }
                else
                {
                    throw new TextParser.Exception(index, String.Format("'{0}' expected", TextParser.BlockEndDelimiter));
                }
            }

            _blocks.Add(block);

            return block;
        }

        internal static Block.Namespace ParseNamespace(string content, string source, ref int index)
        {
            TextParser.SeekNext(content, ref index);
            string name = TextParser.ParseWord(content, ref index, '.');

            Block.Namespace block = new Block.Namespace(name, source, index);

            TextParser.SeekNext(content, ref index);
            if (content[index] != TextParser.BlockBeginDelimiter)
            {
                throw new TextParser.Exception(index, TextParser.BlockBeginDelimiter + " expected");
            }
            index++;

            while (index < content.Length)
            {
                if (TextParser.SeekNext(content, ref index))
                {
                    if (Char.IsLetter(content[index]))
                    {
                        string word = TextParser.ParseWord(content, ref index);

                        if (String.Equals("struct", word, StringComparison.OrdinalIgnoreCase))
                        {
                            Block.Struct child = ParseStruct(content, source, ref index);
                            child.Parent = block;
                            block.Children.Add(child);
                        }
                        else if (String.Equals("enum", word, StringComparison.OrdinalIgnoreCase))
                        {
                            Block.Enum child = ParseEnum(content, source, ref index);
                            child.Parent = block;
                            block.Children.Add(child);
                        }
                        else
                        {
                            throw new TextParser.Exception(index - word.Length);
                        }
                    }
                    else if (content[index] == TextParser.CommentDelimiter)
                    {
                        TextParser.SkipComment(content, ref index);
                    }
                    else if (content[index] == TextParser.BlockEndDelimiter)
                    {
                        index++;
                        break;
                    }
                    else
                    {
                        throw new TextParser.Exception(index);
                    }

                    index++;
                }
                else
                {
                    throw new TextParser.Exception(index, String.Format("'{0}' expected", TextParser.BlockEndDelimiter));
                }
            }

            _blocks.Add(block);

            return block;
        }

        internal static Block.Struct ParseStruct(string content, string source, ref int index)
        {
            TextParser.SeekNext(content, ref index);
            string name = TextParser.ParseWord(content, ref index);

            Block.Struct block = new Block.Struct(name, source, index);

            TextParser.SeekNext(content, ref index);
            if (content[index] != TextParser.BlockBeginDelimiter)
            {
                throw new TextParser.Exception(index, TextParser.BlockBeginDelimiter + " expected");
            }
            index++;

            while (index < content.Length)
            {
                if (TextParser.SeekNext(content, ref index))
                {
                    if (Char.IsLetter(content[index]))
                    {
                        string word = TextParser.ParseWord(content, ref index);

                        if (String.Equals("type", word, StringComparison.OrdinalIgnoreCase))
                        {
                            Block.Member child = ParseMember(content, source, ref index, false);
                            child.Parent = block;
                            block.Members.Add(child.Ordinal, child);
                        }
                        else if (String.Equals("list", word, StringComparison.OrdinalIgnoreCase))
                        {
                            Block.Member child = ParseMember(content, source, ref index, true);
                            child.Parent = block;
                            block.Members.Add(child.Ordinal, child);
                        }
                        else if (String.Equals("struct", word, StringComparison.OrdinalIgnoreCase))
                        {
                            Block.Struct child = ParseStruct(content, source, ref index);
                            child.Parent = block;
                            block.Children.Add(child);
                        }
                        else if (String.Equals("enum", word, StringComparison.OrdinalIgnoreCase))
                        {
                            Block.Enum child = ParseEnum(content, source, ref index);
                            child.Parent = block;
                            block.Children.Add(child);
                        }
                        else
                        {
                            throw new TextParser.Exception(index - word.Length);
                        }
                    }
                    else if (content[index] == TextParser.CommentDelimiter)
                    {
                        TextParser.SkipComment(content, ref index);
                    }
                    else if (content[index] == TextParser.BlockEndDelimiter)
                    {
                        index++;
                        break;
                    }
                    else
                    {
                        throw new TextParser.Exception(index);
                    }
                }
                else
                {
                    throw new TextParser.Exception(index, String.Format("'{0}' expected", TextParser.BlockEndDelimiter));
                }
            }

            _blocks.Add(block);

            return block;
        }

        internal static Block.Member ParseMember(string content, string source, ref int index, bool isList)
        {
            TextParser.SeekNext(content, ref index);
            int initialIndex = index;
            string type = TextParser.ParseWord(content, ref index);
            TextParser.SeekNext(content, ref index);
            string name = TextParser.ParseWord(content, ref index);
            ushort ordinal = 0;

            TextParser.SeekAny(content, ref index, '=');
            index++;
            TextParser.SeekNext(content, ref index);
            if (!UInt16.TryParse(TextParser.ParseWord(content, ref index), out ordinal))
                throw new TextParser.Exception(index, "invalid ordinal");

            Block.Member block = new Block.Member(name, type, isList, source, initialIndex) { Ordinal = ordinal };

            TextParser.SeekNext(content, ref index);

            if (content[index] == '[')
            {
                while (content[index] != ']')
                {
                    index++;
                    string word = TextParser.ParseWord(content, ref index);

                    switch (word.ToLower())
                    {
                        case "expected":
                            if (block.Deprecated)
                                throw new TextParser.Exception(index, "member cannot be annotated as both expected and deprecated");
                            if (block.Excepted)
                                throw new TextParser.Exception(index, "member cannot be annotated as expected more than once");

                            block.Excepted = true;
                            break;
                        case "deprecated":
                            if (block.Deprecated)
                                throw new TextParser.Exception(index, "member cannot be annotated as both deprecated more than once");
                            if (block.Excepted)
                                throw new TextParser.Exception(index, "member cannot be annotated as deprecated and expected");

                            block.Deprecated = true;
                            break;
                        default:
                            throw new TextParser.Exception(index - word.Length, "unexpected annotation value");
                    }
                }
                index++;
            }

            TextParser.SeekAny(content, ref index, ';');
            index++;

            _blocks.Add(block);

            return block;
        }
        #endregion
        #region Writing Methods
        private static void WriteEnum(Block.Enum node, CodeWriter writer)
        {
            bool wroteNamespace = false;

            if (node.Parent != null && node.Parent is Block.Namespace && !(node.Parent is Block.Struct))
            {
                Block.Base parent = node.Parent as Block.Namespace;
                StringBuilder buffer = new StringBuilder();

                while (parent != null)
                {
                    buffer.Append(parent).Append(".");
                    parent = parent.Parent;
                }
                buffer.Remove(buffer.Length - 1, 1);

                writer.Write("namespace {0}{1}", _baseNamespace, buffer.ToString());
                writer.BeginBlock();

                wroteNamespace = true;
            }

            writer.Write("internal enum {0} : uint", node.Name)
                  .BeginBlock();
            {
                foreach (uint ordinal in node.Members.Keys)
                {
                    writer.Write("{0} = {1},", node.Members[ordinal], ordinal);
                }
            }
            writer.EndBlock();

            if (wroteNamespace)
            {
                writer.EndBlock();
            }
        }

        private static void WriteFile(Block.Base root, string path)
        {
            CodeWriter codeWriter = new CodeWriter();

            try
            {
                if (root is Block.Struct)
                {
                    WriteStruct(root as Block.Struct, codeWriter);
                }
                else if (root is Block.Enum)
                {
                    WriteEnum(root as Block.Enum, codeWriter);
                }
                else
                {
                    throw new Exception();
                }
#if DEBUG
                string content = codeWriter.ToString();
#endif
            }
            catch (CsGeneratorException exception)
            {
                if (!_fileLines.ContainsKey(exception.Block.Source))
                    throw exception;

                int linenumber = 0;
                int position = 0;
                for (int i = 0; i < _fileLines[exception.Block.Source].Count; i++)
                {
                    if (_fileLines[exception.Block.Source][i] > exception.Block.Index)
                    {
                        linenumber = i;
                        if (i > 0)
                        {
                            position = _fileLines[exception.Block.Source][i - 1];
                        }
                        position = exception.Block.Index - position;
                        break;
                    }
                }
                Console.Error.WriteLine("Parse error in {0} line {1} character {2}", exception.Block.Source, linenumber + 1, position + 1);
                Console.Error.WriteLine(" {0} for {1}", exception.Message, exception.Block.FullName);
                Debug.Break();
                Environment.Exit(-1);
            }

            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(codeWriter.ToString());
            }
        }

        private static void WriteRoot(Block.Base root, string directory)
        {
            Debug.Assert(root != null);
            Debug.Assert(!String.IsNullOrWhiteSpace(directory));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (root is Block.Struct)
            {
                WriteFile(root, Path.Combine(directory, root.Name + ".Designer.cs"));
            }
            else if (root is Block.Enum)
            {
                WriteFile(root, root.Name + ".Designer.cs");
            }
            else if (root is Block.Namespace)
            {
                foreach (Block.Base child in (root as Block.Namespace).Children)
                {
                    WriteRoot(child, directory);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static void WriteStruct(Block.Struct node, CodeWriter writer)
        {
            bool wroteNamespace = false;

            if (node.Parent != null && (node.Parent is Block.Namespace) && !(node.Parent is Block.Struct))
            {
                writer.Write("namespace {0}{1}", _baseNamespace, node.Parent.FullName);
                writer.BeginBlock();
                wroteNamespace = true;
            }

            writer.Write("internal partial class {0} : BinaryBlocks.IBinaryBlock", node.Name)
                  .BeginBlock();
            {
                foreach (Block.Base child in node.Children)
                {
                    if (child is Block.Struct)
                    {
                        WriteStruct(child as Block.Struct, writer);
                    }
                    else if (child is Block.Enum)
                    {
                        WriteEnum(child as Block.Enum, writer);
                    }
                    else
                    {
                        throw new CsGeneratorException(child, "Invalid block type");
                    }
                }

                CodeWriter accessors = new CodeWriter(writer.Depth);
                CodeWriter constructor = new CodeWriter(writer.Depth + 1);
                CodeWriter delcarations = new CodeWriter(writer.Depth);
                CodeWriter deserialize = new CodeWriter(writer.Depth + 4);
                CodeWriter serialize = new CodeWriter(writer.Depth + 2);

                {
                    Block.Member member = null;
                    foreach (byte ordinal in node.Members.Keys)
                    {
                        member = node.Members[ordinal];

                        string cSharpType = GetMemberCSType(member.TypeName);

                        if (member.Type == BlockType.Unknown || member.Type == BlockType.List)
                        {
                            bool found = false;
                            Block.Base n = node;
                            while (n != null)
                            {
                                if (_definition.Nodes.ContainsKey(n.FullName + "." + member.TypeName))
                                {
                                    if (_definition.Nodes[n.FullName + "." + member.TypeName] is Block.Enum)
                                    {
                                        member.Type |= BlockType.Enum;
                                        found = true;
                                        break;
                                    }
                                    else if (_definition.Nodes[n.FullName + "." + member.TypeName] is Block.Struct)
                                    {
                                        member.Type |= BlockType.Struct;
                                        found = true;
                                        break;
                                    }
                                }
                                n = n.Parent;
                            }
                            if (!found)
                                throw new CsGeneratorException(member, "Block definition not found");
                        }

                        System.Diagnostics.Debug.Assert(member.Type != BlockType.List && member.Type != BlockType.Unknown);

                        if (member.IsList)
                        {
                            constructor.Write("this.{0} = new System.Collections.Generic.List<{1}>();", member.Name, cSharpType);

                            delcarations.Write("private const ushort _{0}_ordinal = {1};", member.Name, ordinal);

                            if (member.Deprecated)
                            {
                                accessors.Write("[System.Obsolete(\"member has been deprecated\")]");
                            }
                            accessors.Write("public System.Collections.Generic.List<{0}> {1} {{ get; private set; }}", cSharpType, member.Name);

                            if ((member.Type & BlockType.Struct) == BlockType.Struct)
                            {
                                deserialize.Write("case _{0}_ordinal:", member.Name)
                                    .IndentMore()
                                    .BeginBlock()
                                        .Write("this.{0} = reader.ReadStructList<{1}>();", member.Name, cSharpType)
                                    .EndBlock("break;")
                                    .IndentLess();

                                serialize.Write("writer.WriteStructList<{0}>(this.{1}, _{1}_ordinal, {2});", cSharpType, member.Name, member.Deprecated ? "BinaryBlocks.BlockFlags.Deprecated" : "BinaryBlocks.BlockFlags.None");
                            }
                            else
                            {
                                deserialize.Write("case _{0}_ordinal:", member.Name)
                                    .IndentMore()
                                    .BeginBlock()
                                        .Write("this.{0} = reader.Read{1}List();", member.Name, member.Type ^ BlockType.List)
                                    .EndBlock("break;")
                                    .IndentLess();

                                serialize.Write("writer.Write{0}List(this.{1}, _{1}_ordinal, {2});", member.Type ^ BlockType.List, member.Name, member.Deprecated ? "BinaryBlocks.BlockFlags.Deprecated" : "BinaryBlocks.BlockFlags.None");
                            }
                        }
                        else
                        {
                            delcarations.Write("private bool _{0}_exists;", member.Name);
                            delcarations.Write("private const ushort _{0}_ordinal = {1};", member.Name, ordinal);
                            delcarations.Write("private {0} _{1}_value;", cSharpType, member.Name);

                            if (member.Deprecated)
                            {
                                accessors.Write("[System.Obsolete(\"member has been deprecated\")]");
                            }
                            accessors.Write("public {0} {1}", cSharpType, member.Name)
                                .BeginBlock()
                                    .Write("get")
                                    .BeginBlock()
                                        .Write("if (_{0}_exists)", member.Name)
                                            .WriteIndented("return _{0}_value;", member.Name)
                                        .Write("throw new System.InvalidOperationException();")
                                    .EndBlock()
                                    .Write("set")
                                    .BeginBlock()
                                        .Write("_{0}_value = value;", member.Name)
                                        .Write("_{0}_exists = true;", member.Name)
                                    .EndBlock()
                                .EndBlock();

                            accessors.Write("public bool {0}_exists", member.Name)
                                .BeginBlock()
                                    .Write("get {{ return _{0}_exists; }}", member.Name)
                                    .Write("set")
                                    .BeginBlock()
                                        .Write("if (value)")
                                            .WriteIndented("throw new System.InvalidOperationException();")
                                        .Write("_{0}_exists = false;", member.Name)
                                    .EndBlock()
                                .EndBlock();

                            if ((member.Type & BlockType.Struct) == BlockType.Struct)
                            {
                                deserialize.Write("case _{0}_ordinal:", member.Name)
                                    .IndentMore()
                                    .BeginBlock()
                                        .Write("this.{0} = reader.ReadStruct<{1}>();", member.Name, cSharpType)
                                    .EndBlock("break;")
                                    .IndentLess();

                                serialize.Write("if (_{0}_exists)", member.Name)
                                    .BeginBlock()
                                        .Write("writer.WriteStruct<{0}>(_{1}_value, _{1}_ordinal, {2});", cSharpType, member.Name, member.Deprecated ? "BinaryBlocks.BlockFlags.Deprecated" : "BinaryBlocks.BlockFlags.None")
                                    .EndBlock();
                            }
                            else if ((member.Type & BlockType.Enum) == BlockType.Enum)
                            {
                                deserialize.Write("case _{0}_ordinal:", member.Name)
                                    .IndentMore()
                                    .BeginBlock()
                                        .Write("this.{0} = ({1})reader.ReadUint();", member.Name, member.TypeName)
                                    .EndBlock("break;")
                                    .IndentLess();

                                serialize.Write("if (_{0}_exists)", member.Name)
                                    .BeginBlock()
                                    .Write("writer.WriteUint((uint)_{0}_value, _{0}_ordinal, {1});", member.Name, member.Deprecated ? "BinaryBlocks.BlockFlags.Deprecated" : "BinaryBlocks.BlockFlags.None")
                                    .EndBlock();
                            }
                            else
                            {
                                deserialize.Write("case _{0}_ordinal:", member.Name)
                                    .IndentMore()
                                    .BeginBlock()
                                        .Write("this.{0} = reader.Read{1}();", member.Name, member.Type)
                                    .EndBlock("break;")
                                    .IndentLess();

                                serialize.Write("if (_{0}_exists)", member.Name)
                                    .BeginBlock()
                                        .Write("writer.Write{0}(_{1}_value, _{1}_ordinal, {2});", member.Type, member.Name, member.Deprecated ? "BinaryBlocks.BlockFlags.Deprecated" : "BinaryBlocks.BlockFlags.None")
                                    .EndBlock();
                            }
                        }
                    }
                }

                writer.Write("public {0}()", node.Name)
                    .BeginBlock()
                        .Merge(constructor)
                    .EndBlock();

                writer.Write("/* Private */")
                      .Merge(delcarations);
                writer.Write("/* Public */")
                      .Merge(accessors);
                writer.Write();
                writer.Write("public void Deserialize(System.IO.Stream stream)");
                writer.BeginBlock()
                    .Write("if (stream == null)")
                        .WriteIndented("throw new System.ArgumentNullException();")
                    .Write("if (!stream.CanRead || !stream.CanSeek)")
                        .WriteIndented("throw new System.InvalidOperationException();")
                    .Write()
                    .Write("using (BinaryBlocks.BinaryBlockReader reader = new BinaryBlocks.BinaryBlockReader(stream))")
                    .BeginBlock()
                        .Write("while (reader.Position < reader.Length)")
                        .BeginBlock()
                            .Write("BinaryBlocks.BinaryBlock block = reader.ReadBinaryBlock();")
                            .Write()
                            .Write("switch (block.Ordinal)")
                                .BeginBlock()
                                    .Merge(deserialize)
                                    .Write("default:")
                                        .WriteIndented("reader.SkipBlock(block);")
                                        .WriteIndented("break;")
                                .EndBlock()
                        .EndBlock()
                    .EndBlock()
                .EndBlock();
                writer.Write();
                writer.Write("public void Serialize(System.IO.Stream stream)");
                writer.BeginBlock()
                    .Write("if (stream == null)")
                        .WriteIndented("throw new System.ArgumentNullException();")
                    .Write("if (!stream.CanWrite)")
                        .WriteIndented("throw new System.InvalidOperationException();")
                    .Write()
                    .Write("using (BinaryBlocks.BinaryBlockWriter writer = new BinaryBlocks.BinaryBlockWriter(stream))")
                    .BeginBlock()
                        .Merge(serialize)
                    .EndBlock()
                .EndBlock();
            }
            writer.EndBlock();

            if (wroteNamespace)
            {
                writer.EndBlock();
            }
        }
        #endregion
        #region Helper Methods
        private static string GetMemberCSType(string parsedType)
        {
            switch (parsedType.ToLower())
            {
                case "byte": return "byte";
                case "char": return "char";
                case "sint": return "int";
                case "uint": return "uint";
                case "slong": return "long";
                case "ulong": return "ulong";
                case "single": return "float";
                case "double": return "double";
                case "string": return "string";
                case "timestamp": return "System.Timestamp";
                case "timespan": return "System.TimeSpan";
                case "guid": return "System.Guid";
                case "blob": return "byte[]";
                default: return parsedType;
            }
        }

        private static bool ValidateBlockName(string name, params char[] allowedCharacters)
        {
            if (!Char.IsLetter(name[0]))
                return false;

            for (int i = 1; i < name.Length; i++)
            {
                if (!(Char.IsLetterOrDigit(name[i]) || allowedCharacters.Contains(name[i])))
                    return false;
            }

            return true;
        }
        #endregion
        #endregion
    }
}
