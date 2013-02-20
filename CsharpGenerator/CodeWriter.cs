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
using System.Text;

namespace BinaryBlocks.CsharpGenerator
{
    internal class CodeWriter
    {
        public const char DefaultIndentChar = ' ';
        public const int DefaultIndentWidth = 4;

        public CodeWriter(int depth)
        {
            _code = new StringBuilder();
            _indent = depth;
            this.IndentChar = CodeWriter.DefaultIndentChar;
            this.IndentDepth = CodeWriter.DefaultIndentWidth;
        }
        public CodeWriter() : this(0) { }

        public int Depth { 
            get { return _indent; }
            private set { _indent = value; }
        }

        public char IndentChar { get; set; }
        public int IndentDepth { get; set; }
        private StringBuilder _code;
        private int _indent;

        public CodeWriter BeginBlock()
        {
            _code.Append(this.IndentChar, this.IndentDepth * _indent)
                 .Append("{")
                 .AppendLine();
            _indent++;

            return this;
        }
        public CodeWriter EndBlock(string code)
        {
            _indent--;
            _code.Append(this.IndentChar, this.IndentDepth * _indent)
                 .AppendFormat("}} {0}", code)
                 .AppendLine();

            return this;
        }

        public CodeWriter EndBlock()
        {
            _indent--;
            _code.Append(' ', 4 * _indent)
                 .Append("}")
                 .AppendLine();

            return this;
        }

        public CodeWriter IndentLess()
        {
            _indent--;
            return this;
        }

        public CodeWriter IndentMore()
        {
            _indent++;
            return this;
        }

        public CodeWriter Merge(CodeWriter writer)
        {
            _code.Append(writer.ToString());

            return this;
        }

        public CodeWriter Write()
        {
            _code.AppendLine();
            return this;
        }

        public CodeWriter Write(string code)
        {
            _code.Append(' ', 4 * _indent)
                 .Append(code)
                 .AppendLine();

            return this;
        }

        public CodeWriter Write(string format, params object[] parameters)
        {
            _code.Append(' ', 4 * _indent)
                 .AppendFormat(format, parameters)
                 .AppendLine();

            return this;
        }

        public CodeWriter WriteIndented(string code)
        {
            _code.Append(' ', 4 * (_indent + 1))
                 .Append(code)
                 .AppendLine();

            return this;
        }

        public CodeWriter WriteIndented(string format, params object[] parameters)
        {
            _code.Append(' ', 4 * (_indent + 1))
                 .AppendFormat(format, parameters)
                 .AppendLine();

            return this;
        }

        public override string ToString()
        {
            return _code.ToString();
        }
    }
}
