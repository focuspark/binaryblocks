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

namespace Isris.BinaryBlocks.CsharpGenerator
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
            //public void WriteBranchDetails(Project.Branch branch, ScreenWriterOptions options = ScreenWriterOptions.None)
            //{
            //    if (!_keepWriting)
            //        return;

            //    this.WriteLine(branch.Name);
            //    if ((options & ScreenWriterOptions.WriteDescription) == ScreenWriterOptions.WriteDescription)
            //    {
            //        if (String.IsNullOrEmpty(branch.Description))
            //        {
            //            this.WriteLine("[no description]");
            //        }
            //        else
            //        {
            //            this.WriteLine(branch.Description);
            //        }
            //    }
            //    if ((options & ScreenWriterOptions.WriteHead) == ScreenWriterOptions.WriteHead)
            //    {
            //        this.WriteLine("Branch Head:");
            //        if (branch.HeadId == Guid.Empty)
            //        {
            //            this.WriteLine("[no head]");
            //        }
            //        else
            //        {
            //            this.WriteChangeDetails(branch.Head, options);
            //        }
            //    }
            //    if ((options & ScreenWriterOptions.WriteBase) == ScreenWriterOptions.WriteBase)
            //    {
            //        this.WriteLine("Branch Base:");
            //        if (branch.HeadId == Guid.Empty)
            //        {
            //            this.WriteLine("[no base]");
            //        }
            //        else
            //        {
            //            this.WriteChangeDetails(branch.Base, options);
            //        }
            //    }
            //}

            //public void WriteBranchHistory(Project.Branch branch, ScreenWriterOptions options = ScreenWriterOptions.None)
            //{
            //    if (!_keepWriting)
            //        return;

            //    this.WriteLine(branch.Name);
            //    if ((options & ScreenWriterOptions.WriteDescription) == ScreenWriterOptions.WriteDescription)
            //    {
            //        if (String.IsNullOrEmpty(branch.Description))
            //        {
            //            this.WriteLine("[no description]");
            //        }
            //        else
            //        {
            //            this.WriteLine(branch.Description);
            //        }
            //    }

            //    this.WriteLine("Brach History:");
            //    this.WriteChangeHistory(branch.Head, options);
            //}

            //public void WriteChangeActions(Change change)
            //{
            //    if (!_keepWriting)
            //        return;
            //    switch (change.Actions.Count)
            //    {
            //        case 0:
            //            WriteLine("Changes: No files");
            //            break;
            //        case 1:
            //            WriteLine("Changes: 1 file");
            //            break;
            //        default:
            //            WriteLine(String.Format("Changes: {0} files", change.Actions.Count));
            //            break;
            //    }
            //    //
            //    foreach (Change.Action.Add add in change.Actions.Adds)
            //    {
            //        WriteLine(String.Format("[+] {0}", add.File));
            //    }
            //    //
            //    foreach (Change.Action.Edit edit in change.Actions.Edits)
            //    {
            //        WriteLine(String.Format("[%] {0}", edit.File));
            //    }
            //    //
            //    foreach (Change.Action.Remove remove in change.Actions.Removes)
            //    {
            //        WriteLine(String.Format("[-] {0}", remove.File));
            //    }
            //    //
            //    foreach (Change.Action.Move move in change.Actions.Moves)
            //    {
            //        WriteLine(String.Format("[>] {0} to {1}", move.Src, move.Dst));
            //    }
            //    //
            //    foreach (Change.Action.Error error in change.Actions.Errors)
            //    {
            //        WriteLine(String.Format("[!] {0}", error.File));
            //    }
            //}

            //public void WriteChangeDetails(Change change, ScreenWriterOptions options = ScreenWriterOptions.None)
            //{
            //    if (!_keepWriting)
            //        return;

            //    this.WriteChangeHeader(change);
            //    this.WriteLine();
            //    if ((options & ScreenWriterOptions.WriteDescription) == ScreenWriterOptions.WriteDescription)
            //    {
            //        if (String.IsNullOrEmpty(change.Description))
            //        {
            //            this.WriteLine("[no description]");
            //        }
            //        else
            //        {
            //            this.WriteLine(change.Description);
            //        }
            //        this.WriteLine();
            //    }
            //    if ((options & ScreenWriterOptions.WriteActions) == ScreenWriterOptions.WriteActions)
            //    {
            //        this.WriteChangeActions(change);
            //        this.WriteLine();
            //    }
            //    if ((options & ScreenWriterOptions.WriteState) == ScreenWriterOptions.WriteState)
            //    {
            //        this.WriteChangeState(change);
            //        this.WriteLine();
            //    }
            //}

            //public void WriteChangeHeader(Change change)
            //{
            //    if (!_keepWriting)
            //        return;

            //    this.WriteLine(String.Format("Change: {0:N}", change.ChangeId), 0, ConsoleColor.Yellow);
            //    this.WriteLine(String.Format("Author: {0} ({1})", change.Username, change.Email), 0, ConsoleColor.Gray);
            //    if (change.Timestamp == DateTime.MinValue)
            //    {
            //        this.WriteLine("Date:   Not Saved", 0, ConsoleColor.Gray);
            //    }
            //    else
            //    {
            //        this.WriteLine(String.Format("Date:   {0:dd MMM yyyy hh:mm tt}", change.Timestamp), 0, ConsoleColor.Gray);
            //    }
            //    this.WriteLine();
            //    if (String.IsNullOrEmpty(change.Synopsis))
            //    {
            //        this.WriteLine("[no synopsis]");
            //    }
            //    else
            //    {
            //        this.WriteLine(change.Synopsis.Trim());
            //    }
            //    switch (change.Type)
            //    {
            //        case Change.ChangeType.Integration:
            //            this.WriteLine("Integration from " + change.ParentId.ToString("N"), DefaultPadding, ConsoleColor.Red);
            //            break;
            //        case Change.ChangeType.Merge:
            //            this.WriteLine("Merge from " + change.ParentId.ToString("N"), DefaultPadding, ConsoleColor.Red);
            //            break;
            //        case Change.ChangeType.Revert:
            //            this.WriteLine("Revert of " + change.ParentId.ToString("N"), DefaultPadding, ConsoleColor.Red);
            //            break;
            //    }
            //}

            //public void WriteChangeHistory(Change change, ScreenWriterOptions options = ScreenWriterOptions.None)
            //{
            //    if (_keepWriting)
            //        return;
            //    //
            //    while (_keepWriting && change != null)
            //    {
            //        this.WriteChangeDetails(change, options);
            //        change = change.Parent;
            //    }
            //}

            //public void WriteChangeState(Change change)
            //{
            //    if (!_keepWriting)
            //        return;
            //    // set column widths at least as wide as the headers, then find the actual widths by looking at each line
            //    int maxNameLength = 9;
            //    int maxSizeLength = 9;
            //    foreach (Change.File file in change.State)
            //    {
            //        maxNameLength = (file.Name.Length > maxNameLength) ? file.Name.Length : maxNameLength;
            //        string s = ConvertToSizeString(file.Size);
            //        maxSizeLength = (s.Length > maxSizeLength) ? s.Length : maxSizeLength;
            //    }
            //    // write the headers
            //    this.WriteLine(String.Format("{0} {1}  {2}", "File Name".PadRight(maxNameLength), "File Hash".PadLeft(32), "File Size"));
            //    // write out each line
            //    foreach (Change.File file in change.State)
            //    {
            //        string s = ConvertToSizeString(file.Size);
            //        this.WriteLine(String.Format("{0}.{1:N}..{2}", file.Name.PadRight(maxNameLength, '.'), file.Hash, s.PadLeft(maxSizeLength, '.')));
            //    }
            //}

            //public void WriteLabelDetails(Project.Label label, ScreenWriterOptions options = ScreenWriterOptions.None)
            //{
            //    // TODO: implement ScreenWriter.WriteLabelDetails
            //    throw new NotImplementedException();
            //}

            //public void WriteLabelHistory(Project.Label label, ScreenWriterOptions option = ScreenWriterOptions.None)
            //{
            //    // TODO: implement ScreenWriter.WriteLabelHistory
            //    throw new NotImplementedException();
            //}

            //public void WriteProjectDetails(Project project)
            //{
            //    // TODO: implement ScreenWriter.WriteProjectDetails
            //    throw new NotImplementedException();
            //}

            //public void WriteSolutionDetails(Solution solution)
            //{
            //    // TODO: implement ScreenWriter.WriteSolutionDetails
            //    throw new NotImplementedException();
            //}

            //public void WriteWorkspaceDetails(Workspace workspace)
            //{
            //    // TODO: implement ScreenWriter.WriteWorkspaceDetails
            //    throw new NotImplementedException();
            //}

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

            private string ConvertToSizeString(ulong bytes)
            {
                const float sizeScaler = 1024.0f;

                string sizeString = null;
                float sizeInKb = (float)bytes / sizeScaler;
                float sizeInMb = sizeInKb / sizeScaler;
                float sizeInGb = sizeInMb / sizeScaler;

                if (sizeInGb >= 1.0f)
                {
                    sizeString = String.Format("{0:###,###,###,###.00} GB", sizeInGb);
                }
                else if (sizeInMb >= 1.0f)
                {
                    sizeString = String.Format("{0:###,###,###,###.00} MB", sizeInMb);
                }
                else if (sizeInKb >= 1.0f)
                {
                    sizeString = String.Format("{0:###,###,###,###.00} KB", sizeInKb);
                }
                else
                {
                    sizeString = String.Format("{0}  B", bytes);
                }

                return sizeString;
            }
            #endregion
        }
    }
}
