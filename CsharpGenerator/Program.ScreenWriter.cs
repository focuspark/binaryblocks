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
using System.Text.RegularExpressions;

using StringReader = System.IO.StringReader;
using TextWriter = System.IO.TextWriter;

namespace BinaryBlocks.CsharpGenerator
{
    partial class Program
    {
        internal partial class ScreenWriter
        {
            #region Constants
            public const int DefaultPadding = 4;
            public const int DefaultTabSize = 4;
            #endregion
            #region Constructors
            internal ScreenWriter()
            {
                _foregroundColor = System.Console.ForegroundColor;
                _backgroundColor = System.Console.BackgroundColor;
                _keepWriting = true;
                _linesWritten = 0;
                _maxCharsPerFrame = System.Console.WindowWidth - DefaultPadding;
                _maxLinesPerFrame = System.Console.WindowHeight;
            }
            #endregion
            #region Members
            private ConsoleColor _backgroundColor;
            private ConsoleColor _foregroundColor;
            private bool _keepWriting;
            private int _linesWritten;
            private int _maxCharsPerFrame;
            private int _maxLinesPerFrame;
            #endregion
            #region Methods
            public void WriteText(string text, int padding = ScreenWriter.DefaultPadding, int tabSize = ScreenWriter.DefaultTabSize)
            {
                #region Argument Validation
                if (String.IsNullOrEmpty(text))
                    return;
                if (padding < 0)
                    throw new ArgumentOutOfRangeException("Padding cannot be negative");
                #endregion
                if (!_keepWriting)
                    return;

                string[] lines = Regex.Split(text, @"[\r\n]{1,2}", RegexOptions.Compiled);
                Match match = null;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int leftPad = 0;
                    //
                    if (Regex.IsMatch(line, @"^\s*$", RegexOptions.Compiled))
                    {
                        this.WriteLine();
                    }
                    else
                    {
                        if ((match = Regex.Match(line, @"^\t+", RegexOptions.Compiled)).Success)
                        {
                            leftPad = match.Length * tabSize + padding;
                        }
                        else
                        {
                            leftPad = padding;
                        }
                        //
                        this.WriteLine(line.Remove(0, match.Length), leftPad, _foregroundColor);
                    }
                }
            }

            public void WriteLine()
            {
                this.WriteLine(" ", 0, _foregroundColor);
            }

            public void WriteLine(string text)
            {
                this.WriteLine(text, DefaultPadding, _foregroundColor);
            }

            public void WriteLine(string text, int padding)
            {
                this.WriteLine(text, padding, _foregroundColor);
            }

            public void WriteLine(string text, int padding, ConsoleColor color)
            {
                if (!_keepWriting)
                    return;

                string pad = new String(' ', padding);
                int maxLength = _maxCharsPerFrame - pad.Length;
                Match match = null;
                //
                if ((match = Regex.Match(text, @"[\n\r]+$", RegexOptions.Compiled)).Success)
                {
                    text = text.Remove(match.Index, match.Length);
                }
                //
                int read = 0;
                while (_keepWriting && read < text.Length)
                {
                    //
                    string line = text.Substring(read);
                    //
                    if (line.Length > maxLength)
                    {
                        line = line.Substring(0, maxLength);
                        line = line.Substring(0, line.LastIndexOf(' '));
                    }
                    if ((match = Regex.Match(line, @"[\n\r]{2}", RegexOptions.Compiled)).Success)
                    {
                        line = line.Substring(0, match.Index);
                        read += match.Length;
                    }
                    if ((match = Regex.Match(line, @"^\s+", RegexOptions.Compiled)).Success)
                    {
                        line = line.Remove(match.Index, match.Length);
                        read += match.Length;
                    }
                    //
                    System.Console.ForegroundColor = color;
                    System.Console.Write(pad);
                    System.Console.WriteLine(line);
                    System.Console.ForegroundColor = _foregroundColor;
                    //
                    read += line.Length;
                    _linesWritten++;
                    //
                    if (_linesWritten < _maxLinesPerFrame - 1)
                        continue;

                    const string WaitMessage = " : <space> to continue, <enter> to advance one line, <q> to quit :";

                    System.Console.Write(WaitMessage);
                    ConsoleKeyInfo key = System.Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            _linesWritten--;
                            break;
                        case ConsoleKey.Q:
                            _keepWriting = false;
                            break;
                        case ConsoleKey.Spacebar:
                            _linesWritten = 0;
                            break;
                    }
                    //
                    int charsToDelete = System.Console.CursorLeft;
                    System.Console.CursorLeft = 0;
                    System.Console.Write(new string(' ', charsToDelete));
                    System.Console.CursorLeft = 0;
                }
            }
            #endregion
        }
    }
}
