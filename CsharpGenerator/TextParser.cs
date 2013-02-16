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
using System.Linq;
using System.Text;

namespace Isris.BinaryBlocks.CsharpGenerator
{
    internal static partial class TextParser
    {
        public const char BlockBeginDelimiter = '{';
        public const char BlockEndDelimiter = '}';
        public const char CommentDelimiter = '/';
        public const char StringDelimiter = '"';

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        public static void SkipComment(string content, ref int index)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);

            const char InlineComment = '/';
            const char BlockComment = '*';

            if (content[index + 1] == InlineComment)
            {
                while (index < content.Length)
                {
                    index++;
                    if (content[index] == '\n')
                    {
                        index += 1;
                        return;
                    }
                }
            }
            else if (content[index + 1] == BlockComment)
            {
                while (index < content.Length)
                {
                    index++;
                    if (content[index] == BlockComment && content[index + 1] == TextParser.CommentDelimiter)
                    {
                        index += 2;
                        return;
                    }
                }
                if (index >= content.Length)
                    throw new Exception(index, String.Format("Comment character encountered and '{0}' or '{1}' expected but not found", InlineComment, BlockComment));
            }
            else
                throw new Exception(index, String.Format("Comment character encountered and '{0}' or '{1}' expected but not found", InlineComment, BlockComment));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ParseString(string content, ref int index)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);
            Debug.Assert(content[index] == TextParser.StringDelimiter);

            StringBuilder buffer = new StringBuilder();

            while (++index < content.Length)
            {
                if (index >= content.Length)
                    throw new Exception(index, TextParser.StringDelimiter + " expected");
                if (content[index] == TextParser.StringDelimiter)
                {
                    if (index + 1 >= content.Length)
                        throw new Exception(index, TextParser.StringDelimiter + " expected");
                    if (content[index + 1] == TextParser.StringDelimiter)
                    {
                        buffer.Append(content[index]);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    buffer.Append(content[index]);
                }
            }

            if (index >= content.Length)
                throw new Exception(index, TextParser.StringDelimiter + " expected");

            return buffer.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool PeekAny(string content, int index, params char[] values)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);

            for (int i = 0; i < values.Length; i++)
            {
                for (int j = index; j < content.Length; j++)
                {
                    if (content[j] == values[i])
                        return true;
                    if (!Char.IsWhiteSpace(content[j]))
                    {
                        if (content[j] == TextParser.CommentDelimiter)
                        {
                            TextParser.SkipComment(content, ref j);
                        }
                        else
                            return false;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Seeks forward from the initial index until it finds one, or more, characters in the content while ignoring whitespace
        /// </summary>
        /// <param name="content">Content to be scanned</param>
        /// <param name="index">Starting position within the content</param>
        /// <param name="values">List of characters to found in order</param>
        public static void SeekAll(string content, ref int index, params char[] values)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);

            for (int i = 0; i < values.Length; i++)
            {
                while (content[index] != values[i])
                {
                    if (!Char.IsWhiteSpace(content[index]))
                    {
                        if (content[index] == TextParser.CommentDelimiter)
                        {
                            TextParser.SkipComment(content, ref index);
                        }
                        else
                        {
                            throw new Exception(index, values[i] + " expected");
                        }
                    }
                    index++;
                }
                // move to the next position in content if there are more values to seek
                if (i + 1 < values.Length)
                {
                    index++;
                }
            }
        }
        /// <summary>
        /// Seeks forward from the initial index until it finds the next non-white space character
        /// </summary>
        /// <param name="content">Content to be scanned</param>
        /// <param name="index">Starting position within the content</param>
        public static bool SeekNext(string content, ref int index)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);

            while (index < content.Length)
            {
                if (!Char.IsWhiteSpace(content[index]))
                {
                    if (content[index] == TextParser.CommentDelimiter)
                    {
                        TextParser.SkipComment(content, ref index);
                    }
                    else
                    {
                        break;
                    }
                }
                index++;
            }

            return index < content.Length;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string ParseBlock(string content, ref int index, char begin, char end)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);
            Debug.Assert(content[index] == begin);

            int start = index;
            while (content[index] != end)
            {
                index++;
                if (index >= content.Length)
                    throw new Exception(index, end + " expected");
            }
            index++;

            return content.Substring(start, index - start);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        /// <param name="allowedCharacters"></param>
        /// <returns></returns>
        public static string ParseWord(string content, ref int index, params char[] allowedCharacters)
        {
            Debug.Assert(content != null);
            Debug.Assert(index >= 0 && index < content.Length);
            Debug.Assert(allowedCharacters != null);

            int start = index;
            while (index < content.Length)
            {
                if (!(Char.IsLetterOrDigit(content[index]) || allowedCharacters.Contains(content[index])))
                    break;
                index++;
            }

            return content.Substring(start, index - start);
        }
    }
}
