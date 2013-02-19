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
using System.Diagnostics;

namespace BinaryBlocks
{
    internal static class Debug
    {
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            System.Diagnostics.Debug.Assert(condition);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            System.Diagnostics.Debug.Assert(condition, message);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string format, params object[] parameters)
        {
            System.Diagnostics.Debug.Assert(condition, String.Format(format, parameters));
        }

        [Conditional("DEBUG")]
        public static void Break()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        [Conditional("DEBUG")]
        public static void Execute(Action function)
        {
            function();
        }

        [Conditional("DEBUG")]
        public static void ExecuteIf(bool condition, Action function)
        {
            if (condition)
            {
                function();
            }
        }

        [Conditional("DEBUG")]
        public static void Log(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        [Conditional("DEBUG")]
        public static void Write(string message)
        {
            System.Diagnostics.Debug.Write("<debug> " + message);
        }

        [Conditional("DEBUG")]
        public static void Write(string format, params object[] parameters)
        {
            System.Diagnostics.Debug.Write(String.Format("<debug> " + format, parameters));
        }

        [Conditional("DEBUG")]
        public static void WriteIf(bool condition, string message)
        {
            if (condition)
            {
                System.Diagnostics.Debug.Write("<debug> " + message);
            }
        }

        [Conditional("DEBUG")]
        public static void WriteIf(bool condition, string format, params object[] parameters)
        {
            if (condition)
            {
                System.Diagnostics.Debug.Write(String.Format("<debug> " + format, parameters));
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine("<debug> " + message);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("<debug> " + format, parameters));
        }

        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string message)
        {
            if (condition)
            {
                System.Diagnostics.Debug.WriteLine("<debug> " + message);
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string format, params object[] parameters)
        {
            if (condition)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("<debug> " + format, parameters));
            }
        }
    }   
}
